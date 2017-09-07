using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public class SummaryHealthHandler
	{
		private Pawn pawn;
		private int lastSummaryHealthPercentCacheTick = -1;
		private float cachedSummaryHealthPercent = 1f;
		public float SummaryHealthPercent
		{
			get
			{
				if (Game.Mode != GameMode.MapPlaying || this.lastSummaryHealthPercentCacheTick != Find.TickManager.TicksGame)
				{
					ProfilerThreadCheck.BeginSample("Calculate health percent");
					List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
					float num = 1f;
					for (int i = 0; i < hediffs.Count; i++)
					{
						if (!(hediffs[i] is Hediff_MissingPart) && hediffs[i].Visible)
						{
							float num2 = hediffs[i].SummaryHealthPercentImpact;
							if (num2 > 0.95f)
							{
								num2 = 0.95f;
							}
							num *= 1f - num2;
						}
					}
					foreach (Hediff_MissingPart current in this.pawn.health.hediffSet.GetMissingPartsCommonAncestors())
					{
						if (!current.Part.def.Activities.NullOrEmpty<Pair<PawnCapacityDef, string>>() || !current.Part.parts.NullOrEmpty<BodyPartRecord>())
						{
							float num3 = current.SummaryHealthPercentImpact;
							if (num3 > 0.95f)
							{
								num3 = 0.95f;
							}
							num *= 1f - num3;
						}
					}
					this.cachedSummaryHealthPercent = Mathf.Clamp(num, 0.05f, 1f);
					ProfilerThreadCheck.EndSample();
					if (Game.Mode == GameMode.MapPlaying)
					{
						this.lastSummaryHealthPercentCacheTick = Find.TickManager.TicksGame;
					}
				}
				return this.cachedSummaryHealthPercent;
			}
		}
		public SummaryHealthHandler(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void Clear()
		{
			this.cachedSummaryHealthPercent = -1f;
			this.lastSummaryHealthPercentCacheTick = -1;
		}
	}
}
