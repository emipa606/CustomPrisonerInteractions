using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.IdeoConversionAttempt))]
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

        if (CustomPrisonerInteractions.PawnWithIdeologyCrisis?.Equals(___pawn) == true)
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
                CustomPrisonerInteractions.InteractionModeField.SetValue(___pawn.guest,
                    PrisonerInteractionModeDefOf.Release);
                extraInteractionsTracker[___pawn] = CustomPrisonerInteractionsMod.Instance.Settings.DefaultReleaseValue;
                "CTRe.pawnconvertedrelease".Translate(___pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit:
            case CustomPrisonerInteractions.ExtraMode.Recruit:
                CustomPrisonerInteractions.InteractionModeField.SetValue(___pawn.guest,
                    PrisonerInteractionModeDefOf.AttemptRecruit);
                "CTRe.pawnconvertedrecruit".Translate(___pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenKill:
            case CustomPrisonerInteractions.ExtraMode.Kill:
                CustomPrisonerInteractions.InteractionModeField.SetValue(___pawn.guest,
                    PrisonerInteractionModeDefOf.Execution);
                "CTRe.pawnconvertedkill".Translate(___pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.Enslave:
            case CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave:
                CustomPrisonerInteractions.InteractionModeField.SetValue(___pawn.guest,
                    PrisonerInteractionModeDefOf.Enslave);
                "CTRe.pawnconvertedenslave".Translate(___pawn.NameFullColored);
                break;
        }

        var messageType = MessageTypeDefOf.PositiveEvent;
        var message = new Message(text, messageType, new LookTargets(___pawn));
        Messages.Message(message);
    }
}