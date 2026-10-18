namespace PartFlow.Api.Models;

public static class PartLifecycle
{
    public static bool CanTransition(PartStatus current, PartStatus next)
    {
        return current == next ||
            (current == PartStatus.Draft && next == PartStatus.Active) ||
            (current == PartStatus.Active && next == PartStatus.Obsolete);
    }
}
