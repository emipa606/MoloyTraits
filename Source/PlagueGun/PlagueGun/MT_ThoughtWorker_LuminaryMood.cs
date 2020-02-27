using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

//namespace MoloyTraits
//{
class MT_ThoughtWorker_LuminaryMood : ThoughtWorker
{
   

    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        MoloyTraitChecker moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        if (moloyTraitChecker == null) return ThoughtState.Inactive;

        if(!moloyTraitChecker.pawnLumed(p)) return ThoughtState.Inactive;

        int lumeLevel = moloyTraitChecker.pawnLumeStage(p);
        if (lumeLevel==99) return ThoughtState.Inactive;
        return ThoughtState.ActiveAtStage(lumeLevel);
    }
}
//}
