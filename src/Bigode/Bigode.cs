using System.Diagnostics;
using System.Text;
using Bigode.Models;

namespace Bigode;

/// <summary>
/// A subset of mustache parser that is AOT compatible.
/// </summary>
/// <param name="fileExtension">File extension (without '.')</param>
/// <param name="disableCachingFiles">Disabled the default cache of AST tree for files (set it to true if you expect files to change in disk)</param>
public class Bigode(string fileExtension = "html", bool disableCachingFiles = false)
{
    private readonly string fileExtension = fileExtension.Replace(".", "");

    private readonly Dictionary<string, List<ASTNode>> cache = [];

    /// <summary>
    /// Parses the document at "filePath" with the given "model".
    /// Any partials used by the document must be placed in the same folder, since paths are resolved relative to
    /// document location, and not relative to Program working dir.
    /// 
    /// Bigode Parse only accepts a subset of mustache original features, and model must be explicitly defined 
    /// with types to support AOT.
    /// </summary>
    /// <param name="filePath">Path to bigode/mustache file</param>
    /// <param name="model">Model to be used during rendering</param>
    /// <returns>A string contained the rendered document</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<string> ParseAsync(string filePath, RenderModel model)
    {
        if (File.Exists(filePath) is false)
            throw new FileNotFoundException($"Bigode file not found: {filePath}");

        try
        {
            string templateContent = await File.ReadAllTextAsync(filePath);

            if (disableCachingFiles || cache.TryGetValue(filePath, out var children) is false)
            {
                var parser = new Parser(templateContent);
                var tokens = parser.Tokenize();
                var ast = Parser.BuildAST(tokens);
                children = ast.Children;

                if (disableCachingFiles is false)
                {
                    cache.Add(filePath, children);
                }
            }

            var sb = new StringBuilder();
            await Render(children, model, Path.GetDirectoryName(filePath)!, sb);
            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new Exception($"Bigode Parsing Error: {e.Message}");
        }
    }

    private async Task Render(List<ASTNode> nodes, RenderModel context, string partialBasePath, StringBuilder sb)
    {
        foreach (var node in nodes)
        {
            if (node.Type == TokenType.TEXT)
            {
                sb.Append(node.Value);
            }
            else if (node.Type == TokenType.VAR)
            {
                if (context.TryGetValue(node.Value, out var nodeVal))
                {
                    if (nodeVal.IsString is false)
                        throw new Exception($"Bigode Render Error: Section {node.Value} invalid model");

                    sb.Append(nodeVal.GetStringValue());
                }
            }
            else if (node.Type == TokenType.SECTION_START)
            {
                var nodeVal = context[node.Value];
                if (nodeVal.IsArray)
                {
                    var loopModels = nodeVal.GetArray();
                    foreach (var loop in loopModels)
                    {
                        await Render(node.Children, loop, partialBasePath, sb);
                    }
                }
                else if (nodeVal.IsLambda)
                {
                    var lambdaSb = new StringBuilder();
                    await Render(node.Children, context, partialBasePath, lambdaSb);
                    var lambda = nodeVal.GetLambda();
                    sb.Append(await lambda(lambdaSb.ToString()));
                }
                else if (nodeVal.IsBool)
                {
                    if (nodeVal.GetBoolValue() is true)
                    {
                        await Render(node.Children, context, partialBasePath, sb);
                    }
                }
                else
                    throw new Exception($"Bigode Render Error: Section {node.Value} invalid model");
            }
            else if (node.Type == TokenType.INV_SECTION_START)
            {
                var nodeVal = context[node.Value];
                if (nodeVal.IsBool && nodeVal.GetBoolValue() is false)
                {
                    await Render(node.Children, context, partialBasePath, sb);
                }
            }
            else if (node.Type == TokenType.PARTIAL)
            {
                string partialPath = Path.Combine(partialBasePath, $"{node.Value}.{fileExtension}");
                sb.Append(await ParseAsync(partialPath, context));
            }
        }
    }
}
