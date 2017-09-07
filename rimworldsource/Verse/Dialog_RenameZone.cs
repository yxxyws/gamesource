using System;
using System.Linq;
namespace Verse
{
	public class Dialog_RenameZone : Dialog_Rename
	{
		private Zone zone;
		public Dialog_RenameZone(Zone zone)
		{
			this.zone = zone;
			this.curName = zone.label;
		}
		protected override AcceptanceReport NameIsValid(string name)
		{
			AcceptanceReport result = base.NameIsValid(name);
			if (!result.Accepted)
			{
				return result;
			}
			if (Find.ZoneManager.AllZones.Any((Zone z) => z.label == name))
			{
				return "NameIsInUse".Translate();
			}
			return true;
		}
		protected override void SetName(string name)
		{
			this.zone.label = this.curName;
			Messages.Message("ZoneGainsName".Translate(new object[]
			{
				this.curName
			}), MessageSound.Benefit);
		}
	}
}
