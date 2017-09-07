using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class ThoughtWorker_Tale : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			foreach (Tale current in Find.TaleManager.AllTales)
			{
				if (current.def == this.def.taleDef)
				{
					return true;
				}
			}
			return false;
		}
		public override float GetOpinionOffset(Pawn p, Pawn other, ThoughtStage stage)
		{
			if (!other.RaceProps.Humanlike)
			{
				return 0f;
			}
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return 0f;
			}
			Tale latestTale = Find.TaleManager.GetLatestTale(this.def.taleDef, other);
			if (latestTale != null)
			{
				float num = 1f;
				if (latestTale.def.type == TaleType.Expirable)
				{
					float value = (float)latestTale.AgeTicks / (latestTale.def.expireDays * 60000f);
					num = Mathf.InverseLerp(1f, this.def.lerpOpinionToZeroAfterDurationPct, value);
				}
				return stage.baseOpinionOffset * num;
			}
			return 0f;
		}
	}
}
