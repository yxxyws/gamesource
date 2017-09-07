using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public static class PawnApparelGenerator
	{
		private static List<ThingStuffPair> potentialApparel;
		private static float cachedFreeWarmLayerMaxPrice;
		private static List<ThingStuffPair> workingApSet;
		static PawnApparelGenerator()
		{
			PawnApparelGenerator.potentialApparel = new List<ThingStuffPair>();
			PawnApparelGenerator.workingApSet = new List<ThingStuffPair>();
			PawnApparelGenerator.Reset();
		}
		public static void Reset()
		{
			PawnApparelGenerator.potentialApparel = ThingStuffPair.AllWith((ThingDef td) => td.IsApparel);
			PawnApparelGenerator.cachedFreeWarmLayerMaxPrice = (float)((int)(StatDefOf.MarketValue.Worker.GetValueAbstract(ThingDefOf.Apparel_Parka, ThingDefOf.Cloth) * 1.3f));
		}
		public static void GenerateStartingApparelFor(Pawn pawn, PawnGenerationRequest request)
		{
			if (!pawn.RaceProps.ToolUser)
			{
				return;
			}
			if (pawn.Faction == null)
			{
				Log.Error("Cannot generate apparel for faction-less pawn " + pawn);
				return;
			}
			pawn.apparel.DestroyAll(DestroyMode.Vanish);
			PawnApparelGenerator.workingApSet.Clear();
			float randomInRange = pawn.kindDef.apparelMoney.RandomInRange;
			NeededWarmth neededWarmth = PawnApparelGenerator.ApparelWarmthNeededNow(pawn, request);
			bool flag = Rand.Value < pawn.kindDef.apparelAllowHeadwearChance;
			StringBuilder stringBuilder = null;
			if (DebugViewSettings.logApparelGeneration)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Generating apparel for " + pawn);
				stringBuilder.AppendLine("Money: " + randomInRange.ToString("F0"));
				stringBuilder.AppendLine("Needed warmth: " + neededWarmth);
				stringBuilder.AppendLine("Headwear allowed: " + flag);
			}
			if (randomInRange >= 0.001f)
			{
				int num = 0;
				while (true)
				{
					PawnApparelGenerator.GenerateWorkingApSetFor(pawn, randomInRange, flag, PawnApparelGenerator.workingApSet);
					if (DebugViewSettings.logApparelGeneration)
					{
						stringBuilder.Append(num.ToString().PadRight(5) + "Trying: " + PawnApparelGenerator.ApSetToString(PawnApparelGenerator.workingApSet));
					}
					if (num >= 10 || Rand.Value >= 0.85f)
					{
						goto IL_1D1;
					}
					float num2 = PawnApparelGenerator.ApSetTotalPrice(PawnApparelGenerator.workingApSet);
					float num3 = Rand.Range(0.45f, 0.8f);
					if (num2 >= randomInRange * num3)
					{
						goto IL_1D1;
					}
					if (DebugViewSettings.logApparelGeneration)
					{
						stringBuilder.AppendLine(string.Concat(new string[]
						{
							" -- Failed: Spent $",
							num2.ToString("F0"),
							", < ",
							(num3 * 100f).ToString("F0"),
							"% of money."
						}));
					}
					IL_305:
					num++;
					continue;
					IL_1D1:
					if (num < 20 && Rand.Value < 0.97f && !PawnApparelGenerator.ApSetCovers(PawnApparelGenerator.workingApSet, BodyPartGroupDefOf.Torso))
					{
						if (DebugViewSettings.logApparelGeneration)
						{
							stringBuilder.AppendLine(" -- Failed: Does not cover torso.");
						}
						goto IL_305;
					}
					if (num < 30 && Rand.Value < 0.8f && PawnApparelGenerator.ApSetCoatButNoShirt(PawnApparelGenerator.workingApSet))
					{
						if (DebugViewSettings.logApparelGeneration)
						{
							stringBuilder.AppendLine(" -- Failed: Coat but no shirt.");
						}
						goto IL_305;
					}
					if (neededWarmth != NeededWarmth.Any && num < 50 && !PawnApparelGenerator.ApSetIsCorrectWarmth(PawnApparelGenerator.workingApSet, neededWarmth))
					{
						if (DebugViewSettings.logApparelGeneration)
						{
							stringBuilder.AppendLine(" -- Failed: Wrong warmth.");
						}
						goto IL_305;
					}
					if (num < 80 && PawnApparelGenerator.ApSetIsNaked(PawnApparelGenerator.workingApSet, pawn.gender))
					{
						if (DebugViewSettings.logApparelGeneration)
						{
							stringBuilder.AppendLine(" -- Failed: Naked.");
						}
						goto IL_305;
					}
					break;
				}
				if (DebugViewSettings.logApparelGeneration)
				{
					stringBuilder.Append(" -- Approved! Total price: $" + PawnApparelGenerator.ApSetTotalPrice(PawnApparelGenerator.workingApSet).ToString("F0"));
				}
			}
			if (neededWarmth == NeededWarmth.Warm && (!pawn.kindDef.apparelIgnoreSeasons || request.forceAddFreeWarmLayerIfNeeded) && !PawnApparelGenerator.ApSetIsCorrectWarmth(PawnApparelGenerator.workingApSet, NeededWarmth.Warm))
			{
				if (DebugViewSettings.logApparelGeneration)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("Trying to give free warm layer.");
				}
				PawnApparelGenerator.AddFreeWarmLayerIfNotPresent(PawnApparelGenerator.workingApSet);
			}
			if (DebugViewSettings.logApparelGeneration)
			{
				Log.Message(stringBuilder.ToString());
			}
			PawnApparelGenerator.GiveApSetToPawn(PawnApparelGenerator.workingApSet, pawn);
		}
		private static void GenerateWorkingApSetFor(Pawn pawn, float money, bool headwearAllowed, List<ThingStuffPair> apSet)
		{
			apSet.Clear();
			float moneyLeft = money;
			List<ThingDef> reqApparel = pawn.kindDef.apparelRequired;
			if (reqApparel != null)
			{
				int i;
				for (i = 0; i < reqApparel.Count; i++)
				{
					ThingStuffPair item = (
						from pa in PawnApparelGenerator.potentialApparel
						where pa.thing == reqApparel[i]
						select pa).RandomElementByWeight((ThingStuffPair pa) => pa.Commonality);
					apSet.Add(item);
					moneyLeft -= item.Price;
				}
			}
			while (true)
			{
				if (Rand.Value < 0.1f)
				{
					break;
				}
				Predicate<ThingStuffPair> pairValidator = delegate(ThingStuffPair pa)
				{
					if (pa.Price > moneyLeft)
					{
						return false;
					}
					if (!headwearAllowed && (pa.thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || pa.thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead)))
					{
						return false;
					}
					if (pa.stuff != null && !pawn.Faction.def.CanUseStuffForApparel(pa.stuff))
					{
						return false;
					}
					if (PawnApparelGenerator.PairOverlapsAnythingInApSet(pa, apSet))
					{
						return false;
					}
					if (!pawn.kindDef.apparelTags.NullOrEmpty<string>())
					{
						bool flag = false;
						for (int i = 0; i < pawn.kindDef.apparelTags.Count; i++)
						{
							for (int j = 0; j < pa.thing.apparel.tags.Count; j++)
							{
								if (pawn.kindDef.apparelTags[i] == pa.thing.apparel.tags[j])
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
						if (!flag)
						{
							return false;
						}
					}
					return true;
				};
				IEnumerable<ThingStuffPair> source = 
					from pa in PawnApparelGenerator.potentialApparel
					where pairValidator(pa)
					select pa;
				if (!source.Any<ThingStuffPair>())
				{
					break;
				}
				ThingStuffPair item2 = source.RandomElementByWeight((ThingStuffPair pa) => pa.Commonality);
				apSet.Add(item2);
				moneyLeft -= item2.Price;
			}
		}
		private static void AddFreeWarmLayerIfNotPresent(List<ThingStuffPair> apSet)
		{
			Predicate<ThingStuffPair> pairValidator = (ThingStuffPair pa) => pa.Price <= PawnApparelGenerator.cachedFreeWarmLayerMaxPrice && PawnApparelGenerator.PairSatisfiesNeededWarmth(pa, NeededWarmth.Warm);
			IEnumerable<ThingStuffPair> source = 
				from pa in PawnApparelGenerator.potentialApparel
				where pairValidator(pa)
				select pa;
			if (!source.Any<ThingStuffPair>())
			{
				Log.Warning("Unable to give free warm layer.");
				return;
			}
			ThingStuffPair parka = source.RandomElementByWeight((ThingStuffPair pa) => pa.Commonality / (pa.Price * pa.Price));
			apSet.RemoveAll((ThingStuffPair pa) => !ApparelUtility.CanWearTogether(pa.thing, parka.thing));
			apSet.Add(parka);
		}
		private static bool PairOverlapsAnythingInApSet(ThingStuffPair pair, List<ThingStuffPair> apSet)
		{
			for (int i = 0; i < apSet.Count; i++)
			{
				if (!ApparelUtility.CanWearTogether(apSet[i].thing, pair.thing))
				{
					return true;
				}
			}
			return false;
		}
		private static NeededWarmth ApparelWarmthNeededNow(Pawn pawn, PawnGenerationRequest request)
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return NeededWarmth.Any;
			}
			NeededWarmth neededWarmth = NeededWarmth.Any;
			Month month = GenDate.CurrentMonth;
			for (int i = 0; i < 2; i++)
			{
				NeededWarmth neededWarmth2 = PawnApparelGenerator.CalculateNeededWarmth(pawn, month);
				if (neededWarmth2 != NeededWarmth.Any)
				{
					neededWarmth = neededWarmth2;
					break;
				}
				if (month == Month.Dec)
				{
					month = Month.Jan;
				}
				else
				{
					month += 1;
				}
			}
			if (!pawn.kindDef.apparelIgnoreSeasons)
			{
				return neededWarmth;
			}
			if (request.forceAddFreeWarmLayerIfNeeded && neededWarmth == NeededWarmth.Warm)
			{
				return neededWarmth;
			}
			return NeededWarmth.Any;
		}
		public static NeededWarmth CalculateNeededWarmth(Pawn pawn, Month month)
		{
			float num = GenTemperature.AverageTemperatureAtWorldCoordsForMonth(Find.Map.WorldCoords, month);
			if (num < pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin, null) - 4f)
			{
				return NeededWarmth.Warm;
			}
			if (num > pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin, null) + 4f)
			{
				return NeededWarmth.Cool;
			}
			return NeededWarmth.Any;
		}
		private static bool ApSetCoatButNoShirt(List<ThingStuffPair> apSet)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < apSet.Count; i++)
			{
				if (apSet[i].thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
				{
					for (int j = 0; j < apSet[i].thing.apparel.layers.Count; j++)
					{
						ApparelLayer apparelLayer = apSet[i].thing.apparel.layers[j];
						if (apparelLayer == ApparelLayer.OnSkin)
						{
							flag2 = true;
						}
						if (apparelLayer == ApparelLayer.Shell || apparelLayer == ApparelLayer.Middle)
						{
							flag = true;
						}
					}
				}
			}
			return flag && !flag2;
		}
		private static bool ApSetCovers(List<ThingStuffPair> apSet, BodyPartGroupDef bp)
		{
			for (int i = 0; i < apSet.Count; i++)
			{
				if (apSet[i].thing.apparel.bodyPartGroups.Contains(bp))
				{
					return true;
				}
			}
			return false;
		}
		private static bool ApSetIsNaked(List<ThingStuffPair> apSet, Gender gender)
		{
			switch (gender)
			{
			case Gender.None:
				return false;
			case Gender.Male:
				return !PawnApparelGenerator.ApSetCovers(apSet, BodyPartGroupDefOf.Legs);
			case Gender.Female:
				return !PawnApparelGenerator.ApSetCovers(apSet, BodyPartGroupDefOf.Legs) || !PawnApparelGenerator.ApSetCovers(apSet, BodyPartGroupDefOf.Torso);
			default:
				return false;
			}
		}
		private static bool PairSatisfiesNeededWarmth(ThingStuffPair pa, NeededWarmth warmth)
		{
			return PawnApparelGenerator.MinComfyTemperatureOffsetSatisfiesNeededWarmth(pa.InsulationCold, warmth);
		}
		private static bool ApSetIsCorrectWarmth(List<ThingStuffPair> apSet, NeededWarmth warmth)
		{
			float num = 0f;
			for (int i = 0; i < apSet.Count; i++)
			{
				if (apSet[i].InsulationCold < num)
				{
					num = apSet[i].InsulationCold;
				}
			}
			return PawnApparelGenerator.MinComfyTemperatureOffsetSatisfiesNeededWarmth(num, warmth);
		}
		private static bool MinComfyTemperatureOffsetSatisfiesNeededWarmth(float offset, NeededWarmth warmth)
		{
			if (warmth == NeededWarmth.Warm)
			{
				return offset <= -40f;
			}
			return warmth != NeededWarmth.Cool || offset > -18f;
		}
		private static void GiveApSetToPawn(List<ThingStuffPair> apSet, Pawn pawn)
		{
			for (int i = 0; i < apSet.Count; i++)
			{
				Apparel apparel = (Apparel)ThingMaker.MakeThing(apSet[i].thing, apSet[i].stuff);
				PawnGenerator.PostProcessGeneratedGear(apparel, pawn);
				if (ApparelUtility.HasPartsToWear(pawn, apparel.def))
				{
					pawn.apparel.Wear(apparel, false);
				}
			}
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			if (wornApparel.Count > 4)
			{
				for (int j = 0; j < wornApparel.Count; j++)
				{
					for (int k = 0; k < wornApparel.Count; k++)
					{
						if (j != k && !ApparelUtility.CanWearTogether(wornApparel[j].def, wornApparel[k].def))
						{
							Log.Error(string.Concat(new object[]
							{
								pawn,
								" generated with apparel that cannot be worn together: ",
								wornApparel[j],
								", ",
								wornApparel[k]
							}));
							return;
						}
					}
				}
			}
		}
		private static float ApSetTotalPrice(List<ThingStuffPair> apSet)
		{
			return apSet.Sum((ThingStuffPair pa) => pa.Price);
		}
		private static string ApSetToString(List<ThingStuffPair> apSet)
		{
			string str = "[";
			for (int i = 0; i < apSet.Count; i++)
			{
				str = str + apSet[i].ToString() + ", ";
			}
			return str + "]";
		}
		internal static void LogGenerationData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("All potential apparel:");
			foreach (ThingStuffPair current in PawnApparelGenerator.potentialApparel)
			{
				stringBuilder.AppendLine(current.ToString());
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
