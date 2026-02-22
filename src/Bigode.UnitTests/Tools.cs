namespace Bigode.UnitTests;

public static class Tools
{
    public static string GetTempFolder()
    {
        return Path.Combine(Path.GetTempPath(), "Bigode.UnitTests");
    }

    public static async Task<string> WriteTempTemplate(string name, string content, string fileExtension = ".html")
    {
        var path = Path.Combine(GetTempFolder(), $"{name}{fileExtension}");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content);
        return path;
    }
}