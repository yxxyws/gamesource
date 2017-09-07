using System;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class DropPod : Thing
	{
		public int age;
		public DropPodInfo info;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.age, "age", 0, false);
			Scribe_Deep.LookDeep<DropPodInfo>(ref this.info, "info", new object[0]);
		}
		public override void Tick()
		{
			this.age++;
			if (this.age > this.info.openDelay)
			{
				this.PodOpen();
			}
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			for (int i = this.info.containedThings.Count - 1; i >= 0; i--)
			{
				this.info.containedThings[i].Destroy(DestroyMode.Vanish);
			}
			base.Destroy(mode);
			if (mode == DestroyMode.Kill)
			{
				for (int j = 0; j < 1; j++)
				{
					Thing thing = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, null);
					GenPlace.TryPlaceThing(thing, base.Position, ThingPlaceMode.Near);
				}
			}
		}
		private void PodOpen()
		{
			for (int i = 0; i < this.info.containedThings.Count; i++)
			{
				Thing thing = this.info.containedThings[i];
				GenPlace.TryPlaceThing(thing, base.Position, ThingPlaceMode.Near);
				Pawn pawn = thing as Pawn;
				if (pawn != null && pawn.RaceProps.Humanlike)
				{
					TaleRecorder.RecordTale(TaleDef.Named("LandedInPod"), new object[]
					{
						pawn
					});
				}
			}
			this.info.containedThings.Clear();
			if (this.info.leaveSlag)
			{
				for (int j = 0; j < 1; j++)
				{
					Thing thing2 = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, null);
					GenPlace.TryPlaceThing(thing2, base.Position, ThingPlaceMode.Near);
				}
			}
			SoundDef.Named("DropPodOpen").PlayOneShot(base.Position);
			this.Destroy(DestroyMode.Vanish);
		}
	}
}
