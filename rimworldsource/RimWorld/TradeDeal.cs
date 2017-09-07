using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class TradeDeal
	{
		private List<Tradeable> tradeables = new List<Tradeable>();
		public int TradeableCount
		{
			get
			{
				return this.tradeables.Count;
			}
		}
		public Tradeable SilverTradeable
		{
			get
			{
				for (int i = 0; i < this.tradeables.Count; i++)
				{
					if (this.tradeables[i].ThingDef == ThingDefOf.Silver)
					{
						return this.tradeables[i];
					}
				}
				return null;
			}
		}
		public IEnumerable<Tradeable> AllTradeables
		{
			get
			{
				return this.tradeables;
			}
		}
		public TradeDeal()
		{
			this.Reset();
		}
		public IEnumerator<Tradeable> GetEnumerator()
		{
			return this.tradeables.GetEnumerator();
		}
		public void Reset()
		{
			this.tradeables.Clear();
			this.AddAllTradeables();
		}
		private void AddAllTradeables()
		{
			foreach (Thing current in TradeSession.trader.ColonyThingsWillingToBuy)
			{
				if (this.InSellablePosition(current))
				{
					this.AddToTradeables(current, Transactor.Colony);
				}
			}
			foreach (Thing current2 in TradeSession.trader.Goods)
			{
				this.AddToTradeables(current2, Transactor.Trader);
			}
			if (this.tradeables.Find((Tradeable x) => x.IsCurrency) == null)
			{
				Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver, null);
				thing.stackCount = 0;
				this.AddToTradeables(thing, Transactor.Trader);
			}
		}
		private bool InSellablePosition(Thing t)
		{
			if (t.Position.Fogged())
			{
				return false;
			}
			Room room = t.Position.GetRoom();
			if (room != null)
			{
				List<Thing> allContainedThings = room.AllContainedThings;
				for (int i = 0; i < allContainedThings.Count; i++)
				{
					Hive hive = allContainedThings[i] as Hive;
					if (hive != null && hive.Dangerous && hive.Position.InHorDistOf(t.Position, 5f))
					{
						return false;
					}
				}
			}
			return true;
		}
		private void AddToTradeables(Thing t, Transactor trans)
		{
			Tradeable tradeable = this.TradeableMatching(t);
			if (tradeable == null)
			{
				Pawn pawn = t as Pawn;
				if (pawn != null)
				{
					tradeable = new Tradeable_Pawn();
				}
				else
				{
					tradeable = new Tradeable();
				}
				this.tradeables.Add(tradeable);
			}
			tradeable.AddThing(t, trans);
		}
		private Tradeable TradeableMatching(Thing thing)
		{
			foreach (Tradeable current in this.tradeables)
			{
				if (TradeUtility.TradeAsOne(thing, current.AnyThing))
				{
					return current;
				}
			}
			return null;
		}
		public void UpdateCurrencyCount()
		{
			float num = 0f;
			foreach (Tradeable current in this.tradeables)
			{
				if (!current.IsCurrency)
				{
					num += current.CurTotalSilverCost;
				}
			}
			this.SilverTradeable.countToDrop = -Mathf.RoundToInt(num);
		}
		public bool TryExecute(out bool actuallyTraded)
		{
			if (this.SilverTradeable.CountPostDealFor(Transactor.Colony) < 0)
			{
				Find.WindowStack.WindowOfType<Dialog_Trade>().FlashSilver();
				Messages.Message("MessageColonyCannotAfford".Translate(), MessageSound.RejectInput);
				actuallyTraded = false;
				return false;
			}
			this.UpdateCurrencyCount();
			this.LimitCurrencyCountToTraderFunds();
			actuallyTraded = false;
			foreach (Tradeable current in this.tradeables)
			{
				if (current.ActionToDo != TradeAction.None)
				{
					actuallyTraded = true;
				}
				current.ResolveTrade();
			}
			this.Reset();
			return true;
		}
		public bool DoesTraderHaveEnoughSilver()
		{
			return this.SilverTradeable.CountPostDealFor(Transactor.Trader) >= 0;
		}
		private void LimitCurrencyCountToTraderFunds()
		{
			if (this.SilverTradeable.countToDrop > this.SilverTradeable.CountHeldBy(Transactor.Trader))
			{
				this.SilverTradeable.countToDrop = this.SilverTradeable.CountHeldBy(Transactor.Trader);
			}
		}
	}
}
