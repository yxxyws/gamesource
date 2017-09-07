using System;
using Verse;
namespace RimWorld
{
	public class CompBreakdownable : ThingComp
	{
		public const string BreakdownSignal = "Breakdown";
		private const int BreakdownMTBTicks = 14400000;
		private bool brokenDownInt;
		public bool BrokenDown
		{
			get
			{
				return this.brokenDownInt;
			}
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<bool>(ref this.brokenDownInt, "brokenDown", false, false);
		}
		public override void PostDraw()
		{
			if (this.brokenDownInt)
			{
				OverlayDrawer.DrawOverlay(this.parent, OverlayTypes.BrokenDown);
			}
		}
		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			Find.Map.GetComponent<BreakdownManager>().Register(this);
		}
		public override void PostDeSpawn()
		{
			base.PostDeSpawn();
			Find.Map.GetComponent<BreakdownManager>().Deregister(this);
		}
		public void CheckForBreakdown()
		{
			if (!this.BrokenDown && Rand.MTBEventOccurs(1.44E+07f, 1f, 1041f))
			{
				this.DoBreakdown();
			}
		}
		public void Notify_Repaired()
		{
			this.brokenDownInt = false;
			if (this.parent is Building_PowerSwitch)
			{
				PowerNetManager.Notfiy_TransmitterTransmitsPowerNowChanged(this.parent.GetComp<CompPower>());
			}
		}
		public void DoBreakdown()
		{
			this.brokenDownInt = true;
			this.parent.BroadcastCompSignal("Breakdown");
			if (this.parent.Faction == Faction.OfColony)
			{
				Find.LetterStack.ReceiveLetter("LetterLabelBuildingBrokenDown".Translate(new object[]
				{
					this.parent.LabelBaseShort
				}), "LetterBuildingBrokenDown".Translate(new object[]
				{
					this.parent.LabelBaseShort
				}), LetterType.BadNonUrgent, this.parent, null);
			}
		}
		public override string CompInspectStringExtra()
		{
			if (this.BrokenDown)
			{
				return "BrokenDown".Translate();
			}
			return null;
		}
	}
}
