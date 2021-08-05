using RimWorld;
using Verse;

//namespace MoloyTraits
//{
internal class MT_ThoughtWorker_TyrantNear : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.health.hediffSet.HasHediff(MoloyTraitDefOf.MT_TyrantIsNear))
        {
            return true;
        }

        return false;
    }
}
//}