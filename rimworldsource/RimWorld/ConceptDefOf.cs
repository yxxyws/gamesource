using System;
using Verse;
namespace RimWorld
{
	public static class ConceptDefOf
	{
		public static ConceptDef CameraDolly;
		public static ConceptDef CameraZoom;
		public static ConceptDef TimeControls;
		public static ConceptDef SpacePause;
		public static ConceptDef Mining;
		public static ConceptDef Forbidding;
		public static ConceptDef EquippingWeapons;
		public static ConceptDef Stockpiles;
		public static ConceptDef StorageTab;
		public static ConceptDef BillsTab;
		public static ConceptDef GrowingFood;
		public static ConceptDef InfoCard;
		public static ConceptDef AnimalTraining;
		public static ConceptDef InspectRoomStats;
		public static ConceptDef ClickingMessages;
		public static ConceptDef HostilityResponse;
		public static ConceptDef Capturing;
		public static ConceptDef Rescuing;
		public static ConceptDef Drafting;
		public static ConceptDef HomeArea;
		public static ConceptDef PrisonerTab;
		public static ConceptDef StoragePriority;
		public static ConceptDef OpeningComms;
		public static ConceptDef StorageTabCategories;
		public static ConceptDef BuildOrbitalTradeBeacon;
		public static ConceptDef RightClickClearLetter;
		public static ConceptDef MedicalOperations;
		public static ConceptDef WorldCameraMovement;
		public static ConceptDef SetGrowingZonePlant;
		public static ConceptDef AnimalsDontAttackDoors;
		public static ConceptDef InteractingWithTraders;
		public static ConceptDef ArrestingCreatesEnemies;
		public static ConceptDef TradeGoodsMustBeNearBeacon;
		public static ConceptDef SwitchFlickingDesignation;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<ConceptDef>(typeof(ConceptDefOf));
		}
	}
}
