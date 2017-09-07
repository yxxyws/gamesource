using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class Zone_Stockpile : Zone, IStoreSettingsParent, ISlotGroupParent
	{
		public StorageSettings settings;
		public SlotGroup slotGroup;
		private static readonly ITab StorageTab = new ITab_Storage();
		public bool StorageTabVisible
		{
			get
			{
				return true;
			}
		}
		public Zone_Stockpile()
		{
		}
		public Zone_Stockpile(StorageSettingsPreset preset) : base(preset.PresetName())
		{
			this.settings = new StorageSettings(this);
			this.settings.SetFromPreset(preset);
			this.slotGroup = new SlotGroup(this);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.LookDeep<StorageSettings>(ref this.settings, "settings", new object[]
			{
				this
			});
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				this.slotGroup = new SlotGroup(this);
			}
		}
		public override void AddCell(IntVec3 sq)
		{
			base.AddCell(sq);
			if (this.slotGroup != null)
			{
				this.slotGroup.Notify_AddedCell(sq);
			}
		}
		public override void RemoveCell(IntVec3 sq)
		{
			base.RemoveCell(sq);
			this.slotGroup.Notify_LostCell(sq);
		}
		public override void Deregister()
		{
			base.Deregister();
			this.slotGroup.Notify_ParentDestroying();
		}
		[DebuggerHidden]
		public override IEnumerable<ITab> GetInspectionTabs()
		{
			Zone_Stockpile.<GetInspectionTabs>c__Iterator87 <GetInspectionTabs>c__Iterator = new Zone_Stockpile.<GetInspectionTabs>c__Iterator87();
			Zone_Stockpile.<GetInspectionTabs>c__Iterator87 expr_07 = <GetInspectionTabs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Zone_Stockpile.<GetGizmos>c__Iterator88 <GetGizmos>c__Iterator = new Zone_Stockpile.<GetGizmos>c__Iterator88();
			<GetGizmos>c__Iterator.<>f__this = this;
			Zone_Stockpile.<GetGizmos>c__Iterator88 expr_0E = <GetGizmos>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public SlotGroup GetSlotGroup()
		{
			return this.slotGroup;
		}
		[DebuggerHidden]
		public IEnumerable<IntVec3> AllSlotCells()
		{
			Zone_Stockpile.<AllSlotCells>c__Iterator89 <AllSlotCells>c__Iterator = new Zone_Stockpile.<AllSlotCells>c__Iterator89();
			<AllSlotCells>c__Iterator.<>f__this = this;
			Zone_Stockpile.<AllSlotCells>c__Iterator89 expr_0E = <AllSlotCells>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public List<IntVec3> AllSlotCellsList()
		{
			return this.cells;
		}
		public StorageSettings GetParentStoreSettings()
		{
			return null;
		}
		public StorageSettings GetStoreSettings()
		{
			return this.settings;
		}
		public string SlotYielderLabel()
		{
			return this.label;
		}
		public void Notify_ReceivedThing(Thing newItem)
		{
		}
		public void Notify_LostThing(Thing newItem)
		{
		}
	}
}
