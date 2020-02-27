using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;


//namespace MoloyTraits
//{
class MT_ThoughtWorker_VenturousQuestCompletedMood : ThoughtWorker
     {
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        MoloyTraitChecker moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        if (moloyTraitChecker == null) return ThoughtState.Inactive;

        if (moloyTraitChecker.ticksSinceQuestCompleted > 3600000) return ThoughtState.ActiveAtStage(0);

        if (moloyTraitChecker.ticksSinceQuestCompleted > 1800000) return ThoughtState.ActiveAtStage(1);

        if (moloyTraitChecker.ticksSinceQuestCompleted > 900000) return ThoughtState.ActiveAtStage(2);

        if (moloyTraitChecker.ticksSinceQuestCompleted > 420000) return ThoughtState.ActiveAtStage(3);

        return ThoughtState.Inactive;
    }

    }
//}
