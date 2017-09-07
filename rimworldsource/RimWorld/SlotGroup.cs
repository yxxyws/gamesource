using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public class SlotGroup
	{
		public ISlotGroupParent parent;
		public StorageSettings Settings
		{
			get
			{
				return this.parent.GetStoreSettings();
			}
		}
		public IEnumerable<Thing> HeldThings
		{
			get
			{
				SlotGroup.<>c__IteratorD5 <>c__IteratorD = new SlotGroup.<>c__IteratorD5();
				<>c__IteratorD.<>f__this = this;
				SlotGroup.<>c__IteratorD5 expr_0E = <>c__IteratorD;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public List<IntVec3> CellsList
		{
			get
			{
				return this.parent.AllSlotCellsList();
			}
		}
		public SlotGroup(ISlotGroupParent parent)
		{
			this.parent = parent;
			Find.SlotGroupManager.AddGroup(this);
		}
		[DebuggerHidden]
		public IEnumerator<IntVec3> GetEnumerator()
		{
			SlotGroup.<GetEnumerator>c__IteratorD6 <GetEnumerator>c__IteratorD = new SlotGroup.<GetEnumerator>c__IteratorD6();
			<GetEnumerator>c__IteratorD.<>f__this = this;
			return <GetEnumerator>c__IteratorD;
		}
		public void Notify_AddedCell(IntVec3 c)
		{
			Find.SlotGroupManager.SetCellFor(c, this);
			ListerHaulables.RecalcAllInCell(c);
		}
		public void Notify_LostCell(IntVec3 c)
		{
			Find.SlotGroupManager.ClearCellFor(c, this);
			ListerHaulables.RecalcAllInCell(c);
		}
		public void Notify_ParentDestroying()
		{
			Find.SlotGroupManager.RemoveGroup(this);
		}
		public override string ToString()
		{
			if (this.parent != null)
			{
				return this.parent.ToString();
			}
			return "NullParent";
		}
	}
}
