using HarmonyLib;
using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(Pawn_GuestTracker), "CanBeBroughtFood", MethodType.Getter)]
public static class Pawn_GuestTracker_CanBeBroughtFood
{
    public static void Postfix(Pawn_GuestTracker __instance, Pawn ___pawn, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if (__instance.ExclusiveInteractionMode != PrisonerInteractionModeDefOf.Release)
        {
            return;
        }

        var extraInterractionsTracker = ___pawn.Map.GetExtraInteractionsTracker();
        if (extraInterractionsTracker == null || !extraInterractionsTracker.Has(___pawn))
        {
            return;
        }

        __result = extraInterractionsTracker[___pawn] is CustomPrisonerInteractions.ExtraMode.ReleaseWhenHealthy
            or CustomPrisonerInteractions.ExtraMode.ReleaseWhenAbleToWalk
            or CustomPrisonerInteractions.ExtraMode.ReleaseWhenNotGuilty;
    }
}