using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(Pawn_GuestTracker), "CapturedBy")]
public static class Pawn_GuestTracker_CapturedBy
{
    public static void Postfix(Pawn_GuestTracker __instance, ref Pawn ___pawn, Pawn byPawn)
    {
        if (CustomPrisonerInteractionsMod.instance.Settings.DefaultNewValue ==
            PrisonerInteractionModeDefOf.NoInteraction)
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

        ___pawn.guest.interactionMode = CustomPrisonerInteractionsMod.instance.Settings.DefaultNewValue;

        var extraInteractionsTracker = byPawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null)
        {
            return;
        }

        if (__instance.interactionMode == PrisonerInteractionModeDefOf.Release)
        {
            extraInteractionsTracker[___pawn] =
                CustomPrisonerInteractionsMod.instance.Settings.DefaultReleaseValue;
            return;
        }

        if (__instance.interactionMode != PrisonerInteractionModeDefOf.Convert)
        {
            return;
        }

        ___pawn.guest.ideoForConversion = Faction.OfPlayer.ideos.PrimaryIdeo;
        extraInteractionsTracker[___pawn] =
            CustomPrisonerInteractionsMod.instance.Settings.DefaultConvertValue;
    }
}