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
        __state = PrisonerInteractionModeDefOf.MaintainOnly;

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

        __state = pawn2.guest.ExclusiveInteractionMode;

        switch (extraInteractionsTracker[pawn2])
        {
            case CustomPrisonerInteractions.ExtraMode.ReduceResistance:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRelease:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenKill:
                if (pawn2.guest.resistance > 0)
                {
                    CustomPrisonerInteractions.InteractionModeField.SetValue(pawn2.guest,
                        PrisonerInteractionModeDefOf.ReduceResistance);
                    //pawn2.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceResistance;
                }

                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave:
            case CustomPrisonerInteractions.ExtraMode.ReduceWill:
                if (pawn2.guest.will > 0)
                {
                    CustomPrisonerInteractions.InteractionModeField.SetValue(pawn2.guest,
                        PrisonerInteractionModeDefOf.ReduceWill);
                    //pawn2.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceWill;
                }

                break;
        }
    }

    public static void Postfix(Thing t, PrisonerInteractionModeDef __state)
    {
        if (__state == PrisonerInteractionModeDefOf.MaintainOnly)
        {
            return;
        }

        var pawn2 = (Pawn)t;

        if (pawn2?.guest == null)
        {
            return;
        }

        if (pawn2.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.ReduceResistance ||
            pawn2.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.ReduceWill)
        {
            CustomPrisonerInteractions.InteractionModeField.SetValue(pawn2.guest, __state);
            //pawn2.guest.interactionMode = __state;
        }
    }
}