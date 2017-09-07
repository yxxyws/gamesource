using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class BodyPartDef : Def
	{
		private List<string> activities;
		public int hitPoints = 100;
		public float oldInjuryBaseChance = 0.2f;
		public float bleedingRateMultiplier = 1f;
		private bool skinCovered;
		public bool useDestroyedOutLabel;
		public ThingDef spawnThingOnRemoved;
		private bool isSolid;
		public bool dontSuggestAmputation;
		public float frostbiteVulnerability;
		public bool beautyRelated;
		public bool isAlive = true;
		[Unsaved]
		private List<Pair<PawnCapacityDef, string>> cachedActivityPairs;
		public List<Pair<PawnCapacityDef, string>> Activities
		{
			get
			{
				if (this.cachedActivityPairs == null)
				{
					this.CacheActivities();
				}
				return this.cachedActivityPairs;
			}
		}
		public bool IsDelicate
		{
			get
			{
				return this.oldInjuryBaseChance >= 0.8f;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			BodyPartDef.<ConfigErrors>c__Iterator132 <ConfigErrors>c__Iterator = new BodyPartDef.<ConfigErrors>c__Iterator132();
			<ConfigErrors>c__Iterator.<>f__this = this;
			BodyPartDef.<ConfigErrors>c__Iterator132 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public bool IsSolid(BodyPartRecord part, List<Hediff> hediffs)
		{
			for (BodyPartRecord bodyPartRecord = part; bodyPartRecord != null; bodyPartRecord = bodyPartRecord.parent)
			{
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i].Part == bodyPartRecord && hediffs[i] is Hediff_AddedPart)
					{
						return hediffs[i].def.addedPartProps.isSolid;
					}
				}
			}
			return this.isSolid;
		}
		public bool IsSkinCovered(BodyPartRecord part, HediffSet body)
		{
			return !body.PartOrAnyAncestorHasDirectlyAddedParts(part) && this.skinCovered;
		}
		public float GetMaxHealth(Pawn pawn)
		{
			return (float)Mathf.CeilToInt((float)this.hitPoints * pawn.HealthScale);
		}
		private void CacheActivities()
		{
			this.cachedActivityPairs = new List<Pair<PawnCapacityDef, string>>();
			try
			{
				if (this.activities != null)
				{
					for (int i = 0; i < this.activities.Count; i++)
					{
						string[] array = this.activities[i].Split(new char[]
						{
							'_'
						});
						PawnCapacityDef first = PawnCapacityDefOf.Consciousness;
						string second = string.Empty;
						if (array.Length >= 1)
						{
							first = DefDatabase<PawnCapacityDef>.GetNamed(array[0], true);
						}
						if (array.Length >= 2)
						{
							second = array[1];
						}
						this.cachedActivityPairs.Add(new Pair<PawnCapacityDef, string>(first, second));
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Could not cache Pawn activities: " + ex.Message);
			}
		}
	}
}
