using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public class CompShipPart : ThingComp
	{
		[DebuggerHidden]
		public override IEnumerable<Command> CompGetGizmosExtra()
		{
			CompShipPart.<CompGetGizmosExtra>c__IteratorF4 <CompGetGizmosExtra>c__IteratorF = new CompShipPart.<CompGetGizmosExtra>c__IteratorF4();
			<CompGetGizmosExtra>c__IteratorF.<>f__this = this;
			CompShipPart.<CompGetGizmosExtra>c__IteratorF4 expr_0E = <CompGetGizmosExtra>c__IteratorF;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public void ShowReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!ShipUtility.LaunchFailReasons((Building)this.parent).Any<string>())
			{
				stringBuilder.AppendLine("ShipReportCanLaunch".Translate());
			}
			else
			{
				stringBuilder.AppendLine("ShipReportCannotLaunch".Translate());
				foreach (string current in ShipUtility.LaunchFailReasons((Building)this.parent))
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(current);
				}
			}
			Dialog_Message window = new Dialog_Message(stringBuilder.ToString(), null);
			Find.WindowStack.Add(window);
		}
	}
}
