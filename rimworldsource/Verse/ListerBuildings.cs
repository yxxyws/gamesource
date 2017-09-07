using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace Verse
{
	public sealed class ListerBuildings
	{
		public List<Building> allBuildingsColonist = new List<Building>();
		public HashSet<Building> allBuildingsColonistCombatTargets = new HashSet<Building>();
		public HashSet<Building> allBuildingsColonistElecFire = new HashSet<Building>();
		public void Add(Building b)
		{
			if (b.def.building != null && b.def.building.isNaturalRock)
			{
				return;
			}
			if (b.Faction == Faction.OfColony)
			{
				this.allBuildingsColonist.Add(b);
				if (b is IAttackTarget)
				{
					this.allBuildingsColonistCombatTargets.Add(b);
				}
			}
			CompProperties_Power compProperties = b.def.GetCompProperties<CompProperties_Power>();
			if (compProperties != null && compProperties.startElectricalFires)
			{
				this.allBuildingsColonistElecFire.Add(b);
			}
		}
		public void Remove(Building b)
		{
			if (b.Faction == Faction.OfColony)
			{
				this.allBuildingsColonist.Remove(b);
				if (b is IAttackTarget)
				{
					this.allBuildingsColonistCombatTargets.Remove(b);
				}
			}
			CompProperties_Power compProperties = b.def.GetCompProperties<CompProperties_Power>();
			if (compProperties != null && compProperties.startElectricalFires)
			{
				this.allBuildingsColonistElecFire.Remove(b);
			}
		}
		public bool ColonistsHaveBuilding(ThingDef def)
		{
			for (int i = 0; i < this.allBuildingsColonist.Count; i++)
			{
				if (this.allBuildingsColonist[i].def == def)
				{
					return true;
				}
			}
			return false;
		}
		public bool ColonistsHaveResearchBench()
		{
			for (int i = 0; i < this.allBuildingsColonist.Count; i++)
			{
				if (this.allBuildingsColonist[i] is Building_ResearchBench)
				{
					return true;
				}
			}
			return false;
		}
		public bool ColonistsHaveBuildingWithPowerOn(ThingDef def)
		{
			for (int i = 0; i < this.allBuildingsColonist.Count; i++)
			{
				if (this.allBuildingsColonist[i].def == def)
				{
					CompPowerTrader compPowerTrader = this.allBuildingsColonist[i].TryGetComp<CompPowerTrader>();
					if (compPowerTrader == null || compPowerTrader.PowerOn)
					{
						return true;
					}
				}
			}
			return false;
		}
		[DebuggerHidden]
		public IEnumerable<Building> AllBuildingsColonistOfEType(ThingCategory eType)
		{
			ListerBuildings.<AllBuildingsColonistOfEType>c__Iterator15E <AllBuildingsColonistOfEType>c__Iterator15E = new ListerBuildings.<AllBuildingsColonistOfEType>c__Iterator15E();
			<AllBuildingsColonistOfEType>c__Iterator15E.eType = eType;
			<AllBuildingsColonistOfEType>c__Iterator15E.<$>eType = eType;
			<AllBuildingsColonistOfEType>c__Iterator15E.<>f__this = this;
			ListerBuildings.<AllBuildingsColonistOfEType>c__Iterator15E expr_1C = <AllBuildingsColonistOfEType>c__Iterator15E;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		[DebuggerHidden]
		public IEnumerable<Building> AllBuildingsColonistOfDef(ThingDef def)
		{
			ListerBuildings.<AllBuildingsColonistOfDef>c__Iterator15F <AllBuildingsColonistOfDef>c__Iterator15F = new ListerBuildings.<AllBuildingsColonistOfDef>c__Iterator15F();
			<AllBuildingsColonistOfDef>c__Iterator15F.def = def;
			<AllBuildingsColonistOfDef>c__Iterator15F.<$>def = def;
			<AllBuildingsColonistOfDef>c__Iterator15F.<>f__this = this;
			ListerBuildings.<AllBuildingsColonistOfDef>c__Iterator15F expr_1C = <AllBuildingsColonistOfDef>c__Iterator15F;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		[DebuggerHidden]
		public IEnumerable<T> AllBuildingsColonistOfClass<T>() where T : Building
		{
			ListerBuildings.<AllBuildingsColonistOfClass>c__Iterator160<T> <AllBuildingsColonistOfClass>c__Iterator = new ListerBuildings.<AllBuildingsColonistOfClass>c__Iterator160<T>();
			<AllBuildingsColonistOfClass>c__Iterator.<>f__this = this;
			ListerBuildings.<AllBuildingsColonistOfClass>c__Iterator160<T> expr_0E = <AllBuildingsColonistOfClass>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
