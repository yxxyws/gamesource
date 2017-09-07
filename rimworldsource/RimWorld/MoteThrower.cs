using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class MoteThrower
	{
		public static Mote ThrowDrift(IntVec3 cell, ThingDef moteDef)
		{
			if (!cell.ShouldSpawnMotesAt() || MoteCounter.Saturated)
			{
				return null;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef, null);
			moteThrown.ScaleUniform = 0.7f;
			moteThrown.exactRotationRate = Rand.Range(-0.05f, 0.05f);
			moteThrown.exactPosition = cell.ToVector3Shifted();
			moteThrown.exactPosition += new Vector3(0.35f, 0f, 0.35f);
			moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f;
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(30, 60), Rand.Range(0.007f, 0.007f));
			GenSpawn.Spawn(moteThrown, cell);
			return moteThrown;
		}
		public static void ThrowStatic(IntVec3 cell, ThingDef moteDef, float scale = 1f)
		{
			MoteThrower.ThrowStatic(cell.ToVector3Shifted(), moteDef, scale);
		}
		public static void ThrowStatic(Vector3 loc, ThingDef moteDef, float scale = 1f)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.Saturated)
			{
				return;
			}
			Mote mote = (Mote)ThingMaker.MakeThing(moteDef, null);
			mote.exactPosition = loc;
			mote.ScaleUniform = scale;
			GenSpawn.Spawn(mote, loc.ToIntVec3());
		}
		public static void ThrowText(Vector3 loc, string text, int ticksBeforeStartFadeout = -1)
		{
			MoteThrower.ThrowText(loc, text, Color.white, ticksBeforeStartFadeout);
		}
		public static void ThrowText(Vector3 loc, string text, Color color, int ticksBeforeStartFadeout = -1)
		{
			IntVec3 intVec = loc.ToIntVec3();
			if (!intVec.InBounds())
			{
				return;
			}
			TextMote textMote = (TextMote)ThingMaker.MakeThing(ThingDefOf.Mote_Text, null);
			textMote.exactPosition = loc;
			textMote.SetVelocityAngleSpeed((float)Rand.Range(5, 35), Rand.Range(0.007f, 0.0075f));
			textMote.text = text;
			textMote.textColor = color;
			if (ticksBeforeStartFadeout >= 0)
			{
				textMote.overrideTicksBeforeStartFadeout = ticksBeforeStartFadeout;
			}
			GenSpawn.Spawn(textMote, intVec);
		}
		public static void ThrowMetaPuffs(CellRect rect)
		{
			if (!Find.TickManager.Paused)
			{
				for (int i = rect.minX; i <= rect.maxX; i++)
				{
					for (int j = rect.minZ; j <= rect.maxZ; j++)
					{
						MoteThrower.ThrowMetaPuffs(new IntVec3(i, 0, j));
					}
				}
			}
		}
		public static void ThrowMetaPuffs(TargetInfo targ)
		{
			Vector3 a = (!targ.HasThing) ? targ.Cell.ToVector3Shifted() : targ.Thing.TrueCenter();
			int num = Rand.RangeInclusive(4, 6);
			for (int i = 0; i < num; i++)
			{
				Vector3 loc = a + new Vector3(Rand.Range(-0.5f, 0.5f), 0f, Rand.Range(-0.5f, 0.5f));
				MoteThrower.ThrowMetaPuff(loc);
			}
		}
		public static void ThrowMetaPuff(Vector3 loc)
		{
			if (!loc.ShouldSpawnMotesAt())
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Mote_MetaPuff", true), null);
			moteThrown.ScaleUniform = 1.9f;
			moteThrown.exactRotationRate = (float)Rand.Range(-1, 1);
			moteThrown.exactPosition = loc;
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 360), Rand.Range(0.01f, 0.013f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		private static MoteThrown NewBaseAirPuff()
		{
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_AirPuff, null);
			moteThrown.ScaleUniform = 1.5f;
			moteThrown.exactRotationRate = (float)Rand.RangeInclusive(-4, 4);
			return moteThrown;
		}
		public static void ThrowAirPuffUp(Vector3 loc)
		{
			if (!loc.ToIntVec3().ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = MoteThrower.NewBaseAirPuff();
			moteThrown.exactPosition = loc;
			moteThrown.exactPosition += new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f));
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(-45, 45), Rand.Range(0.02f, 0.025f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		internal static void ThrowBreathPuff(Vector3 loc, float throwAngle, Vector3 inheritVelocity)
		{
			if (!loc.ToIntVec3().ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = MoteThrower.NewBaseAirPuff();
			moteThrown.exactPosition = loc;
			moteThrown.exactPosition += new Vector3(Rand.Range(-0.005f, 0.005f), 0f, Rand.Range(-0.005f, 0.005f));
			moteThrown.SetVelocityAngleSpeed(throwAngle + (float)Rand.Range(-10, 10), Rand.Range(0.0011f, 0.014f));
			moteThrown.Velocity += inheritVelocity * 0.5f;
			moteThrown.ScaleUniform = Rand.Range(0.6f, 0.7f);
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		public static void ThrowSmoke(Vector3 loc, float size)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
			moteThrown.ScaleUniform = Rand.Range(1.5f, 2.5f) * size;
			moteThrown.exactRotationRate = Rand.Range(-0.5f, 0.5f);
			moteThrown.exactPosition = loc;
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(30, 40), Rand.Range(0.008f, 0.012f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		public static void ThrowFireGlow(IntVec3 c, float size)
		{
			Vector3 vector = c.ToVector3Shifted();
			if (!vector.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			vector += size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
			if (!vector.InBounds())
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_FireGlow, null);
			moteThrown.ScaleUniform = Rand.Range(4f, 6f) * size;
			moteThrown.exactRotationRate = Rand.Range(-0.05f, 0.05f);
			moteThrown.exactPosition = vector;
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 360), Rand.Range(0.0002f, 0.0002f));
			GenSpawn.Spawn(moteThrown, vector.ToIntVec3());
		}
		public static void ThrowMicroSparks(Vector3 loc)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_MicroSparks, null);
			moteThrown.ScaleUniform = Rand.Range(0.8f, 1.2f);
			moteThrown.exactRotationRate = Rand.Range(-0.2f, 0.2f);
			moteThrown.exactPosition = loc;
			moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
			moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(35, 45), Rand.Range(0.02f, 0.02f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		public static void ThrowLightningGlow(Vector3 loc, float size)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Mote_LightningGlow", true), null);
			moteThrown.ScaleUniform = Rand.Range(4f, 6f) * size;
			moteThrown.exactRotationRate = Rand.Range(-0.05f, 0.05f);
			moteThrown.exactPosition = loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 360), Rand.Range(0.0002f, 0.0002f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		public static void PlaceFootprint(Vector3 loc, float rot)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Footprint, null);
			moteThrown.ScaleUniform = 0.5f;
			moteThrown.exactRotation = rot;
			moteThrown.exactPosition = loc;
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		public static void ThrowHorseshoe(Pawn thrower, IntVec3 targetCell)
		{
			if (!thrower.Position.ShouldSpawnMotesAt() || MoteCounter.Saturated)
			{
				return;
			}
			float num = Rand.Range(0.08f, 0.12f);
			Vector3 vector = targetCell.ToVector3Shifted() + Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(SkillDefOf.Shooting).level / 20f) * 1.8f);
			vector.y = thrower.DrawPos.y;
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Horseshoe, null);
			moteThrown.ScaleUniform = 1f;
			moteThrown.exactRotationRate = (float)Rand.Range(-5, 5);
			moteThrown.exactPosition = thrower.DrawPos;
			moteThrown.SetVelocityAngleSpeed((vector - moteThrown.exactPosition).AngleFlat(), num);
			moteThrown.airTicksLeft = Mathf.RoundToInt((moteThrown.exactPosition - vector).MagnitudeHorizontal() / num);
			GenSpawn.Spawn(moteThrown, thrower.Position);
		}
		public static MoteAttached MakeStunOverlay(Thing stunnedThing)
		{
			MoteAttached moteAttached = (MoteAttached)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Mote_Stun", true), null);
			moteAttached.AttachTo(stunnedThing);
			GenSpawn.Spawn(moteAttached, stunnedThing.Position);
			return moteAttached;
		}
		public static void MakeSpeechOverlay(Pawn initiator, Pawn recipient, Texture2D symbol)
		{
			MoteSpeechBubble moteSpeechBubble = (MoteSpeechBubble)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Mote_Speech", true), null);
			moteSpeechBubble.ScaleUniform = 1.25f;
			moteSpeechBubble.SetupSpeechBubble(symbol, recipient);
			moteSpeechBubble.AttachTo(initiator);
			GenSpawn.Spawn(moteSpeechBubble, initiator.Position);
		}
		public static MoteAttached MakeInteractionOverlay(ThingDef moteDef, TargetInfo A, TargetInfo B)
		{
			MoteAttached moteAttached = (MoteAttached)ThingMaker.MakeThing(moteDef, null);
			moteAttached.ScaleUniform = 0.5f;
			moteAttached.AttachTo(A, B);
			GenSpawn.Spawn(moteAttached, A.Cell);
			return moteAttached;
		}
		public static void MakeColonistActionOverlay(Pawn pawn, ThingDef moteDef)
		{
			MoteThrownAttached moteThrownAttached = (MoteThrownAttached)ThingMaker.MakeThing(moteDef, null);
			moteThrownAttached.AttachTo(pawn);
			moteThrownAttached.ScaleUniform = 1.5f;
			moteThrownAttached.SetVelocityAngleSpeed(Rand.Range(20f, 25f), Rand.Range(0.0065f, 0.0065f));
			GenSpawn.Spawn(moteThrownAttached, pawn.Position);
		}
		public static void ThrowDustPuff(IntVec3 cell, float scale)
		{
			Vector3 loc = cell.ToVector3() + new Vector3(Rand.Value, 0f, Rand.Value);
			MoteThrower.ThrowDustPuff(loc, scale);
		}
		public static void ThrowDustPuff(Vector3 loc, float scale)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_DustPuff, null);
			moteThrown.ScaleUniform = 1.9f * scale;
			moteThrown.exactRotationRate = (float)Rand.Range(-1, 1);
			moteThrown.exactPosition = loc;
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 360), Rand.Range(0.01f, 0.013f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
		public static void PlaceTempRoof(IntVec3 cell)
		{
			if (!cell.ShouldSpawnMotesAt())
			{
				return;
			}
			Mote mote = (Mote)ThingMaker.MakeThing(ThingDefOf.Mote_TempRoof, null);
			mote.exactPosition = cell.ToVector3Shifted();
			GenSpawn.Spawn(mote, cell);
		}
		public static void ThrowExplosionCell(IntVec3 cell, ThingDef moteDef, Color color)
		{
			if (!cell.ShouldSpawnMotesAt())
			{
				return;
			}
			Mote mote = (Mote)ThingMaker.MakeThing(moteDef, null);
			mote.exactRotation = (float)(90 * Rand.RangeInclusive(0, 3));
			mote.exactPosition = cell.ToVector3Shifted();
			mote.color = color;
			GenSpawn.Spawn(mote, cell);
			if (Rand.Value < 0.7f)
			{
				MoteThrower.ThrowDustPuff(cell, 1.2f);
			}
		}
		public static void ThrowExplosionInteriorMote(Vector3 loc, ThingDef moteDef)
		{
			if (!loc.ShouldSpawnMotesAt() || MoteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef, null);
			moteThrown.ScaleUniform = Rand.Range(3f, 4.5f);
			moteThrown.exactRotationRate = Rand.Range(-0.5f, 0.5f);
			moteThrown.exactPosition = loc;
			moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 360), Rand.Range(0.008f, 0.012f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3());
		}
	}
}
