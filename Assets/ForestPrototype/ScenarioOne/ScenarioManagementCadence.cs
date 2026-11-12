using System;
using System.Collections.Generic;
using System.Linq;

// Calendar navigation only. These intervals are gameplay calibration, never a treatment prescription.
public static class ScenarioManagementCadence
{
    public const int ReviewGapYears = 4, MaxTicksPerSkip = 5;
    public static int Compute(int current, int reviewYear, IEnumerable<ScenarioOneWorkOrder> orders,
        IEnumerable<ScenarioManagementEvent> events, out int? lastFelling)
    {
        lastFelling = null;
        var work = orders.ToList();
        var history = events.ToList();
        foreach (var order in work)
        {
            if (order.workOrderId <= 0 || order.type != ScenarioWorkType.FellTree
                || order.status != ScenarioWorkStatus.Completed || order.resolvedYear < order.createdYear
                || order.createdYear < 0 || order.resolvedYear > current || order.resolvedYear < 1
                || !string.IsNullOrEmpty(order.validationMessage) || string.IsNullOrEmpty(order.targetTreeId)
                || work.Count(o => o.workOrderId == order.workOrderId) != 1) continue;
            var resolved = history.Where(e => e.workOrderId == order.workOrderId
                && e.eventType == ScenarioManagementEventType.WorkResolved).ToList();
            if (resolved.Count != 1) continue;
            var item = resolved[0];
            if (item.eventId <= 0 || history.Count(e => e.eventId == item.eventId) != 1
                || item.outcome != ScenarioManagementOutcome.Succeeded || item.taskType != order.type
                || item.targetTreeId != order.targetTreeId || item.year != order.resolvedYear) continue;
            lastFelling = Math.Max(lastFelling ?? 0, item.year);
        }
        int candidate = lastFelling.HasValue ? lastFelling.Value + ReviewGapYears : current + ReviewGapYears;
        if (candidate <= current) candidate = current + ReviewGapYears;
        int target = Math.Min(candidate, Math.Min(current + MaxTicksPerSkip, reviewYear));
        return current < 25 ? Math.Min(target, 25) : target;
    }
}
