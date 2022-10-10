using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoloyTraits;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    private static readonly Type patchType = typeof(HarmonyPatches);

    static HarmonyPatches()
    {
        var harmony = new Harmony("rimworld.moloy.moloytraits.main");

        harmony.Patch(AccessTools.Method(typeof(Quest), nameof(Quest.End)),
            postfix: new HarmonyMethod(patchType, nameof(QuestEndPostFix)));
    }

    public static void QuestEndPostFix(Quest __instance, QuestEndOutcome outcome, bool sendLetter = true)
    {
        var moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        switch (outcome)
        {
            case QuestEndOutcome.Fail:
                moloyTraitChecker.questMoods.Add(new MoloyTraitChecker.QuestMood(840000, -1));
                break;

            case QuestEndOutcome.Success:
                moloyTraitChecker.questMoods.Add(new MoloyTraitChecker.QuestMood(900000, +1));
                break;
        }

        moloyTraitChecker.ticksSinceQuestCompleted = 0;
    }
}