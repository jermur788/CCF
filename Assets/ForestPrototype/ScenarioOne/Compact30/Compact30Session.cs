using System;
using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEngine;
using UnityEngine.InputSystem;

// Compact30 E2 management-loop adapter.
//
// This is a narrow Compact-only controller. It does not reimplement forestry: every economy,
// work-order, annual-ecology, juvenile-survival and history result is produced by the existing
// ScenarioOneManager / ForestEcologyController. It adds the Compact teaching and G1 rules that the
// five-rack scenario does not have:
//   * arrive at H1 and explicitly acknowledge arrival;
//   * open the current world/year stand assessment and explicitly acknowledge it;
//   * choose three living Sitka as Crop Trees with the existing C key (guidance only, never a gate);
//   * designate the four fixed 2 Oak + 2 Beech G1 sites (exact position and species);
//   * bind the existing Annual Review acknowledgement to the covering planting year;
//   * hold every annual-advance route until that covering review is acknowledged (CT-3);
//   * expose genuine read-only history and accept a deliberate no-work decision after T1.
//
// Persistence is deliberately absent. The signed Compact teaching keys live in the #690-owned save
// source, which this task may not edit. The session therefore states SAVE INTEGRATION HELD and keeps
// its teaching evidence in memory; it never writes a save, a scene or a production coordinate.

public sealed class Compact30GroupOrder
{
    public int workOrderId;
    public string siteId = "";
    public string speciesId = "";
    public string stockItemId = "";
    public int createdYear;
    public int resolvedYear = -1;
    public string juvenileId = "";
    public ScenarioWorkStatus status = ScenarioWorkStatus.Pending;
    public bool cancelled;
    public bool superseded;
}

public sealed class Compact30Session : MonoBehaviour
{
    public static Compact30Session Instance { get; private set; }
    public static bool IsActive { get; private set; }

    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestPlayer player;

    private Compact30RosterFile roster;
    private Compact30SiteFile sites;
    private string dataProblem = "";

    private readonly List<Compact30GroupOrder> group = new List<Compact30GroupOrder>();
    private readonly List<string> decisions = new List<string>();

    private bool arrivalAcknowledged;
    private bool assessmentAcknowledged;
    private bool assessmentOpen;
    private bool assessmentViewed;
    private int reviewAcknowledgedYear = -1;
    private int lastNoWorkYear = -1;

    private WorkExecutionMethod planningMethod = WorkExecutionMethod.Contractor;
    private bool planningShelter;
    private string message = "";
    private float messageUntil;
    private Material stumpMaterial;
    private Material oakJuvenileMaterial;
    private Material beechJuvenileMaterial;
    private readonly Dictionary<string, GameObject> juvenileVisuals = new Dictionary<string, GameObject>();
    private GUIStyle panelStyle, titleStyle, bodyStyle;

    public bool ArrivalAcknowledged => arrivalAcknowledged;
    public bool AssessmentAcknowledged => assessmentAcknowledged;
    public int ReviewAcknowledgedYear => reviewAcknowledgedYear;
    public string DataProblem => dataProblem;
    public IReadOnlyList<Compact30GroupOrder> Group => group;
    public int LastNoWorkYear => lastNoWorkYear;

    private void Awake()
    {
        Instance = this;
        IsActive = true;
        manager = GetComponent<ScenarioOneManager>() ?? FindFirstObjectByType<ScenarioOneManager>();
        ecology = GetComponent<ForestEcologyController>() ?? FindFirstObjectByType<ForestEcologyController>();
        player = FindFirstObjectByType<ForestPlayer>();
        LoadData();
        if (manager != null)
        {
            manager.ApprovedWorkOrdersImmutable = true;
            manager.SuppressHistoricalCompletion = true;
            manager.AdvanceYearBlocker = CanAdvanceYear;
            manager.AnnualReviewAcknowledged += OnAnnualReviewAcknowledged;
        }
        ForestTree.Felled += OnAuthoredTreeFelled;
    }

    private void OnDestroy()
    {
        ForestTree.Felled -= OnAuthoredTreeFelled;
        if (manager != null)
        {
            manager.AnnualReviewAcknowledged -= OnAnnualReviewAcknowledged;
            if (manager.AdvanceYearBlocker == (Func<bool>)CanAdvanceYear)
                manager.AdvanceYearBlocker = null;
        }
        if (Instance == this) Instance = null;
        IsActive = false;
    }

    private void LoadData()
    {
        TextAsset rosterAsset = Resources.Load<TextAsset>("Compact30Roster");
        TextAsset siteAsset = Resources.Load<TextAsset>("Compact30Sites");
        if (rosterAsset == null || siteAsset == null)
        {
            dataProblem = "Compact30 candidate fixtures are missing from Resources.";
            return;
        }
        roster = JsonUtility.FromJson<Compact30RosterFile>(rosterAsset.text);
        sites = JsonUtility.FromJson<Compact30SiteFile>(siteAsset.text);
        if (!Compact30Rules.RosterIsValid(roster, out string rosterProblem))
        {
            dataProblem = rosterProblem;
            return;
        }
        if (sites == null || sites.sites == null || sites.sites.Count != 4)
        {
            dataProblem = "Compact30 must define exactly four G1 sites.";
            return;
        }
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (Compact30SiteRow site in sites.sites)
            if (site == null || !ids.Add(site.id))
            {
                dataProblem = "Compact30 site ids are missing or duplicated.";
                return;
            }
        if (sites.sites.Count(s => s.species == "sessile-oak") != 2
            || sites.sites.Count(s => s.species == "beech") != 2)
            dataProblem = "Compact30 G1 must be exactly two sessile oak and two European beech.";
    }

    private void Update()
    {
        if (manager == null || dataProblem.Length > 0) return;
        ReconcileGroup();
        UpdateJuvenileVisuals();
        if (manager.AnyPanelOpen || manager.ModalOpen) return;
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.f2Key.wasPressedThisFrame)
        {
            planningMethod = planningMethod == WorkExecutionMethod.Contractor
                ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor;
            Say("Planting method: " + MethodLabel(planningMethod) + ".");
        }
        if (keyboard.f3Key.wasPressedThisFrame)
        {
            planningShelter = !planningShelter;
            Say("Tree shelters for the next designation: " + (planningShelter ? "ON" : "OFF") + ".");
        }
        if (keyboard.jKey.wasPressedThisFrame)
        {
            if (!arrivalAcknowledged) Say("Acknowledge arrival at H1 first (press Y inside H1).");
            else
            {
                assessmentOpen = !assessmentOpen;
                if (assessmentOpen)
                {
                    assessmentViewed = true;
                    Say("Current stand assessment opened. Press Y to record that you assessed it.");
                }
            }
        }
        if (keyboard.yKey.wasPressedThisFrame)
        {
            if (assessmentOpen && arrivalAcknowledged) AcknowledgeAssessment();
            else AcknowledgeArrival();
        }
        if (keyboard.nKey.wasPressedThisFrame) RecordNoWorkDecision();

        if (keyboard.digit1Key.wasPressedThisFrame) TryDesignate(0);
        if (keyboard.digit2Key.wasPressedThisFrame) TryDesignate(1);
        if (keyboard.digit3Key.wasPressedThisFrame) TryDesignate(2);
        if (keyboard.digit4Key.wasPressedThisFrame) TryDesignate(3);
    }

    public void AcknowledgeArrival()
    {
        if (arrivalAcknowledged) { Say("Arrival is already acknowledged."); return; }
        if (!PlayerInH1()) { Say("Walk into the marked H1 arrival area, then press Y."); return; }
        arrivalAcknowledged = true;
        decisions.Add("Year " + Year + ": arrival at H1 acknowledged.");
        Say("Arrival acknowledged. Press J to open the current stand assessment.");
    }

    public void OpenAssessment()
    {
        if (!arrivalAcknowledged) { Say("Acknowledge arrival at H1 first (press Y inside H1)."); return; }
        assessmentOpen = true;
        assessmentViewed = true;
        Say("Current stand assessment opened. Press Y to record that you assessed it.");
    }

    public void AcknowledgeAssessment()
    {
        if (!arrivalAcknowledged) { Say("Acknowledge arrival first."); return; }
        if (!assessmentViewed) { Say("Open the current stand assessment with J, then press Y."); return; }
        if (assessmentAcknowledged) { Say("This stand assessment is already acknowledged."); return; }
        assessmentAcknowledged = true;
        decisions.Add("Year " + Year + ": current stand assessment acknowledged.");
        Say("Assessment acknowledged. Buy two Oak and two Beech saplings in the Work Plan, then designate sites 1-4.");
    }

    public void TryDesignate(int index)
    {
        if (sites == null || index < 0 || index >= sites.sites.Count) return;
        if (!arrivalAcknowledged || !assessmentAcknowledged)
        { Say("Both the H1 arrival and the current stand assessment must be acknowledged first."); return; }
        Compact30SiteRow site = sites.sites[index];
        Compact30GroupOrder existing = CurrentSiteRecord(site.id);
        if (existing != null)
        {
            if (existing.status == ScenarioWorkStatus.Completed && !existing.cancelled)
            { Say(site.id + " is already completed and cannot be replaced."); return; }
            if (existing.status == ScenarioWorkStatus.Approved && !existing.cancelled)
            { Say(site.id + " has Approved work. Cancel it in the Work Plan, then designate a replacement."); return; }
            if (existing.status == ScenarioWorkStatus.Pending && !existing.cancelled)
            {
                // A Pending choice may be corrected before approval. Remove it and create a fresh
                // Pending order with the current method/shelter and a fresh quote and id.
                if (!manager.RemovePendingOrder(existing.workOrderId))
                { Say("The existing Pending order could not be removed; no replacement was created."); return; }
                existing.superseded = true;
            }
        }
        int sameSpecies = group.Count(g => !g.cancelled && !g.superseded && g.speciesId == site.species
            && (IsOpen(g) || (g.status == ScenarioWorkStatus.Completed && g.juvenileId.Length > 0)));
        if (sameSpecies >= 2)
        { Say("The G1 group already holds two " + site.species + " sites."); return; }

        int available = manager.GetStockQuantity(site.stock_item) - manager.GetReservedStockQuantity(site.stock_item);
        if (available <= 0)
        { Say("No unreserved " + site.stock_item + " stock. Buy planting stock in the Work Plan."); return; }

        var stems = new List<Compact30Point>();
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && tree.IsLiving)
                stems.Add(new Compact30Point(tree.transform.position.x, tree.transform.position.z));
        var occupied = new List<Compact30Point>();
        foreach (Compact30GroupOrder record in group)
            if (!record.cancelled && !record.superseded && IsOpen(record))
                occupied.Add(new Compact30Point(Site(record.siteId).x_m, Site(record.siteId).z_m));
        foreach (PlantedJuvenile juvenile in manager.PlantedJuveniles)
            if (juvenile != null && juvenile.alive)
                occupied.Add(new Compact30Point(juvenile.position.x, juvenile.position.z));

        if (!Compact30Rules.ValidateSite(site, sites, stems, occupied, out string reason))
        { Say(reason); return; }

        var position = new Vector3(site.x_m, 0f, site.z_m);
        bool designated;
        manager.CompactSiteDesignationAuthorized = true;
        try
        {
            designated = manager.TryDesignateExactPlanting(site.stock_item, position, planningMethod, planningShelter);
        }
        finally
        {
            manager.CompactSiteDesignationAuthorized = false;
        }
        if (!designated)
        { Say(manager.Feedback); return; }

        ScenarioOneWorkOrder order = manager.WorkOrders
            .Where(o => o != null && o.type == ScenarioWorkType.PlantJuvenile && o.IsOpen
                && o.status == ScenarioWorkStatus.Pending && o.exactPosition
                && Mathf.Abs(o.worldPosition.x - site.x_m) < 0.0001f
                && Mathf.Abs(o.worldPosition.z - site.z_m) < 0.0001f && o.speciesId == site.species)
            .OrderByDescending(o => o.workOrderId).FirstOrDefault();
        if (order == null) { Say("The planting order was not created; no group slot was recorded."); return; }
        var entry = new Compact30GroupOrder
        {
            workOrderId = order.workOrderId,
            siteId = site.id,
            speciesId = site.species,
            stockItemId = site.stock_item,
            createdYear = order.createdYear,
            status = order.status
        };
        group.Add(entry);
        Say(site.id + " designated as Pending order #" + order.workOrderId
            + " (" + MethodLabel(planningMethod) + (planningShelter ? ", sheltered" : ", no shelter") + ").");
    }

    private void ReconcileGroup()
    {
        foreach (Compact30GroupOrder record in group)
        {
            ScenarioOneWorkOrder order = manager.WorkOrders.FirstOrDefault(o => o != null && o.workOrderId == record.workOrderId);
            if (order == null)
            {
                record.cancelled = true;
                continue;
            }
            record.status = order.status;
            record.resolvedYear = order.resolvedYear;
            if (order.status == ScenarioWorkStatus.Completed && record.juvenileId.Length == 0)
            {
                Compact30SiteRow site = Site(record.siteId);
                PlantedJuvenile juvenile = manager.PlantedJuveniles.FirstOrDefault(j => j != null
                    && j.speciesId == record.speciesId && j.plantingYear == order.resolvedYear
                    && Mathf.Abs(j.position.x - site.x_m) < 0.0001f
                    && Mathf.Abs(j.position.z - site.z_m) < 0.0001f);
                if (juvenile != null)
                {
                    record.juvenileId = juvenile.juvenileId;
                    decisions.Add("Year " + order.resolvedYear + ": " + record.siteId + " completed as "
                        + juvenile.juvenileId + " (" + record.speciesId + ").");
                }
            }
        }
    }

    private void OnAnnualReviewAcknowledged()
    {
        if (!GroupIsComplete()) return;
        int covering = CoveringYear;
        if (manager.CurrentEcologicalYear != covering)
        {
            Say("The covering Annual Review year is " + covering + "; no Compact credit was recorded.");
            return;
        }
        if (reviewAcknowledgedYear == covering) return;
        reviewAcknowledgedYear = covering;
        decisions.Add("Year " + covering + ": covering Annual Review acknowledged; T1 teaching complete.");
        Say("Covering Annual Review " + covering + " acknowledged.");
    }

    private bool CanAdvanceYear()
    {
        if (!GroupIsComplete()) return true;
        return reviewAcknowledgedYear == CoveringYear;
    }

    private void UpdateJuvenileVisuals()
    {
        if (manager == null) return;
        foreach (PlantedJuvenile juvenile in manager.PlantedJuveniles)
        {
            if (juvenile == null || string.IsNullOrEmpty(juvenile.juvenileId)) continue;
            bool visible = juvenile.alive && string.IsNullOrEmpty(juvenile.promotedTreeId);
            if (!visible)
            {
                if (juvenileVisuals.TryGetValue(juvenile.juvenileId, out GameObject hidden) && hidden != null)
                    hidden.SetActive(false);
                continue;
            }
            if (!juvenileVisuals.TryGetValue(juvenile.juvenileId, out GameObject visual) || visual == null)
            {
                visual = CreateJuvenileVisual(juvenile);
                juvenileVisuals[juvenile.juvenileId] = visual;
            }
            visual.SetActive(true);
            visual.transform.position = new Vector3(juvenile.position.x, 0f, juvenile.position.z);
            float height = Mathf.Max(0.2f, juvenile.heightMeters);
            Transform trunk = visual.transform.Find("Trunk");
            if (trunk != null)
            {
                trunk.localScale = new Vector3(0.035f, height * 0.5f, 0.035f);
                trunk.localPosition = new Vector3(0f, height * 0.5f, 0f);
            }
            Transform canopy = visual.transform.Find("Canopy");
            if (canopy != null)
            {
                canopy.localScale = Vector3.one * Mathf.Max(0.18f, height * 0.38f);
                canopy.localPosition = new Vector3(0f, height, 0f);
            }
        }
    }

    private GameObject CreateJuvenileVisual(PlantedJuvenile juvenile)
    {
        var root = new GameObject("Compact30 juvenile " + juvenile.juvenileId);
        Material material = JuvenileMaterial(juvenile.speciesId);
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        Collider trunkCollider = trunk.GetComponent<Collider>();
        if (trunkCollider != null) Destroy(trunkCollider);
        trunk.transform.SetParent(root.transform, false);
        trunk.GetComponent<Renderer>().sharedMaterial = material;
        GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        canopy.name = "Canopy";
        Collider canopyCollider = canopy.GetComponent<Collider>();
        if (canopyCollider != null) Destroy(canopyCollider);
        canopy.transform.SetParent(root.transform, false);
        canopy.GetComponent<Renderer>().sharedMaterial = material;
        return root;
    }

    private Material JuvenileMaterial(string speciesId)
    {
        Material existing = speciesId == "sessile-oak" ? oakJuvenileMaterial
            : speciesId == "beech" ? beechJuvenileMaterial : null;
        if (existing != null) return existing;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        if (shader == null) return null;
        var material = new Material(shader)
        {
            name = "Compact30 juvenile " + speciesId,
            color = speciesId == "sessile-oak" ? new Color(0.36f, 0.52f, 0.22f)
                : speciesId == "beech" ? new Color(0.28f, 0.42f, 0.20f)
                : new Color(0.30f, 0.46f, 0.24f)
        };
        if (speciesId == "sessile-oak") oakJuvenileMaterial = material;
        else if (speciesId == "beech") beechJuvenileMaterial = material;
        return material;
    }

    private void OnAuthoredTreeFelled(ForestTree tree)
    {
        if (tree == null || tree.GetComponent<Compact30AuthoredTree>() == null) return;
        Vector3 position = tree.transform.position;
        tree.gameObject.SetActive(false);
        CreateStump(position);
    }

    private void CreateStump(Vector3 position)
    {
        if (stumpMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) return;
            stumpMaterial = new Material(shader) { name = "Compact30 stump", color = new Color(0.38f, 0.28f, 0.18f) };
        }
        GameObject stump = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stump.name = "Compact30 stump";
        Collider collider = stump.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        stump.transform.position = new Vector3(position.x, 0.175f, position.z);
        stump.transform.localScale = new Vector3(0.3f, 0.175f, 0.3f);
        stump.GetComponent<Renderer>().sharedMaterial = stumpMaterial;
    }

    public void RecordNoWorkDecision()
    {
        if (!TeachingComplete) { Say("A deliberate no-work decision is recorded only after the first reviewed planting loop."); return; }
        if (manager.WorkOrders.Any(o => o != null && o.IsOpen))
        { Say("Resolve or cancel open work before recording a no-work year."); return; }
        if (manager.AnnualReports.Count == 0 || manager.AnnualReports[manager.AnnualReports.Count - 1].year != Year)
        { Say("Advance a real year first, then decide whether to act or deliberately not to act."); return; }
        if (lastNoWorkYear == Year) { Say("This year is already recorded as a deliberate no-work decision."); return; }
        lastNoWorkYear = Year;
        decisions.Add("Year " + Year + ": deliberate no-work decision after inspecting the changed stand.");
        Say("Year " + Year + " recorded as a deliberate no-work decision (advisory evidence, no completion credit).");
    }

    // ----- derived evidence ---------------------------------------------------------------------

    public bool GroupIsComplete()
    {
        int oak = 0, beech = 0;
        foreach (Compact30GroupOrder record in group)
        {
            if (record.cancelled || record.superseded) continue;
            if (record.status != ScenarioWorkStatus.Completed || record.juvenileId.Length == 0) continue;
            if (record.speciesId == "sessile-oak") oak++;
            else if (record.speciesId == "beech") beech++;
        }
        return oak == 2 && beech == 2;
    }

    public int CoveringYear
    {
        get
        {
            int year = -1;
            foreach (Compact30GroupOrder record in group)
                if (!record.cancelled && !record.superseded && record.status == ScenarioWorkStatus.Completed
                    && record.juvenileId.Length > 0 && record.resolvedYear > year)
                    year = record.resolvedYear;
            return year;
        }
    }

    public bool TeachingComplete => arrivalAcknowledged && assessmentAcknowledged
        && GroupIsComplete() && reviewAcknowledgedYear >= 0 && reviewAcknowledgedYear == CoveringYear;

    // The existing Annual Review acknowledge button is the one covering-review control. In Compact
    // this is true from the moment the final 2+2 completes until that exact year is acknowledged,
    // so the button is present and the Work Plan advance button is held.
    public bool CoveringReviewPending => GroupIsComplete() && reviewAcknowledgedYear != CoveringYear;

    private int Year => manager != null ? manager.CurrentEcologicalYear : 0;

    private Compact30GroupOrder CurrentSiteRecord(string siteId)
        => group.LastOrDefault(g => g.siteId == siteId && !g.superseded);

    private Compact30SiteRow Site(string siteId) => sites.sites.FirstOrDefault(s => s.id == siteId);

    private static bool IsOpen(Compact30GroupOrder record)
        => record.status == ScenarioWorkStatus.Pending || record.status == ScenarioWorkStatus.Approved;

    private bool PlayerInH1()
    {
        if (player == null) player = FindFirstObjectByType<ForestPlayer>();
        if (player == null || sites == null) return false;
        Vector3 position = player.transform.position;
        return Compact30Rules.InsideH1(position.x, position.z, sites.H1, 0f);
    }

    private static string MethodLabel(WorkExecutionMethod method)
        => method == WorkExecutionMethod.Contractor ? "contractor" : "me (landowner)";

    private void Say(string text)
    {
        message = text;
        messageUntil = Time.unscaledTime + 8f;
        Debug.Log("COMPACT30: " + text, this);
    }

    // ----- presentation -------------------------------------------------------------------------

    private int LivingSitkaCropTrees()
    {
        int count = 0;
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && tree.IsLiving && tree.IsCropTree && tree.Species != null && tree.Species.SpeciesId == "sitka-spruce")
                count++;
        return count;
    }

    private void OnGUI()
    {
        if (manager == null) return;
        if (manager.ModalOpen || manager.AnyPanelOpen) return;
        EnsureStyles();
        if (dataProblem.Length > 0)
        {
            GUI.Box(new Rect(14, 14, 520, 64), GUIContent.none, panelStyle);
            GUI.Label(new Rect(26, 24, 496, 48), "COMPACT30 DATA REFUSED\n" + dataProblem, bodyStyle);
            return;
        }

        float width = 470f;
        float height = 382f;
        var panel = new Rect(14, Screen.height - height - 14, width, height);
        GUI.Box(panel, GUIContent.none, panelStyle);
        float y = panel.y + 10;
        GUI.Label(new Rect(panel.x + 12, y, width - 24, 22), "COMPACT30 — MANAGEMENT LOOP (E2 CANDIDATE)", titleStyle);
        y += 24;
        y = Line(panel, y, "Year " + Year + "   Cash €" + (manager.CashCents / 100.0).ToString("0.00")
            + "   Stock O " + manager.GetStockQuantity("sessile-oak-sapling")
            + " / B " + manager.GetStockQuantity("beech-sapling"));
        y = Line(panel, y, "Arrival " + Tick(arrivalAcknowledged)
            + "   Assessment " + Tick(assessmentAcknowledged)
            + "   Crop Trees (living Sitka) " + LivingSitkaCropTrees() + "/3 guidance");
        y = Line(panel, y, StepText());
        y = Line(panel, y, "Next planting: " + MethodLabel(planningMethod)
            + (planningShelter ? ", sheltered" : ", no shelter") + "   [F2 method] [F3 shelter]");
        y += 2;
        for (int i = 0; i < sites.sites.Count; i++)
        {
            Compact30SiteRow site = sites.sites[i];
            Compact30GroupOrder record = CurrentSiteRecord(site.id);
            string state = record == null ? "ready" : record.cancelled ? "cancelled — replaceable"
                : record.superseded ? "superseded — replaceable"
                : record.status == ScenarioWorkStatus.Completed ? "COMPLETED " + record.juvenileId
                : record.status.ToString() + " #" + record.workOrderId;
            string clearance = Compact30Rules.PathClearance(new Compact30Point(site.x_m, site.z_m), sites).ToString("0.00");
            y = Line(panel, y, "[" + (i + 1) + "] " + site.label + "  cell " + site.cell
                + "  path clearance " + clearance + " m  — " + state);
        }
        if (message.Length > 0 && Time.unscaledTime < messageUntil)
            y = Line(panel, y, message);
        y = Line(panel, y, "Tab Work Plan/approve/advance   J assessment   Y acknowledge   N no-work decision");
        y = Line(panel, y, "SAVE INTEGRATION HELD — this E2 session is in memory; no save file is written.");
        y += 2;
        var history = manager.Interventions;
        for (int i = System.Math.Max(0, history.Count - 3); i < history.Count; i++)
        {
            InterventionSummary entry = history[i];
            y = Line(panel, y, "Year " + entry.Year + " — planted " + entry.Planted
                + ", pruned " + entry.Pruned + ", felled " + entry.Felled
                + ", failed " + entry.TasksFailed + ", net €" + (entry.NetCents / 100.0).ToString("0.00"));
        }

        if (!assessmentOpen) return;
        var box = new Rect(Screen.width * 0.5f - 310f, Screen.height * 0.5f - 210f, 620f, 420f);
        GUI.Box(box, GUIContent.none, panelStyle);
        float ay = box.y + 12;
        GUI.Label(new Rect(box.x + 16, ay, box.width - 32, 24), "Current stand assessment — year " + Year, titleStyle);
        ay += 30;
        ay = Line(box, ay, AssessmentText());
        ay += 6;
        ay = Line(box, ay, "Canopy mean light: " + MeanLight().ToString("0.000")
            + "   Background browse pressure: " + (ecology != null ? ecology.Browsing.BackgroundPressure.ToString("0.00") : "n/a"));
        ay = Line(box, ay, "Crop Trees: three living Sitka are the guidance target; choosing them is not a progression gate.");
        ay = Line(box, ay, "G1: two owned sessile oak and two owned European beech at the four fixed sites. No free placement.");
        ay = Line(box, ay, "Press Y to record that you assessed this current world/year. J closes.");
        if (message.Length > 0 && Time.unscaledTime < messageUntil)
            Line(box, ay, message);
    }

    private string AssessmentText()
    {
        int living = 0, sitka = 0;
        double dbh = 0, height = 0;
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || !tree.IsLiving) continue;
            living++;
            if (tree.Species != null && tree.Species.SpeciesId == "sitka-spruce")
            {
                sitka++;
                dbh += tree.Diameter;
                height += tree.Height;
            }
        }
        double meanDbh = sitka > 0 ? dbh / sitka : 0;
        double meanHeight = sitka > 0 ? height / sitka : 0;
        return "Living stems " + living + " (Sitka " + sitka + ")"
            + "   Mean Sitka DBH " + meanDbh.ToString("0.0") + " cm"
            + "   Mean height " + meanHeight.ToString("0.0") + " m"
            + "\nCash €" + (manager.CashCents / 100.0).ToString("0.00")
            + "   Oak stock " + manager.GetStockQuantity("sessile-oak-sapling")
            + "   Beech stock " + manager.GetStockQuantity("beech-sapling")
            + "   Completed years " + manager.AnnualReports.Count;
    }

    private float MeanLight()
    {
        if (ecology == null || ecology.Cells == null || ecology.Cells.Length == 0) return 0f;
        float sum = 0f;
        foreach (ForestEcologyCell cell in ecology.Cells) sum += cell.Light;
        return sum / ecology.Cells.Length;
    }

    public string Hint => StepText();

    private string StepText()
    {
        if (!arrivalAcknowledged) return "STEP 1: stand in H1 and press Y to acknowledge arrival.";
        if (!assessmentAcknowledged) return "STEP 2: press J to open the current stand assessment, then Y.";
        if (!GroupIsComplete())
        {
            int planted = group.Count(g => !g.cancelled && !g.superseded
                && g.status == ScenarioWorkStatus.Completed && g.juvenileId.Length > 0);
            return "STEP 3: designate 2 Oak + 2 Beech (1-4), approve in the Work Plan, then advance one year. "
                + "Linked plantings " + planted + "/4.";
        }
        if (reviewAcknowledgedYear != CoveringYear)
            return "STEP 4: open the year " + CoveringYear + " Annual Review and acknowledge it. Advance is held until then.";
        return "T1 teaching complete. Inspect the changed stand, plan later work, or press N for a deliberate no-work year. "
            + "Full Compact completion policy is not yet signed.";
    }

    private static string Tick(bool value) => value ? "✓" : "—";

    private float Line(Rect panel, float y, string text)
    {
        float height = text.IndexOf('\n') >= 0 ? 34f : 19f;
        GUI.Label(new Rect(panel.x + 12, y, panel.width - 24, height), text, bodyStyle);
        return y + height;
    }

    private void EnsureStyles()
    {
        if (panelStyle != null) return;
        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = MakeTexture(new Color(0.06f, 0.09f, 0.07f, 0.92f));
        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, wordWrap = true };
        titleStyle.normal.textColor = new Color(0.85f, 0.95f, 0.8f);
        bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true };
        bodyStyle.normal.textColor = Color.white;
    }

    private static Texture2D MakeTexture(Color color)
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
