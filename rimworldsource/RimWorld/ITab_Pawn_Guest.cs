using System;
namespace RimWorld
{
	public class ITab_Pawn_Guest : ITab_Pawn_Visitor
	{
		public override bool IsVisible
		{
			get
			{
				return base.SelPawn.HostFaction == Faction.OfColony && !base.SelPawn.IsPrisoner;
			}
		}
		public ITab_Pawn_Guest()
		{
			this.labelKey = "TabGuest";
		}
	}
}
