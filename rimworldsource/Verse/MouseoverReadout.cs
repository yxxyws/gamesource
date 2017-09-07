using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	internal class MouseoverReadout
	{
		private const float YInterval = 19f;
		private TerrainDef cachedTerrain;
		private string cachedTerrainString;
		private static readonly Vector2 BotLeft = new Vector2(15f, 65f);
		public void MouseoverReadoutOnGUI()
		{
			if (Find.MainTabsRoot.OpenTab != null)
			{
				return;
			}
			GenUI.DrawTextWinterShadow(new Rect(256f, (float)(Screen.height - 256), -256f, 256f));
			Text.Font = GameFont.Small;
			GUI.color = new Color(1f, 1f, 1f, 0.8f);
			IntVec3 c = Gen.MouseCell();
			if (!c.InBounds())
			{
				return;
			}
			float num = 0f;
			Rect rect;
			if (c.Fogged())
			{
				rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
				Widgets.Label(rect, "Undiscovered".Translate());
				GUI.color = Color.white;
				return;
			}
			rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
			Widgets.Label(rect, Find.GlowGrid.PsychGlowAt(c).GetLabel() + " (" + Find.GlowGrid.GameGlowAt(c).ToStringPercent() + ")");
			num += 19f;
			rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
			TerrainDef terrain = c.GetTerrain();
			if (terrain != this.cachedTerrain)
			{
				this.cachedTerrainString = terrain.LabelCap + ((terrain.passability == Traversability.Impassable) ? null : (" (" + "WalkSpeed".Translate(new object[]
				{
					this.SpeedPercentString((float)terrain.pathCost)
				}) + ")"));
				this.cachedTerrain = terrain;
			}
			Widgets.Label(rect, this.cachedTerrainString);
			num += 19f;
			Zone zone = c.GetZone();
			if (zone != null)
			{
				rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
				string label = zone.label;
				Widgets.Label(rect, label);
				num += 19f;
			}
			float depth = Find.SnowGrid.GetDepth(c);
			if (depth > 0.03f)
			{
				rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
				SnowCategory snowCategory = SnowUtility.GetSnowCategory(depth);
				string label2 = SnowUtility.GetDescription(snowCategory) + " (" + "WalkSpeed".Translate(new object[]
				{
					this.SpeedPercentString((float)SnowUtility.MovementTicksAddOn(snowCategory))
				}) + ")";
				Widgets.Label(rect, label2);
				num += 19f;
			}
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing.def.category != ThingCategory.Mote)
				{
					rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
					string labelMouseover = thing.LabelMouseover;
					Widgets.Label(rect, labelMouseover);
					num += 19f;
				}
			}
			RoofDef roof = c.GetRoof();
			if (roof != null)
			{
				rect = new Rect(MouseoverReadout.BotLeft.x, (float)Screen.height - MouseoverReadout.BotLeft.y - num, 999f, 999f);
				Widgets.Label(rect, roof.LabelCap);
				num += 19f;
			}
			GUI.color = Color.white;
		}
		private string SpeedPercentString(float extraPathTicks)
		{
			float f = 13f / (extraPathTicks + 13f);
			return f.ToStringPercent();
		}
	}
}
