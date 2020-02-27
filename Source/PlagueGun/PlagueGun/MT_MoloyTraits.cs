using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;
using HarmonyLib;


//namespace MoloyTraits
//{
[DefOf]
public static class MoloyTraitDefOf
{
    public static TraitDef MT_Conductor;
    public static TraitDef MT_Commander;
    public static TraitDef MT_Tyrant;
    public static TraitDef MT_Peppy;
    public static TraitDef MT_Volatile_Crafter;
    public static TraitDef MT_Luminary;

    public static HediffDef MT_ConductorIsNear;
    public static HediffDef MT_CommanderIsNear;
    public static HediffDef MT_TyrantIsNear;
    public static InspirationDef Frenzy_Go;
}

public enum MOLOYTRAIT
{
    CONDUCTOR,
    COMMANDER,
    TYRANT,
    PEPPY,
    VOLATILE_CRAFTER,
    LUMINARY
}

public class MoloyTraitChecker : WorldComponent
{
    private List<BodyPartDef> MT_BPD = new List<BodyPartDef>();

    private List<MoloyTrait> moloyTraits = new List<MoloyTrait>()
    {
        new MoloyTrait(MOLOYTRAIT.COMMANDER, MoloyTraitDefOf.MT_Commander, 25),
        new MoloyTrait(MOLOYTRAIT.CONDUCTOR, MoloyTraitDefOf.MT_Conductor, 25),
        new MoloyTrait(MOLOYTRAIT.TYRANT, MoloyTraitDefOf.MT_Tyrant, 25),
        new MoloyTrait(MOLOYTRAIT.PEPPY, MoloyTraitDefOf.MT_Peppy,0),
        new MoloyTrait(MOLOYTRAIT.VOLATILE_CRAFTER, MoloyTraitDefOf.MT_Volatile_Crafter,0),
        new MoloyTrait(MOLOYTRAIT.LUMINARY,MoloyTraitDefOf.MT_Luminary, 9999)

    };

    List<Map> maps;

    public List<QuestMood> questMoods = new List<QuestMood>();

    Dictionary<Pawn, LumedPawn> lumedPawns = new Dictionary<Pawn, LumedPawn>();

    private int ticksUntillNextInspirationCheck = 60000;
    private int ticksUntillNextInspirationCheckMax = 60000;
    private int ticksUntilNextTraitCheck = 120;
    private int ticksUntilNextTraitCheckMax = 120;
    public int ticksSinceQuestCompleted = 0;

    public MoloyTraitChecker(World world) : base(world)
    {
        MT_BPD.Add(BodyPartDefOf.Brain);
    }

    public override void WorldComponentTick()
    {

        ticksUntillNextInspirationCheck--;
        ticksUntilNextTraitCheck--;
        ticksSinceQuestCompleted++;

        foreach(QuestMood questMood in questMoods)
        {
            questMood.ticksTillRemoved--;
            if(questMood.ticksTillRemoved <= 0)
            {
                questMoods.Remove(questMood);
            }
        }


        if (ticksUntilNextTraitCheck <= 0)
        {
            lumedPawns.Clear();

            ticksUntilNextTraitCheck = ticksUntilNextTraitCheckMax;
            maps = Find.Maps;
            foreach (Map map in maps)
            {
                List<Pawn> pawns = map.mapPawns.AllPawnsSpawned;
                foreach (Pawn pawn in pawns)
                {
                    foreach (MoloyTrait moloyTrait in moloyTraits)
                    {
                        bool? hasTrait = pawn.story?.traits?.HasTrait(moloyTrait.traitDef);
                        if (hasTrait == true)
                        {
                            switch (moloyTrait.TRAIT)
                            {
                                case MOLOYTRAIT.PEPPY:
                                    CheckForInspiration(pawn, MoloyTraitDefOf.Frenzy_Go, 0.05f);
                                    break;
                                case MOLOYTRAIT.VOLATILE_CRAFTER:
                                    CheckForInspiration(pawn, InspirationDefOf.Inspired_Creativity, 0.1f);
                                    break;
                                case MOLOYTRAIT.CONDUCTOR:
                                    foreach (Pawn affectedPawn in pawns)
                                    {
                                        if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                        {
                                            MAddHediff(affectedPawn, MoloyTraitDefOf.MT_ConductorIsNear, MT_BPD);
                                        }
                                    }
                                    break;
                                case MOLOYTRAIT.COMMANDER:
                                    foreach (Pawn affectedPawn in pawns)
                                    {
                                        if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                        {
                                            MAddHediff(affectedPawn, MoloyTraitDefOf.MT_CommanderIsNear, MT_BPD);
                                        }
                                    }
                                    break;
                                case MOLOYTRAIT.TYRANT:
                                    foreach (Pawn affectedPawn in pawns)
                                    {
                                        if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                        {
                                            MAddHediff(affectedPawn, MoloyTraitDefOf.MT_TyrantIsNear, MT_BPD);
                                        }
                                    }
                                    break;
                                case MOLOYTRAIT.LUMINARY:
                                    foreach (Pawn affectedPawn in pawns)
                                    {
                                        if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                        {
                                            if (pawnLumed(affectedPawn))
                                            {
                                                if (lumedPawns[affectedPawn].distanceTo > pawn.Position.DistanceTo(affectedPawn.Position))
                                                {
                                                    lumedPawns[affectedPawn].distanceTo = pawn.Position.DistanceTo(affectedPawn.Position);
                                                    lumedPawns[affectedPawn].lumeStage = LumedPawn.getStage(pawn);
                                                }
                                            }
                                            else
                                            {
                                                lumedPawns.Add(affectedPawn, new LumedPawn(LumedPawn.getStage(pawn), pawn.Position.DistanceTo(affectedPawn.Position)));
                                            }
                                            //MAddHediff(affectedPawn, MoloyTraitDefOf.MT_TyrantIsNear, MT_BPD);
                                        }
                                    }
                                    break;
                            }

                        }
                    }

                }
            }

            if (ticksUntillNextInspirationCheck <= 0)
            {
                ticksUntillNextInspirationCheck = ticksUntillNextInspirationCheckMax;
            }
        }



    }

    void CheckForInspiration(Pawn pawn, InspirationDef inspiration, float chance)
    {
        if (ticksUntillNextInspirationCheck <= 0)
        {
            if (Rand.Chance(chance))
            {
                pawn.mindState.inspirationHandler.TryStartInspiration(inspiration);
            }
        }
    }

    void MAddHediff(Pawn pawn, HediffDef hediffDef, List<BodyPartDef> bodyPartDefs)
    {
        if (pawn.health.hediffSet.HasHediff(hediffDef))
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            pawn.health.RemoveHediff(hediff);
        };

        bool tryThisHediff = HediffGiverUtility.TryApply(pawn, hediffDef, bodyPartDefs);
    }

    public bool pawnLumed(Pawn pawn)
    {
        return lumedPawns.ContainsKey(pawn);
    }

    public int pawnLumeStage(Pawn pawn)
    {
        return lumedPawns[pawn].lumeStage;
    }

    bool checkAffected(Pawn _active, Pawn _affected, float _range)
    {
        if (_active == _affected) return false;
        if (_active.Faction != _affected.Faction) return false;
        if (_active.Position.DistanceTo(_affected.Position) > _range) return false;
        return true;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ticksUntilNextTraitCheck, "ticksUntilNextTraitCheck");
        Scribe_Values.Look(ref ticksUntillNextInspirationCheck, "ticksUntillNextInspirationCheck");
        Scribe_Values.Look(ref ticksSinceQuestCompleted, "ticksSinceQuestCompleted");
        Scribe_Collections.Look(ref questMoods, "questMoods", LookMode.Deep);
    }

    private class MoloyTrait
    {
        public MOLOYTRAIT TRAIT;
        public TraitDef traitDef;
        public float range;

        public MoloyTrait(MOLOYTRAIT _TRAIT, TraitDef _traitDef, float _range)
        {
            TRAIT = _TRAIT;
            traitDef = _traitDef;
            range = _range;
        }




    }

    private class LumedPawn
    {
        public int lumeStage;
        public float distanceTo;

        public LumedPawn(int _lumeStage, float _distance)
        {
            lumeStage = _lumeStage;
            distanceTo = _distance;
        }

        public static int getStage(Pawn _lumminary)
        {
            float mood = _lumminary.needs.mood.CurLevel;

            if (mood < 0.2f)
            {
                return 0;
            }

            if (mood < 0.4f)
            {
                return 1;
            }

            if (mood < 0.65f)
            {
                return 99;
            }

            if (mood < 0.9f)
            {
                return 2;
            }

            return 3;
        }
    }

    public class QuestMood : IExposable
    {
        public int ticksTillRemoved;
        public int moodStateMod;

        public QuestMood()
        {

        }

        public QuestMood(int ticksTillRemoved, int moodStateMod)
        {
            this.ticksTillRemoved = ticksTillRemoved;
            this.moodStateMod = moodStateMod;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref ticksTillRemoved, "ticksTillRemoved",0);
            Scribe_Values.Look(ref moodStateMod, "moodStateMod",0);
        }
    }
}


//}

namespace MoloyHarmony
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);
        static HarmonyPatches()
        {

            Harmony harmony = new Harmony("rimworld.moloy.moloytraits.main");

            harmony.Patch(original: AccessTools.Method(type: typeof(Quest), name: nameof(Quest.End)), postfix: new HarmonyMethod(methodType: patchType, methodName: nameof(QuestEndPostFix)));


        }

        public static void QuestEndPostFix(Quest __instance, QuestEndOutcome outcome, bool sendLetter = true)
        {
            MoloyTraitChecker moloyTraitChecker = Find.World.GetComponent<MoloyTraitChecker>();

            switch (outcome)
            {
                case QuestEndOutcome.Fail:
                    moloyTraitChecker.questMoods.Add(new MoloyTraitChecker.QuestMood(840000, -1));
                    break;

                case QuestEndOutcome.Success:
                    moloyTraitChecker.questMoods.Add(new MoloyTraitChecker.QuestMood(900000, +1));
                    break;
            }
            moloyTraitChecker.ticksSinceQuestCompleted = 0;

        }
    }


}
