using RimWorld;
using Verse;


//namespace MoloyTraits
//{
internal class MT_ThoughtWorker_VenturousQuestCompletedMood : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        var moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        if (moloyTraitChecker == null)
        {
            return ThoughtState.Inactive;
        }

        switch (moloyTraitChecker.ticksSinceQuestCompleted)
        {
            case > 3600000:
                return ThoughtState.ActiveAtStage(0);
            case > 1800000:
                return ThoughtState.ActiveAtStage(1);
            case > 900000:
                return ThoughtState.ActiveAtStage(2);
            case > 420000:
                return ThoughtState.ActiveAtStage(3);
            default:
                return ThoughtState.Inactive;
        }
    }
}
//}