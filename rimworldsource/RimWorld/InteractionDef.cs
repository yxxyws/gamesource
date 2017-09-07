using System;
using UnityEngine;
using Verse;
using Verse.Grammar;
namespace RimWorld
{
	public class InteractionDef : Def
	{
		private Type workerClass = typeof(InteractionWorker);
		public float socialFightBaseChance;
		public ThoughtDef initiatorThought;
		public SkillDef initiatorXpGainSkill;
		public int initiatorXpGainAmount;
		public ThoughtDef recipientThought;
		public SkillDef recipientXpGainSkill;
		public int recipientXpGainAmount;
		private string symbol;
		public RulePack logRulesInitiator;
		public RulePack logRulesRecipient;
		[Unsaved]
		private InteractionWorker workerInt;
		[Unsaved]
		private Texture2D symbolTex;
		public InteractionWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (InteractionWorker)Activator.CreateInstance(this.workerClass);
				}
				return this.workerInt;
			}
		}
		public Texture2D Symbol
		{
			get
			{
				if (this.symbolTex == null)
				{
					this.symbolTex = ContentFinder<Texture2D>.Get(this.symbol, true);
				}
				return this.symbolTex;
			}
		}
	}
}
