// OFFLINE TEST ADAPTER ONLY: a copy of the HistoryProvenance declaration from
// Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs (same members, same order).
// The production file also holds text classes that need the whole history model, so the offline fixture cannot compile it.
// Never ship this file under Assets. Binding to the real enum is checked separately by
// Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py, which compiles the real sources without this file.
public enum HistoryProvenance
{
    Recorded,
    Current,
    Derived,
    Reconstructed,
    SessionOnly,
    NotRecorded
}
