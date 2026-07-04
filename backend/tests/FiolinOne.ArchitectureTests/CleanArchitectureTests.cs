using FiolinOne.Domain.Common;
using NetArchTest.Rules;
using Xunit;

namespace FiolinOne.ArchitectureTests;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "FiolinOne.Application",
                "FiolinOne.Infrastructure",
                "FiolinOne.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
