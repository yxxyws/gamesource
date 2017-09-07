using System;
using System.Collections.Generic;
namespace Verse
{
	public class ArtifactProperties
	{
		public ThingDef artifact;
		public Type targeterClass;
		public List<Type> effectDoerClasses;
		public bool doCameraShake;
		public bool psychicSensitiveTargetsOnly;
		[Unsaved]
		private ArtifactTargeter targeterInt;
		[Unsaved]
		private List<ArtifactEffectDoer> effectDoersInt;
		public ArtifactTargeter Targeter
		{
			get
			{
				if (this.targeterInt == null)
				{
					this.targeterInt = (ArtifactTargeter)Activator.CreateInstance(this.targeterClass);
					this.targeterInt.artifact = this.artifact;
				}
				return this.targeterInt;
			}
		}
		public IEnumerable<ArtifactEffectDoer> EffectDoers
		{
			get
			{
				ArtifactProperties.<>c__Iterator135 <>c__Iterator = new ArtifactProperties.<>c__Iterator135();
				<>c__Iterator.<>f__this = this;
				ArtifactProperties.<>c__Iterator135 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		private void CreateEffectDoers()
		{
			this.effectDoersInt = new List<ArtifactEffectDoer>();
			foreach (Type current in this.effectDoerClasses)
			{
				ArtifactEffectDoer artifactEffectDoer = (ArtifactEffectDoer)Activator.CreateInstance(current);
				artifactEffectDoer.artifact = this.artifact;
				this.effectDoersInt.Add(artifactEffectDoer);
			}
		}
		public void ResolveReferencesSpecial(ThingDef artifact)
		{
			this.artifact = artifact;
		}
	}
}
