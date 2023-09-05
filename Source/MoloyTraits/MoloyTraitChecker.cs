using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MoloyTraits;

public class MoloyTraitChecker : WorldComponent
{
    private readonly Dictionary<Pawn, LumedPawn> lumedPawns = new Dictionary<Pawn, LumedPawn>();

    private readonly List<MoloyTrait> moloyTraits = new List<MoloyTrait>
    {
        new MoloyTrait(MOLOYTRAIT.COMMANDER, MoloyTraitDefOf.MT_Commander, 25),
        new MoloyTrait(MOLOYTRAIT.CONDUCTOR, MoloyTraitDefOf.MT_Conductor, 25),
        new MoloyTrait(MOLOYTRAIT.TYRANT, MoloyTraitDefOf.MT_Tyrant, 25),
        new MoloyTrait(MOLOYTRAIT.PEPPY, MoloyTraitDefOf.MT_Peppy, 0),
        new MoloyTrait(MOLOYTRAIT.VOLATILE_CRAFTER, MoloyTraitDefOf.MT_Volatile_Crafter, 0),
        new MoloyTrait(MOLOYTRAIT.LUMINARY, MoloyTraitDefOf.MT_Luminary, 9999)
    };

    private readonly List<BodyPartDef> MT_BPD = new List<BodyPartDef>();
    private readonly int ticksUntillNextInspirationCheckMax = 60000;
    private readonly int ticksUntilNextTraitCheckMax = 120;

    private List<Map> maps;

    public List<QuestMood> questMoods = new List<QuestMood>();
    public int ticksSinceQuestCompleted;

    private int ticksUntillNextInspirationCheck = 60000;
    private int ticksUntilNextTraitCheck = 120;

    public MoloyTraitChecker(World world) : base(world)
    {
        MT_BPD.Add(BodyPartDefOf.Brain);
    }

    public override void WorldComponentTick()
    {
        ticksUntillNextInspirationCheck--;
        ticksUntilNextTraitCheck--;
        ticksSinceQuestCompleted++;

        foreach (var questMood in questMoods)
        {
            questMood.ticksTillRemoved--;
            if (questMood.ticksTillRemoved <= 0)
            {
                questMoods.Remove(questMood);
            }
        }

        if (ticksUntilNextTraitCheck > 0)
        {
            return;
        }

        lumedPawns.Clear();

        ticksUntilNextTraitCheck = ticksUntilNextTraitCheckMax;
        maps = Find.Maps;
        foreach (var map in maps)
        {
            var pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.RaceProps.Humanlike);
            foreach (var pawn in pawns)
            {
                var listOfSameFactionPawns = new List<Pawn>();
                foreach (var moloyTrait in moloyTraits)
                {
                    if (pawn.story?.traits?.HasTrait(moloyTrait.traitDef) != true)
                    {
                        continue;
                    }

                    switch (moloyTrait.TRAIT)
                    {
                        case MOLOYTRAIT.PEPPY:
                            CheckForInspiration(pawn, MoloyTraitDefOf.Frenzy_Go, 0.05f);
                            break;
                        case MOLOYTRAIT.VOLATILE_CRAFTER:
                            CheckForInspiration(pawn, InspirationDefOf.Inspired_Creativity, 0.1f);
                            break;
                        case MOLOYTRAIT.CONDUCTOR:
                            if (pawn.Faction == null)
                            {
                                break;
                            }

                            if (!listOfSameFactionPawns.Any())
                            {
                                listOfSameFactionPawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
                                    .Where(x => x.RaceProps.Humanlike).ToList();
                            }

                            foreach (var affectedPawn in listOfSameFactionPawns)
                            {
                                if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                {
                                    MAddHediff(affectedPawn, MoloyTraitDefOf.MT_ConductorIsNear, MT_BPD);
                                }
                            }

                            break;
                        case MOLOYTRAIT.COMMANDER:
                            if (pawn.Faction == null)
                            {
                                break;
                            }

                            if (!listOfSameFactionPawns.Any())
                            {
                                listOfSameFactionPawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
                                    .Where(x => x.RaceProps.Humanlike).ToList();
                            }

                            foreach (var affectedPawn in listOfSameFactionPawns)
                            {
                                if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                {
                                    MAddHediff(affectedPawn, MoloyTraitDefOf.MT_CommanderIsNear, MT_BPD);
                                }
                            }

                            break;
                        case MOLOYTRAIT.TYRANT:
                            if (pawn.Faction == null)
                            {
                                break;
                            }

                            if (!listOfSameFactionPawns.Any())
                            {
                                listOfSameFactionPawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
                                    .Where(x => x.RaceProps.Humanlike).ToList();
                            }

                            foreach (var affectedPawn in listOfSameFactionPawns)
                            {
                                if (checkAffected(pawn, affectedPawn, moloyTrait.range))
                                {
                                    MAddHediff(affectedPawn, MoloyTraitDefOf.MT_TyrantIsNear, MT_BPD);
                                }
                            }

                            break;
                        case MOLOYTRAIT.LUMINARY:
                            if (pawn.Faction == null)
                            {
                                break;
                            }

                            if (!listOfSameFactionPawns.Any())
                            {
                                listOfSameFactionPawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
                                    .Where(x => x.RaceProps.Humanlike).ToList();
                            }

                            foreach (var affectedPawn in listOfSameFactionPawns)
                            {
                                if (!checkAffected(pawn, affectedPawn, moloyTrait.range))
                                {
                                    continue;
                                }

                                var distance = pawn.Position.DistanceTo(affectedPawn.Position);

                                if (pawnLumed(affectedPawn))
                                {
                                    if (!(lumedPawns[affectedPawn].distanceTo > distance))
                                    {
                                        continue;
                                    }

                                    lumedPawns[affectedPawn].distanceTo = distance;
                                    lumedPawns[affectedPawn].lumeStage = LumedPawn.getStage(pawn);
                                }
                                else
                                {
                                    lumedPawns.Add(affectedPawn, new LumedPawn(LumedPawn.getStage(pawn), distance));
                                }
                            }

                            break;
                    }
                }
            }
        }

        if (ticksUntillNextInspirationCheck <= 0)
        {
            ticksUntillNextInspirationCheck = ticksUntillNextInspirationCheckMax;
        }
    }

    private void CheckForInspiration(Pawn pawn, InspirationDef inspiration, float chance)
    {
        if (ticksUntillNextInspirationCheck > 0)
        {
            return;
        }

        if (Rand.Chance(chance))
        {
            pawn.mindState.inspirationHandler.TryStartInspiration(inspiration);
        }
    }

    private void MAddHediff(Pawn pawn, HediffDef hediffDef, List<BodyPartDef> bodyPartDefs)
    {
        if (pawn.health.hediffSet.HasHediff(hediffDef))
        {
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            pawn.health.RemoveHediff(hediff);
        }

        var unused = HediffGiverUtility.TryApply(pawn, hediffDef, bodyPartDefs);
    }

    public bool pawnLumed(Pawn pawn)
    {
        return lumedPawns.ContainsKey(pawn);
    }

    public int pawnLumeStage(Pawn pawn)
    {
        return lumedPawns[pawn].lumeStage;
    }

    private bool checkAffected(Pawn _active, Pawn _affected, float _range)
    {
        if (_active == _affected)
        {
            return false;
        }

        return !(_active.Position.DistanceTo(_affected.Position) > _range);
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ticksUntilNextTraitCheck, "ticksUntilNextTraitCheck");
        Scribe_Values.Look(ref ticksUntillNextInspirationCheck, "ticksUntillNextInspirationCheck");
        Scribe_Values.Look(ref ticksSinceQuestCompleted, "ticksSinceQuestCompleted");
        Scribe_Collections.Look(ref questMoods, "questMoods", LookMode.Deep);

        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        if (questMoods == null)
        {
            questMoods = new List<QuestMood>();
        }
    }

    private class MoloyTrait
    {
        public readonly float range;
        public readonly MOLOYTRAIT TRAIT;
        public readonly TraitDef traitDef;

        public MoloyTrait(MOLOYTRAIT _TRAIT, TraitDef _traitDef, float _range)
        {
            TRAIT = _TRAIT;
            traitDef = _traitDef;
            range = _range;
        }
    }

    private class LumedPawn
    {
        public float distanceTo;
        public int lumeStage;

        public LumedPawn(int _lumeStage, float _distance)
        {
            lumeStage = _lumeStage;
            distanceTo = _distance;
        }

        public static int getStage(Pawn _lumminary)
        {
            var mood = _lumminary.needs.mood.CurLevel;

            switch (mood)
            {
                case < 0.2f:
                    return 0;
                case < 0.4f:
                    return 1;
                case < 0.65f:
                    return 99;
                case < 0.9f:
                    return 2;
                default:
                    return 3;
            }
        }
    }

    public class QuestMood : IExposable
    {
        public int moodStateMod;
        public int ticksTillRemoved;

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
            Scribe_Values.Look(ref ticksTillRemoved, "ticksTillRemoved");
            Scribe_Values.Look(ref moodStateMod, "moodStateMod");
        }
    }
}