using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class WorldInterface
	{
		private const float MinPixelsToDrag = 5f;
		private const float MouseDragDollySpeed = 0.0015f;
		private const float MouseScrollWheelZoomSpeed = 3.5f;
		private const float KeyZoomSpeed = 20f;
		private const float KeyDollySpeed = 0.1f;
		public IntVec2 selectedCoords = IntVec2.Invalid;
		private WorldView worldView;
		private bool viewDragging;
		private Vector2 viewDragStartLoc;
		private static readonly Texture2D SelectedSquareOverlay = ContentFinder<Texture2D>.Get("UI/World/SelectedWorldSquare", true);
		private static readonly Texture2D CurMapSquareOverlay = ContentFinder<Texture2D>.Get("UI/World/MapWorldSquare", true);
		public WorldInterface()
		{
			if (Game.Mode == GameMode.MapPlaying)
			{
				this.selectedCoords = Find.Map.WorldCoords;
			}
			else
			{
				if (MapInitData.landingCoords.IsValid && !Find.World.InBounds(MapInitData.landingCoords))
				{
					Log.Error("Map world coords were out of bounds.");
					MapInitData.landingCoords = IntVec2.Invalid;
				}
				this.selectedCoords = MapInitData.landingCoords;
			}
		}
		public void Draw(Rect mainRect, bool selectingLandingSite = false)
		{
			Rect rect = new Rect(mainRect);
			rect.width = 190f;
			rect.yMin += 30f;
			this.DrawControlPane(rect, selectingLandingSite);
			Rect rect2 = new Rect(mainRect);
			rect2.xMin += 204f;
			rect2.yMin += 30f;
			this.DrawWorldView(rect2);
			Rect rect3 = new Rect(rect2);
			rect3.height = 30f;
			rect3.y -= 30f;
			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Small;
			Widgets.Label(rect3, Find.World.info.name);
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect4 = new Rect(rect2);
			rect4.height = 30f;
			rect4.y -= 30f;
			Text.Anchor = TextAnchor.MiddleRight;
			Text.Font = GameFont.Tiny;
			GUI.color = Color.gray;
			Widgets.Label(rect4, Find.World.info.seedString);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		private void DrawControlPane(Rect rect, bool selectingLandingSite)
		{
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.BeginGroup(rect);
			if (this.selectedCoords.IsValid)
			{
				Rect rect2 = new Rect(0f, 0f, rect.width, 400f);
				this.DrawSquareInfo(rect2, this.selectedCoords);
			}
			Rect rect3 = new Rect(0f, 400f, 80f, 32f);
			if (Widgets.TextButton(rect3, "WorldRenderMode".Translate(), true, false))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (WorldRenderMode current in WorldRenderModeDatabase.AllModes)
				{
					WorldRenderMode localMode = current;
					FloatMenuOption item = new FloatMenuOption(localMode.Label, delegate
					{
						Find.World.renderer.CurMode = localMode;
					}, MenuOptionPriority.Medium, null, null);
					list.Add(item);
				}
				Find.WindowStack.Add(new FloatMenu(list, false));
			}
			Rect rect4 = new Rect(rect3.xMax, rect3.y, rect.width - rect3.xMax, rect3.height);
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect4, Find.World.renderer.CurMode.Label);
			Text.Anchor = TextAnchor.UpperLeft;
			if (selectingLandingSite)
			{
				Rect rect5 = new Rect(0f, rect3.yMax + 15f, rect.width - rect3.x, 32f);
				if (Widgets.TextButton(rect5, "SelectRandomSite".Translate(), true, false))
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
					this.selectedCoords = WorldSquareFinder.RandomDecentLandingSite();
				}
			}
			GUI.EndGroup();
		}
		private void DrawSquareInfo(Rect rect, IntVec2 wc)
		{
			if (this.selectedCoords.IsValid)
			{
				Rect butRect = new Rect(rect.x, rect.y, 24f, 24f);
				if (Widgets.ImageButton(butRect, TexButton.Info))
				{
					Find.WindowStack.Add(new Dialog_InfoCard(Find.World.grid.Get(this.selectedCoords).biome));
				}
				rect.yMin += 28f;
			}
			StringBuilder stringBuilder = new StringBuilder();
			WorldSquare worldSquare = Find.World.grid.Get(wc);
			if (Prefs.DevMode)
			{
				stringBuilder.AppendLine("Debug coords: " + wc);
			}
			Vector2 vector = Find.World.LongLatOf(wc);
			stringBuilder.AppendLine(vector.x.ToString("F2") + "°E " + vector.y.ToString("F2") + "°N");
			stringBuilder.AppendLine("Elevation".Translate() + ": " + worldSquare.elevation.ToString("F0") + "m");
			stringBuilder.AppendLine("AvgTemp".Translate() + ": " + worldSquare.temperature.ToStringTemperature("F1"));
			float celsiusTemp = GenTemperature.AverageTemperatureAtWorldCoordsForMonth(wc, Month.Jan);
			stringBuilder.AppendLine("AvgJanTemp".Translate(new object[]
			{
				celsiusTemp.ToStringTemperature("F1")
			}));
			float celsiusTemp2 = GenTemperature.AverageTemperatureAtWorldCoordsForMonth(wc, Month.Jul);
			stringBuilder.AppendLine("AvgJulTemp".Translate(new object[]
			{
				celsiusTemp2.ToStringTemperature("F1")
			}));
			if (!worldSquare.biome.canBuildBase)
			{
				stringBuilder.AppendLine(worldSquare.biome.LabelCap);
			}
			else
			{
				stringBuilder.AppendLine("Biome".Translate() + ": " + worldSquare.biome.LabelCap);
				stringBuilder.AppendLine("Terrain".Translate() + ": " + worldSquare.hilliness.GetLabel());
				stringBuilder.AppendLine("Rainfall".Translate() + ": " + worldSquare.rainfall.ToString("F0") + "mm");
			}
			if (!worldSquare.biome.implemented)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(worldSquare.biome.LabelCap + " " + "BiomeNotImplemented".Translate());
			}
			Rot4 rot = Find.World.CoastDirectionAt(wc);
			if (rot.IsValid)
			{
				stringBuilder.AppendLine(("HasCoast" + rot.ToString()).Translate());
			}
			if (worldSquare.biome.canBuildBase)
			{
				stringBuilder.AppendLine("StoneTypesHere".Translate() + ": " + GenText.ToCommaList(
					from r in Find.World.NaturalRockTypesIn(wc)
					select r.label));
			}
			stringBuilder.AppendLine(Zone_Growing.GrowingMonthsDescription(wc));
			Faction faction = Current.World.factionManager.FactionInSquare(wc);
			if (faction != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"FactionBase".Translate(),
					": ",
					faction.name,
					" (",
					faction.def.label,
					")"
				}));
			}
			Widgets.Label(rect, stringBuilder.ToString());
		}
		private void DrawWorldView(Rect worldRect)
		{
			Widgets.DrawWindowBackground(worldRect);
			worldRect = worldRect.ContractedBy(1f);
			GUI.BeginGroup(worldRect);
			Rect rect = new Rect(0f, 0f, worldRect.width, worldRect.height);
			if (this.worldView == null)
			{
				this.worldView = new WorldView(rect, Find.World.Size.ToVector2() * 0.5f, (float)Find.World.Size.x);
				Find.World.renderer.SetView(this.worldView);
			}
			Find.World.renderer.Draw();
			if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				this.TrySelectWorldSquare(this.worldView.WorldSquareAt(Event.current.mousePosition));
			}
			KnowledgeAmount knowledgeAmount = KnowledgeAmount.None;
			if (Event.current.type == EventType.KeyDown)
			{
				if (KeyBindingDefOf.MapDollyDown.IsDown)
				{
					this.worldView.TryDolly(new Vector2(0f, -1f) * 0.1f * this.worldView.worldRectWidth);
					knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				}
				if (KeyBindingDefOf.MapDollyUp.IsDown)
				{
					this.worldView.TryDolly(new Vector2(0f, 1f) * 0.1f * this.worldView.worldRectWidth);
					knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				}
				if (KeyBindingDefOf.MapDollyLeft.IsDown)
				{
					this.worldView.TryDolly(new Vector2(1f, 0f) * 0.1f * this.worldView.worldRectWidth);
					knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				}
				if (KeyBindingDefOf.MapDollyRight.IsDown)
				{
					this.worldView.TryDolly(new Vector2(-1f, 0f) * 0.1f * this.worldView.worldRectWidth);
					knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				}
				if (KeyBindingDefOf.MapZoomIn.KeyDownEvent)
				{
					this.worldView.TryZoom(-20f);
					knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				}
				if (KeyBindingDefOf.MapZoomOut.KeyDownEvent)
				{
					this.worldView.TryZoom(20f);
					knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				}
			}
			if (Event.current.button == 2)
			{
				if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
				{
					this.viewDragging = true;
					this.viewDragStartLoc = Event.current.mousePosition;
					Event.current.Use();
				}
				if (Event.current.type == EventType.MouseUp)
				{
					this.viewDragging = false;
					Event.current.Use();
				}
				if (Event.current.type == EventType.MouseDrag)
				{
					if (this.viewDragging && (this.viewDragStartLoc - Event.current.mousePosition).magnitude > 5f)
					{
						Vector2 vector = Event.current.delta;
						vector *= 0.0015f;
						vector *= this.worldView.worldRectWidth;
						this.worldView.TryDolly(vector);
						knowledgeAmount = KnowledgeAmount.GuiFrame;
					}
					Event.current.Use();
				}
			}
			if (Event.current.type == EventType.ScrollWheel)
			{
				this.worldView.TryZoom(Event.current.delta.y * 3.5f);
				knowledgeAmount = KnowledgeAmount.SpecificInteraction;
				Event.current.Use();
			}
			if (knowledgeAmount > KnowledgeAmount.None)
			{
				ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, knowledgeAmount);
			}
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				this.viewDragging = false;
				this.selectedCoords = IntVec2.Invalid;
				Event.current.Use();
			}
			if (this.selectedCoords.IsValid)
			{
				Vector2 vector2 = this.worldView.ScreenLocOf(this.selectedCoords);
				float pixelsPerWorldSquare = this.worldView.PixelsPerWorldSquare;
				Rect position = new Rect(vector2.x - pixelsPerWorldSquare * 0.75f, vector2.y - pixelsPerWorldSquare * 0.75f, pixelsPerWorldSquare * 1.5f, pixelsPerWorldSquare * 1.5f);
				GUI.DrawTexture(position, WorldInterface.SelectedSquareOverlay);
			}
			if (Game.Mode == GameMode.MapPlaying)
			{
				Vector2 vector3 = this.worldView.ScreenLocOf(Find.Map.WorldCoords);
				float pixelsPerWorldSquare2 = this.worldView.PixelsPerWorldSquare;
				Rect position2 = new Rect(vector3.x - pixelsPerWorldSquare2 * 0.75f, vector3.y - pixelsPerWorldSquare2 * 0.75f, pixelsPerWorldSquare2 * 1.5f, pixelsPerWorldSquare2 * 1.5f);
				GUI.DrawTexture(position2, WorldInterface.CurMapSquareOverlay);
			}
			GUI.EndGroup();
		}
		private bool TrySelectWorldSquare(IntVec2 c)
		{
			if (!Find.World.InBounds(c))
			{
				return false;
			}
			this.selectedCoords = c;
			SoundDefOf.ThingSelected.PlayOneShotOnCamera();
			return true;
		}
	}
}
