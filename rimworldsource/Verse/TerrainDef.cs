using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class TerrainDef : BuildableDef
	{
		public enum TerrainEdgeType : byte
		{
			Hard,
			Fade,
			FadeRough
		}
		public string texturePath;
		public TerrainDef.TerrainEdgeType edgeType;
		public int renderPrecedence;
		public List<TerrainAffordance> affordances = new List<TerrainAffordance>();
		public bool layerable;
		public string scatterType;
		public bool takeFootprints;
		public bool avoidWander;
		public bool changeable = true;
		public TerrainDef smoothedTerrain;
		public bool holdSnow = true;
		public Color color = Color.white;
		public ThingDef terrainFilthDef;
		public bool acceptTerrainSourceFilth;
		public bool acceptFilth = true;
		public override Color IconDrawColor
		{
			get
			{
				return this.color;
			}
		}
		public override void PostLoad()
		{
			this.placingDraggableDimensions = 2;
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				Shader shader = null;
				switch (this.edgeType)
				{
				case TerrainDef.TerrainEdgeType.Hard:
					shader = ShaderDatabase.TerrainHard;
					break;
				case TerrainDef.TerrainEdgeType.Fade:
					shader = ShaderDatabase.TerrainFade;
					break;
				case TerrainDef.TerrainEdgeType.FadeRough:
					shader = ShaderDatabase.TerrainFadeRough;
					break;
				}
				this.graphic = GraphicDatabase.Get<Graphic_Terrain>(this.texturePath, shader, Vector2.one, this.color);
				this.graphic.MatSingle.renderQueue = 2000 + this.renderPrecedence;
				if (shader == ShaderDatabase.TerrainFadeRough)
				{
					this.graphic.MatSingle.SetTexture("_AlphaAddTex", TexUI.AlphaAddTex);
				}
			});
			base.PostLoad();
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			TerrainDef.<ConfigErrors>c__Iterator14A <ConfigErrors>c__Iterator14A = new TerrainDef.<ConfigErrors>c__Iterator14A();
			<ConfigErrors>c__Iterator14A.<>f__this = this;
			TerrainDef.<ConfigErrors>c__Iterator14A expr_0E = <ConfigErrors>c__Iterator14A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public static TerrainDef Named(string defName)
		{
			return DefDatabase<TerrainDef>.GetNamed(defName, true);
		}
	}
}
