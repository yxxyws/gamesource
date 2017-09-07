using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class Building_AncientCryptosleepCasket : Building_CryptosleepCasket
	{
		public int groupID = -1;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.groupID, "groupID", 0, false);
		}
		public override void PreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			base.PreApplyDamage(dinfo, out absorbed);
			if (absorbed)
			{
				return;
			}
			if (!this.contentsKnown && this.container.Count > 0 && dinfo.Def.harmsHealth && dinfo.Instigator != null && dinfo.Instigator.Faction != null)
			{
				bool flag = false;
				foreach (Thing current in this.container)
				{
					Pawn pawn = current as Pawn;
					if (pawn != null)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.EjectContents();
				}
			}
			absorbed = false;
		}
		public override void EjectContents()
		{
			List<Thing> list = new List<Thing>();
			if (!this.contentsKnown)
			{
				list.AddRange(this.container);
				list.AddRange(this.UnopenedCasketsInGroup().SelectMany((Building_AncientCryptosleepCasket c) => c.container));
			}
			bool contentsKnown = this.contentsKnown;
			base.EjectContents();
			if (!contentsKnown)
			{
				ThingDef named = DefDatabase<ThingDef>.GetNamed("FilthSlime", true);
				FilthMaker.MakeFilth(base.PositionHeld, named, Rand.Range(8, 12));
				this.SetFaction(null, null);
				foreach (Building_AncientCryptosleepCasket current in this.UnopenedCasketsInGroup())
				{
					current.EjectContents();
				}
				List<Pawn> source = (
					from t in list
					where t is Pawn
					select t).Cast<Pawn>().ToList<Pawn>();
				IEnumerable<Pawn> enumerable = 
					from p in source
					where p.RaceProps.Humanlike && p.GetLord() == null && p.Faction.def == FactionDefOf.SpacerHostile
					select p;
				if (enumerable.Any<Pawn>())
				{
					Faction faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.SpacerHostile);
					LordMaker.MakeNewLord(faction, new LordJob_AssaultColony(faction, false, false, false), enumerable);
				}
			}
		}
		[DebuggerHidden]
		private IEnumerable<Building_AncientCryptosleepCasket> UnopenedCasketsInGroup()
		{
			Building_AncientCryptosleepCasket.<UnopenedCasketsInGroup>c__IteratorDB <UnopenedCasketsInGroup>c__IteratorDB = new Building_AncientCryptosleepCasket.<UnopenedCasketsInGroup>c__IteratorDB();
			<UnopenedCasketsInGroup>c__IteratorDB.<>f__this = this;
			Building_AncientCryptosleepCasket.<UnopenedCasketsInGroup>c__IteratorDB expr_0E = <UnopenedCasketsInGroup>c__IteratorDB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
