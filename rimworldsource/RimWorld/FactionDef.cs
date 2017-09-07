using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class FactionDef : Def
	{
		public RulePackDef factionNameMaker;
		public string fixedName;
		public bool humanlikeFaction = true;
		public bool hidden;
		public List<PawnGroupMaker> pawnGroupMakers;
		public string pawnsPlural = "characters";
		public float raidCommonality;
		public bool canFlee = true;
		public bool canSiege;
		public bool canStageAttacks;
		public float earliestRaidDays;
		public string leaderTitle = "Leader";
		public int requiredCountAtGameStart;
		public bool canMakeRandomly;
		public RulePackDef pawnNameMaker;
		public TechLevel techLevel;
		public string backstoryCategory;
		public List<string> hairTags = new List<string>();
		public ThingFilter apparelStuffFilter;
		public List<TraderKindDef> caravanTraderKinds = new List<TraderKindDef>();
		public List<TraderKindDef> visitorTraderKinds = new List<TraderKindDef>();
		public FloatRange startingGoodwill = FloatRange.Zero;
		public bool mustStartOneEnemy;
		public FloatRange naturalColonyGoodwill = FloatRange.Zero;
		public float goodwillDailyGain = 2f;
		public float goodwillDailyFall = 2f;
		public bool appreciative = true;
		public string homeIconPath;
		public Color homeIconColor = Color.white;
		[Unsaved]
		public Material baseRenderMaterial = BaseContent.BadMat;
		public override void PostLoad()
		{
			base.PostLoad();
			if (!this.homeIconPath.NullOrEmpty())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					this.baseRenderMaterial = MaterialPool.MatFrom(this.homeIconPath, ShaderDatabase.Transparent, this.homeIconColor);
				});
			}
		}
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (this.apparelStuffFilter != null)
			{
				this.apparelStuffFilter.ResolveReferences();
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			FactionDef.<ConfigErrors>c__Iterator71 <ConfigErrors>c__Iterator = new FactionDef.<ConfigErrors>c__Iterator71();
			<ConfigErrors>c__Iterator.<>f__this = this;
			FactionDef.<ConfigErrors>c__Iterator71 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public float MinPointsToGenerateNormalPawnGroup()
		{
			if (this.pawnGroupMakers == null)
			{
				return 2.14748365E+09f;
			}
			IEnumerable<PawnGroupMaker> source = 
				from x in this.pawnGroupMakers
				where x is PawnGroupMaker_Normal
				select x;
			if (!source.Any<PawnGroupMaker>())
			{
				return 2.14748365E+09f;
			}
			return source.Min((PawnGroupMaker pgm) => pgm.MinPointsToGenerateAnything);
		}
		public bool CanUseStuffForApparel(ThingDef stuffDef)
		{
			return this.apparelStuffFilter == null || this.apparelStuffFilter.Allows(stuffDef);
		}
		public static FactionDef Named(string defName)
		{
			return DefDatabase<FactionDef>.GetNamed(defName, true);
		}
	}
}
