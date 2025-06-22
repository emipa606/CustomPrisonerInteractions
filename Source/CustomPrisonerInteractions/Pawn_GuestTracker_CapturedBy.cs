using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.CapturedBy))]
public static class Pawn_GuestTracker_CapturedBy
{
    public static void Postfix(Pawn_GuestTracker __instance, ref Pawn ___pawn, Pawn byPawn)
    {
        if (CustomPrisonerInteractionsMod.Instance.Settings.AutoHemogen && (___pawn.genes == null ||
                                                                            !___pawn.genes.HasActiveGene(
                                                                                GeneDefOf.Hemogenic)))
        {
            ___pawn.guest.ToggleNonExclusiveInteraction(PrisonerInteractionModeDefOf.HemogenFarm, true);
        }

        if (CustomPrisonerInteractionsMod.Instance.Settings.DefaultNewValue ==
            PrisonerInteractionModeDefOf.MaintainOnly)
        {
            return;
        }

        if (!__instance.IsPrisoner)
        {
            return;
        }

        if (___pawn.IsColonist)
        {
            return;
        }

        if (byPawn == null)
        {
            return;
        }

        if (!CustomPrisonerInteractions.CanUsePrisonerInteractionMode(___pawn,
                CustomPrisonerInteractionsMod.Instance.Settings.DefaultNewValue))
        {
            return;
        }

        CustomPrisonerInteractions.InteractionModeField.SetValue(___pawn.guest,
            CustomPrisonerInteractionsMod.Instance.Settings.DefaultNewValue);

        var extraInteractionsTracker = byPawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null)
        {
            return;
        }

        if (__instance.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.Release)
        {
            if (CustomPrisonerInteractions.CanUseExtraMode(___pawn,
                    CustomPrisonerInteractionsMod.Instance.Settings.DefaultReleaseValue))
            {
                extraInteractionsTracker[___pawn] =
                    CustomPrisonerInteractionsMod.Instance.Settings.DefaultReleaseValue;
            }

            return;
        }

        if (__instance.ExclusiveInteractionMode != PrisonerInteractionModeDefOf.Convert &&
            __instance.ExclusiveInteractionMode.defName != "PrisonLabor_workAndConvertOption")
        {
            return;
        }

        ___pawn.guest.ideoForConversion = Faction.OfPlayer.ideos.PrimaryIdeo;
        if (CustomPrisonerInteractions.CanUseExtraMode(___pawn,
                CustomPrisonerInteractionsMod.Instance.Settings.DefaultConvertValue))
        {
            extraInteractionsTracker[___pawn] =
                CustomPrisonerInteractionsMod.Instance.Settings.DefaultConvertValue;
        }
    }
}