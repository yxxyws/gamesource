using RimWorld;
using System;
namespace Verse
{
	public class RootEntry : Root
	{
		public override void RootUpdate()
		{
			base.RootUpdate();
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			MusicManagerEntry.MusicManagerEntryUpdate();
		}
	}
}
