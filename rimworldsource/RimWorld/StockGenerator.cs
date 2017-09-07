using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class StockGenerator
	{
		[Unsaved]
		public TraderKindDef trader;
		public IntRange countRange = IntRange.zero;
		public FloatRange totalPriceRange = FloatRange.Zero;
		public TechLevel maxTechLevel = TechLevel.Transcendent;
		public PriceType price;
		public abstract IEnumerable<Thing> GenerateThings();
		protected Thing TryMakeForStockSingle(ThingDef thingDef, int stackCount)
		{
			if (stackCount <= 0)
			{
				return null;
			}
			if (thingDef.tradeability != Tradeability.Stockable)
			{
				Log.Error("Tried to make non-Stockable thing for trader stock: " + thingDef);
				return null;
			}
			ThingDef stuff = null;
			if (thingDef.MadeFromStuff)
			{
				stuff = (
					from st in DefDatabase<ThingDef>.AllDefs
					where st.IsStuff && st.stuffProps.CanMake(thingDef)
					select st).RandomElementByWeight((ThingDef st) => st.stuffProps.stuffCommonality);
			}
			Thing thing = ThingMaker.MakeThing(thingDef, stuff);
			thing.stackCount = stackCount;
			return thing;
		}
		[DebuggerHidden]
		protected IEnumerable<Thing> TryMakeForStock(ThingDef thingDef, int count)
		{
			StockGenerator.<TryMakeForStock>c__IteratorF6 <TryMakeForStock>c__IteratorF = new StockGenerator.<TryMakeForStock>c__IteratorF6();
			<TryMakeForStock>c__IteratorF.thingDef = thingDef;
			<TryMakeForStock>c__IteratorF.count = count;
			<TryMakeForStock>c__IteratorF.<$>thingDef = thingDef;
			<TryMakeForStock>c__IteratorF.<$>count = count;
			<TryMakeForStock>c__IteratorF.<>f__this = this;
			StockGenerator.<TryMakeForStock>c__IteratorF6 expr_2A = <TryMakeForStock>c__IteratorF;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		protected int RandomCountOf(ThingDef def)
		{
			if (this.countRange.max > 0 && this.totalPriceRange.max <= 0f)
			{
				return this.countRange.RandomInRange;
			}
			if (this.countRange.max <= 0 && this.totalPriceRange.max > 0f)
			{
				return Mathf.Max(1, Mathf.RoundToInt(this.totalPriceRange.RandomInRange / def.BaseMarketValue));
			}
			int num = 0;
			int randomInRange;
			do
			{
				randomInRange = this.countRange.RandomInRange;
				num++;
				if (num > 100)
				{
					break;
				}
			}
			while (!this.totalPriceRange.Includes((float)randomInRange * def.BaseMarketValue));
			return randomInRange;
		}
		public abstract bool HandlesThingDef(ThingDef thingDef);
		public bool TryGetPriceType(ThingDef thingDef, TradeAction action, out PriceType priceType)
		{
			if (!this.HandlesThingDef(thingDef))
			{
				priceType = PriceType.Normal;
				return false;
			}
			if (action == TradeAction.PlayerBuys)
			{
				priceType = this.price;
			}
			else
			{
				priceType = (PriceType)(this.price - PriceType.VeryCheap);
			}
			return true;
		}
		public virtual void PostLoad()
		{
			if (this.price == PriceType.Undefined)
			{
				this.price = PriceType.Normal;
			}
		}
		public virtual void ResolveReferences(TraderKindDef trader)
		{
			this.trader = trader;
		}
		[DebuggerHidden]
		public virtual IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			StockGenerator.<ConfigErrors>c__IteratorF7 <ConfigErrors>c__IteratorF = new StockGenerator.<ConfigErrors>c__IteratorF7();
			StockGenerator.<ConfigErrors>c__IteratorF7 expr_07 = <ConfigErrors>c__IteratorF;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
