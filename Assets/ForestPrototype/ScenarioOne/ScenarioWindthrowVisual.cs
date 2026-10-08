using UnityEngine;

// Presentation rebuilt from existing deadwood, victim and event records.
// Direction is derived; this component adds no persistent world fields.
public sealed class ScenarioWindthrowVisual : MonoBehaviour
{
    public string TreeId { get; private set; }
    public bool HasCrown => crown != null;
    private GameObject rootPlate, rootSource, crown;
    private Transform stem;
    private ForestTree victim;
    private ScenarioDeadwoodRecord record;
    private WindthrowVisualCatalog catalog;

    public void Initialize(ScenarioDeadwoodRecord deadwood, ForestTree tree, float bearing, WindthrowVisualCatalog assets)
    {
        record = deadwood; victim = tree; TreeId = record.treeId; catalog = assets;
        transform.position = record.worldPosition;
        transform.rotation = Quaternion.Euler(0, bearing, 0);
        var log = new GameObject("Directed fallen stem");
        stem = log.transform; stem.SetParent(transform, false);
        stem.localRotation = Quaternion.Euler(0, -90, 0); // authored log local X -> falling local +Z
        log.AddComponent<ScenarioFallenLogVisual>();
        var target = gameObject.AddComponent<BoxCollider>();
        target.isTrigger = true;
        target.size = new Vector3(Mathf.Max(.5f, record.originalDiameterCm / 100f), .6f, Mathf.Max(1f, record.originalHeightMeters * .8f));
        target.center = new Vector3(0, .3f, target.size.z * .5f);
        Refresh(deadwood);
    }

    public void Refresh(ScenarioDeadwoodRecord deadwood)
    {
        record = deadwood;
        stem.localPosition = new Vector3(0, .05f, Mathf.Max(1f, record.originalHeightMeters * .8f) * .5f);
        var logs = SectionFiveVisualCatalog.Load();
        if (logs != null) stem.GetComponent<ScenarioFallenLogVisual>().Refresh(record, logs);
        GameObject desired = catalog == null ? null : record.YearsSinceFall(record.lastDecayYear) >= 4 ? catalog.weatheredRootPlate : catalog.freshRootPlate;
        if (desired != rootSource)
        {
            if (rootPlate != null) { rootPlate.SetActive(false); Destroy(rootPlate); }
            rootSource = desired;
            if (desired != null)
            {
                rootPlate = Instantiate(desired, transform);
                rootPlate.name = "Uprooted root plate";
                rootPlate.transform.localPosition = Vector3.zero;
                rootPlate.transform.localRotation = Quaternion.identity;
                rootPlate.transform.localScale = Vector3.one * Mathf.Clamp(record.originalDiameterCm / 25f, .4f, 2f);
            }
        }
        bool salvaged = record.remainingVolumeM3 <= 0;
        if (record.DecayClass >= 2 || salvaged) SetCrownVisible(false);
        stem.gameObject.SetActive(!salvaged && !HasCrown);
        var target = GetComponent<BoxCollider>();
        if (target != null) target.enabled = !salvaged;
    }

    public void SetCrownVisible(bool visible)
    {
        visible &= record.remainingVolumeM3 > 0f && record.DecayClass < 2 && victim != null;
        if (!visible)
        {
            if (crown != null) { crown.SetActive(false); Destroy(crown); crown = null; }
            stem.gameObject.SetActive(record.remainingVolumeM3 > 0f);
            return;
        }
        if (crown != null) return;
        Transform original = victim.transform.Find("PolishedVisual");
        if (original == null) return;
        crown = Instantiate(original.gameObject, transform);
        crown.name = "Fallen crown prototype";
        crown.transform.localPosition = new Vector3(0, Mathf.Max(.15f, record.originalDiameterCm / 200f), 0);
        crown.transform.localRotation = Quaternion.Euler(90, 0, 0);
        crown.SetActive(true);
        foreach (Collider collider in crown.GetComponentsInChildren<Collider>(true)) Destroy(collider);
        foreach (MonoBehaviour behaviour in crown.GetComponentsInChildren<MonoBehaviour>(true)) { behaviour.enabled = false; Destroy(behaviour); }
        foreach (LODGroup group in crown.GetComponentsInChildren<LODGroup>(true))
            if (group.lodCount > 0) group.ForceLOD(Mathf.Min(2, group.lodCount - 1));
        stem.gameObject.SetActive(false);
    }
}
