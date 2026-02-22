namespace Bigode.Models;

public class Token
{
    public TokenType Type { get; set; }
    public required string Value { get; set; }
    public required string Raw { get; set; }
}