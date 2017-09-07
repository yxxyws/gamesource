using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class IncidentWorker_ResourcePodCrash : IncidentWorker
	{
		private const int MaxStacks = 8;
		private const float MaxMarketValue = 30f;
		public override bool TryExecute(IncidentParms parms)
		{
			ThingDef thingDef = (
				from d in DefDatabase<ThingDef>.AllDefs
				where d.category == ThingCategory.Item && d.tradeability == Tradeability.Stockable && d.equipmentType == EquipmentType.None && d.BaseMarketValue >= 1f && d.BaseMarketValue < 30f && !d.HasComp(typeof(CompHatcher))
				select d).RandomElementByWeight((ThingDef d) => 34.5f - d.BaseMarketValue);
			List<Thing> list = new List<Thing>();
			float num = (float)Rand.Range(150, 900);
			do
			{
				Thing thing = ThingMaker.MakeThing(thingDef, null);
				int num2 = Rand.Range(20, 40);
				if (num2 > thing.def.stackLimit)
				{
					num2 = thing.def.stackLimit;
				}
				if ((float)num2 * thing.def.BaseMarketValue > num)
				{
					num2 = Mathf.FloorToInt(num / thing.def.BaseMarketValue);
				}
				if (num2 == 0)
				{
					num2 = 1;
				}
				thing.stackCount = num2;
				list.Add(thing);
				num -= (float)num2 * thingDef.BaseMarketValue;
			}
			while (list.Count < 8 && num > thingDef.BaseMarketValue);
			IntVec3 intVec = DropCellFinder.RandomDropSpot();
			DropPodUtility.DropThingsNear(intVec, list, 110, true, true, true);
			Find.LetterStack.ReceiveLetter("LetterLabelCargoPodCrash".Translate(), "CargoPodCrash".Translate(), LetterType.Good, intVec, null);
			return true;
		}
	}
}
