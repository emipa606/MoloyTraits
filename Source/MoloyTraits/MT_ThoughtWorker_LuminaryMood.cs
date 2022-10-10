using RimWorld;
using Verse;

namespace MoloyTraits;

internal class MT_ThoughtWorker_LuminaryMood : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        var moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        if (moloyTraitChecker == null)
        {
            return ThoughtState.Inactive;
        }

        if (!moloyTraitChecker.pawnLumed(p))
        {
            return ThoughtState.Inactive;
        }

        var lumeLevel = moloyTraitChecker.pawnLumeStage(p);
        return lumeLevel == 99 ? ThoughtState.Inactive : ThoughtState.ActiveAtStage(lumeLevel);
    }
}