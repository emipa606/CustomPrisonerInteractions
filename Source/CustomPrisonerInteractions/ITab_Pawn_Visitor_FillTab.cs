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

        __state = new Tuple<List<FloatMenuOption>, string>([],
            pawn.guest.ExclusiveInteractionMode.defName);

        var extraInteractionsTracker = pawn.Map.GetExtraInteractionsTracker();
        if (extraInteractionsTracker == null)
        {
            return;
        }

        var currentExtraInterraction = Undefined;

        if (extraInteractionsTracker.Has(pawn))
        {
            currentExtraInterraction = extraInteractionsTracker[pawn];
        }

        switch (pawn.guest.ExclusiveInteractionMode.defName)
        {
            case "Release":
                if (currentExtraInterraction is not (ReleaseWhenHealthy or ReleaseWhenAbleToWalk or ReleaseWhenNotGuilty
                    or None
                    ))
                {
                    extraInteractionsTracker[pawn] = Undefined;
                }

                foreach (var extraMode in new List<ExtraMode>
                         {
                             ReleaseWhenNotGuilty,
                             ReleaseWhenAbleToWalk,
                             ReleaseWhenHealthy
                         })
                {
                    if (CanUseExtraMode(pawn, extraMode))
                    {
                        __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(extraMode),
                            delegate { extraInteractionsTracker[pawn] = extraMode; },
                            MenuOptionPriority.Default, null, null, 29f));
                    }
                }

                break;
            case "Convert":
            case "PrisonLabor_workAndConvertOption":
                if (currentExtraInterraction is ReleaseWhenHealthy or ReleaseWhenAbleToWalk or ReleaseWhenNotGuilty)
                {
                    extraInteractionsTracker[pawn] = Undefined;
                }

                foreach (var extraMode in new List<ExtraMode>
                         {
                             Recruit,
                             ReduceResistance,
                             ReduceResistanceThenRecruit,
                             Enslave,
                             ReduceWill,
                             ReduceWillThenEnslave,
                             Release,
                             ReduceResistanceThenRelease,
                             Kill,
                             ReduceResistanceThenKill
                         })
                {
                    if (CanUseExtraMode(pawn, extraMode))
                    {
                        __state.Item1.Add(new FloatMenuOption(getExtraInterractionExplanation(extraMode),
                            delegate { extraInteractionsTracker[pawn] = extraMode; },
                            MenuOptionPriority.Default, null, null, 29f));
                    }
                }

                break;
            default:
                extraInteractionsTracker[pawn] = Undefined;
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

        if (!extraInteractionsTracker.Has(pawn))
        {
            TryMigrateExtraInteraction(pawn, out _);
        }

        var hasExtraInteraction = extraInteractionsTracker.Has(pawn);
        var currentExtraInterraction = hasExtraInteraction ? extraInteractionsTracker[pawn] : Undefined;

        if (__state.Item2 != pawn.guest.ExclusiveInteractionMode.defName && !hasExtraInteraction ||
            currentExtraInterraction == Undefined)
        {
            switch (pawn.guest.ExclusiveInteractionMode.defName)
            {
                case "Release":
                    if (CanUseExtraMode(pawn, CustomPrisonerInteractionsMod.Instance.Settings.DefaultReleaseValue))

                    {
                        extraInteractionsTracker[pawn] =
                            CustomPrisonerInteractionsMod.Instance.Settings.DefaultReleaseValue;
                        currentExtraInterraction = extraInteractionsTracker[pawn];
                    }

                    break;
                case "Convert":
                case "PrisonLabor_workAndConvertOption":
                    if (CanUseExtraMode(pawn, CustomPrisonerInteractionsMod.Instance.Settings.DefaultConvertValue))
                    {
                        extraInteractionsTracker[pawn] =
                            CustomPrisonerInteractionsMod.Instance.Settings.DefaultConvertValue;
                        currentExtraInterraction = extraInteractionsTracker[pawn];
                    }

                    break;
            }
        }

        ___size += new Vector2(0, buttonSpace);

        __state.Item1.Insert(0,
            new FloatMenuOption(getExtraInterractionExplanation(None),
                delegate { extraInteractionsTracker[pawn] = None; },
                MenuOptionPriority.Default, null, null, 29f));

        var buttonArea = new Rect(0f + margin, ___size.y - buttonSpace - margin, ___size.x - (margin * 2),
            buttonSpace);

        Widgets.Label(buttonArea, "CPI.options".Translate());

        string currentExtraInterractionText = getExtraInterractionExplanation(currentExtraInterraction);

        TooltipHandler.TipRegion(buttonArea.RightPart(0.7f), currentExtraInterractionText);

        if (currentExtraInterractionText.Length > buttonText + 3)
        {
            currentExtraInterractionText = $"{currentExtraInterractionText[..buttonText]}...";
        }

        if (Widgets.ButtonText(buttonArea.RightPart(0.7f), currentExtraInterractionText))
        {
            Find.WindowStack.Add(new FloatMenu(__state.Item1));
        }
    }
}