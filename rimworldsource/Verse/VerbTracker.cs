using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class VerbTracker : IExposable
	{
		public IVerbOwner directOwner;
		private List<Verb> verbs;
		public List<Verb> AllVerbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.InitVerbs();
				}
				return this.verbs;
			}
		}
		public Verb PrimaryVerb
		{
			get
			{
				if (this.verbs == null)
				{
					this.InitVerbs();
				}
				for (int i = 0; i < this.verbs.Count; i++)
				{
					if (this.verbs[i].verbProps.isPrimary)
					{
						return this.verbs[i];
					}
				}
				return null;
			}
		}
		public VerbTracker(IVerbOwner directOwner)
		{
			this.directOwner = directOwner;
		}
		public void VerbsTick()
		{
			if (this.verbs == null)
			{
				return;
			}
			for (int i = 0; i < this.verbs.Count; i++)
			{
				this.verbs[i].VerbTick();
			}
		}
		[DebuggerHidden]
		public IEnumerable<Command> GetVerbsCommands(KeyCode hotKey = KeyCode.None)
		{
			VerbTracker.<GetVerbsCommands>c__Iterator1B2 <GetVerbsCommands>c__Iterator1B = new VerbTracker.<GetVerbsCommands>c__Iterator1B2();
			<GetVerbsCommands>c__Iterator1B.<>f__this = this;
			VerbTracker.<GetVerbsCommands>c__Iterator1B2 expr_0E = <GetVerbsCommands>c__Iterator1B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<Verb>(ref this.verbs, "verbs", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.UpdateVerbsLinksAndProps();
			}
		}
		private void InitVerbs()
		{
			if (this.verbs == null)
			{
				this.verbs = new List<Verb>();
				for (int i = 0; i < this.directOwner.VerbProperties.Count; i++)
				{
					VerbProperties verbProperties = this.directOwner.VerbProperties[i];
					Verb verb = (Verb)Activator.CreateInstance(verbProperties.verbClass);
					verb.loadID = Find.World.uniqueIDsManager.GetNextVerbID();
					this.verbs.Add(verb);
				}
				this.UpdateVerbsLinksAndProps();
			}
		}
		private void UpdateVerbsLinksAndProps()
		{
			if (this.verbs == null)
			{
				return;
			}
			List<VerbProperties> verbProperties = this.directOwner.VerbProperties;
			for (int i = 0; i < this.verbs.Count; i++)
			{
				Verb verb = this.verbs[i];
				verb.verbProps = verbProperties[i];
				CompEquippable compEquippable = this.directOwner as CompEquippable;
				Pawn pawn = this.directOwner as Pawn;
				HediffComp_VerbGiver hediffComp_VerbGiver = this.directOwner as HediffComp_VerbGiver;
				if (compEquippable != null)
				{
					verb.ownerEquipment = compEquippable.parent;
				}
				else
				{
					if (pawn != null)
					{
						verb.caster = pawn;
					}
					else
					{
						if (hediffComp_VerbGiver != null)
						{
							verb.ownerHediffComp = hediffComp_VerbGiver;
							verb.caster = hediffComp_VerbGiver.Pawn;
						}
					}
				}
			}
		}
	}
}
