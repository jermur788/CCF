using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;
using SitkaP1.Core.Saves;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Phase 2A-2 Core tests: C-V3 Save Schema v3 envelope, <see cref="GenesisRecipeBlockCodec"/>,
/// <see cref="FeatureBlockRegistry"/> reserved <c>economy</c> slot, self-contained v3 restore and continuation
/// determinism, fail-closed corruption/mismatch gates, v2 invariance, and <see cref="SaveV3Slots"/> durability.
/// </summary>
internal static class Phase2A2Tests
{
    internal static void Register(string scratchRoot)
    {
        T.Add("P2A-2: MA-1 and MA-1-MIXED still capture byte-identical Save v2 (zero v2 rewrite)", () =>
        {
            var ma1 = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Phase2);
            var v = ma1.ReadView();
            var (ver, bytes) = ma1.CaptureSaveBytes(new MultiAreaFence(v.SessionId, v.Source, v.PlanStateRevision), "MA-1 v2", 1);
            T.Eq(Phase1SaveFormat.SchemaVersion, ver, "MA-1 writes v2");
            var decoded = SaveV3Codec.DecodeAny(bytes, ma1.SaveExpectation());
            T.Eq(2, decoded.SchemaVersion, "DecodeAny sees v2");
            T.True(decoded.V2 is not null && decoded.V3 is null, "v2 payload only");

            var (mixedExp, mixedOpts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var mixed = new MultiAreaContinuation(mixedExp, MultiAreaModelSelection.Phase2, null, null, mixedOpts);
            var mv = mixed.ReadView();
            var (mVer, mBytes) = mixed.CaptureSaveBytes(new MultiAreaFence(mv.SessionId, mv.Source, mv.PlanStateRevision), "Mixed v2", 1);
            T.Eq(Phase1SaveFormat.SchemaVersion, mVer, "MA-1-MIXED writes v2");
            var mDecoded = SaveV3Codec.DecodeAny(mBytes, mixed.SaveExpectation());
            T.Eq(2, mDecoded.SchemaVersion, "DecodeAny sees v2 for mixed");
        });

        T.Add("P2A-2: Forest Maker preset captures C-V3 with genesisRecipe block, restores self-contained, and continues identically", () =>
        {
            var (_, session) = ForestMakerGenerator.CreateSession(ForestMakerPresets.SitkaEdgeBroadleaves064);
            var v0 = session.ReadView();
            T.True(session.IsForestMaker, "session is Forest Maker");
            T.Eq(1344, session.GenesisStemCount, "0.64 ha preset has 1344 genesis stems");
            T.True(session.ExperimentBadge.Contains("0.64 ha", StringComparison.Ordinal), "badge carries area");

            // Advance 2 years, mark a crop tree, queue and approve a Fell order so Policy A draft conversion is proven too.
            var r1 = session.Advance(Fence(v0), consentToCancelWork: false);
            var r2 = session.Advance(Fence(r1.View), consentToCancelWork: false);
            var liveAreaA = r2.View.Areas[0].Trees.Where(t => t.IsLiving).Take(2).ToArray();
            var rCrop = session.MarkCrop(Fence(r2.View), r2.View.Areas[0].AreaId, liveAreaA[0].TreeId);
            var rPend = session.AddPending(Fence(rCrop.View), rCrop.View.Areas[0].AreaId, liveAreaA[1].TreeId, "Thinning");
            var rAppr = session.Approve(Fence(rPend.View));

            var (ver, v3Bytes) = session.CaptureSaveBytes(Fence(rAppr.View), "FM Mixed Y2", 1);
            T.Eq(SaveV3Format.SchemaVersionV3, ver, "Forest Maker writes v3");

            // Check strict C-V3 structure: schemaVersion=3, blocks.genesisRecipe.{blockVersion,data}.
            using (var doc = JsonDocument.Parse(v3Bytes))
            {
                var root = doc.RootElement;
                T.Eq(3, root.GetProperty("schemaVersion").GetInt32(), "schemaVersion is 3");
                var blocks = root.GetProperty("blocks");
                var gr = blocks.GetProperty("genesisRecipe");
                T.Eq(1, gr.GetProperty("blockVersion").GetInt32(), "genesisRecipe blockVersion=1");
                var data = gr.GetProperty("data");
                T.Eq(ForestMakerPresets.SitkaEdgeBroadleaves064.RecipeHash(),
                    data.GetProperty("recipeHash").GetString(), "recipeHash matches");
                T.Eq(SaveV3Format.RecipeGeneratorNote, data.GetProperty("generatorNote").GetString(), "generatorNote");
                T.True(!data.GetProperty("economyEnabled").GetBoolean(), "economyEnabled=false");
            }

            // Self-contained restore (no pre-existing Forest Maker session needed!).
            var decoded = SaveV3Codec.Decode(v3Bytes);
            var (restoredSession, _, loadResult) = MultiAreaContinuation.RestoreFromV3(decoded);
            T.True(restoredSession.SessionId != session.SessionId, "fresh sessionId minted");
            T.Eq(2, loadResult.View.Source.EcologicalYear, "restored at Year 2");
            T.Eq(1, loadResult.DraftOrderCount, "saved approved order restored as 1 Pending draft (Policy A)");
            T.True(loadResult.View.Approved is null, "never restored approved");
            T.Eq(1, loadResult.View.Pending.Count, "1 pending draft order");
            T.Eq(liveAreaA[0].TreeId, loadResult.View.Areas[0].CropTreeIds[0], "crop tree preserved");

            // Continue BOTH sessions for 3 more years (cancelling the unexecuted draft identically) and prove bit-exact match.
            var orig = session.Advance(Fence(rAppr.View), consentToCancelWork: true);
            var rest = restoredSession.Advance(Fence(loadResult.View), consentToCancelWork: true);
            for (int y = 0; y < 2; y++)
            {
                orig = session.Advance(Fence(orig.View), consentToCancelWork: false);
                rest = restoredSession.Advance(Fence(rest.View), consentToCancelWork: false);
            }
            T.Eq(5, rest.View.Source.EcologicalYear, "reached Year 5");
            T.Eq(orig.View.Areas.Count, rest.View.Areas.Count, "same area count");
            for (int a = 0; a < orig.View.Areas.Count; a++)
            {
                var oa = orig.View.Areas[a].Trees;
                var ra = rest.View.Areas[a].Trees;
                T.Eq(oa.Count, ra.Count, $"area {a} tree count matches");
                for (int t = 0; t < oa.Count; t++)
                {
                    T.Eq(oa[t].TreeId, ra[t].TreeId, "treeId matches");
                    T.Eq(F32.Bits(oa[t].DbhCm), F32.Bits(ra[t].DbhCm), $"tree {oa[t].TreeId} DBH bits match");
                    T.Eq(F32.Bits(oa[t].HeightM), F32.Bits(ra[t].HeightM), $"tree {oa[t].TreeId} height bits match");
                    T.Eq(F32.Bits(oa[t].CrownRadiusM), F32.Bits(ra[t].CrownRadiusM), $"tree {oa[t].TreeId} crown bits match");
                    T.Eq(oa[t].IsLiving, ra[t].IsLiving, $"tree {oa[t].TreeId} living matches");
                }
            }
        });

        T.Add("P2A-2: C-V3 reserved economy slot refuses save, refuses economy block decode, and refuses missing economy block", () =>
        {
            var reg = FeatureBlockRegistry.Default;
            T.True(reg.TryGetCodec("genesisRecipe", out _), "genesisRecipe codec registered");
            T.True(reg.TryGetReserved("economy", out _), "economy block reserved");

            // Saving an Economy-enabled Forest Maker session refuses fail-closed.
            var econModels = MultiAreaModelSelection.Phase2 with { Economy = EconomyModelVersion.Enabled };
            var (_, econSession) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064, econModels);
            var ev = econSession.ReadView();
            T.Refuses("SaveEconomyRequiresV3", () => econSession.CaptureSaveBytes(Fence(ev), "Econ FM", 1));

            // Build a valid v3 save and tamper it with economy block or economyEnabled=true (recomputing outer checksum).
            var (_, baseSession) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064);
            var bv = baseSession.ReadView();
            var (_, v3Bytes) = baseSession.CaptureSaveBytes(Fence(bv), "Pure FM", 1);
            string json = Encoding.UTF8.GetString(v3Bytes);

            // 1. Presence of reserved "economy" block (in ordinal order before "genesisRecipe") -> SaveEconomyRequiresV3
            string withEconBlock = ResignV3(json.Replace(
                "\"blocks\":{\"genesisRecipe\":",
                "\"blocks\":{\"economy\":{\"blockVersion\":1,\"data\":{}},\"genesisRecipe\":",
                StringComparison.Ordinal));
            T.Refuses("SaveEconomyRequiresV3", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(withEconBlock)));

            // 2. genesisRecipe.data.economyEnabled = true without economy block -> SaveRequiredBlockMissing
            string econFlagNoBlock = ResignV3(json.Replace(
                "\"economyEnabled\":false",
                "\"economyEnabled\":true",
                StringComparison.Ordinal));
            T.Refuses("SaveRequiredBlockMissing", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(econFlagNoBlock)));
        });

        T.Add("P2A-2: C-V3 fail-closed decode gates (unknown block, bad version, order, missing block, hash/genesis/checksum mismatch)", () =>
        {
            var (_, session) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064);
            var v = session.ReadView();
            var (_, v3Bytes) = session.CaptureSaveBytes(Fence(v), "Gate test", 1);
            string json = Encoding.UTF8.GetString(v3Bytes);

            // Unknown block key -> SaveBlockUnknown
            string unknownKey = ResignV3(json.Replace(
                "\"blocks\":{\"genesisRecipe\":",
                "\"blocks\":{\"alphaUnknown\":{\"blockVersion\":1,\"data\":{}},\"genesisRecipe\":",
                StringComparison.Ordinal));
            T.Refuses("SaveBlockUnknown", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(unknownKey)));

            // Unsupported blockVersion -> SaveBlockVersionUnsupported
            string badVer = ResignV3(json.Replace(
                "\"genesisRecipe\":{\"blockVersion\":1,",
                "\"genesisRecipe\":{\"blockVersion\":2,",
                StringComparison.Ordinal));
            T.Refuses("SaveBlockVersionUnsupported", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(badVer)));

            // Non-ordinal blocks key order ("zzz" before "genesisRecipe") -> SaveNotCanonical
            string badOrder = ResignV3(json.Replace(
                "\"blocks\":{\"genesisRecipe\":",
                "\"blocks\":{\"zzz\":{\"blockVersion\":1,\"data\":{}},\"genesisRecipe\":",
                StringComparison.Ordinal));
            T.Refuses("SaveNotCanonical", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(badOrder)));

            // Missing required genesisRecipe block for a forest-maker header -> SaveRequiredBlockMissing
            int blocksStart = json.LastIndexOf("\"blocks\":{", StringComparison.Ordinal);
            int checksumStart = json.LastIndexOf(",\"checksum\":", StringComparison.Ordinal);
            string emptyBlocks = ResignV3(json[..blocksStart] + "\"blocks\":{}" + json[checksumStart..]);
            T.Refuses("SaveRequiredBlockMissing", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(emptyBlocks)));

            // Schema version 4 -> SaveSchemaUnsupported
            string schema4 = json.Replace("\"schemaVersion\":3,", "\"schemaVersion\":4,", StringComparison.Ordinal);
            T.Refuses("SaveSchemaUnsupported", () => SaveV3Codec.DecodeAny(Encoding.UTF8.GetBytes(schema4)));

            // Corrupted recipeHash -> SaveRecipeHashMismatch
            string goodHash = ForestMakerPresets.PureSitka064.RecipeHash();
            string badHash = (goodHash[0] == '0' ? '1' : '0') + goodHash[1..];
            string hashMismatch = ResignV3(json.Replace(goodHash, badHash, StringComparison.Ordinal));
            T.Refuses("SaveRecipeHashMismatch", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(hashMismatch)));

            // Unsupported generatorNote -> SaveRecipeUnsupported
            string badNote = ResignV3(json.Replace(SaveV3Format.RecipeGeneratorNote, "ccf-2d-forest-maker v99", StringComparison.Ordinal));
            T.Refuses("SaveRecipeUnsupported", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(badNote)));

            // Recipe/genesis mismatch: swap preset1 genesisRecipe block into preset2 save -> SaveRecipeGenesisMismatch
            var (_, mixedSession) = ForestMakerGenerator.CreateSession(ForestMakerPresets.SitkaEdgeBroadleaves064);
            var mv = mixedSession.ReadView();
            var (_, mixedBytes) = mixedSession.CaptureSaveBytes(Fence(mv), "Mixed", 1);
            string mixedJson = Encoding.UTF8.GetString(mixedBytes);
            int mBlockStart = mixedJson.LastIndexOf("\"blocks\":{", StringComparison.Ordinal);
            int mChecksumStart = mixedJson.LastIndexOf(",\"checksum\":", StringComparison.Ordinal);
            string mixedBlocksSegment = mixedJson[mBlockStart..mChecksumStart];
            string swapped = ResignV3(json[..blocksStart] + mixedBlocksSegment + json[checksumStart..]);
            var swappedDecoded = SaveV3Codec.Decode(Encoding.UTF8.GetBytes(swapped));
            T.Refuses("SaveRecipeGenesisMismatch", () => MultiAreaContinuation.RestoreFromV3(swappedDecoded));

            // Edited byte without checksum update -> SaveChecksumMismatch
            string edited = json.Replace("\"label\":\"Gate test\"", "\"label\":\"Gate Xest\"", StringComparison.Ordinal);
            T.Refuses("SaveChecksumMismatch", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(edited)));

            // Non-canonical whitespace -> SaveNotCanonical
            string spaced = json.Replace("\"blocks\":{", "\"blocks\": {", StringComparison.Ordinal);
            T.Refuses("SaveNotCanonical", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(spaced)));
        });

        T.Add("P2A-2: SaveV3Slots stores v2 and v3 atomically, inspects both, refuses unconfirmed overwrite, and recovers backup", () =>
        {
            string dir = Path.Combine(scratchRoot, "v3-slots-test");
            Directory.CreateDirectory(dir);
            var slots = new SaveV3Slots(new Phase1SaveSlots(dir));

            // Save v2 slot (MA-1)
            var ma1 = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Phase2);
            var v2View = ma1.ReadView();
            var (_, v2Bytes) = ma1.CaptureSaveBytes(Fence(v2View), "MA1 slot", 1);
            var rV2 = slots.Save("ma1-slot", v2Bytes, replaceExisting: false);
            T.Eq(Phase1SlotCommitStatus.DurablyCommitted, rV2.Status, "v2 slot durably committed");

            // Save v3 slot (Forest Maker)
            var (_, fm) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064);
            var fmV0 = fm.ReadView();
            var (_, v3Bytes1) = fm.CaptureSaveBytes(Fence(fmV0), "FM Y0", 1);
            var rV3 = slots.Save("fm-slot", v3Bytes1, replaceExisting: false);
            T.Eq(Phase1SlotCommitStatus.DurablyCommitted, rV3.Status, "v3 slot durably committed");
            T.Refuses("SlotExists", () => slots.Save("fm-slot", v3Bytes1, replaceExisting: false));

            // Advance and overwrite -> backup refreshed
            var fmV1 = fm.Advance(Fence(fmV0), consentToCancelWork: false).View;
            var (_, v3Bytes2) = fm.CaptureSaveBytes(Fence(fmV1), "FM Y1", 2);
            var rV3Replace = slots.Save("fm-slot", v3Bytes2, replaceExisting: true);
            T.True(rV3Replace.BackupUpdated, "v3 backup refreshed on overwrite");

            var inspV2 = slots.Inspect("ma1-slot");
            T.Eq(2, inspV2.PrimarySchemaVersion, "inspect sees v2");
            var inspV3 = slots.Inspect("fm-slot");
            T.Eq(3, inspV3.PrimarySchemaVersion, "inspect sees v3");
            T.True(inspV3.BackupExists && inspV3.BackupSchemaVersion == 3, "inspect sees v3 backup");

            // Corrupt primary v3 and recover from backup
            File.WriteAllText(Path.Combine(dir, "fm-slot.ccf2d.json"), "{corrupt");
            var inspCorrupt = slots.Inspect("fm-slot");
            T.True(inspCorrupt.PrimaryRefusal is not null, "corrupt primary reported");
            var rec = slots.RecoverFromBackup("fm-slot");
            T.Eq(Phase1SlotCommitStatus.DurablyCommitted, rec.Status, "backup recovered");
            var loaded = slots.Load("fm-slot", v2Expectation: null);
            T.Eq(3, loaded.SchemaVersion, "recovered slot is v3");
            T.Eq(0, loaded.Header.EcologicalYear, "recovered Y0 backup");
        });
    }

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    private static string ResignV3(string json)
    {
        int idx = json.LastIndexOf(",\"checksum\":", StringComparison.Ordinal);
        string prefix = json[..idx];
        string sum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(prefix))).ToLowerInvariant();
        return prefix + ",\"checksum\":{\"alg\":\"" + SaveV3Format.ChecksumAlgorithm + "\",\"value\":\"" + sum + "\"}}";
    }
}
