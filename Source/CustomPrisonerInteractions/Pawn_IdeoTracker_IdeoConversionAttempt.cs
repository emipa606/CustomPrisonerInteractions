using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(Pawn_IdeoTracker), "IdeoConversionAttempt")]
public static class Pawn_IdeoTracker_IdeoConversionAttempt
{
    public static void Postfix(bool __result, Pawn ___pawn)
    {
        if (!__result)
        {
            return;
        }

        if (!___pawn.IsPrisonerOfColony)
        {
            return;
        }

        if (CustomPrisonerInteractions.pawnWithIdeologyCrisis?.Equals(___pawn) == true)
        {
            return;
        }

        var extraInteractionsTracker = ___pawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null || !extraInteractionsTracker.Has(___pawn))
        {
            return;
        }

        TaggedString text;

        switch (extraInteractionsTracker[___pawn])
        {
            case CustomPrisonerInteractions.ExtraMode.Release:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRelease:
                ___pawn.guest.interactionMode = PrisonerInteractionModeDefOf.Release;
                extraInteractionsTracker[___pawn] = CustomPrisonerInteractionsMod.instance.Settings.DefaultReleaseValue;
                "CTRe.pawnconvertedrelease".Translate(___pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit:
            case CustomPrisonerInteractions.ExtraMode.Recruit:
                ___pawn.guest.interactionMode = PrisonerInteractionModeDefOf.AttemptRecruit;
                "CTRe.pawnconvertedrecruit".Translate(___pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.Enslave:
            case CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave:
                ___pawn.guest.interactionMode = PrisonerInteractionModeDefOf.Enslave;
                "CTRe.pawnconvertedenslave".Translate(___pawn.NameFullColored);
                break;
        }

        var messageType = MessageTypeDefOf.PositiveEvent;
        var message = new Message(text, messageType, new LookTargets(___pawn));
        Messages.Message(message);
    }
}