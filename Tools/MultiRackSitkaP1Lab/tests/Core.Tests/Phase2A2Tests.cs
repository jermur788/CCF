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

        T.Add("P2B-2b: C-V3 economy block registered, Forest Maker Economy saves & restores deterministically, built-in refuses, tampered/missing economy block fails closed", () =>
        {
            var reg = FeatureBlockRegistry.Default;
            T.True(reg.TryGetCodec("genesisRecipe", out _), "genesisRecipe codec registered");
            T.True(reg.TryGetCodec("economy", out var econCodec) && econCodec is not null && econCodec.CurrentBlockVersion == 1, "economy v1 codec registered");
            T.True(!reg.TryGetReserved("economy", out _), "economy is no longer reserved in default registry");

            // Reserved-slot mechanism still fails closed on a custom registry that reserves economy.
            var customReg = new FeatureBlockRegistry();
            customReg.Register(new GenesisRecipeBlockCodec());
            customReg.Reserve(new ReservedFeatureBlockSlot("economy", "SaveEconomyRequiresV3", "reserved in custom registry"));
            T.True(customReg.TryGetReserved("economy", out _), "custom reserved slot still works");

            // Built-in stand (MA-1-MIXED) + Economy still refuses Save v2/v3 with SaveEconomyRequiresV3.
            var econModels = MultiAreaModelSelection.Phase2 with { Economy = EconomyModelVersion.Enabled };
            var (mixedExp, mixedP2, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var builtInEcon = new MultiAreaContinuation(mixedExp, econModels, null, null, mixedP2);
            T.Refuses("SaveEconomyRequiresV3", () => builtInEcon.CaptureSaveBytes(Fence(builtInEcon.ReadView()), "BuiltIn Econ", 1));

            // Forest Maker + Economy: execute mixed Sell + Keep + Leave plan + Landowner Clear + Advance, then Save v3 -> RestoreFromV3.
            var (_, econSession) = ForestMakerGenerator.CreateSession(ForestMakerPresets.SitkaEdgeBroadleaves064, econModels);
            var ev = econSession.ReadView();
            var area0 = ev.Areas[0];
            var live = area0.Trees.Where(t => t.IsLiving).Take(3).ToArray();
            ev = econSession.AddPending(Fence(ev), area0.AreaId, live[0].TreeId, "sell stem").View;
            ev = econSession.AddPending(Fence(ev), area0.AreaId, live[1].TreeId, "keep stem").View;
            ev = econSession.AddPending(Fence(ev), area0.AreaId, live[2].TreeId, "deadwood stem").View;
            ev = econSession.SetFellingDisposition(Fence(ev), area0.AreaId, live[1].TreeId, FellingDisposition.KeepForUse).View;
            ev = econSession.SetFellingDisposition(Fence(ev), area0.AreaId, live[2].TreeId, FellingDisposition.LeaveAsDeadwood).View;
            ev = econSession.AddClearUndergrowth(Fence(ev), area0.AreaId, 0, 0).View;
            ev = econSession.SetOrderExecutor(Fence(ev), EconomyOrderKey.ForClear(area0.AreaId, 0, 0), WorkExecutor.Landowner).View;
            ev = econSession.Approve(Fence(ev)).View;
            var exec = econSession.Execute(Fence(ev), ev.Approved!.PlanId, ev.Approved.ExecutionKey);
            T.True(exec.RefusalCode is null, "mixed economy plan executed: " + exec.RefusalCode);
            ev = econSession.Advance(Fence(exec.View), consentToCancelWork: false).View;

            // Add a pending Fell with a non-default disposition AND approve to verify Policy A (approval dropped, PendingFellDispositions empty on load).
            var nextLive = ev.Areas[0].Trees.First(t => t.IsLiving);
            ev = econSession.AddPending(Fence(ev), area0.AreaId, nextLive.TreeId, "pending across save").View;
            ev = econSession.SetFellingDisposition(Fence(ev), area0.AreaId, nextLive.TreeId, FellingDisposition.KeepForUse).View;
            ev = econSession.Approve(Fence(ev)).View;

            var econBefore = econSession.ReadEconomyView();
            var (schemaVer, econV3Bytes) = econSession.CaptureSaveBytes(Fence(ev), "FM Econ Y1", 1);
            T.Eq(3, schemaVer, "Forest Maker Economy saves as schema v3");
            var decoded = SaveV3Codec.Decode(econV3Bytes);
            T.True(decoded.GenesisRecipe is not null && decoded.GenesisRecipe.EconomyEnabled, "genesisRecipe records economyEnabled=true");
            T.True(decoded.Envelope.Blocks.ContainsKey("economy"), "v3 save contains blocks.economy");

            var (restoredEcon, _, loadResult) = MultiAreaContinuation.RestoreFromV3(decoded);
            T.True(loadResult.RefusalCode is null, "v3 economy restore succeeded: " + loadResult.RefusalCode);
            T.True(restoredEcon.SelectedModels.IsEconomyEnabled, "restored session has Economy enabled");
            T.True(loadResult.View.Approved is null, "Policy A: approved plan is dropped to pending on load");
            var econAfter = restoredEcon.ReadEconomyView();
            T.Eq(econBefore.CashBalanceCents, econAfter.CashBalanceCents, "cash balance restored");
            T.Eq(econBefore.UsedLandownerMinutesThisYear, econAfter.UsedLandownerMinutesThisYear, "used landowner minutes restored");
            T.Eq(econBefore.RemainingLandownerMinutes, econAfter.RemainingLandownerMinutes, "remaining landowner minutes restored");
            T.Eq(econBefore.ExecutedHarvestPlansCount, econAfter.ExecutedHarvestPlansCount, "thinning stage count restored");
            T.Eq(econBefore.RetainedTimber.RetainedTimberCm3, econAfter.RetainedTimber.RetainedTimberCm3, "retained timber cm3 restored");
            T.Eq(econBefore.RetainedTimber.BySpecies.Count, econAfter.RetainedTimber.BySpecies.Count, "retained species count restored");
            T.Eq(econBefore.Settlements.Count, econAfter.Settlements.Count, "settlements restored");
            T.Eq(econBefore.Ledger.Count, econAfter.Ledger.Count, "ledger restored");
            T.Eq(econBefore.YearEndSummaries.Count, econAfter.YearEndSummaries.Count, "year-end summaries restored");
            T.Eq(0, econAfter.PendingFellDispositions.Count, "pending fell dispositions start empty on restored orders");
            T.True(restoredEcon.ReadUiSnapshot().Readout.Areas[0].Canopy.FallenDeadwood.Any(d => d.TreeId == live[2].TreeId && d.Cause == FellCause.Harvest),
                "felled LeaveAsDeadwood DW record survived Save v3 -> Restore");

            // Continue both sessions deterministically for 2 more years.
            var oStep = econSession.Advance(Fence(ev), consentToCancelWork: true);
            var rStep = restoredEcon.Advance(Fence(loadResult.View), consentToCancelWork: true);
            oStep = econSession.Advance(Fence(oStep.View), consentToCancelWork: false);
            rStep = restoredEcon.Advance(Fence(rStep.View), consentToCancelWork: false);
            T.Eq(econSession.ReadEconomyView().CashBalanceCents, restoredEcon.ReadEconomyView().CashBalanceCents, "cash identical after 2 years");
            T.Eq(econSession.ReadEconomyView().YearEndSummaries.Count, restoredEcon.ReadEconomyView().YearEndSummaries.Count, "year-end count identical");

            // Mismatched EconomyOptions must refuse with SaveOptionsMismatch.
            var otherOpts = EconomyOptions.Default with { OpeningCashCents = 500_000L };
            var generated = ForestMakerGenerator.Generate(ForestMakerPresets.SitkaEdgeBroadleaves064);
            var otherSession = new MultiAreaContinuation(generated.Experiment, econModels, stage4: null, stage5: null, phase2: generated.Options, economy: otherOpts, genesisRecipe: generated.Recipe);
            T.Refuses("SaveOptionsMismatch", () => otherSession.RestoreVerifiedV3IntoFreshSession(decoded, FeatureBlockRegistry.Default));

            // Tampered / fail-closed economy v3 saves.
            string econJson = Encoding.UTF8.GetString(econV3Bytes);
            string badEconVer = ResignV3(econJson.Replace("\"economy\":{\"blockVersion\":1,", "\"economy\":{\"blockVersion\":2,", StringComparison.Ordinal));
            T.Refuses("SaveBlockVersionUnsupported", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(badEconVer)));

            string badCash = ResignV3(econJson.Replace("\"cashBalanceCents\":{\"k\":\"i64\",\"b\":\"", "\"cashBalanceCents\":{\"k\":\"i64\",\"b\":\"0000000000000001\"}\"x\":\"", StringComparison.Ordinal));
            T.True(badCash != econJson, "mutated json");
            string tamperedCashHex = econJson;
            int cashIdx = tamperedCashHex.IndexOf("\"cashBalanceCents\":{\"k\":\"i64\",\"b\":\"", StringComparison.Ordinal);
            int hexStart = cashIdx + "\"cashBalanceCents\":{\"k\":\"i64\",\"b\":\"".Length;
            char flipped = tamperedCashHex[hexStart + 15] == '0' ? '1' : '0';
            string cashMismatch = ResignV3(tamperedCashHex[..(hexStart + 15)] + flipped + tamperedCashHex[(hexStart + 16)..]);
            T.Refuses("SaveInconsistent", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(cashMismatch)));

            string extraKey = ResignV3(econJson.Replace("\"yearEndSummaries\":[", "\"extraKey\":1,\"yearEndSummaries\":[", StringComparison.Ordinal));
            T.Refuses("UnknownKey", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(extraKey)));

            // Flipping economyEnabled=false on an economy-block save -> SaveInconsistent
            string econFlagFalseWithBlock = ResignV3(econJson.Replace(
                "\"economyEnabled\":true",
                "\"economyEnabled\":false",
                StringComparison.Ordinal));
            T.Refuses("SaveInconsistent", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(econFlagFalseWithBlock)));

            // Money-Off v3 save tampered with empty economy block -> SaveEconomyRequiresV3 on custom reserved registry, MissingKey on default codec
            var (_, baseSession) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064);
            var (_, v3Bytes) = baseSession.CaptureSaveBytes(Fence(baseSession.ReadView()), "Pure FM", 1);
            string json = Encoding.UTF8.GetString(v3Bytes);

            string withEconBlock = ResignV3(json.Replace(
                "\"blocks\":{\"genesisRecipe\":",
                "\"blocks\":{\"economy\":{\"blockVersion\":1,\"data\":{}},\"genesisRecipe\":",
                StringComparison.Ordinal));
            T.Refuses("SaveEconomyRequiresV3", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(withEconBlock), customReg));
            T.Refuses("MissingKey", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(withEconBlock)));

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
