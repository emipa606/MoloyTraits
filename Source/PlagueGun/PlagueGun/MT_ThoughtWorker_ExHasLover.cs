using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

//namespace MoloyTraits
//{
class MT_ThoughtWorker_ExHasLover : ThoughtWorker
    {
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (LovePartnerRelationUtility.HasAnyLovePartner(p))
        {
            return false;
        }

        

        if(!LovePartnerRelationUtility.HasAnyExLovePartnerOfTheOppositeGender(p) && !LovePartnerRelationUtility.HasAnyExLovePartnerOfTheSameGender(p))
        {
            return false;
        }

        Pawn exLoverPawn = p.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.ExSpouse);
        if(exLoverPawn == null)
        {
            exLoverPawn = p.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.ExLover);
            if(exLoverPawn == null)
            {
                return false;
            }
        }

        if (LovePartnerRelationUtility.HasAnyLovePartner(exLoverPawn))
        {
            return true;
        }
        
        return false;
    }
}
//}
