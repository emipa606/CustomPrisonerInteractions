using Verse;

namespace CustomPrisonerInteractions;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class CustomPrisonerInteractionsSettings : ModSettings
{
    public CustomPrisonerInteractions.ExtraMode DefaultConvertValue;
    public CustomPrisonerInteractions.ExtraMode DefaultReleaseValue;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref DefaultReleaseValue, "DefaultReleaseValue");
        Scribe_Values.Look(ref DefaultConvertValue, "DefaultConvertValue");
    }
}