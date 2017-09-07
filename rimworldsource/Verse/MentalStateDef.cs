using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse.AI;
namespace Verse
{
	public class MentalStateDef : Def
	{
		public Type stateClass = typeof(MentalState);
		public Type workerClass = typeof(MentalStateWorker);
		public float commonality;
		public MentalStateType stateType;
		public bool prisonersCanDo = true;
		public float recoveryChancePerInterval = 1f;
		public int minTicksBeforeRecovery = 500;
		public bool recoverFromSleep;
		public bool recoverFromDowned;
		public ThoughtDef recoveryThought;
		public TaleDef tale;
		public bool blockNormalThoughts;
		public bool isAggro;
		public EffecterDef stateEffecter;
		public Color nameColor = Color.green;
		public string beginLetter;
		public string beginLetterLabel;
		public LetterType beginLetterType = LetterType.BadUrgent;
		public string recoveryMessage;
		public string inspectLine;
		private MentalStateWorker workerInt;
		public MentalStateWorker Worker
		{
			get
			{
				if (this.workerInt == null && this.workerClass != null)
				{
					this.workerInt = (MentalStateWorker)Activator.CreateInstance(this.workerClass);
				}
				return this.workerInt;
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			MentalStateDef.<ConfigErrors>c__Iterator13F <ConfigErrors>c__Iterator13F = new MentalStateDef.<ConfigErrors>c__Iterator13F();
			<ConfigErrors>c__Iterator13F.<>f__this = this;
			MentalStateDef.<ConfigErrors>c__Iterator13F expr_0E = <ConfigErrors>c__Iterator13F;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
