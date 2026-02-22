namespace Bigode.Models;

internal class ASTNode
{
    public TokenType Type { get; set; }
    public required string Value { get; set; }
    public List<ASTNode> Children { get; set; } = [];
}