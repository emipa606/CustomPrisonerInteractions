using System.Collections.Generic;
using System.Linq;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;
using static CustomPrisonerInteractions.CustomPrisonerInteractions;
using static CustomPrisonerInteractions.CustomPrisonerInteractions.ExtraMode;

namespace CustomPrisonerInteractions;

[StaticConstructorOnStartup]
internal class CustomPrisonerInteractionsMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static CustomPrisonerInteractionsMod instance;

    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    private CustomPrisonerInteractionsSettings settings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public CustomPrisonerInteractionsMod(ModContentPack content) : base(content)
    {
        instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(
                ModLister.GetActiveModWithIdentifier("Mlie.CustomPrisonerInteractions"));
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal CustomPrisonerInteractionsSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = GetSettings<CustomPrisonerInteractionsSettings>();
            }

            return settings;
        }
        set => settings = value;
    }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Custom Prisoner Interactions";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        listing_Standard.Label(
            "CPI.defaultfornew".Translate(Settings.DefaultNewValue.LabelCap));
        if (listing_Standard.ButtonText("CPI.change".Translate()))
        {
            var list = new List<FloatMenuOption>();
            foreach (var interactionModeDef in DefDatabase<PrisonerInteractionModeDef>.AllDefsListForReading.OrderBy(
                         def => def.listOrder))
            {
                list.Add(new FloatMenuOption(interactionModeDef.LabelCap,
                    delegate { Settings.DefaultNewValue = interactionModeDef; },
                    MenuOptionPriority.Default, null, null, 29f));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }

        listing_Standard.Gap();
        listing_Standard.Label(
            "CPI.defaultrelease".Translate(getExtraInterractionExplanation(Settings.DefaultReleaseValue)));
        if (listing_Standard.ButtonText("CPI.change".Translate()))
        {
            var list = new List<FloatMenuOption>
            {
                new FloatMenuOption(getExtraInterractionExplanation(None),
                    delegate { Settings.DefaultReleaseValue = None; },
                    MenuOptionPriority.Default, null, null, 29f),
                new FloatMenuOption(getExtraInterractionExplanation(ReleaseWhenHealthy),
                    delegate { Settings.DefaultReleaseValue = ReleaseWhenHealthy; },
                    MenuOptionPriority.Default, null, null, 29f)
            };
            Find.WindowStack.Add(new FloatMenu(list));
        }

        listing_Standard.Gap();
        if (ModLister.IdeologyInstalled)
        {
            listing_Standard.Label(
                "CPI.defaultconvert".Translate(getExtraInterractionExplanation(Settings.DefaultConvertValue)));
            if (listing_Standard.ButtonText("CPI.change".Translate()))
            {
                var list = new List<FloatMenuOption>
                {
                    new FloatMenuOption(getExtraInterractionExplanation(None),
                        delegate { Settings.DefaultConvertValue = None; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(Recruit),
                        delegate { Settings.DefaultConvertValue = Recruit; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(ReduceResistanceThenRecruit),
                        delegate { Settings.DefaultConvertValue = ReduceResistanceThenRecruit; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(Enslave),
                        delegate { Settings.DefaultConvertValue = Enslave; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(ReduceWillThenEnslave),
                        delegate { Settings.DefaultConvertValue = ReduceWillThenEnslave; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(Release),
                        delegate { Settings.DefaultConvertValue = Release; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(ReduceResistanceThenRelease),
                        delegate { Settings.DefaultConvertValue = ReduceResistanceThenRelease; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(Kill),
                        delegate { Settings.DefaultConvertValue = Kill; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new FloatMenuOption(getExtraInterractionExplanation(ReduceResistanceThenKill),
                        delegate { Settings.DefaultConvertValue = ReduceResistanceThenKill; },
                        MenuOptionPriority.Default, null, null, 29f)
                };
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("CPI.version".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }
}