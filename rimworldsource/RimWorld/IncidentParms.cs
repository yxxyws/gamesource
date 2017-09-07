using System;
using Verse;
namespace RimWorld
{
	public class IncidentParms : IExposable
	{
		public Thing target;
		public float points = -1f;
		public IntVec3 spawnCenter = IntVec3.Invalid;
		public Faction faction;
		public bool forced;
		public bool traderCaravan;
		public bool generateFightersOnly;
		public bool generateMeleeOnly;
		public RaidStrategyDef raidStrategy;
		public PawnsArriveMode raidArrivalMode;
		public int raidPodOpenDelay = 140;
		public bool raidForceOneIncap;
		public bool raidNeverFleeIndividual;
		public void ExposeData()
		{
			Scribe_References.LookReference<Thing>(ref this.target, "target", false);
			Scribe_Values.LookValue<float>(ref this.points, "threatPoints", 0f, false);
			Scribe_Values.LookValue<IntVec3>(ref this.spawnCenter, "spawnCenter", default(IntVec3), false);
			Scribe_References.LookReference<Faction>(ref this.faction, "faction", false);
			Scribe_Values.LookValue<bool>(ref this.forced, "forced", false, false);
			Scribe_Values.LookValue<bool>(ref this.traderCaravan, "traderCaravan", false, false);
			Scribe_Values.LookValue<bool>(ref this.generateFightersOnly, "generateFightersOnly", false, false);
			Scribe_Defs.LookDef<RaidStrategyDef>(ref this.raidStrategy, "raidStrategy");
			Scribe_Values.LookValue<PawnsArriveMode>(ref this.raidArrivalMode, "raidArrivalMode", PawnsArriveMode.Undecided, false);
			Scribe_Values.LookValue<int>(ref this.raidPodOpenDelay, "raidPodOpenDelay", 140, false);
			Scribe_Values.LookValue<bool>(ref this.raidForceOneIncap, "raidForceIncap", false, false);
		}
		public override string ToString()
		{
			if (this.target == null && this.points < 0f)
			{
				return string.Empty;
			}
			string text = "(";
			if (this.target != null)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"target=",
					this.target,
					" "
				});
			}
			if (this.points >= 0f)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"points=",
					this.points,
					" "
				});
			}
			if (this.traderCaravan)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"traderCaravan=",
					this.traderCaravan,
					" "
				});
			}
			if (this.generateFightersOnly)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"generateFightersOnly=",
					this.generateFightersOnly,
					" "
				});
			}
			if (this.generateMeleeOnly)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"generateMeleeOnly=",
					this.generateMeleeOnly,
					" "
				});
			}
			if (this.raidStrategy != null)
			{
				text = text + "raidStrategy=" + this.raidStrategy.defName + " ";
			}
			return text + ")";
		}
	}
}
