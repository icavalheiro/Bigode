namespace Bigode.UnitTests.Attributes;

public class BenchmarkOnlyAttribute() : SkipAttribute("This test should only run on benchmarks, set RUN_BENCHMARK=1 to run it")
{
    public override async Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        var envVar = Environment.GetEnvironmentVariable("RUN_BENCHMARK");
        var shouldByEnvVar = envVar == "1" || string.Equals(envVar, "true", StringComparison.InvariantCultureIgnoreCase);
        if (shouldByEnvVar)
            return false;

        return true;
    }
}