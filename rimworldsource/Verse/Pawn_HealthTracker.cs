using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.AI;
using Verse.AI.Group;
namespace Verse
{
	public class Pawn_HealthTracker : IExposable
	{
		private Pawn pawn;
		private PawnHealthState healthState = PawnHealthState.Mobile;
		[Unsaved]
		public Effecter deflectionEffecter;
		public bool forceIncap;
		public HediffSet hediffSet;
		public PawnCapacitiesHandler capacities;
		public BillStack surgeryBills;
		public SummaryHealthHandler summaryHealth;
		public ImmunityHandler immunity;
		public PawnHealthState State
		{
			get
			{
				return this.healthState;
			}
		}
		public bool Downed
		{
			get
			{
				return this.healthState == PawnHealthState.Down;
			}
		}
		public bool Dead
		{
			get
			{
				return this.healthState == PawnHealthState.Dead;
			}
		}
		public bool InPainShock
		{
			get
			{
				return this.hediffSet.Pain >= 0.8f;
			}
		}
		public bool ShouldEverReceiveMedicalCare
		{
			get
			{
				return (this.pawn.playerSettings == null || this.pawn.playerSettings.medCare != MedicalCareCategory.NoCare) && (this.pawn.guest == null || this.pawn.guest.interactionMode != PrisonerInteractionMode.Execution) && Find.DesignationManager.DesignationOn(this.pawn, DesignationDefOf.Slaughter) == null;
			}
		}
		public bool ShouldBeTendedNow
		{
			get
			{
				return this.pawn.playerSettings != null && this.ShouldEverReceiveMedicalCare && this.HasHediffsNeedingTend(false);
			}
		}
		public bool NeedsMedicalRest
		{
			get
			{
				return this.Downed || this.HasHediffsNeedingTend(false) || this.ShouldDoSurgeryNow;
			}
		}
		public bool PrefersMedicalRest
		{
			get
			{
				return this.hediffSet.HasTendedAndHealingInjury || this.hediffSet.HasTendedImmunizableNonInjuryNonMissingPartHediff || this.NeedsMedicalRest;
			}
		}
		public bool ShouldDoSurgeryNow
		{
			get
			{
				return this.surgeryBills.AnyShouldDoNow;
			}
		}
		public Pawn_HealthTracker(Pawn pawn)
		{
			this.pawn = pawn;
			this.hediffSet = new HediffSet(pawn);
			this.capacities = new PawnCapacitiesHandler(pawn);
			this.summaryHealth = new SummaryHealthHandler(pawn);
			this.surgeryBills = new BillStack(pawn);
			this.immunity = new ImmunityHandler(pawn);
		}
		public bool HasHediffsNeedingTend(bool forAlert = false)
		{
			if (this.hediffSet.HasTendableInjury || this.hediffSet.HasFreshMissingPartsCommonAncestor() || this.hediffSet.HasTendableNonInjuryNonMissingPartHediff(forAlert))
			{
				if (!this.pawn.RaceProps.Humanlike)
				{
					if (this.pawn.Faction == Faction.OfColony)
					{
						return true;
					}
					Building_Bed building_Bed = this.pawn.CurrentBed();
					if (building_Bed != null && building_Bed.Faction == Faction.OfColony)
					{
						return true;
					}
				}
				else
				{
					if ((this.pawn.Faction == Faction.OfColony && this.pawn.HostFaction == null) || this.pawn.HostFaction == Faction.OfColony)
					{
						return true;
					}
				}
			}
			return false;
		}
		public void Reset()
		{
			this.healthState = PawnHealthState.Mobile;
			this.hediffSet.Clear();
			this.capacities.Clear();
			this.summaryHealth.Clear();
			this.surgeryBills.Clear();
			this.immunity = new ImmunityHandler(this.pawn);
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<PawnHealthState>(ref this.healthState, "healthState", PawnHealthState.Mobile, false);
			Scribe_Values.LookValue<bool>(ref this.forceIncap, "forceIncap", false, false);
			Scribe_Deep.LookDeep<HediffSet>(ref this.hediffSet, "hediffSet", new object[]
			{
				this.pawn
			});
			Scribe_Deep.LookDeep<BillStack>(ref this.surgeryBills, "surgeryBills", new object[]
			{
				this.pawn
			});
			Scribe_Deep.LookDeep<ImmunityHandler>(ref this.immunity, "immunity", new object[]
			{
				this.pawn
			});
		}
		public void AddHediff(HediffDef def, BodyPartRecord part = null, DamageInfo? dinfo = null)
		{
			this.AddHediff(HediffMaker.MakeHediff(def, this.pawn, null), part, dinfo);
		}
		public void AddHediff(Hediff hediff, BodyPartRecord part = null, DamageInfo? dinfo = null)
		{
			if (part != null)
			{
				hediff.Part = part;
			}
			this.hediffSet.AddHediffDirect(hediff, dinfo);
			this.CheckForStateChange(dinfo, hediff);
			if (this.pawn.RaceProps.hediffGiverSets != null)
			{
				for (int i = 0; i < this.pawn.RaceProps.hediffGiverSets.Count; i++)
				{
					HediffGiverSetDef hediffGiverSetDef = this.pawn.RaceProps.hediffGiverSets[i];
					for (int j = 0; j < hediffGiverSetDef.hediffGivers.Count; j++)
					{
						hediffGiverSetDef.hediffGivers[j].CheckGiveHediffAdded(this.pawn, hediff);
					}
				}
			}
		}
		public void RemoveHediff(Hediff hediff)
		{
			this.hediffSet.hediffs.Remove(hediff);
			this.Notify_HediffChanged(null);
		}
		public void HealHediff(Hediff hediff, int amount)
		{
			this.hediffSet.HealHediff(hediff, amount);
			this.Notify_HediffChanged(hediff);
		}
		public void Notify_HediffChanged(Hediff hediff)
		{
			this.hediffSet.DirtyCache();
			this.CheckForStateChange(null, hediff);
		}
		public void RestorePart(BodyPartRecord part, Hediff diffException = null)
		{
			this.RestorePartResursively(part, diffException);
			this.hediffSet.DirtyCache();
			this.CheckForStateChange(null, null);
		}
		private void RestorePartResursively(BodyPartRecord part, Hediff diffException = null)
		{
			List<Hediff> hediffs = this.hediffSet.hediffs;
			for (int i = hediffs.Count - 1; i >= 0; i--)
			{
				Hediff hediff = hediffs[i];
				if (hediff.Part == part && hediff != diffException)
				{
					hediffs.RemoveAt(i);
				}
			}
			for (int j = 0; j < part.parts.Count; j++)
			{
				this.RestorePartResursively(part.parts[j], diffException);
			}
		}
		public void PreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			if (this.pawn.Spawned)
			{
				if (!this.pawn.Position.Fogged())
				{
					this.pawn.mindState.Active = true;
				}
				Lord lord = this.pawn.GetLord();
				if (lord != null)
				{
					lord.Notify_PawnTookDamage(this.pawn, dinfo);
				}
				if (dinfo.Def.externalViolence)
				{
					GenClamor.DoClamor(this.pawn, 18f, ClamorType.Harm);
				}
				this.pawn.mindState.Notify_DamageTaken(dinfo);
				this.pawn.jobs.Notify_DamageTaken(dinfo);
			}
			if (this.pawn.apparel != null)
			{
				List<Apparel> wornApparel = this.pawn.apparel.WornApparel;
				for (int i = 0; i < wornApparel.Count; i++)
				{
					if (wornApparel[i].CheckPreAbsorbDamage(dinfo))
					{
						absorbed = true;
						return;
					}
				}
			}
			if (this.pawn.Spawned)
			{
				this.pawn.stances.Notify_DamageTaken(dinfo);
				this.pawn.stances.stunner.Notify_DamageApplied(dinfo, !this.pawn.RaceProps.IsFlesh);
				if (dinfo.Def.makesBlood && !dinfo.InstantOldInjury)
				{
					this.TryDropBloodFilth();
				}
			}
			if (this.pawn.Faction != null)
			{
				this.pawn.Faction.Notify_MemberTookDamage(this.pawn, dinfo);
				if (Game.Mode == GameMode.MapPlaying && this.pawn.Faction == Faction.OfColony && dinfo.Def.externalViolence)
				{
					Find.StoryWatcher.watcherDanger.Notify_ColonistHarmedExternally();
				}
			}
			if (this.pawn.RaceProps.IsFlesh && dinfo.Def.externalViolence)
			{
				Pawn pawn = dinfo.Instigator as Pawn;
				if (pawn != null)
				{
					if (pawn.HostileTo(this.pawn))
					{
						this.pawn.relations.canGetRescuedThought = true;
					}
					else
					{
						if (this.pawn.RaceProps.Humanlike && pawn.RaceProps.Humanlike)
						{
							Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)ThoughtMaker.MakeThought(ThoughtDefOf.HarmedMe);
							thought_SocialMemory.SetOtherPawn(pawn);
							this.pawn.needs.mood.thoughts.TryGainThought(thought_SocialMemory);
						}
					}
				}
			}
			absorbed = false;
		}
		public void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (this.ShouldBecomeDead())
			{
				if (!this.pawn.Destroyed)
				{
					this.PawnKilled(new DamageInfo?(dinfo), null);
				}
				return;
			}
			this.ApplyAdditionalHediffs(dinfo, totalDamageDealt);
			if (this.Dead)
			{
				return;
			}
			if (this.pawn.Spawned)
			{
				this.pawn.mindState.mentalStateHandler.Notify_DamageTaken(dinfo);
			}
		}
		private void CheckForStateChange(DamageInfo? dinfo, Hediff hediff)
		{
			if (!this.Dead)
			{
				if (this.ShouldBecomeDead())
				{
					if (!this.pawn.Destroyed)
					{
						this.PawnKilled(dinfo, hediff);
					}
					return;
				}
				if (!this.Downed)
				{
					if (!this.capacities.CanBeAwake || !this.capacities.CapableOf(PawnCapacityDefOf.Moving) || this.InPainShock)
					{
						float num = (!this.pawn.RaceProps.Animal) ? 0.67f : 0.47f;
						if (!this.forceIncap && (this.pawn.Faction == null || this.pawn.Faction.def != FactionDefOf.Colony) && !this.pawn.IsPrisonerOfColony && Rand.Value < num)
						{
							this.PawnKilled(dinfo, null);
							return;
						}
						this.forceIncap = false;
						this.NewlyDowned(dinfo);
						return;
					}
					else
					{
						if (!this.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
						{
							if (this.pawn.carrier != null && this.pawn.carrier.CarriedThing != null && this.pawn.jobs != null && this.pawn.CurJob != null)
							{
								this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
							}
							if (this.pawn.equipment != null && this.pawn.equipment.Primary != null)
							{
								if (this.pawn.InContainer)
								{
									ThingWithComps thingWithComps;
									this.pawn.equipment.TryTransferEquipmentToContainer(this.pawn.equipment.Primary, this.pawn.holder, out thingWithComps);
								}
								else
								{
									if (this.pawn.Spawned)
									{
										ThingWithComps thingWithComps;
										this.pawn.equipment.TryDropEquipment(this.pawn.equipment.Primary, out thingWithComps, this.pawn.Position, true);
									}
									else
									{
										this.pawn.equipment.DestroyEquipment(this.pawn.equipment.Primary);
									}
								}
							}
						}
					}
				}
				else
				{
					if (this.capacities.CanBeAwake && this.capacities.CapableOf(PawnCapacityDefOf.Moving) && !this.InPainShock)
					{
						this.NewlyUndowned();
						return;
					}
				}
			}
		}
		private bool NeedActivityToLive(PawnCapacityDef act)
		{
			if (this.pawn.RaceProps.IsFlesh)
			{
				return act.lethalFlesh;
			}
			return act.lethalMechanoids;
		}
		private bool ShouldBecomeDead()
		{
			if (this.Dead)
			{
				return true;
			}
			if (this.pawn.RaceProps.IsFlesh)
			{
				for (int i = 0; i < this.hediffSet.hediffs.Count; i++)
				{
					if (this.hediffSet.hediffs[i].CauseDeathNow())
					{
						return true;
					}
				}
			}
			List<PawnCapacityDef> allDefsListForReading = DefDatabase<PawnCapacityDef>.AllDefsListForReading;
			for (int j = 0; j < allDefsListForReading.Count; j++)
			{
				PawnCapacityDef pawnCapacityDef = allDefsListForReading[j];
				if (this.NeedActivityToLive(pawnCapacityDef) && !this.capacities.CapableOf(pawnCapacityDef))
				{
					return true;
				}
			}
			float num = PawnCapacityUtility.CalculatePartEfficiency(this.hediffSet, this.pawn.RaceProps.body.corePart, false);
			return num <= 0.0001f;
		}
		private void PawnKilled(DamageInfo? dinfo, Hediff hediff)
		{
			this.healthState = PawnHealthState.Dead;
			if (this.pawn.apparel != null)
			{
				this.pawn.apparel.Notify_PawnKilled(dinfo);
			}
			if (this.pawn.holder != null)
			{
				Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
				if (pawn_CarryTracker != null)
				{
					Thing thing;
					this.pawn.holder.TryDrop(this.pawn, pawn_CarryTracker.pawn.Position, ThingPlaceMode.Near, out thing);
				}
			}
			bool spawned = this.pawn.Spawned;
			bool inContainer = this.pawn.InContainer;
			if (spawned && dinfo.HasValue && dinfo.Value.Def.externalViolence)
			{
				LifeStageUtility.PlayNearestLifestageSound(this.pawn, (LifeStageAge ls) => ls.soundDeath);
			}
			this.surgeryBills.Clear();
			if (spawned)
			{
				this.pawn.DropAndForbidEverything(false);
			}
			Building_Grave building_Grave = null;
			if (this.pawn.ownership != null)
			{
				building_Grave = this.pawn.ownership.AssignedGrave;
			}
			bool flag = this.pawn.InBed();
			float num = 0f;
			if (flag)
			{
				num = this.pawn.CurrentBed().Rotation.AsAngle;
			}
			ThingContainer thingContainer = null;
			if (inContainer)
			{
				thingContainer = this.pawn.holder;
			}
			this.pawn.Destroy(DestroyMode.Kill);
			if (this.pawn.RaceProps.Humanlike)
			{
				if (spawned)
				{
					List<Pawn> allPawnsSpawned = Find.MapPawns.AllPawnsSpawned;
					for (int i = 0; i < allPawnsSpawned.Count; i++)
					{
						Pawn pawn = allPawnsSpawned[i];
						if (pawn.needs.mood != null && pawn.Position.InHorDistOf(this.pawn.Position, 12f) && GenSight.LineOfSight(this.pawn.Position, pawn.Position, false) && pawn.Awake())
						{
							if (pawn.MentalStateDef != MentalStateDefOf.SocialFighting || ((MentalState_SocialFighting)pawn.MentalState).otherPawn != this.pawn)
							{
								if (pawn.Faction == this.pawn.Faction)
								{
									pawn.needs.mood.thoughts.TryGainThought(ThoughtDefOf.WitnessedDeathAlly);
								}
								else
								{
									pawn.needs.mood.thoughts.TryGainThought(ThoughtDefOf.WitnessedDeathStranger);
									pawn.needs.mood.thoughts.TryGainThought(ThoughtDefOf.WitnessedDeathStrangerBloodlust);
								}
							}
						}
					}
				}
				if (dinfo.HasValue && dinfo.Value.Def.externalViolence && dinfo.Value.Instigator != null)
				{
					Pawn pawn2 = dinfo.Value.Instigator as Pawn;
					if (pawn2 != null && !pawn2.Dead && pawn2.needs.mood != null && pawn2.story != null)
					{
						pawn2.needs.mood.thoughts.TryGainThought(ThoughtDefOf.KilledHumanlikeBloodlust);
					}
				}
			}
			if (this.pawn.RaceProps.IsFlesh)
			{
				this.pawn.relations.Notify_PawnKilled(dinfo);
			}
			this.pawn.meleeVerbs.Notify_PawnKilled();
			if (dinfo.HasValue && dinfo.Value.Instigator != null)
			{
				Pawn pawn3 = dinfo.Value.Instigator as Pawn;
				if (pawn3 != null)
				{
					RecordsUtility.Notify_PawnKilled(this.pawn, pawn3);
				}
			}
			if (this.pawn.RaceProps.Humanlike && PawnUtility.ShouldSendNotificationAbout(this.pawn))
			{
				string text = null;
				if (dinfo.HasValue)
				{
					text = string.Format(dinfo.Value.Def.deathMessage, this.pawn.NameStringShort);
				}
				else
				{
					if (hediff != null)
					{
						text = "PawnDiedBecauseOf".Translate(new object[]
						{
							this.pawn.NameStringShort,
							hediff.def.label
						});
					}
				}
				if (!text.NullOrEmpty())
				{
					text = text.AdjustedFor(this.pawn);
					Messages.Message(text, this.pawn, MessageSound.Negative);
				}
				if (this.pawn.IsColonist)
				{
					Find.StoryWatcher.statsRecord.colonistsKilled++;
				}
			}
			Corpse corpse = (Corpse)ThingMaker.MakeThing(this.pawn.RaceProps.corpseDef, null);
			corpse.innerPawn = this.pawn;
			if (building_Grave != null)
			{
				corpse.innerPawn.ownership.ClaimGrave(building_Grave);
			}
			if (flag)
			{
				corpse.innerPawn.Drawer.renderer.wiggler.SetToCustomRotation(num + 180f);
			}
			if (spawned)
			{
				GenPlace.TryPlaceThing(corpse, this.pawn.Position, ThingPlaceMode.Direct);
				corpse.Rotation = this.pawn.Rotation;
				if (this.pawn.RaceProps.Animal && dinfo.HasValue && dinfo.Value.Instigator != null && dinfo.Value.Instigator is Pawn && ((Pawn)dinfo.Value.Instigator).CurJob != null && ((Pawn)dinfo.Value.Instigator).CurJob.def == JobDefOf.Hunt)
				{
					((Pawn)dinfo.Value.Instigator).Reserve(corpse, 1);
				}
				else
				{
					corpse.SetForbiddenIfOutsideHomeArea();
				}
			}
			else
			{
				if (inContainer)
				{
					thingContainer.TryAdd(corpse);
				}
				else
				{
					corpse.Destroy(DestroyMode.Vanish);
				}
			}
			if (this.pawn.RaceProps.DeathActionWorker != null && spawned)
			{
				this.pawn.RaceProps.DeathActionWorker.PawnDied(corpse);
			}
			PawnComponentsUtility.RemoveComponentsOnKilled(this.pawn);
			if (Game.Mode == GameMode.MapPlaying && dinfo.HasValue)
			{
				Pawn pawn4 = dinfo.Value.Instigator as Pawn;
				if (pawn4 == null || pawn4.CurJob == null || !(pawn4.jobs.curDriver is JobDriver_Execute))
				{
					bool flag2 = true;
					if (pawn4 != null)
					{
						if (this.pawn.Faction != Faction.OfColony && this.pawn.kindDef.combatPower >= 280f && pawn4.Faction == Faction.OfColony)
						{
							TaleRecorder.RecordTale(TaleDefOf.KilledMajorColonyEnemy, new object[]
							{
								pawn4,
								this.pawn
							});
							flag2 = false;
						}
						else
						{
							if (this.pawn.IsColonist)
							{
								TaleRecorder.RecordTale(TaleDefOf.KilledColonist, new object[]
								{
									pawn4,
									this.pawn
								});
								flag2 = false;
							}
							else
							{
								if (this.pawn.Faction == Faction.OfColony && this.pawn.RaceProps.Animal)
								{
									TaleRecorder.RecordTale(TaleDefOf.KilledColonyAnimal, new object[]
									{
										pawn4,
										this.pawn
									});
									flag2 = false;
								}
							}
						}
					}
					if (this.pawn.Faction == Faction.OfColony)
					{
						if (!this.pawn.RaceProps.Humanlike && dinfo.Value.Instigator != null && dinfo.Value.Instigator.Faction == Faction.OfColony)
						{
							flag2 = false;
						}
						if (flag2)
						{
							TaleRecorder.RecordTale(TaleDefOf.KilledBy, new object[]
							{
								this.pawn,
								dinfo.Value
							});
						}
					}
				}
			}
			if (this.pawn.Faction != null && this.pawn == this.pawn.Faction.leader)
			{
				this.pawn.Faction.Notify_LeaderDied();
			}
			this.hediffSet.DirtyCache();
		}
		private void NewlyDowned(DamageInfo? dinfo)
		{
			if (this.Downed)
			{
				Log.Error(this.pawn + " was newly downed while already downed.");
				return;
			}
			this.healthState = PawnHealthState.Down;
			this.pawn.NewlyDowned();
			if (this.pawn.MentalState != null)
			{
				this.pawn.mindState.mentalStateHandler.CurState.RecoverFromState();
			}
			if (this.pawn.RaceProps.Humanlike && Game.Mode == GameMode.MapPlaying)
			{
				if (this.pawn.HostileTo(Faction.OfColony))
				{
					ConceptDecider.TeachOpportunity(ConceptDefOf.Capturing, this.pawn, OpportunityType.Important);
				}
				if (this.pawn.Faction == Faction.OfColony)
				{
					ConceptDecider.TeachOpportunity(ConceptDefOf.Rescuing, this.pawn, OpportunityType.Critical);
				}
			}
			if (dinfo.HasValue && dinfo.Value.Instigator != null)
			{
				Pawn pawn = dinfo.Value.Instigator as Pawn;
				if (pawn != null)
				{
					RecordsUtility.Notify_PawnDowned(this.pawn, pawn);
				}
			}
		}
		private void NewlyUndowned()
		{
			if (!this.Downed)
			{
				Log.Error(this.pawn + " was made undowned when already undowned.");
				return;
			}
			this.healthState = PawnHealthState.Mobile;
			if (this.pawn.guest != null)
			{
				this.pawn.guest.Notify_PawnUndowned();
			}
			if (PawnUtility.ShouldSendNotificationAbout(this.pawn))
			{
				Messages.Message("MessageNoLongerDowned".Translate(new object[]
				{
					this.pawn.LabelCap
				}), this.pawn, MessageSound.Benefit);
			}
			if (this.pawn.Spawned && !this.pawn.InBed())
			{
				this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
			}
		}
		private void TryDropBloodFilth()
		{
			if ((this.pawn.Spawned || (this.pawn.holder != null && this.pawn.holder.owner is Pawn_CarryTracker)) && this.pawn.PositionHeld.InBounds() && this.pawn.RaceProps.BloodDef != null && !this.pawn.InContainer && Rand.Value < 0.5f)
			{
				FilthMaker.MakeFilth(this.pawn.PositionHeld, this.pawn.RaceProps.BloodDef, this.pawn.LabelIndefinite(), 1);
			}
		}
		public void HealthTick()
		{
			if (this.Dead)
			{
				return;
			}
			for (int i = this.hediffSet.hediffs.Count - 1; i >= 0; i--)
			{
				Hediff hediff = this.hediffSet.hediffs[i];
				hediff.Tick();
				hediff.PostTick();
			}
			int num = this.hediffSet.hediffs.RemoveAll((Hediff x) => x.ShouldRemove);
			if (num != 0)
			{
				this.Notify_HediffChanged(null);
			}
			if (this.Dead)
			{
				return;
			}
			this.immunity.ImmunityHandlerTick();
			bool flag = false;
			if (this.pawn.IsHashIntervalTick(5000) && !this.Downed && this.hediffSet.HasNaturallyHealingInjuries && (this.pawn.needs.food == null || !this.pawn.needs.food.Starving))
			{
				Hediff_Injury injury = (
					from x in this.hediffSet.GetHediffs<Hediff_Injury>()
					where x.IsNaturallyHealing()
					select x).RandomElement<Hediff_Injury>();
				BodyPartDamageInfo value = new BodyPartDamageInfo(injury);
				this.pawn.TakeDamage(new DamageInfo(DamageDefOf.HealInjury, 1, null, new BodyPartDamageInfo?(value), null));
				flag = true;
			}
			if (this.pawn.IsHashIntervalTick(650) && this.hediffSet.HasTendedAndHealingInjury && (this.pawn.needs.food == null || !this.pawn.needs.food.Starving))
			{
				Hediff_Injury hediff_Injury = (
					from x in this.hediffSet.GetHediffs<Hediff_Injury>()
					where x.IsTendedAndHealing()
					select x).RandomElement<Hediff_Injury>();
				int amount = (!hediff_Injury.IsTendedWell()) ? 1 : 2;
				BodyPartDamageInfo value2 = new BodyPartDamageInfo(hediff_Injury);
				this.pawn.TakeDamage(new DamageInfo(DamageDefOf.HealInjury, amount, null, new BodyPartDamageInfo?(value2), null));
				flag = true;
			}
			if (flag && !this.HasHediffsNeedingTend(false) && !this.PrefersMedicalRest && !this.hediffSet.HasTendedAndHealingInjury && PawnUtility.ShouldSendNotificationAbout(this.pawn))
			{
				Messages.Message("MessageFullyHealed".Translate(new object[]
				{
					this.pawn.LabelCap
				}), this.pawn, MessageSound.Benefit);
			}
			bool flag2 = this.hediffSet.BleedingRate >= 0.1f;
			if (this.pawn.RaceProps.IsFlesh && flag2)
			{
				float num2 = this.hediffSet.BleedingRate * this.pawn.BodySize;
				if (this.pawn.GetPosture() == PawnPosture.Standing)
				{
					num2 *= 0.008f;
				}
				else
				{
					num2 *= 0.0008f;
				}
				if (Rand.Value < num2)
				{
					this.TryDropBloodFilth();
				}
			}
			if (this.pawn.Spawned && this.pawn.IsHashIntervalTick(400))
			{
				float num3 = this.pawn.ComfortableTemperatureRange().max + 150f;
				float temperatureForCell = GenTemperature.GetTemperatureForCell(this.pawn.Position);
				if (temperatureForCell > num3)
				{
					float num4 = temperatureForCell - num3;
					int damage = Mathf.Max(Mathf.RoundToInt(num4 * 0.05f), 1);
					this.TakeBurnDamage(damage);
					if (this.Dead)
					{
						return;
					}
				}
			}
			List<HediffGiverSetDef> hediffGiverSets = this.pawn.RaceProps.hediffGiverSets;
			if (hediffGiverSets != null)
			{
				for (int j = 0; j < hediffGiverSets.Count; j++)
				{
					List<HediffGiver> hediffGivers = hediffGiverSets[j].hediffGivers;
					if (this.pawn.IsHashIntervalTick(60))
					{
						for (int k = 0; k < hediffGivers.Count; k++)
						{
							HediffGiver hediffGiver = hediffGivers[k];
							if (hediffGiver.CheckGiveEverySecond(this.pawn) && PawnUtility.ShouldSendNotificationAbout(this.pawn))
							{
								Find.LetterStack.ReceiveLetter("LetterHediffFromRandomHediffGiverLabel".Translate(new object[]
								{
									this.pawn.LabelBaseShort,
									hediffGiver.hediff.label
								}), "LetterHediffFromRandomHediffGiver".Translate(new object[]
								{
									this.pawn.LabelBaseShort,
									hediffGiver.hediff.label
								}), LetterType.BadNonUrgent, this.pawn, null);
							}
							if (this.pawn.Dead)
							{
								return;
							}
						}
					}
				}
			}
		}
		private void ApplyAdditionalHediffs(DamageInfo dinfo, float totalDamageDealt)
		{
			if (dinfo.Def.additionalHediffs == null)
			{
				return;
			}
			foreach (DamageDefAdditionalHediff current in dinfo.Def.additionalHediffs)
			{
				if (current.hediff != null)
				{
					float num = totalDamageDealt * current.severityPerDamageDealt;
					if (num >= 0f)
					{
						Hediff hediff = HediffMaker.MakeHediff(current.hediff, this.pawn, null);
						hediff.Severity = num;
						this.AddHediff(hediff, null, new DamageInfo?(dinfo));
						if (this.Dead)
						{
							break;
						}
					}
				}
			}
		}
		private void TakeBurnDamage(int damage)
		{
			BodyPartDamageInfo value = new BodyPartDamageInfo(null, new BodyPartDepth?(BodyPartDepth.Outside));
			this.pawn.TakeDamage(new DamageInfo(DamageDefOf.Flame, damage, null, new BodyPartDamageInfo?(value), null));
		}
	}
}
