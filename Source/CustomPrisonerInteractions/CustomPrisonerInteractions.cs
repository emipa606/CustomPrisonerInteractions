using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
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
        ReduceResistanceThenRecruit,
        ReduceWillThenEnslave,
        ReduceResistanceThenRelease,
        ReduceResistance,
        ReduceWill
    }

    private static readonly Dictionary<Map, ExtraInteractionsTracker> ExtraInteractionsTrackers =
        new Dictionary<Map, ExtraInteractionsTracker>();

    static CustomPrisonerInteractions()
    {
        var harmony = new Harmony("Mlie.CustomPrisonerInteractions");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
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
        }

        return "CPI.none".Translate();
    }
}