using RimWorld;
using Verse;

namespace MoloyTraits;

internal class MT_ThoughtWorker_TyrantNear : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        return p.health.hediffSet.HasHediff(MoloyTraitDefOf.MT_TyrantIsNear);
    }
}