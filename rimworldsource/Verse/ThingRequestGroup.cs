using System;
namespace Verse
{
	public enum ThingRequestGroup : byte
	{
		Undefined,
		Nothing,
		Everything,
		HaulableEver,
		HaulableAlways,
		CultivatedPlant,
		Food,
		FoodNotPlantOrTree,
		Corpse,
		Blueprint,
		BuildingArtificial,
		BuildingFrame,
		Pawn,
		PotentialBillGiver,
		Medicine,
		Filth,
		AttackTarget,
		Weapon,
		Refuelable,
		Construction,
		HasGUIOverlay,
		Apparel,
		MinifiedThing,
		Grave,
		Container,
		DropPod
	}
}
