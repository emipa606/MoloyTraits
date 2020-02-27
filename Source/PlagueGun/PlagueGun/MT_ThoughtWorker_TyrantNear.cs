using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

//namespace MoloyTraits
//{
    class MT_ThoughtWorker_TyrantNear : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.health.hediffSet.HasHediff(MoloyTraitDefOf.MT_TyrantIsNear)) return true; else return false;
        }
    }
//}
