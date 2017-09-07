using RimWorld;
using System;
namespace Verse
{
	public class PawnGenerationRequest
	{
		public PawnKindDef kindDef;
		public Faction faction;
		public bool forceGenerateNewPawn;
		public bool newborn;
		public bool allowDead;
		public bool allowDowned;
		public bool canGenerateFamilyRelations = true;
		public bool mustBeCapableOfViolence;
		public float colonistRelationChanceFactor = 1f;
		public bool forceAddFreeWarmLayerIfNeeded;
		public bool allowGay = true;
		public float? fixedAge;
		public Gender? fixedGender;
		public float? fixedSkinWhiteness;
		public string fixedLastName;
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"kindDef=",
				this.kindDef,
				", faction=",
				this.faction,
				", forceGenerateNewPawn=",
				this.forceGenerateNewPawn,
				", newborn=",
				this.newborn,
				", allowDead=",
				this.allowDead,
				", allowDowned=",
				this.allowDowned,
				", canGenerateFamilyRelations=",
				this.canGenerateFamilyRelations,
				", mustBeCapableOfViolence=",
				this.mustBeCapableOfViolence,
				", colonistRelationChanceFactor=",
				this.colonistRelationChanceFactor,
				", forceAddFreeWarmLayerIfNeeded=",
				this.forceAddFreeWarmLayerIfNeeded,
				", allowGay=",
				this.allowGay,
				", fixedAge=",
				this.fixedAge,
				", fixedGender=",
				this.fixedGender,
				", fixedSkinWhiteness=",
				this.fixedSkinWhiteness,
				", fixedLastName=",
				this.fixedLastName
			});
		}
	}
}
