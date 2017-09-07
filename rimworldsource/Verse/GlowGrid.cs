using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public sealed class GlowGrid
	{
		public const int AlphaOfNotOverlit = 0;
		public const int AlphaOfOverlit = 1;
		private const float MaxGameGlowFromNonOverlitGroundLights = 0.6f;
		private const float GameGlowLitThreshold = 0.3f;
		private const float GroundGameGlowFactorUp = 6f;
		public Color32[] glowGrid;
		private bool glowGridDirty;
		private HashSet<CompGlower> litGlowers = new HashSet<CompGlower>();
		private List<IntVec3> initialGlowerLocs = new List<IntVec3>();
		public GlowGrid()
		{
			this.glowGrid = new Color32[CellIndices.NumGridCells];
		}
		public Color32 VisualGlowAt(IntVec3 c)
		{
			return this.glowGrid[CellIndices.CellToIndex(c)];
		}
		public float GameGlowAt(IntVec3 c)
		{
			float num = 0f;
			if (!Find.RoofGrid.Roofed(c))
			{
				num += SkyManager.CurSkyGlow;
			}
			Color32 color = this.glowGrid[CellIndices.CellToIndex(c)];
			if (color.a == 1)
			{
				return 1f;
			}
			int num2 = (int)color.r;
			if ((int)color.g > num2)
			{
				num2 = (int)color.g;
			}
			if ((int)color.b > num2)
			{
				num2 = (int)color.b;
			}
			float num3 = (float)num2 / 255f * 0.6f;
			num3 *= 6f;
			num3 = Mathf.Min(0.6f, num3);
			return Mathf.Max(num, num3);
		}
		public PsychGlow PsychGlowAt(IntVec3 c)
		{
			float num = this.GameGlowAt(c);
			if (num > 0.9f)
			{
				return PsychGlow.Overlit;
			}
			if (num > 0.3f)
			{
				return PsychGlow.Lit;
			}
			return PsychGlow.Dark;
		}
		public void RegisterGlower(CompGlower newGlow)
		{
			this.litGlowers.Add(newGlow);
			this.MarkGlowGridDirty(newGlow.parent.Position);
			if (Game.Mode != GameMode.MapPlaying)
			{
				this.initialGlowerLocs.Add(newGlow.parent.Position);
			}
		}
		public void DeRegisterGlower(CompGlower oldGlow)
		{
			this.litGlowers.Remove(oldGlow);
			this.MarkGlowGridDirty(oldGlow.parent.Position);
		}
		public void MarkGlowGridDirty(IntVec3 loc)
		{
			this.glowGridDirty = true;
			Find.MapDrawer.MapMeshDirty(loc, MapMeshFlag.GroundGlow);
		}
		public void GlowGridUpdate_First()
		{
			if (this.glowGridDirty)
			{
				this.RecalculateAllGlow();
				this.glowGridDirty = false;
			}
		}
		private void RecalculateAllGlow()
		{
			if (Game.Mode != GameMode.MapPlaying)
			{
				return;
			}
			if (this.initialGlowerLocs != null)
			{
				foreach (IntVec3 current in this.initialGlowerLocs)
				{
					this.MarkGlowGridDirty(current);
				}
				this.initialGlowerLocs = null;
			}
			for (int i = 0; i < CellIndices.NumGridCells; i++)
			{
				this.glowGrid[i] = new Color32(0, 0, 0, 0);
			}
			foreach (CompGlower current2 in this.litGlowers)
			{
				GlowFlooder.AddFloodGlowFor(current2);
			}
		}
	}
}
