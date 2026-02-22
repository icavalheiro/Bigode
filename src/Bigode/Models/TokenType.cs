namespace Bigode.Models;

internal enum TokenType
{
    TEXT,
    VAR,
    SECTION_START,
    SECTION_END,
    INV_SECTION_START,
    COMMENT,
    PARTIAL
}