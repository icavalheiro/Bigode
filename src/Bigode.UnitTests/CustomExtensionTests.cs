using Bigode.Models;

namespace Bigode.UnitTests;

public class CustomExtensionTests
{
    private readonly Bigode bigode = new(".mustache");

    [Test]
    public async Task Should_RenderNestedPartials_WithCustomExtension()
    {
        await Tools.WriteTempTemplate("leaf", "I am the {{name}}.", ".mustache");
        await Tools.WriteTempTemplate("branch", "Branch including: {{> leaf }}", ".mustache");
        var templatePath = await Tools.WriteTempTemplate("root", "Root starts: {{> branch }}", ".mustache");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"name", new ("leaf")}
        });

        await Assert.That(result).IsEqualTo("Root starts: Branch including: I am the leaf.");
    }
}