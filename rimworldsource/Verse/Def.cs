using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace Verse
{
	public class Def : Editable
	{
		public const string DefaultDefName = "UnnamedDef";
		[Description("The name of this Def. It is used as an identifier by the game code."), NoTranslate]
		public string defName = "UnnamedDef";
		[DefaultValue(null), Description("A human-readable label used to identify this in game.")]
		public string label;
		[DefaultValue(null), Description("A human-readable description given when the Def is inspected by players.")]
		public string description;
		[Unsaved]
		public ushort shortHash;
		[Unsaved]
		public ushort index = 65535;
		[Unsaved]
		private string cachedLabelCap;
		[Unsaved]
		public ushort debugRandomId = (ushort)Rand.RangeInclusive(0, 65535);
		private static Regex AllowedDefnamesRegex = new Regex("^[a-zA-Z0-9\\-_]*$");
		public string LabelCap
		{
			get
			{
				if (this.label.NullOrEmpty())
				{
					return null;
				}
				if (this.cachedLabelCap.NullOrEmpty())
				{
					this.cachedLabelCap = this.label.CapitalizeFirst();
				}
				return this.cachedLabelCap;
			}
		}
		public override void PostLoad()
		{
			base.PostLoad();
			ShortHashGiver.GiveShortHash(this);
		}
		[DebuggerHidden]
		public virtual IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			Def.<SpecialDisplayStats>c__Iterator6D <SpecialDisplayStats>c__Iterator6D = new Def.<SpecialDisplayStats>c__Iterator6D();
			Def.<SpecialDisplayStats>c__Iterator6D expr_07 = <SpecialDisplayStats>c__Iterator6D;
			expr_07.$PC = -2;
			return expr_07;
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			Def.<ConfigErrors>c__Iterator6E <ConfigErrors>c__Iterator6E = new Def.<ConfigErrors>c__Iterator6E();
			<ConfigErrors>c__Iterator6E.<>f__this = this;
			Def.<ConfigErrors>c__Iterator6E expr_0E = <ConfigErrors>c__Iterator6E;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public virtual void ClearCachedData()
		{
			this.cachedLabelCap = null;
		}
		public override string ToString()
		{
			return this.defName;
		}
		public override int GetHashCode()
		{
			return this.defName.GetHashCode();
		}
	}
}
