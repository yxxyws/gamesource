using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class FloatMenuMaker
	{
		public static bool making;
		public static void TryMakeFloatMenu(Pawn myPawn)
		{
			if (!myPawn.IsColonistPlayerControlled || !myPawn.drafter.CanTakeOrderedJob())
			{
				return;
			}
			if (myPawn.Downed)
			{
				Messages.Message("IsIncapped".Translate(new object[]
				{
					myPawn.LabelCap
				}), myPawn, MessageSound.RejectInput);
				return;
			}
			FloatMenuMaker.making = true;
			List<FloatMenuOption> list = FloatMenuMaker.ChoicesAtFor(Gen.MouseMapPosVector3(), myPawn);
			FloatMenuMaker.making = false;
			if (list.Count == 0)
			{
				return;
			}
			if (list.Count == 1 && list[0].autoTakeable)
			{
				list[0].Chosen(true);
				return;
			}
			FloatMenuMap floatMenuMap = new FloatMenuMap(list, myPawn.LabelCap, Gen.MouseMapPosVector3());
			floatMenuMap.givesColonistOrders = true;
			Find.WindowStack.Add(floatMenuMap);
		}
		public static List<FloatMenuOption> ChoicesAtFor(Vector3 clickPos, Pawn pawn)
		{
			IntVec3 intVec = IntVec3.FromVector3(clickPos);
			DangerUtility.NotifyDirectOrderingThisFrame(pawn);
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if (!intVec.InBounds())
			{
				return list;
			}
			if (pawn.Drafted)
			{
				foreach (TargetInfo current in GenUI.TargetsAt(clickPos, TargetingParameters.ForAttackHostile(), true))
				{
					FloatMenuMaker.<ChoicesAtFor>c__AnonStorey29D <ChoicesAtFor>c__AnonStorey29D = new FloatMenuMaker.<ChoicesAtFor>c__AnonStorey29D();
					<ChoicesAtFor>c__AnonStorey29D.attackTarg = current;
					if (pawn.equipment.Primary != null && !pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.MeleeRange)
					{
						string str;
						Action rangedAct = FloatMenuUtility.GetRangedAttackAction(pawn, <ChoicesAtFor>c__AnonStorey29D.attackTarg, out str);
						string text = "FireAt".Translate(new object[]
						{
							<ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing.LabelCap
						});
						FloatMenuOption floatMenuOption = new FloatMenuOption();
						floatMenuOption.priority = MenuOptionPriority.High;
						if (rangedAct == null)
						{
							text = text + " (" + str + ")";
						}
						else
						{
							floatMenuOption.autoTakeable = true;
							floatMenuOption.action = delegate
							{
								MoteThrower.ThrowStatic(<ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing.DrawPos, ThingDefOf.Mote_FeedbackAttack, 1f);
								rangedAct();
							};
						}
						floatMenuOption.label = text;
						list.Add(floatMenuOption);
					}
					string str2;
					Action meleeAct = FloatMenuUtility.GetMeleeAttackAction(pawn, <ChoicesAtFor>c__AnonStorey29D.attackTarg, out str2);
					Pawn pawn2 = <ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing as Pawn;
					string text2;
					if (pawn2 != null && pawn2.Downed)
					{
						text2 = "MeleeAttackToDeath".Translate(new object[]
						{
							<ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing.LabelCap
						});
					}
					else
					{
						text2 = "MeleeAttack".Translate(new object[]
						{
							<ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing.LabelCap
						});
					}
					Thing thing = <ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing;
					FloatMenuOption floatMenuOption2 = new FloatMenuOption(string.Empty, null, MenuOptionPriority.High, null, thing);
					if (meleeAct == null)
					{
						text2 = text2 + " (" + str2 + ")";
					}
					else
					{
						floatMenuOption2.action = delegate
						{
							MoteThrower.ThrowStatic(<ChoicesAtFor>c__AnonStorey29D.attackTarg.Thing.DrawPos, ThingDefOf.Mote_FeedbackAttack, 1f);
							meleeAct();
						};
					}
					floatMenuOption2.label = text2;
					list.Add(floatMenuOption2);
				}
				if (pawn.RaceProps.Humanlike && !pawn.Downed)
				{
					foreach (TargetInfo current2 in GenUI.TargetsAt(clickPos, TargetingParameters.ForArrest(pawn), true))
					{
						TargetInfo dest = current2;
						if (!pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
						{
							list.Add(new FloatMenuOption("CannotArrest".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null));
						}
						else
						{
							if (!pawn.CanReserve(dest.Thing, 1))
							{
								list.Add(new FloatMenuOption("CannotArrest".Translate() + ": " + "Reserved".Translate(), null, MenuOptionPriority.Medium, null, null));
							}
							else
							{
								Pawn pTarg = (Pawn)dest.Thing;
								Action action = delegate
								{
									Building_Bed building_Bed = RestUtility.FindBedFor(pTarg, pawn, true, false, false);
									if (building_Bed == null)
									{
										Messages.Message("CannotArrest".Translate() + ": " + "NoPrisonerBed".Translate(), pTarg, MessageSound.RejectInput);
										return;
									}
									Job job2 = new Job(JobDefOf.Arrest, pTarg, building_Bed);
									job2.playerForced = true;
									job2.maxNumToCarry = 1;
									pawn.drafter.TakeOrderedJob(job2);
									TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.ArrestingCreatesEnemies);
								};
								List<FloatMenuOption> arg_3F1_0 = list;
								Thing thing = dest.Thing;
								arg_3F1_0.Add(new FloatMenuOption("TryToArrest".Translate(new object[]
								{
									dest.Thing.LabelCap
								}), action, MenuOptionPriority.Medium, null, thing));
							}
						}
					}
				}
				int num = GenRadial.NumCellsInRadius(2.9f);
				IntVec3 curLoc;
				for (int i = 0; i < num; i++)
				{
					curLoc = GenRadial.RadialPattern[i] + intVec;
					if (curLoc.Standable())
					{
						if (curLoc != pawn.Position)
						{
							if (!pawn.CanReach(curLoc, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
							{
								FloatMenuOption item = new FloatMenuOption("CannotGoNoPath".Translate(), null, MenuOptionPriority.Low, null, null);
								list.Add(item);
							}
							else
							{
								Action action2 = delegate
								{
									IntVec3 intVec2 = Pawn_DraftController.BestGotoDestNear(curLoc, pawn);
									Job job2 = new Job(JobDefOf.Goto, intVec2);
									job2.playerForced = true;
									pawn.drafter.TakeOrderedJob(job2);
									MoteThrower.ThrowStatic(intVec2, ThingDefOf.Mote_FeedbackGoto, 1f);
								};
								list.Add(new FloatMenuOption("GoHere".Translate(), action2, MenuOptionPriority.Low, null, null)
								{
									autoTakeable = true
								});
							}
						}
						break;
					}
				}
			}
			if (pawn.RaceProps.Humanlike)
			{
				int num2 = 0;
				if (pawn.story != null)
				{
					num2 = pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
				}
				foreach (Thing current3 in intVec.GetThingList())
				{
					Thing t = current3;
					if (t.def.ingestible != null && pawn.RaceProps.CanEverEat(t) && t.IngestibleNow)
					{
						FloatMenuOption item2;
						if (t.def.ingestible.isPleasureDrug && num2 < 0)
						{
							item2 = new FloatMenuOption("ConsumeThing".Translate(new object[]
							{
								t.LabelBaseShort
							}) + " (" + "Teetotaler".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
						}
						else
						{
							if (!pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
							{
								item2 = new FloatMenuOption("ConsumeThing".Translate(new object[]
								{
									t.LabelBaseShort
								}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
							}
							else
							{
								if (!pawn.CanReserve(t, 1))
								{
									item2 = new FloatMenuOption("ConsumeThing".Translate(new object[]
									{
										t.LabelBaseShort
									}) + " (" + "ReservedBy".Translate(new object[]
									{
										Find.Reservations.FirstReserverOf(t, pawn.Faction).LabelBaseShort
									}) + ")", null, MenuOptionPriority.Medium, null, null);
								}
								else
								{
									item2 = new FloatMenuOption("ConsumeThing".Translate(new object[]
									{
										t.LabelBaseShort
									}), delegate
									{
										t.SetForbidden(false, true);
										Job job2 = new Job(JobDefOf.Ingest, t);
										job2.maxNumToCarry = t.def.ingestible.maxNumToIngestAtOnce;
										pawn.drafter.TakeOrderedJob(job2);
									}, MenuOptionPriority.Medium, null, null);
								}
							}
						}
						list.Add(item2);
					}
				}
				foreach (TargetInfo current4 in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
				{
					Pawn victim = (Pawn)current4.Thing;
					if (!victim.InBed() && pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1) && !victim.IsPrisonerOfColony)
					{
						if ((victim.Faction == Faction.OfColony && victim.MentalStateDef == null) || (victim.Faction != Faction.OfColony && victim.MentalStateDef == null && !victim.IsPrisonerOfColony && (victim.Faction == null || !victim.Faction.HostileTo(Faction.OfColony))))
						{
							List<FloatMenuOption> arg_8E1_0 = list;
							Pawn victim2 = victim;
							arg_8E1_0.Add(new FloatMenuOption("Rescue".Translate(new object[]
							{
								victim.LabelCap
							}), delegate
							{
								Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, false, false, false);
								if (building_Bed == null)
								{
									string str4;
									if (victim.RaceProps.Animal)
									{
										str4 = "NoAnimalBed".Translate();
									}
									else
									{
										str4 = "NoNonPrisonerBed".Translate();
									}
									Messages.Message("CannotRescue".Translate() + ": " + str4, victim, MessageSound.RejectInput);
									return;
								}
								Job job2 = new Job(JobDefOf.Rescue, victim, building_Bed);
								job2.maxNumToCarry = 1;
								job2.playerForced = true;
								pawn.drafter.TakeOrderedJob(job2);
								ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.Rescuing, KnowledgeAmount.Total);
							}, MenuOptionPriority.Medium, null, victim2));
						}
						if (victim.MentalStateDef != null || (victim.RaceProps.Humanlike && victim.Faction != Faction.OfColony))
						{
							List<FloatMenuOption> arg_962_0 = list;
							Pawn victim2 = victim;
							arg_962_0.Add(new FloatMenuOption("Capture".Translate(new object[]
							{
								victim.LabelCap
							}), delegate
							{
								Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, true, false, false);
								if (building_Bed == null)
								{
									Messages.Message("CannotCapture".Translate() + ": " + "NoPrisonerBed".Translate(), victim, MessageSound.RejectInput);
									return;
								}
								Job job2 = new Job(JobDefOf.Capture, victim, building_Bed);
								job2.maxNumToCarry = 1;
								job2.playerForced = true;
								pawn.drafter.TakeOrderedJob(job2);
								ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.Capturing, KnowledgeAmount.Total);
							}, MenuOptionPriority.Medium, null, victim2));
						}
					}
				}
				foreach (TargetInfo current5 in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
				{
					TargetInfo targetInfo = current5;
					Pawn victim = (Pawn)targetInfo.Thing;
					if (victim.Downed && pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1) && Building_CryptosleepCasket.FindCryptosleepCasketFor(victim, pawn) != null)
					{
						string label3 = "CarryToCryptosleepCasket".Translate(new object[]
						{
							targetInfo.Thing.LabelCap
						});
						JobDef jDef = JobDefOf.CarryToCryptosleepCasket;
						Action action3 = delegate
						{
							Building_CryptosleepCasket building_CryptosleepCasket = Building_CryptosleepCasket.FindCryptosleepCasketFor(victim, pawn);
							if (building_CryptosleepCasket == null)
							{
								Messages.Message("CannotCarryToCryptosleepCasket".Translate() + ": " + "NoCryptosleepCasket".Translate(), victim, MessageSound.RejectInput);
								return;
							}
							Job job2 = new Job(jDef, victim, building_CryptosleepCasket);
							job2.maxNumToCarry = 1;
							job2.playerForced = true;
							pawn.drafter.TakeOrderedJob(job2);
						};
						List<FloatMenuOption> arg_A80_0 = list;
						Pawn victim2 = victim;
						arg_A80_0.Add(new FloatMenuOption(label3, action3, MenuOptionPriority.Medium, null, victim2));
					}
				}
				foreach (TargetInfo current6 in GenUI.TargetsAt(clickPos, TargetingParameters.ForStrip(pawn), true))
				{
					TargetInfo stripTarg = current6;
					FloatMenuOption item3;
					if (!pawn.CanReach(stripTarg, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						item3 = new FloatMenuOption("CannotStrip".Translate(new object[]
						{
							stripTarg.Thing.LabelCap
						}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
					}
					else
					{
						if (!pawn.CanReserveAndReach(stripTarg, PathEndMode.ClosestTouch, Danger.Deadly, 1))
						{
							item3 = new FloatMenuOption("CannotStrip".Translate(new object[]
							{
								stripTarg.Thing.LabelCap
							}) + " (" + "ReservedBy".Translate(new object[]
							{
								Find.Reservations.FirstReserverOf(stripTarg, pawn.Faction).LabelBaseShort
							}) + ")", null, MenuOptionPriority.Medium, null, null);
						}
						else
						{
							item3 = new FloatMenuOption("Strip".Translate(new object[]
							{
								stripTarg.Thing.LabelCap
							}), delegate
							{
								stripTarg.Thing.SetForbidden(false, false);
								Job job2 = new Job(JobDefOf.Strip, stripTarg);
								job2.playerForced = true;
								pawn.drafter.TakeOrderedJob(job2);
							}, MenuOptionPriority.Medium, null, null);
						}
					}
					list.Add(item3);
				}
				if (pawn.equipment != null)
				{
					ThingWithComps equipment = null;
					List<Thing> thingList = intVec.GetThingList();
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j].TryGetComp<CompEquippable>() != null)
						{
							equipment = (ThingWithComps)thingList[j];
							break;
						}
					}
					if (equipment != null)
					{
						string text3 = GenLabel.ThingLabel(equipment.def, equipment.Stuff, 1);
						FloatMenuOption item4;
						if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
						{
							item4 = new FloatMenuOption("CannotEquip".Translate(new object[]
							{
								text3
							}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
						}
						else
						{
							if (!pawn.CanReserve(equipment, 1))
							{
								item4 = new FloatMenuOption("CannotEquip".Translate(new object[]
								{
									text3
								}) + " (" + "ReservedBy".Translate(new object[]
								{
									Find.Reservations.FirstReserverOf(equipment, pawn.Faction).LabelBaseShort
								}) + ")", null, MenuOptionPriority.Medium, null, null);
							}
							else
							{
								if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
								{
									item4 = new FloatMenuOption("CannotEquip".Translate(new object[]
									{
										text3
									}) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
								}
								else
								{
									string text4 = "Equip".Translate(new object[]
									{
										text3
									});
									if (equipment.def.IsRangedWeapon && pawn.story != null && pawn.story.traits.HasTrait(TraitDefOf.Brawler))
									{
										text4 = text4 + " " + "EquipWarningBrawler".Translate();
									}
									item4 = new FloatMenuOption(text4, delegate
									{
										equipment.SetForbidden(false, true);
										Job job2 = new Job(JobDefOf.Equip, equipment);
										job2.playerForced = true;
										pawn.drafter.TakeOrderedJob(job2);
										MoteThrower.ThrowStatic(equipment.DrawPos, ThingDefOf.Mote_FeedbackEquip, 1f);
										ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
									}, MenuOptionPriority.Medium, null, null);
								}
							}
						}
						list.Add(item4);
					}
				}
				if (pawn.apparel != null)
				{
					Apparel apparel = Find.ThingGrid.ThingAt<Apparel>(intVec);
					if (apparel != null)
					{
						FloatMenuOption item5;
						if (!pawn.CanReach(apparel, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
						{
							item5 = new FloatMenuOption("CannotWear".Translate(new object[]
							{
								apparel.Label
							}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
						}
						else
						{
							if (!pawn.CanReserve(apparel, 1))
							{
								Pawn pawn3 = Find.Reservations.FirstReserverOf(apparel, pawn.Faction);
								item5 = new FloatMenuOption("CannotWear".Translate(new object[]
								{
									apparel.Label
								}) + " (" + "ReservedBy".Translate(new object[]
								{
									pawn3.LabelBaseShort
								}) + ")", null, MenuOptionPriority.Medium, null, null);
							}
							else
							{
								if (!ApparelUtility.HasPartsToWear(pawn, apparel.def))
								{
									item5 = new FloatMenuOption("CannotWear".Translate(new object[]
									{
										apparel.Label
									}) + " (" + "CannotWearBecauseOfMissingBodyParts".Translate() + ")", null, MenuOptionPriority.Medium, null, null);
								}
								else
								{
									item5 = new FloatMenuOption("ForceWear".Translate(new object[]
									{
										apparel.LabelBaseShort
									}), delegate
									{
										apparel.SetForbidden(false, true);
										Job job2 = new Job(JobDefOf.Wear, apparel);
										job2.playerForced = true;
										pawn.drafter.TakeOrderedJob(job2);
									}, MenuOptionPriority.Medium, null, null);
								}
							}
						}
						list.Add(item5);
					}
				}
				if (pawn.equipment != null && pawn.equipment.Primary != null)
				{
					Thing thing2 = Find.ThingGrid.ThingAt(intVec, ThingDefOf.EquipmentRack);
					if (thing2 != null)
					{
						if (!pawn.CanReach(thing2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
						{
							list.Add(new FloatMenuOption("CannotDeposit".Translate(new object[]
							{
								pawn.equipment.Primary.LabelCap,
								thing2.def.label
							}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null));
						}
						else
						{
							foreach (IntVec3 c in GenAdj.CellsOccupiedBy(thing2))
							{
								if (c.GetStorable() == null && pawn.CanReserveAndReach(c, PathEndMode.ClosestTouch, Danger.Deadly, 1))
								{
									Action action4 = delegate
									{
										ThingWithComps t;
										if (pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out t, pawn.Position, true))
										{
											t.SetForbidden(false, true);
											Job job2 = new Job(JobDefOf.HaulToCell, t, c);
											job2.haulMode = HaulMode.ToCellStorage;
											job2.maxNumToCarry = 1;
											job2.playerForced = true;
											pawn.drafter.TakeOrderedJob(job2);
										}
									};
									list.Add(new FloatMenuOption("Deposit".Translate(new object[]
									{
										pawn.equipment.Primary.LabelCap,
										thing2.def.label
									}), action4, MenuOptionPriority.Medium, null, null));
									break;
								}
							}
						}
					}
					if (pawn.equipment != null && GenUI.TargetsAt(clickPos, TargetingParameters.ForSelf(pawn), true).Any<TargetInfo>())
					{
						Action action5 = delegate
						{
							ThingWithComps thingWithComps;
							pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out thingWithComps, pawn.Position, true);
							pawn.drafter.TakeOrderedJob(new Job(JobDefOf.Wait, 20, false));
						};
						list.Add(new FloatMenuOption("Drop".Translate(new object[]
						{
							pawn.equipment.Primary.LabelCap
						}), action5, MenuOptionPriority.Medium, null, null));
					}
				}
				foreach (TargetInfo current7 in GenUI.TargetsAt(clickPos, TargetingParameters.ForTrade(), true))
				{
					TargetInfo dest2 = current7;
					if (!pawn.CanReach(dest2, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						list.Add(new FloatMenuOption("CannotTrade".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null));
					}
					else
					{
						if (!pawn.CanReserve(dest2.Thing, 1))
						{
							list.Add(new FloatMenuOption("CannotTrade".Translate() + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Medium, null, null));
						}
						else
						{
							Pawn pTarg = (Pawn)dest2.Thing;
							Action action6 = delegate
							{
								Job job2 = new Job(JobDefOf.TradeWithPawn, pTarg);
								job2.playerForced = true;
								pawn.drafter.TakeOrderedJob(job2);
								ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.InteractingWithTraders, KnowledgeAmount.Total);
							};
							string str3 = string.Empty;
							if (pTarg.Faction != null)
							{
								str3 = " (" + pTarg.Faction.name + ")";
							}
							List<FloatMenuOption> arg_142E_0 = list;
							Thing thing = dest2.Thing;
							arg_142E_0.Add(new FloatMenuOption("TradeWith".Translate(new object[]
							{
								pTarg.LabelBaseShort
							}) + str3, action6, MenuOptionPriority.Medium, null, thing));
						}
					}
				}
				foreach (Thing current8 in Find.ThingGrid.ThingsAt(intVec))
				{
					foreach (FloatMenuOption current9 in current8.GetFloatMenuOptions(pawn))
					{
						list.Add(current9);
					}
				}
			}
			if (!pawn.Drafted)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (Thing current10 in Find.ThingGrid.ThingsAt(intVec))
				{
					flag2 = true;
					if (pawn.CanReach(current10, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						flag = true;
						break;
					}
				}
				if (flag2 && !flag)
				{
					list.Add(new FloatMenuOption("(" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null));
					return list;
				}
				foreach (Thing current11 in Find.ThingGrid.ThingsAt(intVec))
				{
					Pawn pawn4 = Find.Reservations.FirstReserverOf(current11, pawn.Faction);
					if (pawn4 != null && pawn4 != pawn)
					{
						list.Add(new FloatMenuOption("IsReservedBy".Translate(new object[]
						{
							current11.LabelBaseShort.CapitalizeFirst(),
							pawn4.LabelBaseShort
						}), null, MenuOptionPriority.Medium, null, null));
					}
					else
					{
						JobGiver_Work jobGiver_Work = pawn.thinker.TryGetMainTreeThinkNode<JobGiver_Work>();
						if (jobGiver_Work != null)
						{
							foreach (WorkTypeDef current12 in DefDatabase<WorkTypeDef>.AllDefsListForReading)
							{
								for (int k = 0; k < current12.workGiversByPriority.Count; k++)
								{
									WorkGiver_Scanner workGiver_Scanner = current12.workGiversByPriority[k].Worker as WorkGiver_Scanner;
									if (workGiver_Scanner != null)
									{
										if (workGiver_Scanner.def.directOrderable)
										{
											if (!workGiver_Scanner.ShouldSkip(pawn))
											{
												JobFailReason.Clear();
												Job job;
												if (!workGiver_Scanner.HasJobOnThingForced(pawn, current11))
												{
													job = null;
												}
												else
												{
													job = workGiver_Scanner.JobOnThingForced(pawn, current11);
												}
												if (workGiver_Scanner.PotentialWorkThingRequest.Accepts(current11) || (workGiver_Scanner.PotentialWorkThingsGlobal(pawn) != null && workGiver_Scanner.PotentialWorkThingsGlobal(pawn).Contains(current11)))
												{
													if (job == null)
													{
														if (JobFailReason.HaveReason)
														{
															string label2 = "CannotGenericWork".Translate(new object[]
															{
																workGiver_Scanner.def.verb,
																current11.LabelBaseShort
															}) + " (" + JobFailReason.Reason + ")";
															list.Add(new FloatMenuOption(label2, null, MenuOptionPriority.Medium, null, null));
														}
													}
													else
													{
														WorkTypeDef workType = workGiver_Scanner.def.workType;
														Action action7 = null;
														PawnCapacityDef pawnCapacityDef = workGiver_Scanner.MissingRequiredCapacity(pawn);
														string label;
														if (pawnCapacityDef != null)
														{
															label = "CannotMissingHealthActivities".Translate(new object[]
															{
																pawnCapacityDef.label
															});
														}
														else
														{
															if (pawn.jobs.curJob != null && pawn.jobs.curJob.JobIsSameAs(job))
															{
																label = "CannotGenericAlreadyAm".Translate(new object[]
																{
																	workType.gerundLabel,
																	current11.LabelBaseShort
																});
															}
															else
															{
																if (pawn.workSettings.GetPriority(workType) == 0)
																{
																	label = "CannotPrioritizeIsNotA".Translate(new object[]
																	{
																		pawn.NameStringShort,
																		workType.pawnLabel
																	});
																}
																else
																{
																	if (job.def == JobDefOf.Research && current11 is Building_ResearchBench)
																	{
																		label = "CannotPrioritizeResearch".Translate();
																	}
																	else
																	{
																		if (current11.IsForbidden(pawn))
																		{
																			label = "CannotPrioritizeForbidden".Translate(new object[]
																			{
																				current11.Label
																			});
																		}
																		else
																		{
																			if (!pawn.CanReach(current11, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
																			{
																				label = current11.Label + ": " + "NoPath".Translate();
																			}
																			else
																			{
																				label = "PrioritizeGeneric".Translate(new object[]
																				{
																					workGiver_Scanner.def.gerund,
																					current11.Label
																				});
																				Job localJob = job;
																				WorkTypeDef localWorkTypeDef = workType;
																				action7 = delegate
																				{
																					pawn.thinker.GetMainTreeThinkNode<JobGiver_Work>().TryStartPrioritizedWorkOn(pawn, localJob, localWorkTypeDef);
																				};
																			}
																		}
																	}
																}
															}
														}
														if (!list.Any((FloatMenuOption op) => op.label == label))
														{
															list.Add(new FloatMenuOption(label, action7, MenuOptionPriority.Medium, null, null));
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			foreach (FloatMenuOption current13 in pawn.GetExtraFloatMenuOptionsFor(intVec))
			{
				list.Add(current13);
			}
			DangerUtility.DoneDirectOrdering();
			return list;
		}
	}
}
