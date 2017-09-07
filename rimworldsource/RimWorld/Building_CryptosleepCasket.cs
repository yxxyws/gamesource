using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorld
{
	public class Building_CryptosleepCasket : Building_Casket
	{
		public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
		{
			if (base.TryAcceptThing(thing, allowSpecialEffects))
			{
				if (allowSpecialEffects)
				{
					SoundDef.Named("CryptosleepCasketAccept").PlayOneShot(base.Position);
				}
				return true;
			}
			return false;
		}
		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			Building_CryptosleepCasket.<GetFloatMenuOptions>c__IteratorD9 <GetFloatMenuOptions>c__IteratorD = new Building_CryptosleepCasket.<GetFloatMenuOptions>c__IteratorD9();
			<GetFloatMenuOptions>c__IteratorD.myPawn = myPawn;
			<GetFloatMenuOptions>c__IteratorD.<$>myPawn = myPawn;
			<GetFloatMenuOptions>c__IteratorD.<>f__this = this;
			Building_CryptosleepCasket.<GetFloatMenuOptions>c__IteratorD9 expr_1C = <GetFloatMenuOptions>c__IteratorD;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_CryptosleepCasket.<GetGizmos>c__IteratorDA <GetGizmos>c__IteratorDA = new Building_CryptosleepCasket.<GetGizmos>c__IteratorDA();
			<GetGizmos>c__IteratorDA.<>f__this = this;
			Building_CryptosleepCasket.<GetGizmos>c__IteratorDA expr_0E = <GetGizmos>c__IteratorDA;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void EjectContents()
		{
			ThingDef named = DefDatabase<ThingDef>.GetNamed("FilthSlime", true);
			foreach (Thing current in this.container)
			{
				Pawn pawn = current as Pawn;
				if (pawn != null)
				{
					PawnComponentsUtility.AddComponentsForSpawn(pawn);
					pawn.filth.GainFilth(named);
					pawn.health.AddHediff(HediffDefOf.CryptosleepSickness, null, null);
				}
			}
			if (!base.Destroyed)
			{
				SoundDef.Named("CryptosleepCasketEject").PlayOneShot(SoundInfo.InWorld(base.Position, MaintenanceType.None));
			}
			base.EjectContents();
		}
		public static Building_CryptosleepCasket FindCryptosleepCasketFor(Pawn p, Pawn traveler)
		{
			IEnumerable<ThingDef> enumerable = 
				from def in DefDatabase<ThingDef>.AllDefs
				where typeof(Building_CryptosleepCasket).IsAssignableFrom(def.thingClass)
				select def;
			foreach (ThingDef current in enumerable)
			{
				Predicate<Thing> validator = (Thing x) => ((Building_CryptosleepCasket)x).GetContainer().Count == 0;
				Building_CryptosleepCasket building_CryptosleepCasket = (Building_CryptosleepCasket)GenClosest.ClosestThingReachable(p.Position, ThingRequest.ForDef(current), PathEndMode.InteractionCell, TraverseParms.For(traveler, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, -1, false);
				if (building_CryptosleepCasket != null)
				{
					return building_CryptosleepCasket;
				}
			}
			return null;
		}
	}
}
