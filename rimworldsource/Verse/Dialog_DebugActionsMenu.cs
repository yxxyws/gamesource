using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse.AI;
using Verse.AI.Group;
namespace Verse
{
	public class Dialog_DebugActionsMenu : Dialog_DebugOptionLister
	{
		private const float DebugOptionsGap = 7f;
		public Dialog_DebugActionsMenu()
		{
			this.forcePause = true;
		}
		protected override void DoListingItems()
		{
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Slash)
			{
				Event.current.Use();
				this.Close(true);
			}
			if (Game.Mode == GameMode.MapPlaying)
			{
				this.DoListingItems_GameModeMap();
			}
		}
		[DebuggerHidden]
		private IEnumerable<float> PointsOptions()
		{
			Dialog_DebugActionsMenu.<PointsOptions>c__Iterator18D <PointsOptions>c__Iterator18D = new Dialog_DebugActionsMenu.<PointsOptions>c__Iterator18D();
			Dialog_DebugActionsMenu.<PointsOptions>c__Iterator18D expr_07 = <PointsOptions>c__Iterator18D;
			expr_07.$PC = -2;
			return expr_07;
		}
		private void DoListingItems_GameModeMap()
		{
			Text.Font = GameFont.Tiny;
			this.DoLabel("Incidents");
			base.DrawDebugAction("Execute incident...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (IncidentDef current in 
					from d in DefDatabase<IncidentDef>.AllDefs
					orderby d.defName
					select d)
				{
					IncidentDef localDef = current;
					string text = localDef.defName;
					if (!localDef.Worker.StorytellerCanUseNow())
					{
						text += " [NO]";
					}
					list.Add(new DebugMenuOption(text, DebugMenuOptionMode.Action, delegate
					{
						IncidentParms parms = new IncidentParms();
						if (localDef.pointsScaleable)
						{
							IncidentMaker incidentMaker = Find.Storyteller.incidentMakers.First((IncidentMaker x) => x is IncidentMaker_MainClassic || x is IncidentMaker_MainRandom);
							parms = incidentMaker.ParmsNow(localDef.category);
						}
						localDef.Worker.TryExecute(parms);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Execute incident with...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (IncidentDef current in 
					from d in DefDatabase<IncidentDef>.AllDefs
					where d.pointsScaleable
					orderby d.defName
					select d)
				{
					IncidentDef localDef = current;
					string text = localDef.defName;
					if (!localDef.Worker.StorytellerCanUseNow())
					{
						text += " [NO]";
					}
					list.Add(new DebugMenuOption(text, DebugMenuOptionMode.Action, delegate
					{
						IncidentParms parms = new IncidentParms();
						List<DebugMenuOption> list2 = new List<DebugMenuOption>();
						foreach (float num in this.PointsOptions())
						{
							float localPoints = num;
							list2.Add(new DebugMenuOption(num + " points", DebugMenuOptionMode.Action, delegate
							{
								parms.points = localPoints;
								localDef.Worker.TryExecute(parms);
							}));
						}
						Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Execute raid with...", delegate
			{
				IncidentMaker incidentMaker = Find.Storyteller.incidentMakers.First((IncidentMaker x) => x is IncidentMaker_MainClassic || x is IncidentMaker_MainRandom);
				IncidentParms parms = incidentMaker.ParmsNow(IncidentCategory.ThreatBig);
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (Faction current in Find.FactionManager.AllFactions)
				{
					Faction localFac = current;
					list.Add(new DebugMenuOption(localFac.name + " (" + localFac.def.defName + ")", DebugMenuOptionMode.Action, delegate
					{
						parms.faction = localFac;
						List<DebugMenuOption> list2 = new List<DebugMenuOption>();
						foreach (float num in this.PointsOptions())
						{
							float localPoints = num;
							list2.Add(new DebugMenuOption(num + " points", DebugMenuOptionMode.Action, delegate
							{
								parms.points = localPoints;
								List<DebugMenuOption> list3 = new List<DebugMenuOption>();
								foreach (RaidStrategyDef current2 in DefDatabase<RaidStrategyDef>.AllDefs)
								{
									RaidStrategyDef localStrat = current2;
									string text = localStrat.defName;
									if (!localStrat.Worker.CanUseWith(parms))
									{
										text += " [NO]";
									}
									list3.Add(new DebugMenuOption(text, DebugMenuOptionMode.Action, delegate
									{
										parms.raidStrategy = localStrat;
										this.DoRaid(parms);
									}));
								}
								Find.WindowStack.Add(new Dialog_DebugOptionListLister(list3));
							}));
						}
						Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			Action<int> DoRandomEnemyRaid = delegate(int pts)
			{
				this.Close(true);
				IncidentParms incidentParms = new IncidentParms();
				incidentParms.points = (float)pts;
				DefDatabase<IncidentDef>.GetNamed("RaidEnemy", true).Worker.TryExecute(incidentParms);
			};
			base.DrawDebugAction("Raid (35 pts)", delegate
			{
				DoRandomEnemyRaid(35);
			});
			base.DrawDebugAction("Raid (75 pts)", delegate
			{
				DoRandomEnemyRaid(75);
			});
			base.DrawDebugAction("Raid (300 pts)", delegate
			{
				DoRandomEnemyRaid(300);
			});
			base.DrawDebugAction("Raid (400 pts)", delegate
			{
				DoRandomEnemyRaid(400);
			});
			base.DrawDebugAction("Raid  (1000 pts)", delegate
			{
				DoRandomEnemyRaid(1000);
			});
			base.DrawDebugAction("Raid  (3000 pts)", delegate
			{
				DoRandomEnemyRaid(3000);
			});
			this.DoGap();
			this.DoLabel("Actions - Misc");
			base.DrawDebugAction("Change weather...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (WeatherDef current in DefDatabase<WeatherDef>.AllDefs)
				{
					WeatherDef localWeather = current;
					list.Add(new DebugMenuOption(localWeather.LabelCap, DebugMenuOptionMode.Action, delegate
					{
						Find.WeatherManager.TransitionTo(localWeather);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Start song...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (SongDef current in DefDatabase<SongDef>.AllDefs)
				{
					SongDef localSong = current;
					list.Add(new DebugMenuOption(localSong.defName, DebugMenuOptionMode.Action, delegate
					{
						Find.MusicManagerMap.ForceStartSong(localSong, false);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			if (Find.MapConditionManager.ActiveConditions.Count > 0)
			{
				base.DrawDebugAction("End map condition ...", delegate
				{
					List<DebugMenuOption> list = new List<DebugMenuOption>();
					foreach (MapCondition current in Find.MapConditionManager.ActiveConditions)
					{
						MapCondition localMc = current;
						list.Add(new DebugMenuOption(localMc.LabelCap, DebugMenuOptionMode.Action, delegate
						{
							localMc.End();
						}));
					}
					Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
				});
			}
			base.DrawDebugAction("Add prisoner", delegate
			{
				this.AddGuest(true);
			});
			base.DrawDebugAction("Add guest", delegate
			{
				this.AddGuest(false);
			});
			base.DrawDebugAction("Force enemy assault", delegate
			{
				foreach (Lord current in Find.LordManager.lords)
				{
					LordToil_Stage lordToil_Stage = current.CurLordToil as LordToil_Stage;
					if (lordToil_Stage != null)
					{
						foreach (Transition current2 in current.Graph.transitions)
						{
							if (current2.sources.Contains(lordToil_Stage) && current2.target is LordToil_AssaultColony)
							{
								Messages.Message("Debug forcing to assault toil: " + current.faction, MessageSound.SeriousAlert);
								current.GotoToil(current2.target);
								return;
							}
						}
					}
				}
			});
			base.DrawDebugAction("Force enemy flee", delegate
			{
				foreach (Lord current in Find.LordManager.lords)
				{
					if (current.faction.HostileTo(Faction.OfColony) && current.faction.def.canFlee)
					{
						LordToil lordToil = current.Graph.lordToils.FirstOrDefault((LordToil st) => st is LordToil_PanicFlee);
						if (lordToil != null)
						{
							current.GotoToil(lordToil);
						}
					}
				}
			});
			base.DrawDebugAction("Destroy all things", delegate
			{
				foreach (Thing current in Find.ListerThings.AllThings.ToList<Thing>())
				{
					current.Destroy(DestroyMode.Vanish);
					Pawn pawn = current as Pawn;
					if (pawn != null)
					{
						Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
					}
				}
			});
			base.DrawDebugAction("Destroy all plants", delegate
			{
				foreach (Thing current in Find.ListerThings.AllThings.ToList<Thing>())
				{
					if (current is Plant)
					{
						current.Destroy(DestroyMode.Vanish);
					}
				}
			});
			base.DrawDebugAction("Unload unused assets", delegate
			{
				LongEventHandler.QueueLongEvent(delegate
				{
					Resources.UnloadUnusedAssets();
				}, "UnloadingUnusedAssets", false, null);
			});
			base.DrawDebugAction("Name colony", delegate
			{
				Find.WindowStack.Add(new Dialog_NameColony());
			});
			base.DrawDebugAction("Next lesson", delegate
			{
				ConceptDecider.DebugForceInitiateBestLessonNow();
			});
			base.DrawDebugAction("Regen all map mesh sections", delegate
			{
				Find.Map.mapDrawer.RegenerateEverythingNow();
			});
			base.DrawDebugAction("Finish all research", delegate
			{
				Find.ResearchManager.DebugSetAllProjectsFinished();
				Messages.Message("All research finished.", MessageSound.Benefit);
			});
			base.DrawDebugAction("Replace all trade ships", delegate
			{
				Find.PassingShipManager.DebugSendAllShipsAway();
				for (int i = 0; i < 5; i++)
				{
					DefDatabase<IncidentDef>.GetNamed("OrbitalTraderArrival", true).Worker.TryExecute(new IncidentParms());
				}
			});
			base.DrawDebugAction("Change camera config...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (Type current in typeof(CameraMapConfig).AllSubclasses())
				{
					Type localType = current;
					list.Add(new DebugMenuOption(localType.Name, DebugMenuOptionMode.Action, delegate
					{
						Find.CameraMap.config = (CameraMapConfig)Activator.CreateInstance(localType);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Force ship countdown", delegate
			{
				ShipCountdown.InitiateCountdown(null);
			});
			base.DrawDebugAction("Force queue big threat", delegate
			{
				IncidentMaker_MainClassic incidentMaker_MainClassic = (IncidentMaker_MainClassic)Find.Storyteller.incidentMakers.First((IncidentMaker x) => x is IncidentMaker_MainClassic);
				incidentMaker_MainClassic.ForceQueueThreatBig();
			});
			base.DrawDebugAction("Flash trade drop spot", delegate
			{
				IntVec3 intVec = DropCellFinder.TradeDropSpot();
				Find.DebugDrawer.FlashCell(intVec, 0f, null);
				Log.Message("trade drop spot: " + intVec);
			});
			base.DrawDebugAction("Kill faction leader", delegate
			{
				Pawn leader = (
					from x in Find.FactionManager.AllFactions
					where x.leader != null
					select x).RandomElement<Faction>().leader;
				int num = 0;
				while (!leader.Dead)
				{
					if (++num > 1000)
					{
						Log.Warning("Could not kill faction leader.");
						break;
					}
					leader.TakeDamage(new DamageInfo(DamageDefOf.Bullet, 30, null, 0f, null, null));
				}
			});
			base.DrawDebugAction("Spawn world pawn", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				Action<Pawn> act = delegate(Pawn p)
				{
					List<DebugMenuOption> list2 = new List<DebugMenuOption>();
					foreach (PawnKindDef current2 in 
						from x in DefDatabase<PawnKindDef>.AllDefs
						where x.race == p.def
						select x)
					{
						PawnKindDef kLocal = current2;
						list2.Add(new DebugMenuOption(kLocal.defName, DebugMenuOptionMode.Tool, delegate
						{
							PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest();
							pawnGenerationRequest.kindDef = kLocal;
							pawnGenerationRequest.faction = p.Faction;
							PawnGenerator.RedressPawn(p, pawnGenerationRequest);
							GenSpawn.Spawn(p, Gen.MouseCell());
							DebugTools.curTool = null;
						}));
					}
					Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
				};
				foreach (Pawn current in Find.WorldPawns.AllPawnsAlive)
				{
					Pawn pLocal = current;
					list.Add(new DebugMenuOption(current.LabelBaseShort, DebugMenuOptionMode.Action, delegate
					{
						act(pLocal);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Refog map", delegate
			{
				FloodFillerFog.TestRefogMap();
			});
			this.DoGap();
			this.DoLabel("Tools - General");
			base.DrawDebugTool("Tool: Destroy", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()).ToList<Thing>())
				{
					Pawn pawn = current as Pawn;
					if (pawn != null)
					{
						current.Destroy(DestroyMode.Vanish);
						Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
					}
					else
					{
						current.Destroy(DestroyMode.Vanish);
					}
				}
			});
			base.DrawDebugTool("Tool: 10 damage", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()).ToList<Thing>())
				{
					current.TakeDamage(new DamageInfo(DamageDefOf.Crush, 10, null, null, null));
				}
			});
			base.DrawDebugTool("Tool: 5000 damage", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()).ToList<Thing>())
				{
					current.TakeDamage(new DamageInfo(DamageDefOf.Crush, 5000, null, null, null));
				}
			});
			base.DrawDebugTool("Tool: Clear area 21x21", delegate
			{
				CellRect r = CellRect.CenteredOn(Gen.MouseCell(), 10);
				GenDebug.ClearArea(r);
			});
			base.DrawDebugTool("Tool: Rock 21x21", delegate
			{
				CellRect cellRect = CellRect.CenteredOn(Gen.MouseCell(), 10);
				cellRect.ClipInsideMap();
				ThingDef def = ThingDef.Named("Granite");
				foreach (IntVec3 current in cellRect)
				{
					GenSpawn.Spawn(def, current);
				}
			});
			this.DoGap();
			base.DrawDebugTool("Tool: Explosion (bomb)", delegate
			{
				GenExplosion.DoExplosion(Gen.MouseCell(), 3.9f, DamageDefOf.Bomb, null, null, null, null, null, 0f, false, null, 0f);
			});
			base.DrawDebugTool("Tool: Explosion (flame)", delegate
			{
				GenExplosion.DoExplosion(Gen.MouseCell(), 3.9f, DamageDefOf.Flame, null, null, null, null, null, 0f, false, null, 0f);
			});
			base.DrawDebugTool("Tool: Explosion (stun)", delegate
			{
				GenExplosion.DoExplosion(Gen.MouseCell(), 3.9f, DamageDefOf.Stun, null, null, null, null, null, 0f, false, null, 0f);
			});
			base.DrawDebugTool("Tool: Explosion (EMP)", delegate
			{
				GenExplosion.DoExplosion(Gen.MouseCell(), 3.9f, DamageDefOf.EMP, null, null, null, null, null, 0f, false, null, 0f);
			});
			base.DrawDebugTool("Tool: Explosion (extinguisher)", delegate
			{
				ThingDef filthFireFoam = ThingDefOf.FilthFireFoam;
				GenExplosion.DoExplosion(Gen.MouseCell(), 10f, DamageDefOf.Extinguish, null, null, null, null, filthFireFoam, 1f, true, null, 0f);
			});
			base.DrawDebugTool("Tool: Lightning strike", delegate
			{
				Find.WeatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(Gen.MouseCell()));
			});
			this.DoGap();
			base.DrawDebugTool("Tool: Add snow", delegate
			{
				SnowUtility.AddSnowRadial(Gen.MouseCell(), 5f, 1f);
			});
			base.DrawDebugTool("Tool: Remove snow", delegate
			{
				SnowUtility.AddSnowRadial(Gen.MouseCell(), 5f, -1f);
			});
			base.DrawDebugAction("Clear all snow", delegate
			{
				foreach (IntVec3 current in Find.Map.AllCells)
				{
					Find.SnowGrid.SetDepth(current, 0f);
				}
			});
			base.DrawDebugTool("Tool: Push heat (10)", delegate
			{
				GenTemperature.PushHeat(Gen.MouseCell(), 10f);
			});
			base.DrawDebugTool("Tool: Push heat (10000)", delegate
			{
				GenTemperature.PushHeat(Gen.MouseCell(), 10000f);
			});
			base.DrawDebugTool("Tool: Push heat (-1000)", delegate
			{
				GenTemperature.PushHeat(Gen.MouseCell(), -1000f);
			});
			this.DoGap();
			base.DrawDebugTool("Tool: Spawn grass seed", delegate
			{
				GenPlantReproduction.TrySpawnSeed(Gen.MouseCell(), ThingDef.Named("PlantPovertyGrass"), SeedTargFindMode.ReproduceSeed, null);
			});
			base.DrawDebugTool("Tool: Finish plant growth", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()))
				{
					Plant plant = current as Plant;
					if (plant != null)
					{
						plant.growth = 1f;
					}
				}
			});
			base.DrawDebugTool("Tool: Grow 1 day", delegate
			{
				IntVec3 intVec = Gen.MouseCell();
				Plant plant = intVec.GetPlant();
				if (plant != null && plant.def.plant != null)
				{
					int num = (int)((1f - plant.growth) * plant.def.plant.growDays);
					if (num >= 60000)
					{
						plant.age += 60000;
					}
					else
					{
						if (num > 0)
						{
							plant.age += num;
						}
					}
					plant.growth += 1f / plant.def.plant.growDays;
					if ((double)plant.growth > 1.0)
					{
						plant.growth = 1f;
					}
					Find.MapDrawer.SectionAt(intVec).RegenerateAllLayers();
				}
			});
			base.DrawDebugTool("Tool: Grow to maturity", delegate
			{
				IntVec3 intVec = Gen.MouseCell();
				Plant plant = intVec.GetPlant();
				if (plant != null && plant.def.plant != null)
				{
					int num = (int)((1f - plant.growth) * plant.def.plant.growDays);
					plant.age += num;
					plant.growth = 1f;
					Find.MapDrawer.SectionAt(intVec).RegenerateAllLayers();
				}
			});
			this.DoGap();
			base.DrawDebugTool("Tool: Regen section", delegate
			{
				Find.MapDrawer.SectionAt(Gen.MouseCell()).RegenerateAllLayers();
			});
			base.DrawDebugTool("Tool: Randomize color", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()))
				{
					CompColorable compColorable = current.TryGetComp<CompColorable>();
					if (compColorable != null)
					{
						current.SetColor(GenColor.RandomColorOpaque(), true);
					}
				}
			});
			base.DrawDebugTool("Tool: Rot 3 days", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()))
				{
					CompRottable compRottable = current.TryGetComp<CompRottable>();
					if (compRottable != null)
					{
						compRottable.rotProgress += 180000f;
					}
				}
			});
			base.DrawDebugTool("Tool: Fuel -20%", delegate
			{
				foreach (Thing current in Find.ThingGrid.ThingsAt(Gen.MouseCell()))
				{
					CompRefuelable compRefuelable = current.TryGetComp<CompRefuelable>();
					if (compRefuelable != null)
					{
						compRefuelable.ConsumeFuel(compRefuelable.Props.fuelCapacity * 0.2f);
					}
				}
			});
			base.DrawDebugAction("Tool: Use scatterer", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugTools_MapGen.Options_Scatterers()));
			});
			base.DrawDebugTool("Tool: Add trap memory", delegate
			{
				foreach (Faction current in Find.World.factionManager.AllFactions)
				{
					current.TacticalMemory.TrapRevealed(Gen.MouseCell());
				}
				Find.DebugDrawer.FlashCell(Gen.MouseCell(), 0f, "added");
			});
			base.DrawDebugTool("Tool: Test flood unfog", delegate
			{
				FloodFillerFog.TestFloodUnfog(Gen.MouseCell());
			});
			base.DrawDebugTool("Tool: Flash closewalk cell 30", delegate
			{
				IntVec3 c = CellFinder.RandomClosewalkCellNear(Gen.MouseCell(), 30);
				Find.DebugDrawer.FlashCell(c, 0f, null);
			});
			base.DrawDebugTool("Tool: Flash walk path", delegate
			{
				WalkPathFinder.DebugFlashWalkPath(Gen.MouseCell(), 8);
			});
			base.DrawDebugTool("Tool: Flash skygaze cell", delegate
			{
				Pawn pawn = Find.MapPawns.FreeColonists.First<Pawn>();
				IntVec3 c;
				RCellFinder.TryFindSkygazeCell(Gen.MouseCell(), pawn, out c);
				Find.DebugDrawer.FlashCell(c, 0f, null);
				MoteThrower.ThrowText(c.ToVector3Shifted(), "for " + pawn.Label, Color.white, -1);
			});
			base.DrawDebugTool("Tool: Flash direct flee dest", delegate
			{
				Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
				if (pawn == null)
				{
					Find.DebugDrawer.FlashCell(Gen.MouseCell(), 0f, "select a pawn");
				}
				else
				{
					IntVec3 c;
					if (RCellFinder.TryFindDirectFleeDestination(Gen.MouseCell(), 9f, pawn, out c))
					{
						Find.DebugDrawer.FlashCell(c, 0.5f, null);
					}
					else
					{
						Find.DebugDrawer.FlashCell(Gen.MouseCell(), 0.8f, "not found");
					}
				}
			});
			base.DrawDebugAction("Tool: Flash spectators cells", delegate
			{
				Action<bool> act = delegate(bool bestSideOnly)
				{
					DebugTool tool = null;
					IntVec3 firstCorner;
					tool = new DebugTool("first watch rect corner...", delegate
					{
						firstCorner = Gen.MouseCell();
						DebugTools.curTool = new DebugTool("second watch rect corner...", delegate
						{
							IntVec3 intVec = Gen.MouseCell();
							CellRect spectateRect = CellRect.FromLimits(Mathf.Min(firstCorner.x, intVec.x), Mathf.Min(firstCorner.z, intVec.z), Mathf.Max(firstCorner.x, intVec.x), Mathf.Max(firstCorner.z, intVec.z));
							SpectateRectSide allowedSides = SpectateRectSide.All;
							if (bestSideOnly)
							{
								allowedSides = SpectatorCellFinder.FindSingleBestSide(spectateRect, SpectateRectSide.All, 1);
							}
							SpectatorCellFinder.DebugFlashPotentialSpectatorCells(spectateRect, allowedSides, 1);
							DebugTools.curTool = tool;
						}, delegate
						{
							IntVec3 intVec = Gen.MouseCell();
							Vector3 position = firstCorner.ToVector3Shifted();
							Vector3 position2 = intVec.ToVector3Shifted();
							if (position.x < position2.x)
							{
								position.x -= 0.5f;
								position2.x += 0.5f;
							}
							else
							{
								position.x += 0.5f;
								position2.x -= 0.5f;
							}
							if (position.z < position2.z)
							{
								position.z -= 0.5f;
								position2.z += 0.5f;
							}
							else
							{
								position.z += 0.5f;
								position2.z -= 0.5f;
							}
							Vector3 vector = Find.CameraMapGameObject.camera.WorldToScreenPoint(position);
							Vector3 vector2 = Find.CameraMapGameObject.camera.WorldToScreenPoint(position2);
							Vector2 vector3 = new Vector2(vector.x, (float)Screen.height - vector.y);
							Vector2 vector4 = new Vector2(vector2.x, (float)Screen.height - vector2.y);
							Widgets.DrawBox(new Rect(vector3.x, vector3.y, vector4.x - vector3.x, vector4.y - vector3.y), 3);
						});
					}, null);
					DebugTools.curTool = tool;
				};
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				list.Add(new DebugMenuOption("All sides", DebugMenuOptionMode.Action, delegate
				{
					act(false);
				}));
				list.Add(new DebugMenuOption("Best side only", DebugMenuOptionMode.Action, delegate
				{
					act(true);
				}));
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugToolForPawns("Tool: Flash TryFindRandomPawnExitCell", delegate(Pawn p)
			{
				IntVec3 intVec;
				if (CellFinder.TryFindRandomPawnExitCell(p, out intVec))
				{
					Find.DebugDrawer.FlashCell(intVec, 0.5f, null);
					Find.DebugDrawer.FlashLine(p.Position, intVec);
				}
				else
				{
					Find.DebugDrawer.FlashCell(p.Position, 0.2f, "no exit cell");
				}
			});
			this.DoGap();
			this.DoLabel("Tools - Pawns");
			base.DrawDebugToolForPawns("Tool: Down", delegate(Pawn p)
			{
				HealthUtility.GiveInjuriesToForceDowned(p);
			});
			base.DrawDebugToolForPawns("Tool: Kill", delegate(Pawn p)
			{
				HealthUtility.GiveInjuriesToKill(p);
			});
			base.DrawDebugToolForPawns("Tool: Surgery fail catastrophic", delegate(Pawn p)
			{
				HealthUtility.GiveInjuriesOperationFailureCatastrophic(p);
			});
			base.DrawDebugToolForPawns("Tool: Surgery fail minor", delegate(Pawn p)
			{
				HealthUtility.GiveInjuriesOperationFailureMinor(p);
			});
			base.DrawDebugAction("Tool: Apply damage...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugTools_Health.Options_ApplyDamage()));
			});
			base.DrawDebugAction("Tool: Add Hediff...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugTools_Health.Options_AddHediff()));
			});
			base.DrawDebugToolForPawns("Tool: Force vomit...", delegate(Pawn p)
			{
				p.jobs.StartJob(new Job(JobDefOf.Vomit), JobCondition.InterruptForced, null, true, true, null);
			});
			base.DrawDebugTool("Tool: Food -20%", delegate
			{
				this.OffsetNeed(NeedDefOf.Food, -0.2f);
			});
			base.DrawDebugTool("Tool: Rest -20%", delegate
			{
				this.OffsetNeed(NeedDefOf.Rest, -0.2f);
			});
			base.DrawDebugTool("Tool: Joy -20%", delegate
			{
				this.OffsetNeed(NeedDefOf.Joy, -0.2f);
			});
			base.DrawDebugToolForPawns("Tool: Max skills", delegate(Pawn p)
			{
				if (p.skills != null)
				{
					foreach (SkillDef current in DefDatabase<SkillDef>.AllDefs)
					{
						p.skills.Learn(current, 1E+08f);
					}
					this.DustPuffFrom(p);
				}
				if (p.training != null)
				{
					foreach (TrainableDef current2 in DefDatabase<TrainableDef>.AllDefs)
					{
						Pawn trainer = Find.MapPawns.FreeColonistsSpawned.RandomElement<Pawn>();
						bool flag;
						if (p.training.CanAssignToTrain(current2, out flag).Accepted)
						{
							p.training.Train(current2, trainer);
						}
					}
				}
			});
			base.DrawDebugAction("Tool: Mental state...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				list.Add(new DebugMenuOption("(random)", DebugMenuOptionMode.Tool, delegate
				{
					foreach (Pawn current2 in (
						from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
						where t is Pawn
						select t).Cast<Pawn>())
					{
						current2.mindState.mentalStateStarter.TryStartMentalState(null);
						this.DustPuffFrom(current2);
					}
				}));
				foreach (MentalStateDef current in DefDatabase<MentalStateDef>.AllDefs)
				{
					MentalStateDef locBrDef = current;
					string text = locBrDef.defName;
					if (!current.Worker.StateCanOccur(Find.MapPawns.FreeColonists.First<Pawn>()))
					{
						text += " (cntoccr)";
					}
					list.Add(new DebugMenuOption(text, DebugMenuOptionMode.Tool, delegate
					{
						foreach (Pawn current2 in (
							from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
							where t is Pawn
							select t).Cast<Pawn>())
						{
							Pawn locP = current2;
							if (locBrDef != MentalStateDefOf.SocialFighting)
							{
								locP.mindState.mentalStateHandler.StartMentalState(locBrDef);
								this.DustPuffFrom(locP);
							}
							else
							{
								DebugTools.curTool = new DebugTool("...with", delegate
								{
									Pawn pawn = (Pawn)(
										from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
										where t is Pawn
										select t).FirstOrDefault<Thing>();
									if (pawn != null)
									{
										if (!InteractionUtility.HasAnySocialFightProvokingThought(locP, pawn))
										{
											Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.Insulted);
											thought_SocialMemory.SetOtherPawn(pawn);
											locP.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
											Messages.Message("Dev: auto added negative thought.", locP, MessageSound.Standard);
										}
										locP.interactions.StartSocialFight(pawn);
										DebugTools.curTool = null;
									}
								}, null);
							}
						}
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Tool: Give trait...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (TraitDef current in DefDatabase<TraitDef>.AllDefs)
				{
					TraitDef trDef = current;
					for (int j = 0; j < current.degreeDatas.Count; j++)
					{
						int i = j;
						list.Add(new DebugMenuOption(string.Concat(new object[]
						{
							trDef.degreeDatas[i].label,
							" (",
							trDef.degreeDatas[j].degree,
							")"
						}), DebugMenuOptionMode.Tool, delegate
						{
							foreach (Pawn current2 in (
								from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
								where t is Pawn
								select t).Cast<Pawn>())
							{
								if (current2.story != null)
								{
									Trait trait = new Trait(trDef);
									trait.degree = trDef.degreeDatas[i].degree;
									current2.story.traits.allTraits.Add(trait);
									this.DustPuffFrom(current2);
								}
							}
						}));
					}
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugToolForPawns("Tool: Give good thought", delegate(Pawn p)
			{
				if (p.needs.mood != null)
				{
					p.needs.mood.thoughts.TryGainThought(ThoughtDef.Named("DebugGood"));
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Give bad thought", delegate(Pawn p)
			{
				if (p.needs.mood != null)
				{
					p.needs.mood.thoughts.TryGainThought(ThoughtDef.Named("DebugBad"));
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Make faction hostile", delegate(Pawn p)
			{
				if (p.Faction != null && !p.Faction.HostileTo(Faction.OfColony))
				{
					p.Faction.SetHostileTo(Faction.OfColony, true);
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Make faction neutral", delegate(Pawn p)
			{
				if (p.Faction != null && p.Faction.HostileTo(Faction.OfColony))
				{
					p.Faction.SetHostileTo(Faction.OfColony, false);
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugTool("Tool: Clear bound unfinished things", delegate
			{
				foreach (Building_WorkTable current in (
					from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
					where t is Building_WorkTable
					select t).Cast<Building_WorkTable>())
				{
					foreach (Bill current2 in current.BillStack)
					{
						Bill_ProductionWithUft bill_ProductionWithUft = current2 as Bill_ProductionWithUft;
						if (bill_ProductionWithUft != null)
						{
							bill_ProductionWithUft.ClearBoundUft();
						}
					}
				}
			});
			base.DrawDebugToolForPawns("Tool: Force birthday", delegate(Pawn p)
			{
				p.ageTracker.AgeBiologicalTicks = (long)((p.ageTracker.AgeBiologicalYears + 1) * 3600000 + 1);
				p.ageTracker.DebugForceBirthdayBiological();
			});
			base.DrawDebugToolForPawns("Tool: Recruit", delegate(Pawn p)
			{
				if (p.Faction != Faction.OfColony && p.RaceProps.Humanlike)
				{
					InteractionWorker_RecruitAttempt.DoRecruit(Find.MapPawns.FreeColonists.RandomElement<Pawn>(), p, 1f);
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Damage apparel", delegate(Pawn p)
			{
				if (p.apparel != null && p.apparel.WornApparelCount > 0)
				{
					p.apparel.WornApparel.RandomElement<Apparel>().TakeDamage(new DamageInfo(DamageDefOf.Deterioration, 30, null, null, null));
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Tame animal", delegate(Pawn p)
			{
				if (p.RaceProps.Animal && p.Faction != Faction.OfColony)
				{
					InteractionWorker_RecruitAttempt.DoRecruit(Find.MapPawns.FreeColonists.FirstOrDefault<Pawn>(), p, 1f);
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Give birth", delegate(Pawn p)
			{
				Hediff_Pregnant.DoBirthSpawn(p, null);
				this.DustPuffFrom(p);
			});
			base.DrawDebugToolForPawns("Tool: Name animal by nuzzling", delegate(Pawn p)
			{
				if ((p.Name == null || p.Name.Numerical) && p.RaceProps.Animal)
				{
					PawnUtility.GiveNameBecauseOfNuzzle(Find.MapPawns.FreeColonists.First<Pawn>(), p);
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Activate HediffGiver", delegate(Pawn p)
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				if (p.RaceProps.hediffGiverSets != null)
				{
					foreach (HediffGiver current in p.RaceProps.hediffGiverSets.SelectMany((HediffGiverSetDef set) => set.hediffGivers))
					{
						HediffGiver localHdg = current;
						list.Add(new FloatMenuOption(localHdg.hediff.defName, delegate
						{
							localHdg.TryApply(p, null);
						}, MenuOptionPriority.Medium, null, null));
					}
				}
				if (list.Any<FloatMenuOption>())
				{
					Find.WindowStack.Add(new FloatMenu(list, false));
					this.DustPuffFrom(p);
				}
			});
			base.DrawDebugToolForPawns("Tool: Add/remove pawn relation", delegate(Pawn p)
			{
				if (!p.RaceProps.IsFlesh)
				{
					return;
				}
				Action<bool> act = delegate(bool add)
				{
					if (add)
					{
						List<DebugMenuOption> list2 = new List<DebugMenuOption>();
						foreach (PawnRelationDef current in DefDatabase<PawnRelationDef>.AllDefs)
						{
							if (!current.implied)
							{
								PawnRelationDef defLocal = current;
								list2.Add(new DebugMenuOption(defLocal.defName, DebugMenuOptionMode.Action, delegate
								{
									List<DebugMenuOption> list4 = new List<DebugMenuOption>();
									IOrderedEnumerable<Pawn> orderedEnumerable = 
										from x in PawnUtility.AllPawnsMapOrWorldAlive
										where x.RaceProps.IsFlesh
										orderby x.def == p.def descending, x.IsWorldPawn()
										select x;
									foreach (Pawn current2 in orderedEnumerable)
									{
										if (p != current2)
										{
											if (!defLocal.familyByBloodRelation || current2.def == p.def)
											{
												if (!p.relations.DirectRelationExists(defLocal, current2))
												{
													Pawn otherLocal = current2;
													list4.Add(new DebugMenuOption(otherLocal.LabelBaseShort + " (" + otherLocal.KindLabel + ")", DebugMenuOptionMode.Action, delegate
													{
														p.relations.AddDirectRelation(defLocal, otherLocal);
													}));
												}
											}
										}
									}
									Find.WindowStack.Add(new Dialog_DebugOptionListLister(list4));
								}));
							}
						}
						Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
					}
					else
					{
						List<DebugMenuOption> list3 = new List<DebugMenuOption>();
						List<DirectPawnRelation> directRelations = p.relations.DirectRelations;
						for (int i = 0; i < directRelations.Count; i++)
						{
							DirectPawnRelation rel = directRelations[i];
							list3.Add(new DebugMenuOption(rel.def.defName + " - " + rel.otherPawn.LabelBaseShort, DebugMenuOptionMode.Action, delegate
							{
								p.relations.RemoveDirectRelation(rel);
							}));
						}
						Find.WindowStack.Add(new Dialog_DebugOptionListLister(list3));
					}
				};
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				list.Add(new DebugMenuOption("Add", DebugMenuOptionMode.Action, delegate
				{
					act(true);
				}));
				list.Add(new DebugMenuOption("Remove", DebugMenuOptionMode.Action, delegate
				{
					act(false);
				}));
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugToolForPawns("Tool: Add opinion thoughts about", delegate(Pawn p)
			{
				if (!p.RaceProps.Humanlike)
				{
					return;
				}
				Action<bool> act = delegate(bool good)
				{
					foreach (Pawn current in 
						from x in Find.MapPawns.AllPawnsSpawned
						where x.RaceProps.Humanlike
						select x)
					{
						if (p != current)
						{
							IEnumerable<ThoughtDef> source = DefDatabase<ThoughtDef>.AllDefs.Where((ThoughtDef x) => typeof(Thought_SocialMemory).IsAssignableFrom(x.thoughtClass) && ((good && x.stages[0].baseOpinionOffset > 0f) || (!good && x.stages[0].baseOpinionOffset < 0f)));
							if (source.Any<ThoughtDef>())
							{
								int num = Rand.Range(2, 5);
								for (int i = 0; i < num; i++)
								{
									ThoughtDef def = source.RandomElement<ThoughtDef>();
									Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(def);
									thought_SocialMemory.SetOtherPawn(p);
									current.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
								}
							}
						}
					}
				};
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				list.Add(new DebugMenuOption("Good", DebugMenuOptionMode.Action, delegate
				{
					act(true);
				}));
				list.Add(new DebugMenuOption("Bad", DebugMenuOptionMode.Action, delegate
				{
					act(false);
				}));
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugToolForPawns("Tool: Try develop bond relation", delegate(Pawn p)
			{
				if (p.Faction == null)
				{
					return;
				}
				if (p.RaceProps.Humanlike)
				{
					IEnumerable<Pawn> source = 
						from x in Find.MapPawns.AllPawnsSpawned
						where x.RaceProps.Animal && x.Faction == p.Faction
						select x;
					if (source.Any<Pawn>())
					{
						RelationsUtility.TryDevelopBondRelation(p, source.RandomElement<Pawn>(), 999999f);
					}
				}
				else
				{
					if (p.RaceProps.Animal)
					{
						IEnumerable<Pawn> source2 = 
							from x in Find.MapPawns.AllPawnsSpawned
							where x.RaceProps.Humanlike && x.Faction == p.Faction
							select x;
						if (source2.Any<Pawn>())
						{
							RelationsUtility.TryDevelopBondRelation(source2.RandomElement<Pawn>(), p, 999999f);
						}
					}
				}
			});
			base.DrawDebugToolForPawns("Tool: Start marriage ceremony", delegate(Pawn p)
			{
				if (!p.RaceProps.Humanlike)
				{
					return;
				}
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (Pawn current in 
					from x in Find.MapPawns.AllPawnsSpawned
					where x.RaceProps.Humanlike
					select x)
				{
					if (p != current)
					{
						Pawn otherLocal = current;
						list.Add(new DebugMenuOption(otherLocal.LabelBaseShort + " (" + otherLocal.KindLabel + ")", DebugMenuOptionMode.Action, delegate
						{
							if (!p.relations.DirectRelationExists(PawnRelationDefOf.Fiance, otherLocal))
							{
								p.relations.TryRemoveDirectRelation(PawnRelationDefOf.Lover, otherLocal);
								p.relations.TryRemoveDirectRelation(PawnRelationDefOf.Spouse, otherLocal);
								p.relations.AddDirectRelation(PawnRelationDefOf.Fiance, otherLocal);
								Messages.Message("Dev: auto added fiance relation.", p, MessageSound.Standard);
							}
							if (!Find.VoluntarilyJoinableLordsStarter.TryStartMarriageCeremony(p, otherLocal))
							{
								Messages.Message("Could not find any valid marriage site.", MessageSound.Negative);
							}
						}));
					}
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Tool: Start party", delegate
			{
				if (!Find.VoluntarilyJoinableLordsStarter.TryStartParty())
				{
					Messages.Message("Could not find any valid party spot or organizer.", MessageSound.Negative);
				}
			});
			base.DrawDebugToolForPawns("Tool: Start prison break", delegate(Pawn p)
			{
				if (!p.IsPrisoner)
				{
					return;
				}
				PrisonBreakUtility.StartPrisonBreak(p);
			});
			base.DrawDebugToolForPawns("Tool: Pass to world", delegate(Pawn p)
			{
				p.DeSpawn();
				Find.WorldPawns.PassToWorld(p, PawnDiscardDecideMode.Keep);
			});
			this.DoGap();
			base.DrawDebugToolForPawns("Tool: EndCurrentJob(" + JobCondition.InterruptForced.ToString() + ")", delegate(Pawn p)
			{
				p.jobs.EndCurrentJob(JobCondition.InterruptForced);
				this.DustPuffFrom(p);
			});
			base.DrawDebugToolForPawns("Tool: CheckForJobOverride", delegate(Pawn p)
			{
				p.jobs.CheckForJobOverride();
				this.DustPuffFrom(p);
			});
			base.DrawDebugToolForPawns("Tool: Toggle job logging", delegate(Pawn p)
			{
				p.jobs.debugLog = !p.jobs.debugLog;
				this.DustPuffFrom(p);
				MoteThrower.ThrowText(p.DrawPos, p.LabelBaseShort + "\n" + ((!p.jobs.debugLog) ? "OFF" : "ON"), -1);
			});
			base.DrawDebugToolForPawns("Tool: Toggle stance logging", delegate(Pawn p)
			{
				p.stances.debugLog = !p.stances.debugLog;
				this.DustPuffFrom(p);
			});
			this.DoGap();
			this.DoLabel("Tools - Spawning");
			base.DrawDebugAction("Tool: Spawn pawn", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (PawnKindDef current in 
					from kd in DefDatabase<PawnKindDef>.AllDefs
					orderby kd.defName
					select kd)
				{
					PawnKindDef localKindDef = current;
					list.Add(new DebugMenuOption(localKindDef.defName, DebugMenuOptionMode.Tool, delegate
					{
						Faction faction = FactionUtility.DefaultFactionFrom(localKindDef.defaultFactionType);
						Pawn newPawn = PawnGenerator.GeneratePawn(localKindDef, faction);
						GenSpawn.Spawn(newPawn, Gen.MouseCell());
						if (faction != null && faction != Faction.OfColony)
						{
							Lord lord = null;
							if (Find.MapPawns.SpawnedPawnsInFaction(faction).Any((Pawn p) => p != newPawn))
							{
								Predicate<Thing> validator = (Thing p) => p != newPawn && ((Pawn)p).GetLord() != null;
								Pawn p2 = (Pawn)GenClosest.ClosestThing_Global(newPawn.Position, Find.MapPawns.SpawnedPawnsInFaction(faction), 99999f, validator);
								lord = p2.GetLord();
							}
							if (lord == null)
							{
								LordJob_DefendPoint lordJob = new LordJob_DefendPoint(newPawn.Position);
								lord = LordMaker.MakeNewLord(faction, lordJob, null);
							}
							lord.AddPawn(newPawn);
						}
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Tool: Spawn weapon...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (ThingDef current in 
					from def in DefDatabase<ThingDef>.AllDefs
					where def.equipmentType == EquipmentType.Primary
					select def)
				{
					ThingDef localDef = current;
					list.Add(new DebugMenuOption(localDef.LabelCap, DebugMenuOptionMode.Tool, delegate
					{
						DebugThingPlaceHelper.DebugSpawn(localDef, Gen.MouseCell(), -1, false);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugAction("Tool: Try place near thing...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugThingPlaceHelper.TryPlaceOptionsForStackCount(1, false)));
			});
			base.DrawDebugAction("Tool: Try place direct stacks of 25...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugThingPlaceHelper.TryPlaceOptionsForStackCount(25, true)));
			});
			base.DrawDebugAction("Tool: Try place near stacks of 25...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugThingPlaceHelper.TryPlaceOptionsForStackCount(25, false)));
			});
			base.DrawDebugAction("Tool: Try place near stacks of 75...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugThingPlaceHelper.TryPlaceOptionsForStackCount(75, false)));
			});
			base.DrawDebugAction("Tool: Try place near stacks of 500...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugThingPlaceHelper.TryPlaceOptionsForStackCount(500, false)));
			});
			base.DrawDebugAction("Tool: Set terrain...", delegate
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (TerrainDef current in DefDatabase<TerrainDef>.AllDefs)
				{
					TerrainDef localDef = current;
					list.Add(new DebugMenuOption(localDef.LabelCap, DebugMenuOptionMode.Tool, delegate
					{
						if (Gen.MouseCell().InBounds())
						{
							Find.TerrainGrid.SetTerrain(Gen.MouseCell(), localDef);
						}
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			});
			base.DrawDebugTool("Tool: Make filth x100", delegate
			{
				for (int i = 0; i < 100; i++)
				{
					IntVec3 c = Gen.MouseCell() + GenRadial.RadialPattern[i];
					if (c.InBounds() && c.Walkable())
					{
						FilthMaker.MakeFilth(c, ThingDefOf.FilthDirt, 2);
						MoteThrower.ThrowMetaPuff(c.ToVector3Shifted());
					}
				}
			});
			base.DrawDebugAction("Tool: Try place direct thing...", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugThingPlaceHelper.TryPlaceOptionsForStackCount(1, true)));
			});
			this.DoGap();
			this.DoLabel("Autotests");
			base.DrawDebugAction("Make colony (full)", delegate
			{
				Autotests_ColonyMaker.MakeColony_Full();
			});
			base.DrawDebugAction("Make colony (bills)", delegate
			{
				Autotests_ColonyMaker.MakeColony_Bills();
			});
			base.DrawDebugAction("Make colony (filth)", delegate
			{
				Autotests_ColonyMaker.MakeColony_Filth();
			});
			base.DrawDebugAction("Make colony (hauling)", delegate
			{
				Autotests_ColonyMaker.MakeColony_Hauling();
			});
			base.DrawDebugAction("Make colony (fire)", delegate
			{
				Autotests_ColonyMaker.MakeColony_Fire();
			});
			base.DrawDebugAction("Test force downed x100", delegate
			{
				for (int i = 0; i < 100; i++)
				{
					PawnKindDef random = DefDatabase<PawnKindDef>.GetRandom();
					Pawn pawn = PawnGenerator.GeneratePawn(random, FactionUtility.DefaultFactionFrom(random.defaultFactionType));
					GenSpawn.Spawn(pawn, CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(), 1000));
					HealthUtility.GiveInjuriesToForceDowned(pawn);
					if (pawn.Dead)
					{
						Log.Error(string.Concat(new object[]
						{
							"Pawn died while force downing: ",
							pawn,
							" at ",
							pawn.Position
						}));
						return;
					}
				}
			});
			base.DrawDebugAction("Test force kill x100", delegate
			{
				for (int i = 0; i < 100; i++)
				{
					PawnKindDef random = DefDatabase<PawnKindDef>.GetRandom();
					Pawn pawn = PawnGenerator.GeneratePawn(random, FactionUtility.DefaultFactionFrom(random.defaultFactionType));
					GenSpawn.Spawn(pawn, CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(), 1000));
					HealthUtility.GiveInjuriesToKill(pawn);
					if (!pawn.Dead)
					{
						Log.Error(string.Concat(new object[]
						{
							"Pawn died not die: ",
							pawn,
							" at ",
							pawn.Position
						}));
						return;
					}
				}
			});
			base.DrawDebugAction("Test generate pawn x1000", delegate
			{
				for (int i = 0; i < 1000; i++)
				{
					PawnKindDef random = DefDatabase<PawnKindDef>.GetRandom();
					Pawn pawn = PawnGenerator.GeneratePawn(random, FactionUtility.DefaultFactionFrom(random.defaultFactionType));
					if (pawn.Dead)
					{
						Log.Error("Pawn is dead");
					}
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
				}
			});
		}
		private void DoLabel(string label)
		{
			this.listing.DoLabel(label);
			this.totalOptionsHeight += Text.CalcHeight(label, 300f) * 1.2f;
		}
		private void DoGap()
		{
			this.listing.DoGap(7f);
			this.totalOptionsHeight += 7f;
		}
		private void DoRaid(IncidentParms parms)
		{
			IncidentDef incidentDef;
			if (parms.faction.HostileTo(Faction.OfColony))
			{
				incidentDef = IncidentDef.Named("RaidEnemy");
			}
			else
			{
				incidentDef = IncidentDef.Named("RaidFriendly");
			}
			incidentDef.Worker.TryExecute(parms);
		}
		private void DebugGiveResource(ThingDef resDef, int count)
		{
			Pawn pawn = Find.MapPawns.FreeColonistsSpawned.RandomElement<Pawn>();
			int i = count;
			int num = 0;
			while (i > 0)
			{
				int num2 = Math.Min(resDef.stackLimit, i);
				i -= num2;
				Thing thing = ThingMaker.MakeThing(resDef, null);
				thing.stackCount = num2;
				if (!GenPlace.TryPlaceThing(thing, pawn.Position, ThingPlaceMode.Near))
				{
					break;
				}
				num += num2;
			}
			Messages.Message(string.Concat(new object[]
			{
				"Made ",
				num,
				" ",
				resDef,
				" near ",
				pawn,
				"."
			}), MessageSound.Benefit);
		}
		private void OffsetNeed(NeedDef nd, float offsetPct)
		{
			foreach (Pawn current in (
				from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
				where t is Pawn
				select t).Cast<Pawn>())
			{
				Need need = current.needs.TryGetNeed(nd);
				if (need != null)
				{
					need.CurLevel += offsetPct * need.MaxLevel;
					this.DustPuffFrom(current);
				}
			}
		}
		private void DustPuffFrom(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				pawn.Drawer.Notify_DebugAffected();
			}
		}
		private void AddGuest(bool prisoner)
		{
			foreach (Building_Bed current in Find.ListerBuildings.AllBuildingsColonistOfClass<Building_Bed>())
			{
				if (current.ForPrisoners == prisoner && !current.owners.Any<Pawn>())
				{
					PawnKindDef pawnKindDef;
					if (!prisoner)
					{
						pawnKindDef = PawnKindDefOf.SpaceRefugee;
					}
					else
					{
						pawnKindDef = (
							from pk in DefDatabase<PawnKindDef>.AllDefs
							where pk.defaultFactionType != null && pk.defaultFactionType != FactionDefOf.Colony && pk.RaceProps.Humanlike
							select pk).RandomElement<PawnKindDef>();
					}
					Faction faction = FactionUtility.DefaultFactionFrom(pawnKindDef.defaultFactionType);
					Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, faction);
					GenSpawn.Spawn(pawn, current.Position);
					foreach (ThingWithComps current2 in pawn.equipment.AllEquipment.ToList<ThingWithComps>())
					{
						ThingWithComps thingWithComps;
						if (pawn.equipment.TryDropEquipment(current2, out thingWithComps, pawn.Position, true))
						{
							thingWithComps.Destroy(DestroyMode.Vanish);
						}
					}
					pawn.inventory.container.Clear();
					pawn.ownership.ClaimBedIfNonMedical(current);
					pawn.guest.SetGuestStatus(Faction.OfColony, prisoner);
					break;
				}
			}
		}
	}
}
