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
        ReduceWill
    }

    private static readonly Dictionary<Map, ExtraInteractionsTracker> ExtraInteractionsTrackers =
        new Dictionary<Map, ExtraInteractionsTracker>();

    public static Pawn pawnWithIdeologyCrisis = null;

    static CustomPrisonerInteractions()
    {
        var harmony = new Harmony("Mlie.CustomPrisonerInteractions");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        if (CustomPrisonerInteractionsMod.instance.Settings.DefaultNewValue == null)
        {
            CustomPrisonerInteractionsMod.instance.Settings.DefaultNewValue =
                PrisonerInteractionModeDefOf.NoInteraction;
        }
    }

    internal static ExtraInteractionsTracker GetExtraInteractionsTracker(this Map map)
    {
        if (map == null)
        {
            return null;
        }

        ExtraInteractionsTracker value;

        if (!ExtraInteractionsTrackers.ContainsKey(map))
        {
            value = ExtraInteractionsTrackers[map] = map.GetComponent<ExtraInteractionsTracker>();
        }
        else
        {
            value = ExtraInteractionsTrackers[map];
        }

        return value;
    }

    internal static TaggedString getExtraInterractionExplanation(ExtraMode extraMode)
    {
        switch (extraMode)
        {
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