using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class TargetingParameters
	{
		public bool canTargetLocations;
		public bool canTargetSelf;
		public bool canTargetPawns = true;
		public bool canTargetFires;
		public bool canTargetBuildings = true;
		public bool canTargetItems;
		public List<Faction> onlyTargetFactions;
		public Predicate<TargetInfo> validator;
		public bool onlyTargetFlammables;
		public Thing targetSpecificThing;
		public bool mustBeSelectable;
		public bool neverTargetDoors;
		public bool neverTargetIncapacitated;
		public bool onlyTargetBarriers;
		public bool onlyTargetDamagedThings;
		public bool worldObjectTargetsMustBeAutoAttackable = true;
		public bool onlyTargetIncapacitatedPawns;
		public bool CanTarget(TargetInfo targ)
		{
			if (this.validator != null && !this.validator(targ))
			{
				return false;
			}
			if (targ.Thing == null)
			{
				return this.canTargetLocations;
			}
			if (this.neverTargetDoors && targ.Thing.def.thingClass == typeof(Building_Door))
			{
				return false;
			}
			if (this.onlyTargetDamagedThings && targ.Thing.HitPoints == targ.Thing.MaxHitPoints)
			{
				return false;
			}
			if (this.onlyTargetFlammables && !targ.Thing.FlammableNow)
			{
				return false;
			}
			if (this.mustBeSelectable && !targ.Thing.SelectableNow())
			{
				return false;
			}
			if (this.targetSpecificThing != null && targ.Thing == this.targetSpecificThing)
			{
				return true;
			}
			if (this.canTargetFires && targ.Thing.def == ThingDefOf.Fire)
			{
				return true;
			}
			if (this.canTargetPawns && targ.Thing.def.category == ThingCategory.Pawn)
			{
				if (((Pawn)targ.Thing).Downed)
				{
					if (this.neverTargetIncapacitated)
					{
						return false;
					}
				}
				else
				{
					if (this.onlyTargetIncapacitatedPawns)
					{
						return false;
					}
				}
				return this.onlyTargetFactions == null || this.onlyTargetFactions.Contains(targ.Thing.Faction);
			}
			if (this.canTargetBuildings && targ.Thing.def.category == ThingCategory.Building)
			{
				return (!this.onlyTargetBarriers || targ.Thing.def.regionBarrier) && (this.onlyTargetFactions == null || this.onlyTargetFactions.Contains(targ.Thing.Faction));
			}
			return this.canTargetItems && (!this.worldObjectTargetsMustBeAutoAttackable || targ.Thing.def.isAutoAttackableWorldObject);
		}
		public static TargetingParameters ForSelf(Pawn p)
		{
			return new TargetingParameters
			{
				targetSpecificThing = p,
				canTargetPawns = false,
				canTargetBuildings = false,
				worldObjectTargetsMustBeAutoAttackable = false
			};
		}
		public static TargetingParameters ForArrest(Pawn arrester)
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetBuildings = false,
				worldObjectTargetsMustBeAutoAttackable = false,
				validator = delegate(TargetInfo targ)
				{
					if (!targ.HasThing)
					{
						return false;
					}
					Pawn pawn = targ.Thing as Pawn;
					return pawn != null && pawn != arrester && pawn.CanBeArrested();
				}
			};
		}
		public static TargetingParameters ForAttackHostile()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = true;
			targetingParameters.canTargetItems = true;
			targetingParameters.worldObjectTargetsMustBeAutoAttackable = true;
			targetingParameters.validator = ((TargetInfo targ) => targ.HasThing && (targ.Thing.HostileTo(Faction.OfColony) || (targ.Thing is Pawn && !((Pawn)targ.Thing).RaceProps.Humanlike)));
			return targetingParameters;
		}
		public static TargetingParameters ForAttackAny()
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetBuildings = true,
				canTargetItems = true,
				worldObjectTargetsMustBeAutoAttackable = true
			};
		}
		public static TargetingParameters ForRescue(Pawn p)
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				onlyTargetIncapacitatedPawns = true,
				canTargetBuildings = false,
				worldObjectTargetsMustBeAutoAttackable = false
			};
		}
		public static TargetingParameters ForStrip(Pawn p)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetItems = true;
			targetingParameters.worldObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = ((TargetInfo targ) => targ.HasThing && StrippableUtility.CanBeStrippedByColony(targ.Thing));
			return targetingParameters;
		}
		public static TargetingParameters ForTrade()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.worldObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = delegate(TargetInfo x)
			{
				ITrader trader = x.Thing as ITrader;
				return trader != null && trader.CanTradeNow;
			};
			return targetingParameters;
		}
	}
}
