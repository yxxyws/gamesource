using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
using Verse.Sound;
namespace Verse
{
	public class Artifact : ThingWithComps, IUsable
	{
		private const float CameraShakeMag = 1f;
		private Thing artifactTarget;
		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			Artifact.<GetFloatMenuOptions>c__Iterator188 <GetFloatMenuOptions>c__Iterator = new Artifact.<GetFloatMenuOptions>c__Iterator188();
			<GetFloatMenuOptions>c__Iterator.myPawn = myPawn;
			<GetFloatMenuOptions>c__Iterator.<$>myPawn = myPawn;
			<GetFloatMenuOptions>c__Iterator.<>f__this = this;
			Artifact.<GetFloatMenuOptions>c__Iterator188 expr_1C = <GetFloatMenuOptions>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		public void UsedBy(Pawn user)
		{
			if (this.artifactTarget != null && !this.def.artifact.Targeter.GetTargetingParameters().CanTarget(this.artifactTarget))
			{
				return;
			}
			SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera();
			user.records.Increment(RecordDefOf.ArtifactsActivated);
			if (this.def.artifact.doCameraShake)
			{
				Find.CameraMap.shaker.DoShake(1f);
			}
			foreach (Thing current in this.def.artifact.Targeter.GetTargets(this.artifactTarget))
			{
				foreach (ArtifactEffectDoer current2 in this.def.artifact.EffectDoers)
				{
					current2.DoEffect(user, current);
				}
			}
			this.Destroy(DestroyMode.Vanish);
		}
		private void StartJob(Pawn user)
		{
			Job newJob = new Job(JobDefOf.UseArtifact, this);
			user.drafter.TakeOrderedJob(newJob);
		}
	}
}
