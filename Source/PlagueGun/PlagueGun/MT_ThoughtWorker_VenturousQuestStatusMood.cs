using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;


/*namespace MoloyTraits
{*/
class MT_ThoughtWorker_VenturousQuestStatusMood : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        MoloyTraitChecker moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        if (moloyTraitChecker == null) return ThoughtState.Inactive;

        int finalMod = 0;
        foreach(MoloyTraitChecker.QuestMood questMood in moloyTraitChecker.questMoods)
        {
            finalMod += questMood.moodStateMod;
        }

        if (finalMod == 0) return ThoughtState.Inactive;
        if (finalMod <= -4) return ThoughtState.ActiveAtStage(0);
        if (finalMod == -3) return ThoughtState.ActiveAtStage(1);
        if (finalMod == -2) return ThoughtState.ActiveAtStage(2);
        if (finalMod == -1) return ThoughtState.ActiveAtStage(3);
        if (finalMod == 1) return ThoughtState.ActiveAtStage(4);
        if (finalMod == 2) return ThoughtState.ActiveAtStage(5);
        return ThoughtState.ActiveAtStage(6);
    }

}
//}
