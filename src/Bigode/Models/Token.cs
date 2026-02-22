namespace Bigode.Models;

internal class Token
{
    public TokenType Type { get; set; }
    public required string Value { get; set; }
    public required string Raw { get; set; }
}