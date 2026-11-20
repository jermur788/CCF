using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable author gate. No scene/save writes. Copy to Assets/ForestPrototype only for a leased run.
public static class Sen1CompactCollisionVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity");
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetEnvironmentVariable("SEN1_COLLISION_OUT") == null) return;
        new GameObject("#886 disposable collision gate").AddComponent<Sen1CompactCollisionRunner>();
    }
}

public sealed class Sen1CompactCollisionRunner : MonoBehaviour
{
    private readonly List<string> log = new List<string>();
    private readonly List<string> rows = new List<string> { "id,dbhCm,height,scaleX,scaleY,scaleZ,colliders,worldRadius,boundsHeight,baseY,centerX,centerZ,rootX,rootZ,rendererBaseY" };
    private int checks, failures;
    private bool Baseline => Environment.GetEnvironmentVariable("SEN1_COLLISION_BASELINE") == "1";
    private void Check(bool ok, string message) { checks++; if (!ok) failures++; log.Add((ok ? "PASS " : "FAIL ") + message); }
    private void Info(string message) => log.Add("INFO " + message);
    private IEnumerator Start()
    {
        Exception crash = null;
        IEnumerator test = Test();
        while (true)
        {
            bool more = false; object current = null;
            try { more = test.MoveNext(); if (more) current = test.Current; }
            catch (Exception e) { crash = e; }
            if (crash != null || !more) break;
            yield return current;
        }
        if (crash != null) { failures++; log.Add("FAIL " + crash); }
        string verdict = "SEN1_COLLISION_" + (failures == 0 ? "PASS" : "FAIL") + " checks=" + checks + " failures=" + failures + " baseline=" + Baseline;
        string output = Environment.GetEnvironmentVariable("SEN1_COLLISION_OUT");
        Directory.CreateDirectory(output);
        File.WriteAllLines(Path.Combine(output, "colliders.csv"), rows);
        File.WriteAllLines(Path.Combine(output, "verdict.txt"), log.Concat(new[] { verdict }));
        foreach (string line in log) Debug.Log("SEN1_COLLISION " + line);
        Debug.Log(verdict);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failures == 0 ? 0 : 1);
    }
    private IEnumerator Test()
    {
        for (int i = 0; i < 60; i++) yield return null;
        var trees = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.GetComponent<Compact30AuthoredTree>() != null).OrderBy(t => t.TreeId).ToArray();
        Check(trees.Length == 175 && trees.Select(t => t.TreeId).Distinct().Count() == 175, "175 unique authored trees");
        Check(trees.All(t => t.IsLiving && t.MarkType == TreeMarkType.None), "initially living and unmarked");
        Check(UnityEngine.Object.FindFirstObjectByType<ForestSaveController>() == null, "no save controller in disposable Compact world");
        var player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        var controller = player.GetComponent<CharacterController>();
        Vector3 spawn = player.transform.position;
        player.enabled = false; // Test drives the real authored CharacterController.Move, with deterministic steps.
        Info("player radius=" + controller.radius + " height=" + controller.height + " step=" + controller.stepOffset + " skin=" + controller.skinWidth);
        Physics.SyncTransforms();
        foreach (ForestTree tree in trees)
        {
            Collider[] all = tree.GetComponentsInChildren<Collider>();
            CapsuleCollider cap = all.OfType<CapsuleCollider>().FirstOrDefault();
            Check(all.Length == 1 && cap != null && cap.enabled && !cap.isTrigger && cap.gameObject.layer == tree.gameObject.layer,
                tree.TreeId + " exactly one enabled solid same-layer capsule (no decorative colliders)");
            if (cap == null) continue;
            Bounds b = cap.bounds; Vector3 s = tree.transform.lossyScale;
            float r = Mathf.Max(b.extents.x, b.extents.z);
            float renderBase = tree.GetComponentsInChildren<Renderer>().Min(x => x.bounds.min.y);
            rows.Add(string.Join(",", tree.TreeId, tree.Diameter, tree.Height, s.x, s.y, s.z, all.Length, r, b.size.y, b.min.y,
                b.center.x, b.center.z, tree.transform.position.x, tree.transform.position.z, renderBase));
            Check(Mathf.Abs(r - tree.Diameter / 200f) < 0.002f && Mathf.Abs(b.size.y - Mathf.Max(1.2f, tree.Height)) < 0.01f
                && Mathf.Abs(b.min.y - tree.transform.position.y) < 0.002f
                && Vector2.Distance(new Vector2(b.center.x,b.center.z), new Vector2(tree.transform.position.x,tree.transform.position.z)) < 0.002f,
                tree.TreeId + " world radius/height/base/centre match authoritative stem");
        }
        var sorted = trees.OrderBy(t => t.transform.lossyScale.y).ToArray();
        var samples = new[] { sorted[0], sorted[sorted.Length/2], sorted[sorted.Length-1], trees[30], trees[100] }.Distinct().ToArray();
        Collider[] treeColliders = trees.SelectMany(t => t.GetComponentsInChildren<Collider>()).ToArray();
        // Isolate each real scene stem to attribute contact to it; restore every collider afterwards.
        foreach (Collider c in treeColliders) c.enabled = false;
        foreach (ForestTree tree in samples)
        {
            CapsuleCollider cap = tree.GetComponentInChildren<CapsuleCollider>(); cap.enabled = true;
            Physics.SyncTransforms();
            Vector3 center = tree.transform.position;
            Teleport(controller, center + Vector3.right * 5f);
            for (int i = 0; i < 280; i++) controller.Move(new Vector3(-0.025f, -0.01f, 0));
            float contact = Vector2.Distance(new Vector2(player.transform.position.x,player.transform.position.z),new Vector2(center.x,center.z));
            Info("CONTACT " + tree.TreeId + " rootScale=" + tree.transform.lossyScale.y + " dbh=" + tree.Diameter + " distance=" + contact + " radius=" + cap.bounds.extents.x);
            Check(Mathf.Abs(contact - (tree.Diameter / 200f + controller.radius)) < controller.skinWidth + 0.025f,
                tree.TreeId + " CharacterController stops at trunk plus body radius");
            // Aim at breast height: both interaction reaches should select this exact stem.
            Vector3 eye = center + new Vector3(2f, 1.3f, 0f);
            Check(AimedTree(eye, Vector3.left, 3.5f) == tree,
                tree.TreeId + " 3.5m inspection ray resolves same stem");
            Check(AimedTree(center + new Vector3(7f,1.3f,0f), Vector3.left, 8f) == tree,
                tree.TreeId + " 8m marking ray resolves same stem");
            cap.enabled = false;
        }
        foreach (Collider c in treeColliders) c.enabled = true;
        Physics.SyncTransforms();
        if (Baseline) yield break;

        // Real in-scene travel with all authored colliders enabled, in multiple clear between-stem gaps.
        int walks = 0;
        foreach (ForestTree tree in samples)
        {
            Vector3 from = tree.transform.position + new Vector3(-1f,0,0.8f);
            Vector3 to = from + Vector3.right * 2f;
            // Only use lanes demonstrably clear for the body radius, including scenery.
            if (Physics.CheckCapsule(from+Vector3.up*0.35f,from+Vector3.up*1.45f,0.32f,~0,QueryTriggerInteraction.Ignore)) continue;
            if (Physics.CapsuleCast(from+Vector3.up*0.35f,from+Vector3.up*1.45f,0.32f,Vector3.right,2f,~0,QueryTriggerInteraction.Ignore)) continue;
            Teleport(controller, from);
            for(int i=0;i<80;i++) controller.Move(new Vector3(0.025f,-0.01f,0));
            Check(Vector2.Distance(new Vector2(player.transform.position.x,player.transform.position.z),new Vector2(to.x,to.z)) < 0.06f,
                tree.TreeId + " real between-tree travel without distant blocking"); walks++;
        }
        Check(walks >= 3,"at least three unobstructed natural scene walks ("+walks+")");
        var marking = UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        var manager = UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>();
        for (int i=0;i<3;i++) { marking.ToggleCropTree(trees[i]); marking.ToggleCropTree(trees[i]); marking.ToggleCropTree(trees[i]); }
        yield return null;
        Check(manager.CropTreeCount == 3, "C toggle/unmark/remark leaves three Crop Trees");
        ForestTree fell = trees[10]; marking.ToggleMark(fell); yield return null;
        Check(fell.IsMarkedForFell, "X designates separate Fell tree");
        marking.ToggleMark(fell); marking.ToggleMark(fell);
        Check(fell.IsMarkedForFell,"X unmark/remark still works");
        Vector3 stumpPosition = fell.transform.position;
        Teleport(controller, spawn);
        var session = UnityEngine.Object.FindFirstObjectByType<Compact30Session>();
        session.AcknowledgeArrival(); session.OpenAssessment(); session.AcknowledgeAssessment();
        Check(session.ArrivalAcknowledged && session.AssessmentAcknowledged,"real session assessment ready");
        Check(manager.ApprovePendingWork(),"approve real Fell work");
        Check(manager.AdvanceYear(),"Year1 resolves real Fell work");
        yield return null;
        Check(manager.CurrentEcologicalYear == 1 && !fell.IsLiving && fell.IsStump,"Year1 felling leaves nonliving identity");
        Check(manager.WorkOrders.Count(o=>o.type==ScenarioWorkType.FellTree && o.targetTreeId==fell.TreeId && o.status==ScenarioWorkStatus.Completed)==1,"one completed Fell order");
        Check(fell.GetComponentsInChildren<Collider>().All(c=>!c.enabled),"old living obstruction disabled after felling");
        var stump = GameObject.Find("Compact30 stump " + fell.TreeId);
        Check(stump != null,"visible replacement stump exists");
        Collider stumpCollider = stump != null ? stump.GetComponent<Collider>() : null;
        Renderer stumpRenderer = stump != null ? stump.GetComponent<Renderer>() : null;
        Check(stumpCollider != null && stumpCollider.enabled && !stumpCollider.isTrigger,"stump has solid collider");
        if(stumpCollider != null)
        {
            Info("STUMP renderer="+stumpRenderer.bounds+" collider="+stumpCollider.bounds);
            Check(Vector3.Distance(stumpCollider.bounds.center,stumpRenderer.bounds.center)<0.002f && Vector3.Distance(stumpCollider.bounds.size,stumpRenderer.bounds.size)<0.002f,"stump physical bounds match visible mesh bounds");
            // Isolate stump only for attributed contact and step test.
            foreach(Collider c in treeColliders) if(c!=null)c.enabled=false;
            Teleport(controller,stumpPosition+Vector3.right);
            float highest=0f;
            for(int i=0;i<100;i++){controller.Move(new Vector3(-0.025f,-0.01f,0));highest=Mathf.Max(highest,controller.transform.position.y);}
            Info("STUMP traversal xDelta="+(controller.transform.position.x-stumpPosition.x)+" highestFeet="+highest);
            Check(controller.transform.position.x < stumpPosition.x-0.5f && highest>=stumpRenderer.bounds.max.y-0.05f,"real controller steps across visible low stump rather than through wood");
            foreach(Collider c in treeColliders) if(c!=null && c.GetComponentInParent<ForestTree>().IsLiving)c.enabled=true;
        }
        fell.Fell(); yield return null;
        Check(UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Count(t=>t.name=="Compact30 stump "+fell.TreeId)==1,"repeat Fell creates no duplicate stump");
        Check(!Physics.Raycast(stumpPosition+new Vector3(0,1.3f,0),Vector3.right,0.1f),"no old trunk collider at breast height after Fell");
        ForestTree dead = trees[20];
        Check(dead.ApplyMortality("#886 disposable negative witness", 1), "real biological nonliving transition accepted");
        Physics.SyncTransforms();
        Check(!dead.gameObject.activeInHierarchy && dead.GetComponentsInChildren<Collider>(true).All(c => c.bounds.size == Vector3.zero), "nonliving authored stem has no active physical obstruction");
        Check(trees.Where(t=>t.IsLiving).All(t=>t.GetComponentsInChildren<Collider>().Count(c=>c.enabled)==1),"surviving stems retain one collider after annual step");
    }
    private static ForestTree AimedTree(Vector3 origin, Vector3 direction, float reach)
    {
        // Match production's self-collider exclusion before taking the nearest hit.
        var hits = Physics.RaycastAll(origin, direction, reach).Where(h => h.collider.GetComponentInParent<ForestPlayer>() == null).OrderBy(h => h.distance).ToArray();
        return hits.Length == 0 ? null : hits[0].collider.GetComponentInParent<ForestTree>();
    }
    private static void Teleport(CharacterController controller, Vector3 point)
    {
        controller.enabled=false; controller.transform.position=point; controller.enabled=true; Physics.SyncTransforms();
    }
}
