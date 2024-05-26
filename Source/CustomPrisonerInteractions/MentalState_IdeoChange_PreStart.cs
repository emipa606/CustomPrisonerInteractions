using HarmonyLib;
using Verse;
using Verse.AI;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(MentalState_IdeoChange), nameof(MentalState_IdeoChange.PreStart))]
public static class MentalState_IdeoChange_PreStart
{
    public static void Prefix(Pawn ___pawn)
    {
        CustomPrisonerInteractions.pawnWithIdeologyCrisis = ___pawn;
    }

    public static void Postfix()
    {
        CustomPrisonerInteractions.pawnWithIdeologyCrisis = null;
    }
}