using System;
using Verse.Sound;
namespace Verse
{
	public static class LifeStageUtility
	{
		public static void PlayNearestLifestageSound(Pawn pawn, Func<LifeStageAge, SoundDef> getter)
		{
			SoundDef soundDef;
			float pitchFactor;
			float volumeFactor;
			LifeStageUtility.GetNearestLifestageSound(pawn, getter, out soundDef, out pitchFactor, out volumeFactor);
			if (soundDef == null)
			{
				return;
			}
			SoundInfo info = SoundInfo.InWorld(pawn.Position, MaintenanceType.None);
			info.pitchFactor = pitchFactor;
			info.volumeFactor = volumeFactor;
			soundDef.PlayOneShot(info);
		}
		private static void GetNearestLifestageSound(Pawn pawn, Func<LifeStageAge, SoundDef> getter, out SoundDef def, out float pitch, out float volume)
		{
			int num = pawn.ageTracker.CurLifeStageIndex;
			LifeStageAge lifeStageAge;
			while (true)
			{
				lifeStageAge = pawn.RaceProps.lifeStageAges[num];
				def = getter(lifeStageAge);
				if (def != null)
				{
					break;
				}
				num++;
				if (num < 0 || num >= pawn.RaceProps.lifeStageAges.Count)
				{
					goto IL_8D;
				}
			}
			pitch = pawn.ageTracker.CurLifeStage.voxPitch / lifeStageAge.def.voxPitch;
			volume = pawn.ageTracker.CurLifeStage.voxVolume / lifeStageAge.def.voxVolume;
			return;
			IL_8D:
			def = null;
			pitch = (volume = 1f);
		}
	}
}
