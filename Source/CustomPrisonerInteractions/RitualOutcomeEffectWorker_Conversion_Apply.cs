using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_Conversion), "Apply")]
public static class RitualOutcomeEffectWorker_Conversion_Apply
{
    public static void Prefix(LordJob_Ritual jobRitual, out Ideo __state)
    {
        var pawn = jobRitual.PawnWithRole("convertee");
        __state = pawn.ideo.Ideo;
    }

    public static void Postfix(LordJob_Ritual jobRitual, Ideo __state)
    {
        var pawn = jobRitual.PawnWithRole("convertee");

        if (!pawn.IsPrisonerOfColony)
        {
            return;
        }

        if (__state == pawn.ideo.Ideo)
        {
            return;
        }

        var extraInteractionsTracker = pawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null || !extraInteractionsTracker.Has(pawn))
        {
            return;
        }

        TaggedString text;

        switch (extraInteractionsTracker[pawn])
        {
            case CustomPrisonerInteractions.ExtraMode.Release:
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRelease:
                pawn.guest.interactionMode = PrisonerInteractionModeDefOf.Release;
                extraInteractionsTracker[pawn] = CustomPrisonerInteractionsMod.instance.Settings.DefaultReleaseValue;
                "CTRe.pawnconvertedrelease".Translate(pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit:
            case CustomPrisonerInteractions.ExtraMode.Recruit:
                pawn.guest.interactionMode = PrisonerInteractionModeDefOf.AttemptRecruit;
                "CTRe.pawnconvertedrecruit".Translate(pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenKill:
            case CustomPrisonerInteractions.ExtraMode.Kill:
                pawn.guest.interactionMode = PrisonerInteractionModeDefOf.Execution;
                "CTRe.pawnconvertedkill".Translate(pawn.NameFullColored);
                break;
            case CustomPrisonerInteractions.ExtraMode.Enslave:
            case CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave:
                pawn.guest.interactionMode = PrisonerInteractionModeDefOf.Enslave;
                "CTRe.pawnconvertedenslave".Translate(pawn.NameFullColored);
                break;
        }

        var messageType = MessageTypeDefOf.PositiveEvent;
        var message = new Message(text, messageType, new LookTargets(pawn));
        Messages.Message(message);
    }
}