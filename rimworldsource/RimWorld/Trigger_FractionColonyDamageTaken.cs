using System;
using UnityEngine;
using Verse;
using Verse.AI.Group;
namespace RimWorld
{
	public class Trigger_FractionColonyDamageTaken : Trigger
	{
		private float desiredColonyDamageFraction;
		private float minDamage;
		private TriggerData_FractionColonyDamageTaken Data
		{
			get
			{
				return (TriggerData_FractionColonyDamageTaken)this.data;
			}
		}
		public Trigger_FractionColonyDamageTaken(float desiredColonyDamageFraction, float minDamage = 3.40282347E+38f)
		{
			this.data = new TriggerData_FractionColonyDamageTaken();
			this.desiredColonyDamageFraction = desiredColonyDamageFraction;
			this.minDamage = minDamage;
		}
		public override void SourceToilBecameActive(bool previousToilWasSourceToilToo)
		{
			if (!previousToilWasSourceToilToo)
			{
				this.Data.startColonyDamage = Find.StoryWatcher.watcherDamage.DamageTakenEver;
			}
		}
		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			if (signal.type == TriggerSignalType.Tick)
			{
				float num = Mathf.Max((float)lord.initialColonyHealthTotal * this.desiredColonyDamageFraction, this.minDamage);
				return Find.StoryWatcher.watcherDamage.DamageTakenEver > this.Data.startColonyDamage + num;
			}
			return false;
		}
	}
}
