using RimWorld;
using Verse;

namespace CustomPrisonerInteractions;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class CustomPrisonerInteractionsSettings : ModSettings
{
    public bool AutoHemogen;
    public CustomPrisonerInteractions.ExtraMode DefaultConvertValue;
    public PrisonerInteractionModeDef DefaultNewValue;
    public CustomPrisonerInteractions.ExtraMode DefaultReleaseValue;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref AutoHemogen, "AutoHemogen");
        Scribe_Defs.Look(ref DefaultNewValue, "DefaultNewValue");
        Scribe_Values.Look(ref DefaultReleaseValue, "DefaultReleaseValue");
        Scribe_Values.Look(ref DefaultConvertValue, "DefaultConvertValue");
    }
}