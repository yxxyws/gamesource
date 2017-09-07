using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class CompAffectedByFacilities : ThingComp
	{
		public const float MaxDistToLinkToFacility = 8f;
		private List<Thing> linkedFacilities = new List<Thing>();
		public static Material InactiveFacilityLineMat = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, new Color(1f, 0.5f, 0.5f));
		private static Dictionary<ThingDef, int> alreadyReturnedCount = new Dictionary<ThingDef, int>();
		private List<ThingDef> alreadyUsed = new List<ThingDef>();
		public List<Thing> LinkedFacilitiesListForReading
		{
			get
			{
				return this.linkedFacilities;
			}
		}
		private IEnumerable<Thing> ThingsICanLinkTo
		{
			get
			{
				CompAffectedByFacilities.<>c__IteratorEC <>c__IteratorEC = new CompAffectedByFacilities.<>c__IteratorEC();
				<>c__IteratorEC.<>f__this = this;
				CompAffectedByFacilities.<>c__IteratorEC expr_0E = <>c__IteratorEC;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public bool CanLinkTo(Thing facility)
		{
			if (!this.CanPotentiallyLinkTo(facility.def, facility.Position, facility.Rotation))
			{
				return false;
			}
			if (!this.IsValidFacilityForMe(facility))
			{
				return false;
			}
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.linkedFacilities[i] == facility)
				{
					return false;
				}
			}
			return true;
		}
		public static bool CanPotentiallyLinkTo_Static(Thing facility, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			return CompAffectedByFacilities.CanPotentiallyLinkTo_Static(facility.def, facility.Position, facility.Rotation, myDef, myPos, myRot) && CompAffectedByFacilities.IsPotentiallyValidFacilityForMe_Static(facility, myDef, myPos, myRot);
		}
		public bool CanPotentiallyLinkTo(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot)
		{
			if (!CompAffectedByFacilities.CanPotentiallyLinkTo_Static(facilityDef, facilityPos, facilityRot, this.parent.def, this.parent.Position, this.parent.Rotation))
			{
				return false;
			}
			if (!this.IsPotentiallyValidFacilityForMe(facilityDef, facilityPos, facilityRot))
			{
				return false;
			}
			int num = 0;
			bool flag = false;
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.linkedFacilities[i].def == facilityDef)
				{
					num++;
					if (this.IsBetter(facilityDef, facilityPos, facilityRot, this.linkedFacilities[i]))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				return true;
			}
			CompProperties_Facility compProperties = facilityDef.GetCompProperties<CompProperties_Facility>();
			return num + 1 <= compProperties.maxSimultaneous;
		}
		public static bool CanPotentiallyLinkTo_Static(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			CompProperties_Facility compProperties = facilityDef.GetCompProperties<CompProperties_Facility>();
			if (compProperties.mustBePlacedAdjacent)
			{
				bool flag = false;
				foreach (IntVec3 current in GenAdj.CellsOccupiedBy(myPos, myRot, myDef.size))
				{
					foreach (IntVec3 current2 in GenAdj.CellsOccupiedBy(facilityPos, facilityRot, facilityDef.size))
					{
						if (Mathf.Abs(current.x - current2.x) <= 1 && Mathf.Abs(current.z - current2.z) <= 1)
						{
							flag = true;
							goto IL_CB;
						}
					}
				}
				IL_CB:
				if (!flag)
				{
					return false;
				}
			}
			else
			{
				Vector3 a = Gen.TrueCenter(myPos, myRot, myDef.size, myDef.Altitude);
				Vector3 b = Gen.TrueCenter(facilityPos, facilityRot, facilityDef.size, facilityDef.Altitude);
				if (Vector3.Distance(a, b) > 8f)
				{
					return false;
				}
			}
			return true;
		}
		public bool IsValidFacilityForMe(Thing facility)
		{
			return CompAffectedByFacilities.IsPotentiallyValidFacilityForMe_Static(facility, this.parent.def, this.parent.Position, this.parent.Rotation);
		}
		private bool IsPotentiallyValidFacilityForMe(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot)
		{
			if (!CompAffectedByFacilities.IsPotentiallyValidFacilityForMe_Static(facilityDef, facilityPos, facilityRot, this.parent.def, this.parent.Position, this.parent.Rotation))
			{
				return false;
			}
			CompProperties_Facility compProperties = facilityDef.GetCompProperties<CompProperties_Facility>();
			if (compProperties.canLinkToMedBedsOnly)
			{
				Building_Bed building_Bed = this.parent as Building_Bed;
				if (building_Bed == null || !building_Bed.Medical)
				{
					return false;
				}
			}
			return true;
		}
		private static bool IsPotentiallyValidFacilityForMe_Static(Thing facility, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			return CompAffectedByFacilities.IsPotentiallyValidFacilityForMe_Static(facility.def, facility.Position, facility.Rotation, myDef, myPos, myRot);
		}
		private static bool IsPotentiallyValidFacilityForMe_Static(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			CellRect startRect = GenAdj.OccupiedRect(myPos, myRot, myDef.size);
			CellRect endRect = GenAdj.OccupiedRect(facilityPos, facilityRot, facilityDef.size);
			bool result = false;
			for (int i = startRect.minZ; i <= startRect.maxZ; i++)
			{
				for (int j = startRect.minX; j <= startRect.maxX; j++)
				{
					for (int k = endRect.minZ; k <= endRect.maxZ; k++)
					{
						for (int l = endRect.minX; l <= endRect.maxX; l++)
						{
							IntVec3 start = new IntVec3(j, 0, i);
							IntVec3 end = new IntVec3(l, 0, k);
							if (GenSight.LineOfSight(start, end, startRect, endRect))
							{
								result = true;
								return result;
							}
						}
					}
				}
			}
			return result;
		}
		public void Notify_NewLink(Thing facility)
		{
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.linkedFacilities[i] == facility)
				{
					Log.Error("Notify_NewLink was called but the link is already here.");
					return;
				}
			}
			Thing potentiallySupplantedFacility = this.GetPotentiallySupplantedFacility(facility.def, facility.Position, facility.Rotation);
			if (potentiallySupplantedFacility != null)
			{
				potentiallySupplantedFacility.TryGetComp<CompFacility>().Notify_LinkRemoved(this.parent);
				this.linkedFacilities.Remove(potentiallySupplantedFacility);
			}
			this.linkedFacilities.Add(facility);
		}
		public void Notify_LinkRemoved(Thing thing)
		{
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.linkedFacilities[i] == thing)
				{
					this.linkedFacilities.RemoveAt(i);
					return;
				}
			}
			Log.Error("Notify_LinkRemoved was called but there is no such link here.");
		}
		public void Notify_FacilityDespawned()
		{
			this.RelinkAll();
		}
		public void Notify_LOSBlockerSpawnedOrDespawned()
		{
			this.RelinkAll();
		}
		public void Notify_ThingChanged()
		{
			this.RelinkAll();
		}
		public override void PostSpawnSetup()
		{
			this.LinkToNearbyFacilities();
		}
		public override void PostDeSpawn()
		{
			this.UnlinkAll();
		}
		public override void PostDrawExtraSelectionOverlays()
		{
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.IsFacilityActive(this.linkedFacilities[i]))
				{
					GenDraw.DrawLineBetween(this.parent.TrueCenter(), this.linkedFacilities[i].TrueCenter());
				}
				else
				{
					GenDraw.DrawLineBetween(this.parent.TrueCenter(), this.linkedFacilities[i].TrueCenter(), CompAffectedByFacilities.InactiveFacilityLineMat);
				}
			}
		}
		private bool IsBetter(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, Thing thanThisFacility)
		{
			if (facilityDef != thanThisFacility.def)
			{
				Log.Error("Comparing two different facility defs.");
				return false;
			}
			Vector3 b = Gen.TrueCenter(facilityPos, facilityRot, facilityDef.size, facilityDef.Altitude);
			Vector3 a = this.parent.TrueCenter();
			float num = Vector3.Distance(a, b);
			float num2 = Vector3.Distance(a, thanThisFacility.TrueCenter());
			if (num != num2)
			{
				return num < num2;
			}
			if (facilityPos.x != thanThisFacility.Position.x)
			{
				return facilityPos.x < thanThisFacility.Position.x;
			}
			return facilityPos.z < thanThisFacility.Position.z;
		}
		[DebuggerHidden]
		public static IEnumerable<Thing> PotentialThingsToLinkTo(ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			CompAffectedByFacilities.<PotentialThingsToLinkTo>c__IteratorED <PotentialThingsToLinkTo>c__IteratorED = new CompAffectedByFacilities.<PotentialThingsToLinkTo>c__IteratorED();
			<PotentialThingsToLinkTo>c__IteratorED.myDef = myDef;
			<PotentialThingsToLinkTo>c__IteratorED.myPos = myPos;
			<PotentialThingsToLinkTo>c__IteratorED.myRot = myRot;
			<PotentialThingsToLinkTo>c__IteratorED.<$>myDef = myDef;
			<PotentialThingsToLinkTo>c__IteratorED.<$>myPos = myPos;
			<PotentialThingsToLinkTo>c__IteratorED.<$>myRot = myRot;
			CompAffectedByFacilities.<PotentialThingsToLinkTo>c__IteratorED expr_31 = <PotentialThingsToLinkTo>c__IteratorED;
			expr_31.$PC = -2;
			return expr_31;
		}
		public static void DrawLinesToPotentialThingsToLinkTo(ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			Vector3 a = Gen.TrueCenter(myPos, myRot, myDef.size, myDef.Altitude);
			foreach (Thing current in CompAffectedByFacilities.PotentialThingsToLinkTo(myDef, myPos, myRot))
			{
				GenDraw.DrawLineBetween(a, current.TrueCenter());
			}
		}
		public void DrawRedLineToPotentiallySupplantedFacility(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot)
		{
			Thing potentiallySupplantedFacility = this.GetPotentiallySupplantedFacility(facilityDef, facilityPos, facilityRot);
			if (potentiallySupplantedFacility != null)
			{
				GenDraw.DrawLineBetween(this.parent.TrueCenter(), potentiallySupplantedFacility.TrueCenter(), CompAffectedByFacilities.InactiveFacilityLineMat);
			}
		}
		private Thing GetPotentiallySupplantedFacility(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot)
		{
			Thing thing = null;
			int num = 0;
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.linkedFacilities[i].def == facilityDef)
				{
					if (thing == null)
					{
						thing = this.linkedFacilities[i];
					}
					num++;
				}
			}
			if (num == 0)
			{
				return null;
			}
			CompProperties_Facility compProperties = facilityDef.GetCompProperties<CompProperties_Facility>();
			if (num + 1 <= compProperties.maxSimultaneous)
			{
				return null;
			}
			Thing thing2 = thing;
			for (int j = 0; j < this.linkedFacilities.Count; j++)
			{
				if (facilityDef == this.linkedFacilities[j].def)
				{
					if (this.IsBetter(thing2.def, thing2.Position, thing2.Rotation, this.linkedFacilities[j]))
					{
						thing2 = this.linkedFacilities[j];
					}
				}
			}
			return thing2;
		}
		public float GetStatOffset(StatDef stat)
		{
			float num = 0f;
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				if (this.IsFacilityActive(this.linkedFacilities[i]))
				{
					CompProperties_Facility compProperties = this.linkedFacilities[i].def.GetCompProperties<CompProperties_Facility>();
					if (compProperties.statOffsets != null)
					{
						num += compProperties.statOffsets.GetStatOffsetFromList(stat);
					}
				}
			}
			return num;
		}
		public void GetStatsExplanation(StatDef stat, StringBuilder sb)
		{
			this.alreadyUsed.Clear();
			bool flag = false;
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				bool flag2 = false;
				for (int j = 0; j < this.alreadyUsed.Count; j++)
				{
					if (this.alreadyUsed[j] == this.linkedFacilities[i].def)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					if (this.IsFacilityActive(this.linkedFacilities[i]))
					{
						CompProperties_Facility compProperties = this.linkedFacilities[i].def.GetCompProperties<CompProperties_Facility>();
						if (compProperties.statOffsets != null)
						{
							float num = compProperties.statOffsets.GetStatOffsetFromList(stat);
							if (num != 0f)
							{
								if (!flag)
								{
									flag = true;
									sb.AppendLine();
									sb.AppendLine("StatsReport_Facilities".Translate() + ":");
								}
								int num2 = 0;
								for (int k = 0; k < this.linkedFacilities.Count; k++)
								{
									if (this.IsFacilityActive(this.linkedFacilities[k]) && this.linkedFacilities[k].def == this.linkedFacilities[i].def)
									{
										num2++;
									}
								}
								num *= (float)num2;
								sb.Append("    ");
								if (num2 != 1)
								{
									sb.Append(num2.ToString() + "x ");
								}
								sb.AppendLine(this.linkedFacilities[i].LabelCap + ": " + num.ToStringByStyle(stat.toStringStyle, ToStringNumberSense.Offset));
								this.alreadyUsed.Add(this.linkedFacilities[i].def);
							}
						}
					}
				}
			}
		}
		private void RelinkAll()
		{
			this.LinkToNearbyFacilities();
		}
		public bool IsFacilityActive(Thing facility)
		{
			return facility.TryGetComp<CompFacility>().CanBeActive;
		}
		private void LinkToNearbyFacilities()
		{
			this.UnlinkAll();
			foreach (Thing current in this.ThingsICanLinkTo)
			{
				this.linkedFacilities.Add(current);
				current.TryGetComp<CompFacility>().Notify_NewLink(this.parent);
			}
		}
		private void UnlinkAll()
		{
			for (int i = 0; i < this.linkedFacilities.Count; i++)
			{
				this.linkedFacilities[i].TryGetComp<CompFacility>().Notify_LinkRemoved(this.parent);
			}
			this.linkedFacilities.Clear();
		}
	}
}
