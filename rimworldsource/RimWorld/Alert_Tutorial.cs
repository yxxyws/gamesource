using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public abstract class Alert_Tutorial : Alert
	{
		private static readonly Color TutorialColor;
		public override AlertPriority Priority
		{
			get
			{
				return AlertPriority.Tutorial;
			}
		}
		public override Color ArrowColor
		{
			get
			{
				return Alert_Tutorial.TutorialColor;
			}
		}
		protected override Color BGColor
		{
			get
			{
				return Alert_Tutorial.TutorialColor;
			}
		}
		static Alert_Tutorial()
		{
			// Note: this type is marked as 'beforefieldinit'.
			ColorInt colorInt = new ColorInt(142, 84, 35);
			Alert_Tutorial.TutorialColor = colorInt.ToColor;
		}
	}
}
