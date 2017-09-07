using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class Tale : ILoadReferenceable, IExposable
	{
		public TaleDef def;
		public int id;
		private int uses;
		public int date = -1;
		public TaleData_Surroundings surroundings;
		public int AgeTicks
		{
			get
			{
				return Find.TickManager.TicksAbs - this.date;
			}
		}
		public int Uses
		{
			get
			{
				return this.uses;
			}
		}
		public bool Unused
		{
			get
			{
				return this.uses == 0;
			}
		}
		public virtual int DominantPawnId
		{
			get
			{
				return -1;
			}
		}
		public float InterestLevel
		{
			get
			{
				float num = this.def.baseInterest;
				num /= (float)(1 + this.uses * 3);
				float from = 0f;
				switch (this.def.type)
				{
				case TaleType.Volatile:
					from = 50f;
					break;
				case TaleType.Expirable:
					from = this.def.expireDays;
					break;
				case TaleType.PermanentHistorical:
					from = 50f;
					break;
				}
				float value = (float)(this.AgeTicks / 60000);
				num *= Mathf.InverseLerp(from, 0f, value);
				if (num < 0.01f)
				{
					num = 0.01f;
				}
				return num;
			}
		}
		public bool Expired
		{
			get
			{
				return this.Unused && this.def.type == TaleType.Expirable && (float)this.AgeTicks > this.def.expireDays * 60000f;
			}
		}
		public virtual string ShortSummary
		{
			get
			{
				return this.def.LabelCap;
			}
		}
		public Tale()
		{
			this.date = Find.TickManager.TicksAbs;
		}
		public Tale(IntVec3 occurrenceLocation)
		{
			this.date = Find.TickManager.TicksAbs;
			this.surroundings = TaleData_Surroundings.GenerateFrom(occurrenceLocation);
		}
		public virtual void GenerateTestData()
		{
			this.date = Rand.Range(-108000000, -7200000);
			this.surroundings = TaleData_Surroundings.GenerateRandom();
		}
		public virtual bool Concerns(Thing th)
		{
			return false;
		}
		public virtual void ExposeData()
		{
			Scribe_Defs.LookDef<TaleDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.id, "id", 0, false);
			Scribe_Values.LookValue<int>(ref this.uses, "uses", 0, false);
			Scribe_Values.LookValue<int>(ref this.date, "date", 0, false);
			Scribe_Deep.LookDeep<TaleData_Surroundings>(ref this.surroundings, "surroundings", new object[0]);
		}
		public void Notify_NewlyUsed()
		{
			this.uses++;
		}
		public void Notify_ReferenceDestroyed()
		{
			if (this.uses == 0)
			{
				Log.Warning("Called reference destroyed method on tale " + this + " but uses count is 0.");
				return;
			}
			this.uses--;
		}
		[DebuggerHidden]
		public IEnumerable<Rule> GetTextGenerationRules()
		{
			Tale.<GetTextGenerationRules>c__IteratorBB <GetTextGenerationRules>c__IteratorBB = new Tale.<GetTextGenerationRules>c__IteratorBB();
			<GetTextGenerationRules>c__IteratorBB.<>f__this = this;
			Tale.<GetTextGenerationRules>c__IteratorBB expr_0E = <GetTextGenerationRules>c__IteratorBB;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		[DebuggerHidden]
		protected virtual IEnumerable<Rule> SpecialTextGenerationRules()
		{
			Tale.<SpecialTextGenerationRules>c__IteratorBC <SpecialTextGenerationRules>c__IteratorBC = new Tale.<SpecialTextGenerationRules>c__IteratorBC();
			Tale.<SpecialTextGenerationRules>c__IteratorBC expr_07 = <SpecialTextGenerationRules>c__IteratorBC;
			expr_07.$PC = -2;
			return expr_07;
		}
		public string GetUniqueLoadID()
		{
			return "Tale_" + this.id;
		}
		public override int GetHashCode()
		{
			return this.id;
		}
		public override string ToString()
		{
			string str = string.Concat(new object[]
			{
				"(#",
				this.id,
				": ",
				this.ShortSummary,
				"(age=",
				((float)this.AgeTicks / 60000f).ToString("F2"),
				" interest=",
				this.InterestLevel
			});
			if (this.Unused && this.def.type == TaleType.Expirable)
			{
				str = str + ", expireDays=" + this.def.expireDays.ToString("F2");
			}
			return str + ")";
		}
	}
}
