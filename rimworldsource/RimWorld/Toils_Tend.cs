using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorld
{
	public class Toils_Tend
	{
		public const float NoMedicinePotency = 0.5f;
		private static List<Hediff_MissingPart> bleedingStumps = new List<Hediff_MissingPart>();
		private static List<Hediff> otherHediffs = new List<Hediff>();
		public static Toil PickupMedicine(TargetIndex ind, Pawn injured)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Thing thing = curJob.GetTarget(ind).Thing;
				int medicineCountToFullyHeal = Medicine.GetMedicineCountToFullyHeal(injured);
				curJob.maxNumToCarry = medicineCountToFullyHeal;
				int count = Mathf.Min(thing.stackCount, medicineCountToFullyHeal);
				actor.carrier.TryStartCarry(thing, count);
				if (thing.Spawned)
				{
					Find.Reservations.Release(thing, actor);
				}
				curJob.SetTarget(ind, actor.carrier.CarriedThing);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
		public static Toil FinalizeTend(Pawn patient)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				actor.skills.Learn(SkillDefOf.Medicine, 500f);
				Medicine medicine = (Medicine)actor.jobs.curJob.targetB.Thing;
				Toils_Tend.DoTend(actor, patient, medicine);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
		private static void DoTend(Pawn doctor, Pawn patient, Medicine medicine)
		{
			if (!patient.health.HasHediffsNeedingTend(false))
			{
				return;
			}
			if (medicine != null && medicine.Destroyed)
			{
				medicine = null;
			}
			float num = (medicine == null) ? 0f : medicine.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null);
			float num2 = (medicine == null) ? 0.5f : num;
			num2 *= doctor.GetStatValue(StatDefOf.BaseHealingQuality, true);
			if (patient.InBed())
			{
				num2 *= patient.CurrentBed().GetStatValue(StatDefOf.MedicalTreatmentQualityFactor, true);
			}
			if (patient.health.hediffSet.GetInjuriesTendable().Any<Hediff_Injury>())
			{
				float num3 = 0f;
				int num4 = 0;
				foreach (Hediff_Injury current in 
					from x in patient.health.hediffSet.GetInjuriesTendable()
					orderby x.Severity descending
					select x)
				{
					float num5 = Mathf.Min(current.Severity, 20f);
					if (num3 + num5 > 20f)
					{
						break;
					}
					num3 += num5;
					current.Tended(num2, num4);
					if (medicine == null)
					{
						break;
					}
					num4++;
				}
			}
			else
			{
				Toils_Tend.bleedingStumps.Clear();
				List<Hediff_MissingPart> missingPartsCommonAncestors = patient.health.hediffSet.GetMissingPartsCommonAncestors();
				for (int i = 0; i < missingPartsCommonAncestors.Count; i++)
				{
					if (missingPartsCommonAncestors[i].IsFresh)
					{
						Toils_Tend.bleedingStumps.Add(missingPartsCommonAncestors[i]);
					}
				}
				if (Toils_Tend.bleedingStumps.Count > 0)
				{
					Toils_Tend.bleedingStumps.RandomElement<Hediff_MissingPart>().IsFresh = false;
					Toils_Tend.bleedingStumps.Clear();
				}
				else
				{
					Toils_Tend.otherHediffs.Clear();
					Toils_Tend.otherHediffs.AddRange(patient.health.hediffSet.GetTendableNonInjuryNonMissingPartHediffs());
					Hediff hediff;
					if (Toils_Tend.otherHediffs.TryRandomElement(out hediff))
					{
						HediffCompProperties hediffCompProperties = hediff.def.CompPropsFor(typeof(HediffComp_Tendable));
						if (hediffCompProperties != null && hediffCompProperties.tendAllAtOnce)
						{
							int num6 = 0;
							for (int j = 0; j < Toils_Tend.otherHediffs.Count; j++)
							{
								if (Toils_Tend.otherHediffs[j].def == hediff.def)
								{
									Toils_Tend.otherHediffs[j].Tended(num2, num6);
									num6++;
								}
							}
						}
						else
						{
							hediff.Tended(num2, 0);
						}
					}
					Toils_Tend.otherHediffs.Clear();
				}
			}
			if (patient.HostFaction == null && patient.Faction != null && patient.Faction != doctor.Faction)
			{
				patient.Faction.AffectGoodwillWith(doctor.Faction, 0.3f);
			}
			patient.records.Increment(RecordDefOf.TimesTendedTo);
			doctor.records.Increment(RecordDefOf.TimesTendedOther);
			if (medicine != null)
			{
				if (num > 0.9f)
				{
					SoundDef.Named("TechMedicineUsed").PlayOneShot(patient.Position);
				}
				if (medicine.stackCount > 1)
				{
					medicine.stackCount--;
				}
				else
				{
					if (!medicine.Destroyed)
					{
						medicine.Destroy(DestroyMode.Vanish);
					}
				}
			}
		}
	}
}
