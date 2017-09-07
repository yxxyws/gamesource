using System;
using System.Collections.Generic;
using Verse;
namespace RimWorld
{
	public class MapCondition : IExposable
	{
		public MapConditionDef def;
		public int startTick;
		public int duration = -1;
		public virtual string Label
		{
			get
			{
				return this.def.label;
			}
		}
		public virtual string LabelCap
		{
			get
			{
				return this.Label.CapitalizeFirst();
			}
		}
		public virtual bool Expired
		{
			get
			{
				return Find.TickManager.TicksGame > this.startTick + this.duration;
			}
		}
		public virtual bool RainAllowed
		{
			get
			{
				return true;
			}
		}
		public int TicksPassed
		{
			get
			{
				return Find.TickManager.TicksGame - this.startTick;
			}
		}
		public int TicksLeft
		{
			get
			{
				return this.duration - this.TicksPassed;
			}
		}
		public virtual string TooltipString
		{
			get
			{
				return string.Concat(new string[]
				{
					this.def.LabelCap,
					"\n",
					"Started".Translate(),
					": ",
					GenDate.DateFullStringAt(GenDate.TickGameToAbs(this.startTick)),
					"\n",
					"Lasted".Translate(),
					": ",
					this.TicksPassed.TicksToPeriodString(true),
					"\n\n",
					this.def.description
				});
			}
		}
		public virtual void ExposeData()
		{
			Scribe_Defs.LookDef<MapConditionDef>(ref this.def, "def");
			Scribe_Values.LookValue<int>(ref this.startTick, "startTick", 0, false);
			Scribe_Values.LookValue<int>(ref this.duration, "duration", 0, false);
		}
		public virtual void MapConditionTick()
		{
		}
		public virtual void MapConditionDraw()
		{
		}
		public virtual void End()
		{
			if (this.def.endMessage != null)
			{
				Messages.Message(this.def.endMessage, MessageSound.Standard);
			}
			Find.MapConditionManager.ActiveConditions.Remove(this);
		}
		public virtual float TemperatureOffset()
		{
			return 0f;
		}
		public virtual float SkyTargetLerpFactor()
		{
			return 0f;
		}
		public virtual SkyTarget? SkyTarget()
		{
			return null;
		}
		public virtual float AnimalDensityFactor()
		{
			return 1f;
		}
		public virtual bool AllowEnjoyableOutsideNow()
		{
			return true;
		}
		public virtual List<SkyOverlay> SkyOverlays()
		{
			return null;
		}
		public virtual void DoCellSteadyEffects(IntVec3 c)
		{
		}
	}
}
