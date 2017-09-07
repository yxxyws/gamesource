using System;
using System.Collections.Generic;
namespace Verse.AI.Group
{
	public class Transition
	{
		public List<LordToil> sources;
		public LordToil target;
		public List<Trigger> triggers = new List<Trigger>();
		public List<TransitionAction> preActions = new List<TransitionAction>();
		public Transition(LordToil firstSource, LordToil target)
		{
			this.sources = new List<LordToil>();
			this.sources.Add(firstSource);
			this.target = target;
		}
		public void AddSource(LordToil source)
		{
			this.sources.Add(source);
		}
		public void AddTrigger(Trigger trigger)
		{
			this.triggers.Add(trigger);
		}
		public void AddAction(TransitionAction action)
		{
			this.preActions.Add(action);
		}
		public void SourceToilBecameActive(bool previousToilWasSourceToilToo)
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				this.triggers[i].SourceToilBecameActive(previousToilWasSourceToilToo);
			}
		}
		public bool CheckSignal(Lord lord, TriggerSignal signal)
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].ActivateOn(lord, signal))
				{
					if (DebugViewSettings.logLordToilTransitions)
					{
						Log.Message(string.Concat(new object[]
						{
							"Transitioning ",
							this.sources,
							" to ",
							this.target,
							" by trigger ",
							this.triggers[i],
							" on signal ",
							signal
						}));
					}
					this.Execute(lord);
					return true;
				}
			}
			return false;
		}
		public void Execute(Lord lord)
		{
			for (int i = 0; i < this.preActions.Count; i++)
			{
				this.preActions[i].DoAction(this);
			}
			lord.GotoToil(this.target);
		}
	}
}
