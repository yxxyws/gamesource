using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public abstract class Designator : Command
	{
		protected bool useMouseIcon;
		public SoundDef soundDragSustain;
		public SoundDef soundDragChanged;
		protected SoundDef soundSucceeded;
		protected SoundDef soundFailed = SoundDefOf.DesignateFailed;
		public virtual int DraggableDimensions
		{
			get
			{
				return 0;
			}
		}
		public virtual bool DragDrawMeasurements
		{
			get
			{
				return false;
			}
		}
		protected override bool DoTooltip
		{
			get
			{
				return false;
			}
		}
		public Designator()
		{
			this.activateSound = SoundDefOf.SelectDesignator;
		}
		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			DesignatorManager.Select(this);
		}
		public virtual AcceptanceReport CanDesignateThing(Thing t)
		{
			throw new NotImplementedException();
		}
		public virtual void DesignateThing(Thing t)
		{
			throw new NotImplementedException();
		}
		public abstract AcceptanceReport CanDesignateCell(IntVec3 loc);
		public virtual void DesignateMultiCell(IEnumerable<IntVec3> cells)
		{
			bool flag = false;
			bool somethingSucceeded = false;
			foreach (IntVec3 current in cells)
			{
				AcceptanceReport acceptanceReport = this.CanDesignateCell(current);
				if (acceptanceReport.Accepted)
				{
					this.DesignateSingleCell(current);
					somethingSucceeded = true;
				}
				else
				{
					if (!flag)
					{
						Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
						flag = true;
					}
				}
			}
			this.Finalize(somethingSucceeded);
		}
		public virtual void DesignateSingleCell(IntVec3 c)
		{
			throw new NotImplementedException();
		}
		public void Finalize(bool somethingSucceeded)
		{
			if (somethingSucceeded)
			{
				this.FinalizeDesignationSucceeded();
			}
			else
			{
				this.FinalizeDesignationFailed();
			}
		}
		protected virtual void FinalizeDesignationSucceeded()
		{
			if (this.soundSucceeded != null)
			{
				this.soundSucceeded.PlayOneShotOnCamera();
			}
		}
		protected virtual void FinalizeDesignationFailed()
		{
			if (this.soundFailed != null)
			{
				this.soundFailed.PlayOneShotOnCamera();
			}
			if (DesignatorManager.Dragger.FailureReason != null)
			{
				Messages.Message(DesignatorManager.Dragger.FailureReason, MessageSound.RejectInput);
			}
		}
		public virtual string LabelCapReverseDesignating(Thing t)
		{
			return this.LabelCap;
		}
		public virtual string DescReverseDesignating(Thing t)
		{
			return this.Desc;
		}
		public virtual Texture2D IconReverseDesignating(Thing t)
		{
			return this.icon;
		}
		public virtual void DrawMouseAttachments()
		{
			if (this.useMouseIcon)
			{
				GenUI.DrawMouseAttachment(this.icon, string.Empty);
			}
		}
		public virtual void DrawPanelReadout(ref float curY, float width)
		{
		}
		public virtual void DoExtraGuiControls(float leftX, float bottomY)
		{
		}
		public virtual void SelectedUpdate()
		{
		}
		public virtual void Rotate(RotationDirection rotDir)
		{
		}
		public virtual bool CanRemainSelected()
		{
			return true;
		}
		public virtual void Selected()
		{
		}
	}
}
