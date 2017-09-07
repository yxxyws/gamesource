using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public sealed class ThoughtHandler : IExposable
	{
		public Pawn pawn;
		private List<Thought> thoughts = new List<Thought>();
		private List<ThoughtDef> distinctThoughtDefs = new List<ThoughtDef>();
		private static List<ISocialThought> tmpSocialThoughts = new List<ISocialThought>();
		private HashSet<ThoughtDef> distinctSocialThoughts = new HashSet<ThoughtDef>();
		public List<Thought> Thoughts
		{
			get
			{
				return this.thoughts;
			}
		}
		public List<ThoughtDef> DistinctThoughtDefs
		{
			get
			{
				return this.distinctThoughtDefs;
			}
		}
		public ThoughtHandler(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void ExposeData()
		{
			List<Thought> list;
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				list = new List<Thought>();
				for (int i = 0; i < this.thoughts.Count; i++)
				{
					if (this.thoughts[i].def.IsMemory)
					{
						list.Add(this.thoughts[i]);
					}
				}
			}
			else
			{
				list = this.thoughts;
			}
			Scribe_Collections.LookList<Thought>(ref list, "thoughts", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				this.thoughts = list.ToList<Thought>();
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				for (int j = this.thoughts.Count - 1; j > 0; j--)
				{
					if (this.thoughts[j].def == null)
					{
						this.thoughts.RemoveAt(j);
					}
					else
					{
						this.thoughts[j].pawn = this.pawn;
					}
				}
				this.distinctThoughtDefs.Clear();
				for (int k = 0; k < this.thoughts.Count; k++)
				{
					if (!this.distinctThoughtDefs.Contains(this.thoughts[k].def))
					{
						this.distinctThoughtDefs.Add(this.thoughts[k].def);
					}
				}
			}
		}
		public IEnumerable<Thought> ThoughtsOfDef(ThoughtDef def)
		{
			return 
				from tho in this.thoughts
				where tho.def == def
				select tho;
		}
		internal void Notify_PostMapInit()
		{
			this.GainSituationalThoughtsAsNeeded();
			for (int i = 0; i < this.pawn.story.traits.allTraits.Count; i++)
			{
				TraitDegreeData currentData = this.pawn.story.traits.allTraits[i].CurrentData;
				if (currentData.permaThought != null)
				{
					this.TryGainThought(currentData.permaThought);
				}
			}
		}
		public void ThoughtInterval()
		{
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				this.thoughts[i].ThoughtInterval();
			}
			this.RemoveExpiredThoughts();
			this.GainSituationalThoughtsAsNeeded();
		}
		public void RecalculateSituationalThoughts()
		{
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				Thought_Situational thought_Situational = this.thoughts[i] as Thought_Situational;
				if (thought_Situational != null)
				{
					thought_Situational.RecalculateState();
				}
			}
			this.RemoveExpiredThoughts();
			this.GainSituationalThoughtsAsNeeded();
		}
		private void RemoveExpiredThoughts()
		{
			for (int i = this.thoughts.Count - 1; i >= 0; i--)
			{
				Thought thought = this.thoughts[i];
				if (thought.ShouldDiscard)
				{
					this.RemoveThought(thought);
					if (thought.def.nextThought != null)
					{
						this.TryGainThought(thought.def.nextThought);
					}
				}
			}
		}
		private void GainSituationalThoughtsAsNeeded()
		{
			List<ThoughtDef> allDefsListForReading = DefDatabase<ThoughtDef>.AllDefsListForReading;
			int count = allDefsListForReading.Count;
			for (int i = 0; i < count; i++)
			{
				if (allDefsListForReading[i].Worker != null && allDefsListForReading[i].Worker.CurrentState(this.pawn).Active)
				{
					this.TryGainThought(allDefsListForReading[i]);
				}
			}
		}
		public float MoodOffsetOfThoughtGroup(ThoughtDef groupDef)
		{
			float num = 0f;
			float num2 = 1f;
			float num3 = 0f;
			int num4 = 0;
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				Thought thought = this.thoughts[i];
				if (thought.def == groupDef)
				{
					num += thought.MoodOffset();
					num3 += num2;
					num2 *= thought.def.stackedEffectMultiplier;
					num4++;
				}
			}
			float num5 = num / (float)num4;
			return num5 * num3;
		}
		public int OpinionOffsetOfThoughtGroup(ThoughtDef groupDef, Pawn otherPawn)
		{
			ProfilerThreadCheck.BeginSample("OpinionOffsetOfThoughtGroup()");
			ThoughtHandler.tmpSocialThoughts.Clear();
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				if (this.thoughts[i].def == groupDef)
				{
					ISocialThought socialThought = this.thoughts[i] as ISocialThought;
					if (socialThought != null && socialThought.OpinionOffset(otherPawn) != 0f)
					{
						ThoughtHandler.tmpSocialThoughts.Add(socialThought);
					}
				}
			}
			if (groupDef.IsMemory && groupDef.stackedEffectMultiplier != 1f)
			{
				ThoughtHandler.tmpSocialThoughts.Sort((ISocialThought a, ISocialThought b) => ((Thought_Memory)a).age.CompareTo(((Thought_Memory)b).age));
			}
			float num = 0f;
			float num2 = 1f;
			for (int j = 0; j < ThoughtHandler.tmpSocialThoughts.Count; j++)
			{
				num += ThoughtHandler.tmpSocialThoughts[j].OpinionOffset(otherPawn) * num2;
				num2 *= ((Thought)ThoughtHandler.tmpSocialThoughts[j]).def.stackedEffectMultiplier;
			}
			ThoughtHandler.tmpSocialThoughts.Clear();
			ProfilerThreadCheck.EndSample();
			if (num == 0f)
			{
				return 0;
			}
			if (num > 0f)
			{
				return Mathf.Max(Mathf.RoundToInt(num), 1);
			}
			return Mathf.Min(Mathf.RoundToInt(num), -1);
		}
		public void TryGainThought(ThoughtDef def)
		{
			this.TryGainThought(ThoughtMaker.MakeThought(def));
		}
		public void TryGainThought(Thought newThought)
		{
			if (this.pawn.MentalState != null && this.pawn.MentalStateDef.blockNormalThoughts && !newThought.def.validWhileInMentalState && !(newThought is ISocialThought))
			{
				return;
			}
			if (newThought.def.nullifyingTraits != null)
			{
				for (int i = 0; i < newThought.def.nullifyingTraits.Count; i++)
				{
					if (this.pawn.story.traits.HasTrait(newThought.def.nullifyingTraits[i]))
					{
						return;
					}
				}
			}
			if (newThought.def.requiredTraits != null)
			{
				for (int j = 0; j < newThought.def.requiredTraits.Count; j++)
				{
					if (!this.pawn.story.traits.HasTrait(newThought.def.requiredTraits[j]))
					{
						return;
					}
				}
			}
			if (newThought.def.nullifiedIfNotColonist && !this.pawn.IsColonist)
			{
				return;
			}
			if (ThoughtUtility.IsSituationalThoughtNullifiedByHediffs(newThought.def, this.pawn))
			{
				return;
			}
			if (ThoughtUtility.IsThoughtNullifiedByOwnTales(newThought.def, this.pawn))
			{
				return;
			}
			newThought.pawn = this.pawn;
			if (!this.distinctThoughtDefs.Contains(newThought.def))
			{
				this.distinctThoughtDefs.Add(newThought.def);
			}
			newThought.PreAdd();
			if (!newThought.TryMergeWithExistingThought())
			{
				this.thoughts.Add(newThought);
			}
			if (newThought.def.stackLimitPerPawn >= 0)
			{
				Thought_SocialMemory thought_SocialMemory = (Thought_SocialMemory)newThought;
				while (this.NumSocialMemoryThoughtsOfDef(newThought.def, thought_SocialMemory.otherPawnID) > newThought.def.stackLimitPerPawn)
				{
					this.RemoveThought(this.OldestSocialMemoryOfDef(newThought.def, thought_SocialMemory.otherPawnID));
				}
			}
			if (newThought.def.stackLimit >= 0)
			{
				while (this.NumThoughtsOfDef(newThought.def) > newThought.def.stackLimit)
				{
					if (newThought is Thought_Memory)
					{
						this.RemoveThought(this.OldestMemoryOfDef(newThought.def));
					}
					else
					{
						this.RemoveThought(this.thoughts.Find((Thought x) => x.def == newThought.def));
					}
				}
			}
		}
		public HashSet<ThoughtDef> DistinctSocialThoughtDefs(Pawn otherPawn)
		{
			this.distinctSocialThoughts.Clear();
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				ISocialThought socialThought = this.thoughts[i] as ISocialThought;
				if (socialThought != null && socialThought.OpinionOffset(otherPawn) != 0f)
				{
					this.distinctSocialThoughts.Add(this.thoughts[i].def);
				}
			}
			return this.distinctSocialThoughts;
		}
		public int NumThoughtsOfDef(ThoughtDef def)
		{
			int num = 0;
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				if (this.thoughts[i].def == def)
				{
					num++;
				}
			}
			return num;
		}
		public int NumSocialMemoryThoughtsOfDef(ThoughtDef def, int otherPawnID)
		{
			int num = 0;
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				if (this.thoughts[i].def == def)
				{
					Thought_SocialMemory thought_SocialMemory = this.thoughts[i] as Thought_SocialMemory;
					if (thought_SocialMemory != null && thought_SocialMemory.otherPawnID == otherPawnID)
					{
						num++;
					}
				}
			}
			return num;
		}
		public Thought_Memory OldestMemoryOfDef(ThoughtDef def)
		{
			Thought_Memory result = null;
			int num = -9999;
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				if (this.thoughts[i].def == def)
				{
					Thought_Memory thought_Memory = this.thoughts[i] as Thought_Memory;
					if (thought_Memory != null && thought_Memory.age > num)
					{
						result = thought_Memory;
						num = thought_Memory.age;
					}
				}
			}
			return result;
		}
		public Thought_SocialMemory OldestSocialMemoryOfDef(ThoughtDef def, int otherPawnID)
		{
			Thought_SocialMemory result = null;
			int num = -9999;
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				if (this.thoughts[i].def == def)
				{
					Thought_SocialMemory thought_SocialMemory = this.thoughts[i] as Thought_SocialMemory;
					if (thought_SocialMemory != null && thought_SocialMemory.otherPawnID == otherPawnID && thought_SocialMemory.age > num)
					{
						result = thought_SocialMemory;
						num = thought_SocialMemory.age;
					}
				}
			}
			return result;
		}
		public void RemoveThought(Thought th)
		{
			if (!this.thoughts.Remove(th))
			{
				Log.Warning("Tried to remove thought of def " + th.def.defName + " but it's not here.");
				return;
			}
			if (!(
				from otherThought in this.thoughts
				where otherThought.def == th.def && !otherThought.ShouldDiscard
				select otherThought).Any<Thought>())
			{
				this.distinctThoughtDefs.Remove(th.def);
			}
		}
		public void RemoveSocialThoughts(ThoughtDef def, Pawn otherPawn)
		{
			while (true)
			{
				Thought thought = this.thoughts.Find(delegate(Thought x)
				{
					if (x.def != def)
					{
						return false;
					}
					Thought_SocialMemory thought_SocialMemory = x as Thought_SocialMemory;
					if (thought_SocialMemory == null)
					{
						Log.Warning("Tried to remove social thoughts of def " + def.defName + " but it's not a social memory thought.");
						return false;
					}
					return thought_SocialMemory.otherPawnID == otherPawn.thingIDNumber;
				});
				if (thought == null)
				{
					break;
				}
				this.RemoveThought(thought);
			}
		}
		public void RemoveThoughtsOfDef(ThoughtDef def)
		{
			while (true)
			{
				Thought thought = this.thoughts.Find((Thought x) => x.def == def);
				if (thought == null)
				{
					break;
				}
				this.RemoveThought(thought);
			}
		}
		public void Notify_HediffChanged()
		{
			for (int i = this.thoughts.Count - 1; i >= 0; i--)
			{
				Thought thought = this.thoughts[i];
				if (ThoughtUtility.IsSituationalThoughtNullifiedByHediffs(thought.def, this.pawn))
				{
					this.RemoveThought(thought);
				}
			}
		}
		public void Notify_FactionChanged()
		{
			if (!this.pawn.IsColonist)
			{
				for (int i = this.thoughts.Count - 1; i >= 0; i--)
				{
					Thought thought = this.thoughts[i];
					if (thought.def.nullifiedIfNotColonist)
					{
						this.RemoveThought(thought);
					}
				}
			}
		}
		public void Notify_TaleAdded()
		{
			for (int i = this.thoughts.Count - 1; i >= 0; i--)
			{
				Thought thought = this.thoughts[i];
				if (ThoughtUtility.IsThoughtNullifiedByOwnTales(thought.def, this.pawn))
				{
					this.RemoveThought(thought);
				}
			}
		}
		internal void Notify_MentalStateChanged()
		{
			for (int i = this.thoughts.Count - 1; i >= 0; i--)
			{
				if (!(this.thoughts[i] is ISocialThought))
				{
					this.RemoveThought(this.thoughts[i]);
				}
			}
		}
		internal float TotalMood()
		{
			float num = 0f;
			List<ThoughtDef> list = this.DistinctThoughtDefs;
			for (int i = 0; i < list.Count; i++)
			{
				num += this.MoodOffsetOfThoughtGroup(list[i]);
			}
			return num;
		}
	}
}
