﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(WorkGiver_Warden_ReleasePrisoner), nameof(WorkGiver_Warden_ReleasePrisoner.JobOnThing))]
public static class WorkGiver_Warden_ReleasePrisoner_JobOnThing
{
    public static bool Prefix(Thing t)
    {
        var pawn2 = (Pawn)t;

        if (pawn2?.IsPrisonerOfColony == false)
        {
            return true;
        }

        if (pawn2?.guest == null)
        {
            return true;
        }

        var extraInteractionsTracker = pawn2.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null || !extraInteractionsTracker.Has(pawn2))
        {
            return true;
        }

        if (extraInteractionsTracker[pawn2] is not CustomPrisonerInteractions.ExtraMode.ReleaseWhenHealthy
            and not CustomPrisonerInteractions.ExtraMode.ReleaseWhenAbleToWalk
            and not CustomPrisonerInteractions.ExtraMode.ReleaseWhenNotGuilty)
        {
            return true;
        }

        if (pawn2.guest.ExclusiveInteractionMode != PrisonerInteractionModeDefOf.Release)
        {
            return true;
        }

        if (extraInteractionsTracker[pawn2] is CustomPrisonerInteractions.ExtraMode.ReleaseWhenHealthy)
        {
            return !HealthAIUtility.ShouldSeekMedicalRest(pawn2) &&
                   !pawn2.health.hediffSet.HasNaturallyHealingInjury() &&
                   !pawn2.health.hediffSet.HasTendedAndHealingInjury();
        }

        if (extraInteractionsTracker[pawn2] is CustomPrisonerInteractions.ExtraMode.ReleaseWhenAbleToWalk)
        {
            return !pawn2.Downed;
        }

        return pawn2.guilt is not { IsGuilty: true };
    }
}