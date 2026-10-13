// Rule and wording for the question asked before a year is advanced with planned work that nobody approved.
//
// Only approved work is carried out when time advances. Orders that are still pending are kept, but the year passes
// without them, so a player who marked trees, opened the Work Plan and pressed Advance would lose a year of a century
// scenario without being told. This does not stop that: waiting a year on purpose is a legitimate choice, so the player
// is always offered "Advance anyway". It also never approves anything for them.
//
// Pure (no UnityEngine) so Tools/Verification/ScenarioOneUx can check it on plain .NET. The caller counts the orders:
//   pending  = status Pending and no validation problem (the same orders "Approve pending work" would approve);
//   approved = status Approved.
public static class AdvanceGuard
{
    public const string ReturnLabel = "Back to the plan";
    public const string ApproveLabel = "Approve pending work";
    public const string AdvanceAnywayLabel = "Advance anyway";

    public readonly struct Decision
    {
        public readonly bool Warn;
        public readonly string Headline;
        public readonly string Detail;

        public Decision(bool warn, string headline, string detail)
        {
            Warn = warn;
            Headline = headline;
            Detail = detail;
        }
    }

    // Warn only when there is planned work and none of it is approved, i.e. the year would pass with no work done.
    // If something is approved the year does something; the remaining pending orders simply wait, as they always have.
    public static Decision Evaluate(int pendingCount, int approvedCount)
    {
        if (pendingCount <= 0 || approvedCount > 0)
            return new Decision(false, "", "");
        string tasks = pendingCount == 1 ? "1 planned task is" : pendingCount + " planned tasks are";
        return new Decision(true,
            "You have planned work that has not been approved.",
            tasks + " still pending. Only approved work is carried out when you advance, so this year would pass with no work done. "
            + "Go back and approve the plan if you want it carried out, or advance anyway if you mean to wait.");
    }
}
