using System;
using UnityEngine;
namespace Verse
{
	public class MoteAttachee
	{
		public TargetInfo target;
		public Vector3 lastDrawPos;
		public bool isSet;
		public TargetInfo Attachee
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
				if (this.target.IsValid)
				{
					this.isSet = true;
					this.UpdateLastDrawPos();
				}
			}
		}
		public void UpdateLastDrawPos()
		{
			if (this.target.HasThing)
			{
				this.lastDrawPos = this.target.Thing.DrawPos;
			}
			else
			{
				this.lastDrawPos = this.target.Cell.ToVector3Shifted();
			}
		}
	}
}
