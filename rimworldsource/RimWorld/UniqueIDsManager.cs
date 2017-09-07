using System;
using Verse;
namespace RimWorld
{
	public class UniqueIDsManager : IExposable
	{
		private int nextThingID;
		private int nextBillID;
		private int nextFactionID;
		private int nextVerbID;
		private int nextLordID;
		private int nextTaleID;
		private int nextPassingShipID;
		public int GetNextThingID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextThingID);
		}
		public int GetNextBillID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextBillID);
		}
		public int GetNextFactionID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextFactionID);
		}
		public int GetNextVerbID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextVerbID);
		}
		public int GetNextLordID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextLordID);
		}
		public int GetNextTaleID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextTaleID);
		}
		public int GetNextPassingShipID()
		{
			return UniqueIDsManager.GetNextID(ref this.nextPassingShipID);
		}
		private static int GetNextID(ref int nextID)
		{
			if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.LoadingVars)
			{
				Log.Warning("Getting next unique ID during saving or loading. This may cause bugs.");
			}
			int result = nextID;
			nextID++;
			if (nextID == 2147483647)
			{
				Log.Warning("Next ID is at max value. Resetting to 0. This may cause bugs.");
				nextID = 0;
			}
			return result;
		}
		public void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.nextThingID, "nextThingID", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextBillID, "nextBillID", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextFactionID, "nextFactionID", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextVerbID, "nextVerbID", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextLordID, "nextLordID", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextTaleID, "nextTaleID", 0, false);
			Scribe_Values.LookValue<int>(ref this.nextPassingShipID, "nextPassingShipID", 0, false);
		}
	}
}
