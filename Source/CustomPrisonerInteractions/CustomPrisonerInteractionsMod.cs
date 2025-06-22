using System.Collections.Generic;
using System.Linq;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomPrisonerInteractions;

[StaticConstructorOnStartup]
internal class CustomPrisonerInteractionsMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static CustomPrisonerInteractionsMod Instance;

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
        Instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }


    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal CustomPrisonerInteractionsSettings Settings
    {
        get
        {
            settings ??= GetSettings<CustomPrisonerInteractionsSettings>();

            return settings;
        }
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
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.Gap();
        listingStandard.Label(
            "CPI.defaultfornew".Translate(Settings.DefaultNewValue.LabelCap));
        if (listingStandard.ButtonText("CPI.change".Translate()))
        {
            var list = new List<FloatMenuOption>();
            foreach (var interactionModeDef in DefDatabase<PrisonerInteractionModeDef>.AllDefsListForReading
                         .Where(def => !def.isNonExclusiveInteraction).OrderBy(def => def.listOrder))
            {
                list.Add(new FloatMenuOption(interactionModeDef.LabelCap,
                    delegate { Settings.DefaultNewValue = interactionModeDef; },
                    MenuOptionPriority.Default, null, null, 29f));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }

        if (ModsConfig.BiotechActive)
        {
            listingStandard.CheckboxLabeled("CPI.autohemogen".Translate(), ref Settings.AutoHemogen);
        }
        else
        {
            Settings.AutoHemogen = false;
        }

        listingStandard.Gap();
        listingStandard.Label(
            "CPI.defaultrelease".Translate(
                CustomPrisonerInteractions.getExtraInterractionExplanation(Settings.DefaultReleaseValue)));
        if (listingStandard.ButtonText("CPI.change".Translate()))
        {
            var list = new List<FloatMenuOption>
            {
                new(
                    CustomPrisonerInteractions.getExtraInterractionExplanation(
                        CustomPrisonerInteractions.ExtraMode.None),
                    delegate { Settings.DefaultReleaseValue = CustomPrisonerInteractions.ExtraMode.None; },
                    MenuOptionPriority.Default, null, null, 29f),
                new(
                    CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                        .ReleaseWhenNotGuilty),
                    delegate
                    {
                        Settings.DefaultReleaseValue = CustomPrisonerInteractions.ExtraMode.ReleaseWhenNotGuilty;
                    },
                    MenuOptionPriority.Default, null, null, 29f),
                new(
                    CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                        .ReleaseWhenAbleToWalk),
                    delegate
                    {
                        Settings.DefaultReleaseValue = CustomPrisonerInteractions.ExtraMode.ReleaseWhenAbleToWalk;
                    },
                    MenuOptionPriority.Default, null, null, 29f),
                new(
                    CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                        .ReleaseWhenHealthy),
                    delegate
                    {
                        Settings.DefaultReleaseValue = CustomPrisonerInteractions.ExtraMode.ReleaseWhenHealthy;
                    },
                    MenuOptionPriority.Default, null, null, 29f)
            };
            Find.WindowStack.Add(new FloatMenu(list));
        }

        listingStandard.Gap();
        if (ModLister.IdeologyInstalled)
        {
            listingStandard.Label(
                "CPI.defaultconvert".Translate(
                    CustomPrisonerInteractions.getExtraInterractionExplanation(Settings.DefaultConvertValue)));
            if (listingStandard.ButtonText("CPI.change".Translate()))
            {
                var list = new List<FloatMenuOption>
                {
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .None),
                        delegate { Settings.DefaultConvertValue = CustomPrisonerInteractions.ExtraMode.None; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .Recruit),
                        delegate { Settings.DefaultConvertValue = CustomPrisonerInteractions.ExtraMode.Recruit; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .ReduceResistanceThenRecruit),
                        delegate
                        {
                            Settings.DefaultConvertValue =
                                CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRecruit;
                        },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .Enslave),
                        delegate { Settings.DefaultConvertValue = CustomPrisonerInteractions.ExtraMode.Enslave; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .ReduceWillThenEnslave),
                        delegate
                        {
                            Settings.DefaultConvertValue = CustomPrisonerInteractions.ExtraMode.ReduceWillThenEnslave;
                        },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .Release),
                        delegate { Settings.DefaultConvertValue = CustomPrisonerInteractions.ExtraMode.Release; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .ReduceResistanceThenRelease),
                        delegate
                        {
                            Settings.DefaultConvertValue =
                                CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenRelease;
                        },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .Kill),
                        delegate { Settings.DefaultConvertValue = CustomPrisonerInteractions.ExtraMode.Kill; },
                        MenuOptionPriority.Default, null, null, 29f),
                    new(
                        CustomPrisonerInteractions.getExtraInterractionExplanation(CustomPrisonerInteractions.ExtraMode
                            .ReduceResistanceThenKill),
                        delegate
                        {
                            Settings.DefaultConvertValue =
                                CustomPrisonerInteractions.ExtraMode.ReduceResistanceThenKill;
                        },
                        MenuOptionPriority.Default, null, null, 29f)
                };
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("CPI.version".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }
}