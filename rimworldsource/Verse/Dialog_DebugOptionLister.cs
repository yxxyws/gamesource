using System;
using System.Linq;
using UnityEngine;
namespace Verse
{
	public abstract class Dialog_DebugOptionLister : Dialog_OptionLister
	{
		protected void DrawDebugAction(string label, Action action)
		{
			if (!base.FilterAllows(label))
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
			}
			if (this.listing.DoButtonDebug(label))
			{
				this.Close(true);
				action();
			}
			GUI.color = Color.white;
			if (Event.current.type == EventType.Layout)
			{
				this.totalOptionsHeight += 24f;
			}
		}
		protected void DrawDebugTool(string label, Action toolAction)
		{
			if (!base.FilterAllows(label))
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
			}
			if (this.listing.DoButtonDebug(label))
			{
				this.Close(true);
				DebugTools.curTool = new DebugTool(label, toolAction, null);
			}
			GUI.color = Color.white;
			if (Event.current.type == EventType.Layout)
			{
				this.totalOptionsHeight += 24f;
			}
		}
		protected void DrawDebugToolForPawns(string label, Action<Pawn> pawnAction)
		{
			this.DrawDebugTool(label, delegate
			{
				if (Gen.MouseCell().InBounds())
				{
					foreach (Pawn current in (
						from t in Find.ThingGrid.ThingsAt(Gen.MouseCell())
						where t is Pawn
						select t).Cast<Pawn>().ToList<Pawn>())
					{
						pawnAction(current);
					}
				}
			});
		}
		protected void DrawDebugLabelCheckbox(string label, ref bool checkOn)
		{
			if (!base.FilterAllows(label))
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
			}
			this.listing.DoLabelCheckboxDebug(label, ref checkOn);
			GUI.color = Color.white;
			if (Event.current.type == EventType.Layout)
			{
				this.totalOptionsHeight += 24f;
			}
		}
	}
}
