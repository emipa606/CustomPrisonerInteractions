using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(WorkGiver_Warden_ReleasePrisoner), "JobOnThing")]
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

        if (extraInteractionsTracker[pawn2] != CustomPrisonerInteractions.ExtraMode.ReleaseWhenHealthy)
        {
            return true;
        }

        if (pawn2.guest.interactionMode != PrisonerInteractionModeDefOf.Release)
        {
            return true;
        }

        return !HealthAIUtility.ShouldSeekMedicalRest(pawn2);
    }
}