using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
public static class Pawn_InteractionsTracker_TryInteractWith
{
    public static void Prefix(Pawn recipient, out PrisonerInteractionModeDef __state)
    {
        __state = PrisonerInteractionModeDefOf.MaintainOnly;

        if (recipient?.IsPrisonerOfColony == false)
        {
            return;
        }

        if (recipient?.guest == null)
        {
            return;
        }

        var extraInteractionsTracker = recipient.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null || !extraInteractionsTracker.Has(recipient))
        {
            return;
        }

        __state = recipient.guest.ExclusiveInteractionMode;

        switch (extraInteractionsTracker[recipient])
        {
            case CustomPrisonerInteractions.ExtraMode.ReduceResistance:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRelease:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenKill:
                if (recipient.guest.resistance > 0)
                {
                    CustomPrisonerInteractions.InteractionModeField.SetValue(recipient.guest,
                        PrisonerInteractionModeDefOf.ReduceResistance);
                    //recipient.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceResistance;
                }

                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave:
            case CustomPrisonerInteractions.ExtraMode.ReduceWill:
                if (recipient.guest.will > 0)
                {
                    CustomPrisonerInteractions.InteractionModeField.SetValue(recipient.guest,
                        PrisonerInteractionModeDefOf.ReduceWill);
                    //recipient.guest.interactionMode = PrisonerInteractionModeDefOf.ReduceWill;
                }

                break;
        }
    }

    public static void Postfix(Pawn recipient, PrisonerInteractionModeDef __state)
    {
        if (__state == PrisonerInteractionModeDefOf.MaintainOnly)
        {
            return;
        }

        if (recipient?.guest == null)
        {
            return;
        }

        if (recipient.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.ReduceResistance ||
            recipient.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.ReduceWill)
        {
            CustomPrisonerInteractions.InteractionModeField.SetValue(recipient.guest, __state);
            //recipient.guest.interactionMode = __state;
        }
    }
}