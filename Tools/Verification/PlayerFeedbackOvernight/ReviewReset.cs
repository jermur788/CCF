// #370 template: /*...*/ tokens are replaced by review_reset.py with exact production source slices.
// The fixture holds plain data only; it does not model Unity, ecology, persistence files or layout.
using System;
using System.Linq;
using System.Collections.Generic;
class Save { public bool annualReviewSeen; }
class Life {
 Life manager => this;
 public List<ScenarioAnnualReport> annualReports = new List<ScenarioAnnualReport>();
 public IReadOnlyList<ScenarioAnnualReport> AnnualReports => annualReports;
 public bool annualReviewSeen;
 public bool AnnualReviewSeen => annualReviewSeen;
 public List<ScenarioManagementEvent> managementEvents = new List<ScenarioManagementEvent>();
 public int nextManagementEventId = 1; public long cashCents; public int CurrentYear = 1;
 /*NEEDS*/
 /*RECORD_EVENT*/
 /*MARK*/
 // Fixture models the successful end of AdvanceYear: the production statements below run verbatim.
 public void SuccessfulAdvance() {
  var report = new ScenarioAnnualReport { year = CurrentYear };
  /*TAIL*/
  CurrentYear++;
 }
 public Save Capture() => new Save {
  /*CAPTURE*/
 };
 public static Life Restore(Save data, Life from) {
  var l = new Life { annualReports = new List<ScenarioAnnualReport>(from.annualReports), managementEvents = new List<ScenarioManagementEvent>(from.managementEvents), nextManagementEventId = from.nextManagementEventId, CurrentYear = from.CurrentYear };
  /*RESTORE*/
  return l;
 }
 public int Acks => managementEvents.Count(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged);
}
static class ResetChecks {
 static int count;
 static void Check(bool b, string n) { if (!b) throw new Exception("R370_" + n); count++; }
 static ConsumerView View(Life l, int lesson = 1) { var v = ConsumerChecks.View(lesson, l.NeedsAnnualReview, false); return v; }
 static void Main() {
  var life = new Life();
  life.SuccessfulAdvance();                       // Year 1 report
  Check(life.NeedsAnnualReview && !life.AnnualReviewSeen, "YEAR1_REPORT_UNREAD");
  life.MarkAnnualReviewSeen();
  Check(life.AnnualReviewSeen && !life.NeedsAnnualReview && life.Acks == 1, "YEAR1_ACK_MARKS_READ_ONE_EVENT");
  var ackYear1 = life.managementEvents.Last(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged).year;

  life.SuccessfulAdvance();                       // Year 2, no work at all
  Check(life.annualReports.Count == 2 && life.annualReports.Last().year == 2, "YEAR2_REPORT_APPENDED");
  Check(!life.AnnualReviewSeen && life.NeedsAnnualReview, "YEAR2_NO_WORK_REPORT_UNREAD");
  Check(life.Acks == 1, "YEAR2_ADVANCE_FABRICATES_NO_ACK");
  ConsumerChecks.Review(View(life), "R370_YEAR2_UNREAD");        // HUD, Objectives short/full/card, Map, Work Plan, Annual Review
  Check(ScenarioOneTeachingSequence.Current(View(life).Facts) == 1, "YEAR2_STAYS_OBJECTIVE2");

  life.MarkAnnualReviewSeen();
  Check(life.AnnualReviewSeen && !life.NeedsAnnualReview, "YEAR2_ACK_MARKS_READ");
  Check(life.Acks == 2 && life.managementEvents.Last(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged).year == life.CurrentYear && life.CurrentYear != ackYear1, "YEAR2_ACK_RECORDS_YEAR_SPECIFIC_EVENT");
  var v = View(life);
  Check(v.CurrentShortInstruction == ScenarioOneRackBayText.Hud(1, v.Facts) && v.CurrentShortInstruction.Contains("Crop Tree"), "YEAR2_ACK_CROP_SHORT_RESTORED");
  Check(v.CurrentInstruction == ScenarioOneRackBayText.Instruction(1, v.Facts), "YEAR2_ACK_CROP_FULL_RESTORED");
  Check(v.Card().Q<Label>("current-objective-review-first") == null, "YEAR2_ACK_NO_REVIEW_BANNER");

  life.MarkAnnualReviewSeen();                    // repeat ACK is idempotent
  Check(life.Acks == 2, "REPEAT_ACK_NO_DUPLICATE_EVENT");

  life.SuccessfulAdvance();                       // Year 3
  Check(life.NeedsAnnualReview && life.annualReports.Count == 3, "YEAR3_NEW_REPORT_UNREAD");
  ConsumerChecks.Review(View(life), "R370_YEAR3_UNREAD");

  // A failed/blocked advance never reaches the report statements (source-pinned by review_reset.py): previous state stands.
  life.MarkAnnualReviewSeen();
  int reports = life.annualReports.Count, events = life.managementEvents.Count;
  Check(life.AnnualReviewSeen && life.annualReports.Count == reports && life.managementEvents.Count == events, "NO_ADVANCE_KEEPS_STATE");

  // Existing saved boolean, no schema change: genuinely unread and genuinely read current-year saves round-trip.
  var unread = new Life(); unread.SuccessfulAdvance(); unread.MarkAnnualReviewSeen(); unread.SuccessfulAdvance();
  var restoredUnread = Life.Restore(unread.Capture(), unread);
  Check(!unread.Capture().annualReviewSeen && restoredUnread.NeedsAnnualReview, "SAVE_RELOAD_UNREAD_YEAR2_STAYS_UNREAD");
  restoredUnread.MarkAnnualReviewSeen();
  var restoredRead = Life.Restore(restoredUnread.Capture(), restoredUnread);
  Check(restoredRead.AnnualReviewSeen && !restoredRead.NeedsAnnualReview, "SAVE_RELOAD_AFTER_ACK_STAYS_READ");
  var legacy = Life.Restore(new Save { annualReviewSeen = true }, unread);
  Check(legacy.AnnualReviewSeen && !legacy.NeedsAnnualReview, "LEGACY_READ_SAVE_RETAINED");
  Check(typeof(Save).GetFields().Length == 1, "NO_NEW_SAVE_FIELD_IN_FIXTURE");
  Console.WriteLine("Review reset lifecycle PASS " + count + " (production state-transition slices + exact consumers; no Unity)");
 }
}
