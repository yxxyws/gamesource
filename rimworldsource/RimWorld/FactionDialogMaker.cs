using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public static class FactionDialogMaker
	{
		private const float MinRelationsToCommunicate = -70f;
		private const float MinRelationsFriendly = 40f;
		private const int GiftSilverAmount = 300;
		private const float GiftSilverGoodwillChange = 12f;
		private const float MilitaryAidRelsChange = -25f;
		private static DiaNode root;
		private static Pawn negotiator;
		private static Faction faction;
		public static DiaNode FactionDialogFor(Pawn negotiator, Faction faction)
		{
			FactionDialogMaker.negotiator = negotiator;
			FactionDialogMaker.faction = faction;
			string text = (faction.leader != null) ? faction.leader.Name.ToStringFull : faction.name;
			if (faction.ColonyGoodwill < -70f)
			{
				FactionDialogMaker.root = new DiaNode("FactionGreetingHostile".Translate(new object[]
				{
					text
				}));
			}
			else
			{
				if (faction.ColonyGoodwill < 40f)
				{
					string text2 = "FactionGreetingWary".Translate(new object[]
					{
						text,
						negotiator.LabelBaseShort
					});
					text2 = text2.AdjustedFor(negotiator);
					FactionDialogMaker.root = new DiaNode(text2);
					FactionDialogMaker.root.options.Add(FactionDialogMaker.OfferGiftOption());
				}
				else
				{
					FactionDialogMaker.root = new DiaNode("FactionGreetingWarm".Translate(new object[]
					{
						text,
						negotiator.LabelBaseShort
					}));
					FactionDialogMaker.root.options.Add(FactionDialogMaker.OfferGiftOption());
					FactionDialogMaker.root.options.Add(FactionDialogMaker.RequestMilitaryAidOption());
				}
			}
			if (Prefs.DevMode)
			{
				foreach (DiaOption current in FactionDialogMaker.DebugOptions())
				{
					FactionDialogMaker.root.options.Add(current);
				}
			}
			DiaOption diaOption = new DiaOption("(" + "Disconnect".Translate() + ")");
			diaOption.resolveTree = true;
			FactionDialogMaker.root.options.Add(diaOption);
			return FactionDialogMaker.root;
		}
		[DebuggerHidden]
		private static IEnumerable<DiaOption> DebugOptions()
		{
			FactionDialogMaker.<DebugOptions>c__IteratorA7 <DebugOptions>c__IteratorA = new FactionDialogMaker.<DebugOptions>c__IteratorA7();
			FactionDialogMaker.<DebugOptions>c__IteratorA7 expr_07 = <DebugOptions>c__IteratorA;
			expr_07.$PC = -2;
			return expr_07;
		}
		private static DiaOption OfferGiftOption()
		{
			int num = (
				from t in TradeUtility.AllLaunchableThings
				where t.def == ThingDefOf.Silver
				select t).Sum((Thing t) => t.stackCount);
			if (num < 300)
			{
				DiaOption diaOption = new DiaOption("OfferGift".Translate() + " (" + "NeedSilverLaunchable".Translate(new object[]
				{
					300
				}) + ")");
				diaOption.Disable("NotEnoughSilver".Translate());
				return diaOption;
			}
			float goodwillDelta = 12f * FactionDialogMaker.negotiator.GetStatValue(StatDefOf.GiftImpact, true);
			DiaOption diaOption2 = new DiaOption("OfferGift".Translate() + " (" + "SilverForGoodwill".Translate(new object[]
			{
				300,
				goodwillDelta.ToString("#####0")
			}) + ")");
			diaOption2.action = delegate
			{
				TradeUtility.LaunchThingsOfType(ThingDefOf.Silver, 300, null);
				FactionDialogMaker.faction.AffectGoodwillWith(Faction.OfColony, goodwillDelta);
			};
			string text = "SilverGiftSent".Translate(new object[]
			{
				FactionDialogMaker.faction.name,
				Mathf.RoundToInt(goodwillDelta)
			});
			diaOption2.link = new DiaNode(text)
			{
				options = 
				{
					FactionDialogMaker.OKToRoot()
				}
			};
			return diaOption2;
		}
		private static DiaOption RequestMilitaryAidOption()
		{
			DiaOption diaOption = new DiaOption("RequestMilitaryAid".Translate(new object[]
			{
				-25f
			}));
			if (Find.AttackTargetsCache.TargetsHostileToColony.Any((IAttackTarget x) => !x.ThreatDisabled()))
			{
				if (!Find.AttackTargetsCache.TargetsHostileToColony.Any((IAttackTarget p) => ((Thing)p).Faction != null && ((Thing)p).Faction.HostileTo(FactionDialogMaker.faction)))
				{
					IEnumerable<Faction> source = (
						from x in Find.AttackTargetsCache.TargetsHostileToColony
						where !x.ThreatDisabled()
						select x into pa
						select ((Thing)pa).Faction into fa
						where fa != null && !fa.HostileTo(FactionDialogMaker.faction)
						select fa).Distinct<Faction>();
					string arg_13E_0 = "MilitaryAidConfirmMutualEnemy";
					object[] expr_106 = new object[2];
					expr_106[0] = FactionDialogMaker.faction.name;
					expr_106[1] = GenText.ToCommaList(
						from fa in source
						select fa.name);
					DiaNode diaNode = new DiaNode(arg_13E_0.Translate(expr_106));
					DiaOption diaOption2 = new DiaOption("CallConfirm".Translate());
					diaOption2.action = new Action(FactionDialogMaker.CallForAid);
					diaOption2.link = FactionDialogMaker.FightersSent();
					DiaOption diaOption3 = new DiaOption("CallCancel".Translate());
					diaOption3.linkLateBind = FactionDialogMaker.ResetToRoot();
					diaNode.options.Add(diaOption2);
					diaNode.options.Add(diaOption3);
					diaOption.link = diaNode;
					return diaOption;
				}
			}
			diaOption.action = new Action(FactionDialogMaker.CallForAid);
			diaOption.link = FactionDialogMaker.FightersSent();
			return diaOption;
		}
		private static DiaNode FightersSent()
		{
			return new DiaNode("MilitaryAidSent".Translate(new object[]
			{
				FactionDialogMaker.faction.name
			}))
			{
				options = 
				{
					FactionDialogMaker.OKToRoot()
				}
			};
		}
		private static void CallForAid()
		{
			FactionDialogMaker.faction.AffectGoodwillWith(Faction.OfColony, -25f);
			IncidentParms incidentParms = new IncidentParms();
			incidentParms.faction = FactionDialogMaker.faction;
			incidentParms.points = (float)Rand.Range(150, 400);
			DefDatabase<IncidentDef>.GetNamed("RaidFriendly", true).Worker.TryExecute(incidentParms);
		}
		private static DiaOption OKToRoot()
		{
			return new DiaOption("OK".Translate())
			{
				linkLateBind = FactionDialogMaker.ResetToRoot()
			};
		}
		private static Func<DiaNode> ResetToRoot()
		{
			return () => FactionDialogMaker.FactionDialogFor(FactionDialogMaker.negotiator, FactionDialogMaker.faction);
		}
	}
}
