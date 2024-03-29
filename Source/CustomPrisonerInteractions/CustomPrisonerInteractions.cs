﻿using System.Collections.Generic;
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

    public static Pawn pawnWithIdeologyCrisis = null;

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

    private static bool ColonyHasAnyBloodfeeder(Map map)
    {
        if (!ModsConfig.BiotechActive)
        {
            return false;
        }

        foreach (var item in map.mapPawns.FreeColonistsAndPrisonersSpawned)
        {
            if (item.IsBloodfeeder())
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsStudiable(Pawn pawn)
    {
        if (!ModsConfig.AnomalyActive)
        {
            return false;
        }

        if (!pawn.TryGetComp<CompStudiable>(out var comp) || !comp.EverStudiable())
        {
            return false;
        }

        if (pawn.kindDef.studiableAsPrisoner)
        {
            return !pawn.everBrainWiped;
        }

        return false;
    }

    public static bool CanUseExtraMode(Pawn pawn, ExtraMode mode)
    {
        if (!pawn.guest.Recruitable && mode is ExtraMode.Recruit or ExtraMode.ReduceResistanceThenRecruit)
        {
            return false;
        }

        return !pawn.IsWildMan() || mode is not (ExtraMode.Recruit or ExtraMode.ReduceResistanceThenRecruit
            or ExtraMode.ReduceResistance or ExtraMode.ReduceResistanceThenKill
            or ExtraMode.ReduceResistanceThenRelease);
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

        if (mode.hideIfNoBloodfeeders && pawn.MapHeld != null && !ColonyHasAnyBloodfeeder(pawn.MapHeld))
        {
            return false;
        }

        if (mode.hideOnHemogenicPawns && ModsConfig.BiotechActive && pawn.genes != null &&
            pawn.genes.HasGene(GeneDefOf.Hemogenic))
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

        if (mode.hideIfNotStudiableAsPrisoner && !IsStudiable(pawn))
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