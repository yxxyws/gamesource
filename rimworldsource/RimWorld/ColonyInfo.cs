using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public sealed class ColonyInfo : IExposable
	{
		private string colonyName = string.Empty;
		public float realPlayTime;
		public float lastInputRealTime;
		public bool ColonyHasName
		{
			get
			{
				return !this.colonyName.NullOrEmpty();
			}
		}
		public string ColonyName
		{
			get
			{
				if (!this.ColonyHasName)
				{
					return "Colony".Translate();
				}
				return this.colonyName;
			}
			set
			{
				this.colonyName = value;
			}
		}
		public void ColonyInfoOnGUI()
		{
			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseMove || Event.current.type == EventType.KeyDown)
			{
				this.lastInputRealTime = Time.realtimeSinceStartup;
			}
		}
		public void ColonyInfoUpdate()
		{
			if (Time.realtimeSinceStartup < this.lastInputRealTime + 90f && Find.MainTabsRoot.OpenTab != MainTabDefOf.Menu && Game.Mode == GameMode.MapPlaying && !Find.WindowStack.IsOpen<Dialog_Options>())
			{
				this.realPlayTime += Find.RealTime.realDeltaTime;
			}
		}
		public void ColonyInfoTick()
		{
			if (!this.ColonyHasName && Find.TickManager.TicksGame % 1000 == 200 && GenDate.DaysPassed > 2 && Find.MapPawns.FreeColonistsSpawnedCount >= 2 && !Find.GameEnder.gameEnding)
			{
				if (!Find.AttackTargetsCache.TargetsHostileToColony.Any((IAttackTarget x) => !x.ThreatDisabled()))
				{
					Find.WindowStack.Add(new Dialog_NameColony());
				}
			}
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<string>(ref this.colonyName, "colonyName", null, false);
			Scribe_Values.LookValue<float>(ref this.realPlayTime, "realPlayTime", 0f, false);
		}
	}
}
