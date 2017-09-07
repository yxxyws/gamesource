using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse.Steam;
namespace Verse
{
	public class Dialog_DebugLogMenu : Dialog_DebugOptionLister
	{
		public Dialog_DebugLogMenu()
		{
			this.forcePause = true;
		}
		protected override void DoListingItems()
		{
			this.listing.DoLabel("Logging");
			base.DrawDebugAction("Log test future incidents", delegate
			{
				IncidentMakerUtility.DebugLogTestFutureIncidents();
			});
			base.DrawDebugAction("Log misc incident chances", delegate
			{
				Log.Message("(note that some incident makers never yield misc incidents)");
				List<IncidentMaker> incidentMakers = Find.Storyteller.incidentMakers;
				for (int j = 0; j < incidentMakers.Count; j++)
				{
					incidentMakers[j].DebugLogIncidentChances(IncidentCategory.Misc);
				}
			});
			base.DrawDebugAction("Log test names", delegate
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (RulePackDef current in DefDatabase<RulePackDef>.AllDefs)
				{
					RulePackDef localNamer = current;
					FloatMenuOption item = new FloatMenuOption(localNamer.defName, delegate
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int j = 0; j < 200; j++)
						{
							stringBuilder.AppendLine(NameGenerator.GenerateName(localNamer, null));
						}
						Log.Message(stringBuilder.ToString());
					}, MenuOptionPriority.Medium, null, null);
					list.Add(item);
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			});
			base.DrawDebugAction("Log plant proportions", delegate
			{
				GenPlant.WritePlantProportions();
				Messages.Message("Plant proportions written to log.", MessageSound.Benefit);
			});
			base.DrawDebugAction("Log weapon/apparel gen data", delegate
			{
				PawnApparelGenerator.LogGenerationData();
				PawnWeaponGenerator.LogGenerationData();
			});
			base.DrawDebugAction("Log trader stock gen data", delegate
			{
				TraderStockGenerator.LogGenerationData();
			});
			base.DrawDebugAction("Log quality gen data", delegate
			{
				QualityUtility.LogGenerationData();
			});
			base.DrawDebugAction("Log generated names", delegate
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (RulePackDef current in DefDatabase<RulePackDef>.AllDefs)
				{
					RulePackDef localRp = current;
					FloatMenuOption item = new FloatMenuOption(localRp.defName, delegate
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("Test names for " + localRp.defName + ":");
						for (int j = 0; j < 200; j++)
						{
							stringBuilder.AppendLine(NameGenerator.GenerateName(localRp, null));
						}
						Log.Message(stringBuilder.ToString());
					}, MenuOptionPriority.Medium, null, null);
					list.Add(item);
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			});
			base.DrawDebugAction("Log draw list", delegate
			{
				Find.DynamicDrawManager.LogDynamicDrawThings();
			});
			base.DrawDebugAction("Log things list ", delegate
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				IEnumerator enumerator = Enum.GetValues(typeof(ThingRequestGroup)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ThingRequestGroup localRg2 = (ThingRequestGroup)((byte)enumerator.Current);
						ThingRequestGroup localRg = localRg2;
						FloatMenuOption item = new FloatMenuOption(localRg.ToString(), delegate
						{
							StringBuilder stringBuilder = new StringBuilder();
							List<Thing> list2 = Find.ListerThings.ThingsInGroup(localRg);
							stringBuilder.AppendLine(string.Concat(new object[]
							{
								"Global things in group ",
								localRg,
								" (count ",
								list2.Count,
								")"
							}));
							Log.Message(DebugLogsUtility.ThingListToUniqueCountString(list2));
						}, MenuOptionPriority.Medium, null, null);
						list.Add(item);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			});
			base.DrawDebugAction("Log weather chances", delegate
			{
				Find.Storyteller.weatherDecider.LogWeatherChances();
			});
			base.DrawDebugAction("Log song selection info", delegate
			{
				Find.MusicManagerMap.LogSongSelectionData();
			});
			base.DrawDebugAction("Log temperature data", delegate
			{
				GenTemperature.DebugLogTemps();
			});
			base.DrawDebugAction("Log plant data", delegate
			{
				GenPlant.LogPlantData();
			});
			base.DrawDebugAction("Log celestial glow", delegate
			{
				GenCelestial.LogSunGlowForYear();
			});
			base.DrawDebugAction("Log age injuries", delegate
			{
				AgeInjuryUtility.LogOldInjuryCalculations();
			});
			base.DrawDebugAction("Log pawn groups made", delegate
			{
				PawnGroupMakerUtility.LogPawnGroupsMade();
			});
			base.DrawDebugAction("Log all loaded assets", delegate
			{
				DebugLogWriter.LogAllLoadedAssets();
			});
			base.DrawDebugAction("Log all graphics in database", delegate
			{
				GraphicDatabase.DebugLogAllGraphics();
			});
			base.DrawDebugAction("Log database tales list", delegate
			{
				Find.TaleManager.LogTales();
			});
			base.DrawDebugAction("Log database tales interest", delegate
			{
				Find.TaleManager.LogTaleInterestSummary();
			});
			base.DrawDebugAction("Log database tales descs", delegate
			{
				TaleTester.LogTalesInDatabase();
			});
			base.DrawDebugAction("Log random tales descs", delegate
			{
				TaleTester.LogGeneratedTales(40);
			});
			base.DrawDebugAction("Log specific tale descs", delegate
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (TaleDef current in DefDatabase<TaleDef>.AllDefs)
				{
					TaleDef localDef = current;
					FloatMenuOption item = new FloatMenuOption(localDef.defName, delegate
					{
						TaleTester.LogSpecificTale(localDef, 40);
					}, MenuOptionPriority.Medium, null, null);
					list.Add(item);
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			});
			base.DrawDebugAction("Log taleless descs", delegate
			{
				TaleTester.LogDescriptionsTaleless();
			});
			base.DrawDebugAction("Log rand tests", delegate
			{
				Rand.LogRandTests();
			});
			base.DrawDebugAction("Log PawnName tests", new Action(Dialog_DebugLogMenu.TestPawnNames));
			base.DrawDebugAction("Log crop balance", new Action(this.LogCropBalance));
			base.DrawDebugAction("Log passability/fill", new Action(Dialog_DebugLogMenu.LogPassabilityFill));
			base.DrawDebugAction("Log Steam Workshop status", delegate
			{
				SteamWorkshop.LogStatus();
			});
			if (Game.Mode == GameMode.MapPlaying)
			{
				base.DrawDebugAction("Log ListerPawns", delegate
				{
					Find.MapPawns.LogListedPawns();
				});
				base.DrawDebugAction("Log wind speeds", delegate
				{
					WindManager.LogWindSpeeds();
				});
			}
			base.DrawDebugAction("Log math perf", delegate
			{
				GenMath.LogTestMathPerf();
			});
			base.DrawDebugAction("Log MeshPool stats", delegate
			{
				MeshPool.LogStats();
			});
			base.DrawDebugAction("Log kidnapped pawns", delegate
			{
				Find.FactionManager.LogKidnappedPawns();
			});
			base.DrawDebugAction("Log world pawns", delegate
			{
				Find.WorldPawns.LogWorldPawns();
			});
			base.DrawDebugAction("Log lords", delegate
			{
				Find.LordManager.LogLords();
			});
			base.DrawDebugAction("Log animals per biome", delegate
			{
				this.LogAnimalsPerBiome();
			});
			base.DrawDebugAction("Tables: population intent", delegate
			{
				Find.Storyteller.intenderPopulation.DoTables_PopulationIntents();
			});
			base.DrawDebugAction("Tables: pop-adj recruit difficulty", delegate
			{
				Pawn_GuestTracker.DoTables_PopIntentRecruitDifficulty();
			});
			MethodInfo[] methods = typeof(TableMaker).GetMethods(BindingFlags.Static | BindingFlags.Public);
			MethodInfo mi;
			for (int i = 0; i < methods.Length; i++)
			{
				mi = methods[i];
				string name = mi.Name;
				if (name.StartsWith("DoTables_"))
				{
					base.DrawDebugAction("Tables: " + GenText.SplitCamelCase(name.Substring(9)), delegate
					{
						mi.Invoke(null, null);
					});
				}
			}
		}
		private static void TestPawnNames()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("------Testing parsing");
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'Nick' Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'Nick' von Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John von Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'Nick Hell' Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'Nick Hell' von Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John Nick Hell von Smith"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John O'Neil"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'O'Neil"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'O'Farley' Neil"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("John 'O'''Farley' Neil"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("O'Shea 'O'Farley' O'Neil"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("Missing 'Lastname'"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("Missing 'Lastnamewithspace'     "));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("Unbalanc'ed 'De'limiters'     "));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult("\t"));
			stringBuilder.AppendLine(Dialog_DebugLogMenu.PawnNameTestResult(string.Empty));
			stringBuilder.AppendLine("------Testing ResolveMissingPieces consistency");
			for (int i = 0; i < 20; i++)
			{
				NameTriple nameTriple = new NameTriple("John", null, "Last");
				nameTriple.ResolveMissingPieces(null);
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					nameTriple.ToString(),
					"       [",
					nameTriple.First,
					"] [",
					nameTriple.Nick,
					"] [",
					nameTriple.Last,
					"]"
				}));
			}
			Log.Message(stringBuilder.ToString());
		}
		private static string PawnNameTestResult(string rawName)
		{
			NameTriple nameTriple = NameTriple.FromString(rawName);
			nameTriple.ResolveMissingPieces(null);
			return string.Concat(new string[]
			{
				rawName,
				" -> ",
				nameTriple.ToString(),
				"       [",
				nameTriple.First,
				"] [",
				nameTriple.Nick,
				"] [",
				nameTriple.Last,
				"]"
			});
		}
		private static void LogPassabilityFill()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
			{
				if (current.passability != Traversability.Standable || current.fillPercent > 0f)
				{
					stringBuilder.Append(string.Concat(new string[]
					{
						current.defName,
						" - pass=",
						current.passability.ToString(),
						", fill=",
						current.fillPercent.ToStringPercent()
					}));
					if (current.passability == Traversability.Impassable && current.fillPercent < 0.1f)
					{
						stringBuilder.Append("   ALERT, impassable with low fill");
					}
					if (current.passability != Traversability.Impassable && current.fillPercent > 0.8f)
					{
						stringBuilder.Append("    ALERT, passabile with very high fill");
					}
					stringBuilder.AppendLine();
				}
			}
			Log.Message(stringBuilder.ToString());
		}
		private void LogCropBalance()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = string.Concat(new string[]
			{
				"defName".PadRight(30),
				"growtime".PadRight(15),
				"nutrition".PadRight(15),
				"nut/growtime".PadRight(15),
				"yieldMktVal".PadRight(15),
				"yieldMktVal/time".PadRight(15)
			});
			stringBuilder.AppendLine(value);
			foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
			{
				if (current.plant != null && current.plant.Sowable)
				{
					float growDays = current.plant.growDays;
					float harvestYield = current.plant.harvestYield;
					float num = (current.plant.harvestedThingDef == null || current.plant.harvestedThingDef.ingestible == null) ? 0f : current.plant.harvestedThingDef.ingestible.nutrition;
					float num2 = harvestYield * num;
					float num3 = (current.plant.harvestedThingDef == null) ? 0f : (harvestYield * current.plant.harvestedThingDef.GetStatValueAbstract(StatDefOf.MarketValue, null));
					string value2 = string.Concat(new string[]
					{
						current.defName.PadRight(30),
						growDays.ToString("F2").PadRight(15),
						num2.ToString("F2").PadRight(15),
						(num2 / growDays).ToString("F2").PadRight(15),
						num3.ToString("F2").PadRight(15),
						(num3 / growDays).ToString("F2").PadRight(15)
					});
					stringBuilder.AppendLine(value2);
				}
			}
			Log.Message(stringBuilder.ToString());
		}
		private void LogAnimalsPerBiome()
		{
			IEnumerable<BiomeDef> enumerable = 
				from d in DefDatabase<BiomeDef>.AllDefs
				where d.animalDensity > 0f
				select d;
			IOrderedEnumerable<PawnKindDef> orderedEnumerable = 
				from d in DefDatabase<PawnKindDef>.AllDefs
				where d.race.race.Animal
				orderby (!d.race.race.predator) ? 0 : 1
				select d;
			string text = string.Empty;
			text += "name      commonality     commonalityShare\n\n";
			foreach (BiomeDef b in enumerable)
			{
				float num = orderedEnumerable.Sum((PawnKindDef a) => b.CommonalityOfAnimal(a));
				float f = (
					from a in orderedEnumerable
					where a.race.race.predator
					select a).Sum((PawnKindDef a) => b.CommonalityOfAnimal(a)) / num;
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					b.label,
					"   (predators: ",
					f.ToStringPercent("F2"),
					")"
				});
				foreach (PawnKindDef current in orderedEnumerable)
				{
					float num2 = b.CommonalityOfAnimal(current);
					if (num2 > 0f)
					{
						text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							"\n    ",
							current.label,
							"       ",
							num2.ToString("F3"),
							"       ",
							(num2 / num).ToStringPercent("F2")
						});
					}
				}
				text += "\n\n";
			}
			Log.Message(text);
		}
	}
}
