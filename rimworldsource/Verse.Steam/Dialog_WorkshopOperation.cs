using Steamworks;
using System;
using UnityEngine;
namespace Verse.Steam
{
	internal class Dialog_WorkshopOperation : Window
	{
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2(600f, 400f);
			}
		}
		public Dialog_WorkshopOperation()
		{
			this.forcePause = true;
			this.closeOnEscapeKey = true;
		}
		public override void DoWindowContents(Rect inRect)
		{
			EItemUpdateStatus eItemUpdateStatus;
			float num;
			SteamWorkshop.GetUpdateStatus(out eItemUpdateStatus, out num);
			WorkshopInteractStage curStage = SteamWorkshop.CurStage;
			if (curStage == WorkshopInteractStage.None && eItemUpdateStatus == EItemUpdateStatus.k_EItemUpdateStatusInvalid)
			{
				this.Close(true);
				return;
			}
			string text = string.Empty;
			if (curStage != WorkshopInteractStage.None)
			{
				text += curStage.GetLabel();
				text += "\n\n";
			}
			if (eItemUpdateStatus != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
			{
				text += eItemUpdateStatus.GetLabel();
				if (num > 0f)
				{
					text = text + " (" + num.ToStringPercent() + ")";
				}
			}
			Widgets.Label(inRect, text);
		}
	}
}
