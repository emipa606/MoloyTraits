using RimWorld;
using Verse;

namespace MoloyTraits;

internal class MT_ThoughtWorker_ExHasLover : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (LovePartnerRelationUtility.HasAnyLovePartner(p))
        {
            return false;
        }


        if (!LovePartnerRelationUtility.HasAnyExLovePartnerOfTheOppositeGender(p) &&
            !LovePartnerRelationUtility.HasAnyExLovePartnerOfTheSameGender(p))
        {
            return false;
        }

        var exLoverPawn = p.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.ExSpouse);
        if (exLoverPawn != null)
        {
            return LovePartnerRelationUtility.HasAnyLovePartner(exLoverPawn);
        }

        exLoverPawn = p.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.ExLover);
        return exLoverPawn != null && LovePartnerRelationUtility.HasAnyLovePartner(exLoverPawn);
    }
}