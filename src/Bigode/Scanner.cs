namespace Bigode;

internal class Scanner(string input)
{
    private readonly string input = input;
    private readonly int length = input.Length;
    private int cursor = 0;

    public bool IsEOF()
    {
        return cursor >= length;
    }

    public bool Match(string seq)
    {
        if (IsEOF()) return false;

        for (int i = 0; i < seq.Length; i++)
        {
            if (cursor + i >= length || input[cursor + i] != seq[i])
                return false;
        }
        return true;
    }

    public void Advance(int n = 1)
    {
        cursor += n;
    }

    public string ScanUntil(string seq)
    {
        int start = cursor;
        while (!IsEOF() && !Match(seq))
        {
            Advance();
        }
        return input[start..cursor];
    }
}