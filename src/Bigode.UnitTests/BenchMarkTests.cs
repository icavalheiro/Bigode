using Bigode.Models;

namespace Bigode.UnitTests;

public class BenchMarkTests
{
    private readonly Bigode bigode = new(".html");

    // [Test]
    public async Task Should_RenderThousand_InASecond()
    {
        await Tools.WriteTempTemplate("leaf_bench", "I am the {{name}}.");
        await Tools.WriteTempTemplate("branch_bench", "Branch including: {{> leaf_bench }}");
        var templatePath = await Tools.WriteTempTemplate("root_bench", "Root starts: {{> branch_bench }}");
        var model = new RenderModel
        {
            {"name", new ("leaf")}
        };

        var startTime = DateTime.Now;
        for (var i = 0; i < 1_000; i++)
        {
            await bigode.Parse(templatePath, model);
        }
        var endTime = DateTime.Now;
        var observed = TimeSpan.FromTicks(startTime.Ticks - endTime.Ticks);
        await Assert.That(observed).IsLessThan(TimeSpan.FromSeconds(1));
    }
}