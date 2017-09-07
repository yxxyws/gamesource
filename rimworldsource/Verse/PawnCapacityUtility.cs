using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public static class PawnCapacityUtility
	{
		public static float CalculatePartEfficiency(HediffSet diffSet, BodyPartRecord part, bool ignoreAddedParts = false)
		{
			BodyPartRecord rec;
			for (rec = part.parent; rec != null; rec = rec.parent)
			{
				if (diffSet.HasDirectlyAddedPartFor(rec))
				{
					return (
						from x in diffSet.GetHediffs<Hediff_AddedPart>()
						where x.Part == rec
						select x).First<Hediff_AddedPart>().def.addedPartProps.partEfficiency;
				}
			}
			float num = diffSet.GetPartHealth(part) / part.def.GetMaxHealth(diffSet.pawn);
			if (!ignoreAddedParts)
			{
				for (int i = 0; i < diffSet.hediffs.Count; i++)
				{
					Hediff_AddedPart hediff_AddedPart = diffSet.hediffs[i] as Hediff_AddedPart;
					if (hediff_AddedPart != null && hediff_AddedPart.Part == part)
					{
						num *= hediff_AddedPart.def.addedPartProps.partEfficiency;
					}
				}
			}
			float num2 = -1f;
			for (int j = 0; j < diffSet.hediffs.Count; j++)
			{
				if (diffSet.hediffs[j].Part == part && diffSet.hediffs[j].CurStage != null)
				{
					HediffStage curStage = diffSet.hediffs[j].CurStage;
					num *= curStage.partEfficiencyFactor;
					if (curStage.setMinPartEfficiency > num2)
					{
						num2 = curStage.setMinPartEfficiency;
					}
				}
			}
			if (num > 0.0001f)
			{
				num = Mathf.Max(num, num2);
			}
			return Mathf.Max(num, 0f);
		}
		public static float CalculateNaturalPartsAverageEfficiency(HediffSet diffSet, BodyPartGroupDef bodyPartGroup)
		{
			float num = 0f;
			int num2 = 0;
			IEnumerable<BodyPartRecord> enumerable = 
				from x in diffSet.GetNotMissingParts(null, null)
				where x.groups.Contains(bodyPartGroup)
				select x;
			foreach (BodyPartRecord current in enumerable)
			{
				if (!diffSet.PartOrAnyAncestorHasDirectlyAddedParts(current))
				{
					num += PawnCapacityUtility.CalculatePartEfficiency(diffSet, current, false);
				}
				num2++;
			}
			if (num2 == 0 || num < 0f)
			{
				return 0f;
			}
			return num / (float)num2;
		}
		public static bool BodyCanEverDoActivity(BodyDef bodyDef, PawnCapacityDef capacity)
		{
			return !bodyDef.GetActivityGroups(capacity).NullOrEmpty<string>();
		}
		public static float CalculateEfficiency(HediffSet diffSet, PawnCapacityDef capacity)
		{
			BodyDef body = diffSet.pawn.RaceProps.body;
			if (body == null)
			{
				return 1f;
			}
			float minEff = 999999f;
			bool flag = false;
			List<string> activityGroups = body.GetActivityGroups(capacity);
			for (int i = 0; i < activityGroups.Count; i++)
			{
				flag = true;
				float num = 0f;
				int num2 = 0;
				foreach (BodyPartRecord current in body.GetParts(capacity, activityGroups[i]))
				{
					float num3 = PawnCapacityUtility.CalculatePartEfficiency(diffSet, current, false);
					num += num3;
					num2++;
				}
				float num4 = num / (float)num2;
				if (num4 < minEff)
				{
					minEff = num4;
				}
			}
			if (!flag)
			{
				return 0f;
			}
			if (minEff > 0.001f)
			{
				float max = 99999f;
				Action<List<PawnCapacityModifier>, BodyPartRecord> action = delegate(List<PawnCapacityModifier> capMods, BodyPartRecord part)
				{
					for (int k = 0; k < capMods.Count; k++)
					{
						PawnCapacityModifier pawnCapacityModifier = capMods[k];
						if (pawnCapacityModifier.capacity == capacity)
						{
							if (pawnCapacityModifier.offset != 0f)
							{
								float num5 = 1f;
								if (part != null)
								{
									num5 = PawnCapacityUtility.CalculatePartEfficiency(diffSet, part, false);
								}
								minEff += pawnCapacityModifier.offset * num5;
							}
							if (pawnCapacityModifier.setMax < max)
							{
								max = pawnCapacityModifier.setMax;
							}
						}
					}
				};
				for (int j = 0; j < diffSet.hediffs.Count; j++)
				{
					List<PawnCapacityModifier> capMods2 = diffSet.hediffs[j].CapMods;
					if (capMods2 != null)
					{
						action(capMods2, diffSet.hediffs[j].Part);
					}
				}
				minEff += PawnCapacityUtility.PainCapacityEfficiencyOffset(diffSet, capacity);
				if (minEff > max)
				{
					minEff = max;
				}
			}
			return Mathf.Max(minEff, 0f);
		}
		private static float PainCapacityEfficiencyOffset(HediffSet hediffs, PawnCapacityDef capacity)
		{
			if (capacity == PawnCapacityDefOf.Consciousness)
			{
				return -Mathf.Clamp(GenMath.LerpDouble(0.1f, 1f, 0f, 0.4f, hediffs.Pain), 0f, 0.4f);
			}
			return 0f;
		}
	}
}
