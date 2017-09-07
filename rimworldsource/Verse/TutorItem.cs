using System;
using System.Collections.Generic;
namespace Verse
{
	public abstract class TutorItem
	{
		protected List<ConceptDef> showSignalsList = new List<ConceptDef>();
		public virtual bool Relevant
		{
			get
			{
				return true;
			}
		}
		public abstract void TutorItemOnGUI();
		public bool ShowOnSignal(ConceptDef signal)
		{
			return this.showSignalsList.Contains(signal);
		}
	}
}
