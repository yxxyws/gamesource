using System;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class MainTabWindow_Animals : MainTabWindow_PawnList
	{
		private const float TopAreaHeight = 65f;
		private const float MasterWidth = 170f;
		private const float AreaAllowedWidth = 350f;
		private const float PaddingBeforeIcons = 10f;
		private const float IconSize = 24f;
		private static readonly Texture2D PregnantIcon = Resources.Load<Texture2D>("Textures/UI/Icons/Pregnant");
		public override Vector2 RequestedTabSize
		{
			get
			{
				return new Vector2(1010f, 65f + (float)base.PawnsCount * 30f + 65f);
			}
		}
		protected override void BuildPawnList()
		{
			this.pawns = (
				from p in Find.MapPawns.PawnsInFaction(Faction.OfColony)
				where p.RaceProps.Animal
				orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
				select p).ToList<Pawn>();
		}
		public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
			Rect position = new Rect(0f, 0f, fillRect.width, 65f);
			GUI.BeginGroup(position);
			float num = 175f;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerLeft;
			Rect rect = new Rect(num, 0f, 170f, position.height + 3f);
			Widgets.Label(rect, "Master".Translate());
			num += 170f;
			Rect rect2 = new Rect(num, 0f, 350f, Mathf.Round(position.height / 2f));
			Text.Font = GameFont.Small;
			if (Widgets.TextButton(rect2, "ManageAreas".Translate(), true, false))
			{
				Find.WindowStack.Add(new Dialog_ManageAreas());
			}
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerCenter;
			Rect rect3 = new Rect(num, 0f, 350f, position.height + 3f);
			Widgets.Label(rect3, "AllowedArea".Translate());
			num += 350f;
			GUI.EndGroup();
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			Rect outRect = new Rect(0f, position.height, fillRect.width, fillRect.height - position.height);
			base.DrawRows(outRect);
		}
		protected override void DrawPawnRow(Rect rect, Pawn p)
		{
			GUI.BeginGroup(rect);
			float num = 175f;
			if (p.training.IsCompleted(TrainableDefOf.Obedience))
			{
				Rect rect2 = new Rect(num, 0f, 170f, rect.height);
				Rect rect3 = rect2.ContractedBy(2f);
				string label = TrainableUtility.MasterString(p);
				Text.Font = GameFont.Small;
				if (Widgets.TextButton(rect3, label, true, false))
				{
					TrainableUtility.OpenMasterSelectMenu(p);
				}
			}
			num += 170f;
			Rect rect4 = new Rect(num, 0f, 350f, rect.height);
			AreaAllowedGUI.DoAllowedAreaSelectors(rect4, p, AllowedAreaMode.Animal);
			num += 350f;
			num += 10f;
			Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)p.health.hediffSet.hediffs.Find((Hediff x) => x.def == HediffDefOf.Pregnant && x.Visible);
			if (hediff_Pregnant != null)
			{
				Rect rect5 = new Rect(num, (rect.height - 24f) / 2f, 24f, 24f);
				GUI.DrawTexture(rect5, MainTabWindow_Animals.PregnantIcon);
				float gestationProgress = hediff_Pregnant.GestationProgress;
				int num2 = (int)(p.RaceProps.gestationPeriodDays * 60000f);
				int numTicks = (int)(gestationProgress * (float)num2);
				TooltipHandler.TipRegion(rect5, new TipSignal("PregnantIconDesc".Translate(new object[]
				{
					numTicks.TicksToPeriodString(true),
					num2.TicksToPeriodString(true)
				}), rect5.GetHashCode()));
				num += 24f;
			}
			GUI.EndGroup();
		}
	}
}
