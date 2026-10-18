using PartFlow.Api.Models;

namespace PartFlow.Tests;

public class PartLifecycleTests
{
    [Theory]
    [InlineData(PartStatus.Draft, PartStatus.Draft)]
    [InlineData(PartStatus.Draft, PartStatus.Active)]
    [InlineData(PartStatus.Active, PartStatus.Active)]
    [InlineData(PartStatus.Active, PartStatus.Obsolete)]
    [InlineData(PartStatus.Obsolete, PartStatus.Obsolete)]
    public void CanTransition_ReturnsTrue_ForAllowedTransitions(
        PartStatus current, PartStatus next)
    {
        Assert.True(PartLifecycle.CanTransition(current, next));
    }

    [Theory]
    [InlineData(PartStatus.Draft, PartStatus.Obsolete)]
    [InlineData(PartStatus.Active, PartStatus.Draft)]
    [InlineData(PartStatus.Obsolete, PartStatus.Draft)]
    [InlineData(PartStatus.Obsolete, PartStatus.Active)]
    public void CanTransition_ReturnsFalse_ForForbiddenTransitions(
        PartStatus current, PartStatus next)
    {
        Assert.False(PartLifecycle.CanTransition(current, next));
    }
}
