using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(WorkGiver_Warden_Enslave), "JobOnThing")]
public static class WorkGiver_Warden_Enslave_JobOnThing
{
    public static void Prefix(Thing t, out PrisonerInteractionModeDef __state)
    {
        var pawn2 = (Pawn)t;
        __state = PrisonerInteractionModeDefOf.NoInteraction;

        if (pawn2?.IsPrisonerOfColony == false)
        {
            return;
        }

        if (pawn2?.guest == null)
        {
            return;
        }

        var extraInteractionsTracker = pawn2.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null || !extraInteractionsTracker.Has(pawn2))
        {
            return;
        }

        __state = pawn2.guest.interactionMode;

        if (extraInteractionsTracker[pawn2] == CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave)
        {
            pawn2.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceWill;
        }
    }

    public static void Postfix(Thing t, PrisonerInteractionModeDef __state)
    {
        if (__state == PrisonerInteractionModeDefOf.NoInteraction)
        {
            return;
        }

        var pawn2 = (Pawn)t;

        if (pawn2?.guest == null)
        {
            return;
        }

        pawn2.guest.interactionMode = __state;
    }
}