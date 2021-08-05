using RimWorld;
using Verse;


/*namespace MoloyTraits
{*/
internal class MT_ThoughtWorker_VenturousQuestStatusMood : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        var moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

        if (moloyTraitChecker == null)
        {
            return ThoughtState.Inactive;
        }

        var finalMod = 0;
        foreach (var questMood in moloyTraitChecker.questMoods)
        {
            finalMod += questMood.moodStateMod;
        }

        switch (finalMod)
        {
            case 0:
                return ThoughtState.Inactive;
            case <= -4:
                return ThoughtState.ActiveAtStage(0);
            case -3:
                return ThoughtState.ActiveAtStage(1);
            case -2:
                return ThoughtState.ActiveAtStage(2);
            case -1:
                return ThoughtState.ActiveAtStage(3);
            case 1:
                return ThoughtState.ActiveAtStage(4);
            case 2:
                return ThoughtState.ActiveAtStage(5);
            default:
                return ThoughtState.ActiveAtStage(6);
        }
    }
}
//}