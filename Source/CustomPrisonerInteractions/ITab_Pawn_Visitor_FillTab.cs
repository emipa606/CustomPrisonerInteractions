using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using static CustomPrisonerInteractions.CustomPrisonerInteractions;
using static CustomPrisonerInteractions.CustomPrisonerInteractions.ExtraMode;

namespace CustomPrisonerInteractions;

[HarmonyPatch(typeof(ITab_Pawn_Visitor), "FillTab")]
[HarmonyAfter("Harmony_PrisonLabor")]
public class ITab_Pawn_Visitor_FillTab
{
    private const float buttonSpace = 34f;
    private const float margin = 10f;
    private const int buttonText = 23;

    public static void Prefix(out Tuple<List<FloatMenuOption>, string> __state)
    {
        __state = null;
        if (Find.Selector.SingleSelectedThing is not Pawn pawn)
        {
            return;
        }

        __state = new Tuple<List<FloatMenuOption>, string>(new List<FloatMenuOption>(),
            pawn.guest.interactionMode.defName);

        var extraInteractionsTracker = pawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null)
        {
            return;
        }

        var currentExtraInterraction = None;

        if (extraInteractionsTracker.Has(pawn))
        {
            currentExtraInterraction = extraInteractionsTracker[pawn];
        }

        switch (pawn.guest.interactionMode.defName)
        {
            case "Release":
                if (currentExtraInterraction is not (ReleaseWhenHealthy
                    or None))
                {
                    extraInteractionsTracker[pawn] = None;
                }

                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReleaseWhenHealthy),
                    delegate { extraInteractionsTracker[pawn] = ReleaseWhenHealthy; },
                    MenuOptionPriority.Default, null, null, 29f));

                break;
            case "Convert":
                if (currentExtraInterraction is ReleaseWhenHealthy)
                {
                    extraInteractionsTracker[pawn] = None;
                }

                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(Recruit),
                    delegate { extraInteractionsTracker[pawn] = Recruit; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReduceResistance),
                    delegate { extraInteractionsTracker[pawn] = ReduceResistance; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReduceResistanceThenRecruit),
                    delegate
                    {
                        extraInteractionsTracker[pawn] =
                            ReduceResistanceThenRecruit;
                    },
                    MenuOptionPriority.Default, null, null, 29f));

                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(Enslave),
                    delegate { extraInteractionsTracker[pawn] = Enslave; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReduceWill),
                    delegate { extraInteractionsTracker[pawn] = ReduceWill; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReduceWillThenEnslave),
                    delegate { extraInteractionsTracker[pawn] = ReduceWillThenEnslave; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(Release),
                    delegate { extraInteractionsTracker[pawn] = Release; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReduceResistanceThenRelease),
                    delegate
                    {
                        extraInteractionsTracker[pawn] =
                            ReduceResistanceThenRelease;
                    },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(Kill),
                    delegate { extraInteractionsTracker[pawn] = Kill; },
                    MenuOptionPriority.Default, null, null, 29f));
                __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(ReduceResistanceThenKill),
                    delegate
                    {
                        extraInteractionsTracker[pawn] =
                            ReduceResistanceThenKill;
                    },
                    MenuOptionPriority.Default, null, null, 29f));
                break;
            default:
                extraInteractionsTracker[pawn] = None;
                break;
        }
    }

    public static void Postfix(ref Vector2 ___size, Tuple<List<FloatMenuOption>, string> __state)
    {
        if (__state == null || !__state.Item1.Any())
        {
            return;
        }

        if (Find.Selector.SingleSelectedThing is not Pawn pawn)
        {
            return;
        }

        var extraInteractionsTracker = pawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null)
        {
            return;
        }

        if (__state.Item2 != pawn.guest.interactionMode.defName)
        {
            switch (pawn.guest.interactionMode.defName)
            {
                case "Release":
                    extraInteractionsTracker[pawn] =
                        CustomPrisonerInteractionsMod.instance.Settings.DefaultReleaseValue;
                    break;
                case "Convert":
                    extraInteractionsTracker[pawn] =
                        CustomPrisonerInteractionsMod.instance.Settings.DefaultConvertValue;
                    break;
            }
        }

        ___size += new Vector2(0, buttonSpace);

        __state.Item1.Insert(0,
            new FloatMenuOption(getExtraInterractionExplanation(None),
                delegate { extraInteractionsTracker[pawn] = None; },
                MenuOptionPriority.Default, null, null, 29f));

        var buttonArea = new Rect(0f + margin, ___size.y - buttonSpace - margin, ___size.x - (margin * 2), buttonSpace);

        Widgets.Label(buttonArea, "CPI.options".Translate());

        var currentExtraInterraction = None;

        if (extraInteractionsTracker.Has(pawn))
        {
            currentExtraInterraction = extraInteractionsTracker[pawn];
        }

        string currentExtraInterractionText = getExtraInterractionExplanation(currentExtraInterraction);

        TooltipHandler.TipRegion(buttonArea.RightPart(0.7f), currentExtraInterractionText);

        if (currentExtraInterractionText.Length > buttonText + 3)
        {
            currentExtraInterractionText = $"{currentExtraInterractionText.Substring(0, buttonText)}...";
        }

        if (Widgets.ButtonText(buttonArea.RightPart(0.7f), currentExtraInterractionText))
        {
            Find.WindowStack.Add(new FloatMenu(__state.Item1));
        }
    }
}