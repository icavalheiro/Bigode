using Bigode.Models;

namespace Bigode;

internal class Parser(string template)
{
    private readonly Scanner scanner = new(template);
    private readonly string openTag = "{{";
    private readonly string closeTag = "}}";

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (!scanner.IsEOF())
        {
            string text = scanner.ScanUntil(openTag);
            if (text.Length > 0)
            {
                tokens.Add(new Token { Type = TokenType.TEXT, Value = text, Raw = text });
            }

            if (scanner.IsEOF()) break;

            scanner.Advance(openTag.Length);

            string tagContent = scanner.ScanUntil(closeTag);

            scanner.Advance(closeTag.Length);

            ProcessTag(tagContent, tokens);
        }

        return tokens;
    }

    private static void ProcessTag(string content, List<Token> tokens)
    {
        string trimmed = content.Trim();
        if (trimmed.Length == 0)
            return;

        char firstChar = trimmed[0];
        TokenType type = TokenType.VAR;
        string val = trimmed;

        if (firstChar == '#')
        {
            type = TokenType.SECTION_START;
            val = trimmed[1..].Trim();
        }
        else if (firstChar == '/')
        {
            type = TokenType.SECTION_END;
            val = trimmed[1..].Trim();
        }
        else if (firstChar == '^')
        {
            type = TokenType.INV_SECTION_START;
            val = trimmed[1..].Trim();
        }
        else if (firstChar == '!')
        {
            type = TokenType.COMMENT;
            val = trimmed[1..].Trim();
        }
        else if (firstChar == '>')
        {
            type = TokenType.PARTIAL;
            val = trimmed[1..].Trim();
        }

        tokens.Add(new Token
        {
            Type = type,
            Value = val,
            Raw = content
        });
    }

    public static ASTNode BuildAST(List<Token> tokens)
    {
        var root = new ASTNode
        {
            Type = TokenType.SECTION_START,
            Value = "root"
        };
        var stack = new Stack<ASTNode>();
        stack.Push(root);

        foreach (var token in tokens)
        {
            var currentParent = stack.Peek();

            if (token.Type == TokenType.TEXT || token.Type == TokenType.VAR || token.Type == TokenType.PARTIAL)
            {
                currentParent.Children.Add(new ASTNode
                {
                    Type = token.Type,
                    Value = token.Value,
                    Children = []
                });
            }
            else if (token.Type == TokenType.SECTION_START || token.Type == TokenType.INV_SECTION_START)
            {
                var node = new ASTNode
                {
                    Type = token.Type,
                    Value = token.Value,
                    Children = []
                };
                currentParent.Children.Add(node);
                stack.Push(node);
            }
            else if (token.Type == TokenType.SECTION_END)
            {
                if (token.Value != currentParent.Value && currentParent.Value != "root")
                    throw new Exception($"Bigode: Mismatched section. Expected {currentParent.Value}, got {token.Value}");

                stack.Pop();
            }
        }

        if (stack.Count > 1)
            throw new Exception($"Bigode: Unclosed section \"{stack.Peek().Value}\"");

        return root;
    }
}