using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(WorkGiver_Warden_Chat), "JobOnThing")]
public static class WorkGiver_Warden_Chat_JobOnThing
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

        switch (extraInteractionsTracker[pawn2])
        {
            case CustomPrisonerInteractions.ExtraMode.ReduceResistance:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRelease:
                if (pawn2.guest.resistance > 0)
                {
                    pawn2.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceResistance;
                }

                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave:
            case CustomPrisonerInteractions.ExtraMode.ReduceWill:
                if (pawn2.guest.will > 0)
                {
                    pawn2.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceWill;
                }

                break;
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

        if (pawn2.guest.interactionMode == PrisonerInteractionModeDefOf.ReduceResistance ||
            pawn2.guest.interactionMode == PrisonerInteractionModeDefOf.ReduceWill)
        {
            pawn2.guest.interactionMode = __state;
        }
    }
}