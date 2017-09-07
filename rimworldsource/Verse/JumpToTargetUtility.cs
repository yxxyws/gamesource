using System;
namespace Verse
{
	public static class JumpToTargetUtility
	{
		public static void TryJumpAndSelect(Thing thing)
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return;
			}
			Find.CameraMap.JumpTo(thing.PositionHeld);
			if (thing.Spawned)
			{
				Find.Selector.ClearSelection();
				Find.Selector.Select(thing, true, true);
			}
		}
		public static void TryJumpAndSelect(TargetInfo target)
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return;
			}
			if (!target.IsValid)
			{
				return;
			}
			if (target.HasThing)
			{
				JumpToTargetUtility.TryJumpAndSelect(target.Thing);
			}
			else
			{
				Find.CameraMap.JumpTo(target.Cell);
			}
		}
	}
}
