using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_ManTurret : JobDriver
	{
		private const float ShellSearchRadius = 40f;
		private static bool GunNeedsLoading(Building b)
		{
			Building_TurretGun building_TurretGun = b as Building_TurretGun;
			return building_TurretGun != null && building_TurretGun.def.building.turretShellDef != null && !building_TurretGun.loaded;
		}
		public static Thing FindAmmoForTurret(Pawn pawn, Thing gun)
		{
			Predicate<Thing> validator = (Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t, 1);
			return GenClosest.ClosestThingReachable(gun.Position, ThingRequest.ForDef(gun.def.building.turretShellDef), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 40f, validator, null, -1, false);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_ManTurret.<MakeNewToils>c__Iterator2E <MakeNewToils>c__Iterator2E = new JobDriver_ManTurret.<MakeNewToils>c__Iterator2E();
			<MakeNewToils>c__Iterator2E.<>f__this = this;
			JobDriver_ManTurret.<MakeNewToils>c__Iterator2E expr_0E = <MakeNewToils>c__Iterator2E;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
