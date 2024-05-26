using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[StaticConstructorOnStartup]
public static class CustomPrisonerInteractions
{
    public enum ExtraMode
    {
        Undefined,
        None,
        ReleaseWhenHealthy,
        Recruit,
        Release,
        Enslave,
        Kill,
        ReduceResistanceThenRecruit,
        ReduceWillThenEnslave,
        ReduceResistanceThenRelease,
        ReduceResistanceThenKill,
        ReduceResistance,
        ReduceWill,
        ReleaseWhenAbleToWalk,
        ReleaseWhenNotGuilty
    }

    public static readonly FieldInfo InteractionModeField;

    private static readonly Dictionary<Map, ExtraInteractionsTracker> ExtraInteractionsTrackers =
        new Dictionary<Map, ExtraInteractionsTracker>();

    private static readonly MethodInfo isStudiableMethod = AccessTools.Method(typeof(ITab_Pawn_Visitor), "IsStudiable");

    private static readonly MethodInfo colonyHasAnyBloodfeederMethod =
        AccessTools.Method(typeof(ITab_Pawn_Visitor), "ColonyHasAnyBloodfeeder");

    public static Pawn pawnWithIdeologyCrisis = null;

    public static readonly Dictionary<ExtraMode, PrisonerInteractionModeDef> ModeDictionary =
        new Dictionary<ExtraMode, PrisonerInteractionModeDef>
        {
            [ExtraMode.Kill] = PrisonerInteractionModeDefOf.Execution,
            [ExtraMode.Release] = PrisonerInteractionModeDefOf.Release,
            [ExtraMode.ReleaseWhenHealthy] = PrisonerInteractionModeDefOf.Release,
            [ExtraMode.ReleaseWhenAbleToWalk] = PrisonerInteractionModeDefOf.Release,
            [ExtraMode.ReleaseWhenNotGuilty] = PrisonerInteractionModeDefOf.Release,
            [ExtraMode.ReduceResistance] = PrisonerInteractionModeDefOf.ReduceResistance,
            [ExtraMode.ReduceResistanceThenKill] = PrisonerInteractionModeDefOf.Execution,
            [ExtraMode.ReduceResistanceThenRecruit] = PrisonerInteractionModeDefOf.AttemptRecruit,
            [ExtraMode.ReduceResistanceThenRelease] = PrisonerInteractionModeDefOf.Release,
            [ExtraMode.Recruit] = PrisonerInteractionModeDefOf.AttemptRecruit,
            [ExtraMode.Enslave] = PrisonerInteractionModeDefOf.Enslave,
            [ExtraMode.ReduceWill] = PrisonerInteractionModeDefOf.ReduceWill,
            [ExtraMode.ReduceWillThenEnslave] = PrisonerInteractionModeDefOf.Enslave
        };

    static CustomPrisonerInteractions()
    {
        var harmony = new Harmony("Mlie.CustomPrisonerInteractions");
        InteractionModeField = AccessTools.Field(typeof(Pawn_GuestTracker), "interactionMode");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        if (CustomPrisonerInteractionsMod.instance.Settings.DefaultNewValue == null)
        {
            CustomPrisonerInteractionsMod.instance.Settings.DefaultNewValue =
                PrisonerInteractionModeDefOf.MaintainOnly;
        }
    }

    public static bool CanUseExtraMode(Pawn pawn, ExtraMode mode)
    {
        return !ModeDictionary.ContainsKey(mode) || CanUsePrisonerInteractionMode(pawn, ModeDictionary[mode]);
    }

    public static bool CanUsePrisonerInteractionMode(Pawn pawn, PrisonerInteractionModeDef mode)
    {
        if (!pawn.guest.Recruitable && mode.hideIfNotRecruitable)
        {
            return false;
        }

        if (pawn.IsWildMan() && !mode.allowOnWildMan)
        {
            return false;
        }

        if (mode.hideIfNoBloodfeeders && pawn.MapHeld != null &&
            (bool)colonyHasAnyBloodfeederMethod.Invoke(null, [pawn.MapHeld]) == false)
        {
            return false;
        }

        if (mode.hideOnHemogenicPawns && ModsConfig.BiotechActive && pawn.genes != null &&
            pawn.genes.HasActiveGene(GeneDefOf.Hemogenic))
        {
            return false;
        }

        if (!mode.allowInClassicIdeoMode && Find.IdeoManager.classicMode)
        {
            return false;
        }

        if (!ModsConfig.AnomalyActive)
        {
            return true;
        }

        if (mode.hideIfNotStudiableAsPrisoner && (bool)isStudiableMethod.Invoke(null, [pawn]) == false)
        {
            return false;
        }

        return !mode.hideIfGrayFleshNotAppeared || Find.Anomaly.hasSeenGrayFlesh;
    }

    internal static ExtraInteractionsTracker GetExtraInteractionsTracker(this Map map)
    {
        if (map == null)
        {
            return null;
        }

        ExtraInteractionsTracker value;

        if (!ExtraInteractionsTrackers.TryGetValue(map, out var tracker))
        {
            value = ExtraInteractionsTrackers[map] = map.GetComponent<ExtraInteractionsTracker>();
        }
        else
        {
            value = tracker;
        }

        return value;
    }

    internal static TaggedString getExtraInterractionExplanation(ExtraMode extraMode)
    {
        switch (extraMode)
        {
            case ExtraMode.ReleaseWhenNotGuilty:
                return "CPI.releasewhennotguilty".Translate();
            case ExtraMode.ReleaseWhenAbleToWalk:
                return "CPI.releasewhenabletowalk".Translate();
            case ExtraMode.ReleaseWhenHealthy:
                return "CPI.releasewhenhealthy".Translate();
            case ExtraMode.Recruit:
                return "CPI.thenrecruit".Translate();
            case ExtraMode.Release:
                return "CPI.thenrelease".Translate();
            case ExtraMode.Enslave:
                return "CPI.thenenslave".Translate();
            case ExtraMode.ReduceResistanceThenRecruit:
                return "CPI.reduceresistancefirstthenrecruit".Translate();
            case ExtraMode.ReduceWillThenEnslave:
                return "CPI.reducewillfirstthenenslave".Translate();
            case ExtraMode.ReduceResistanceThenRelease:
                return "CPI.reduceresistancefirstthenrelease".Translate();
            case ExtraMode.ReduceResistance:
                return "CPI.reduceresistancefirst".Translate();
            case ExtraMode.ReduceWill:
                return "CPI.reducewillfirst".Translate();
            case ExtraMode.Kill:
                return "CPI.thenkill".Translate();
            case ExtraMode.ReduceResistanceThenKill:
                return "CPI.reduceresistancefirstthenkill".Translate();
        }

        return "CPI.none".Translate();
    }
}