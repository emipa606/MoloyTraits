using RimWorld;
using Verse;

namespace MoloyTraits;

internal class MT_ThoughtWorker_ExHasNoLover : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!LovePartnerRelationUtility.HasAnyLovePartner(p))
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
            return !LovePartnerRelationUtility.HasAnyLovePartner(exLoverPawn);
        }

        exLoverPawn = p.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.ExLover);
        if (exLoverPawn == null)
        {
            return false;
        }

        return !LovePartnerRelationUtility.HasAnyLovePartner(exLoverPawn);
    }
}