using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public abstract class BuildableDef : Def
	{
		public List<StatModifier> statBases;
		public Traversability passability;
		public int pathCost;
		public float fertility = -1f;
		public List<ThingCount> costList;
		public int costStuffCount = -1;
		public TerrainAffordance terrainAffordanceNeeded;
		public List<ThingDef> buildingPrerequisites;
		public List<ResearchProjectDef> researchPrerequisites;
		public int placingDraggableDimensions;
		public EffecterDef repairEffect;
		public EffecterDef constructEffect;
		public Rot4 defaultPlacingRot = Rot4.North;
		[Unsaved]
		public ThingDef blueprintDef;
		[Unsaved]
		public ThingDef installBlueprintDef;
		[Unsaved]
		public ThingDef frameDef;
		public string uiIconPath;
		public AltitudeLayer altitudeLayer = AltitudeLayer.Item;
		[Unsaved]
		public Texture2D uiIcon = BaseContent.BadTex;
		[Unsaved]
		public Graphic graphic = BaseContent.BadGraphic;
		public bool menuHidden;
		public float specialDisplayRadius;
		public List<Type> placeWorkers;
		public string designationCategory;
		public KeyCode designationHotKey;
		[Unsaved]
		private List<PlaceWorker> placeWorkersInstantiatedInt;
		public abstract Color IconDrawColor
		{
			get;
		}
		public virtual IntVec2 Size
		{
			get
			{
				return new IntVec2(1, 1);
			}
		}
		public Material DrawMatSingle
		{
			get
			{
				if (this.graphic == null)
				{
					return null;
				}
				return this.graphic.MatSingle;
			}
		}
		public float Altitude
		{
			get
			{
				return Altitudes.AltitudeFor(this.altitudeLayer);
			}
		}
		public List<PlaceWorker> PlaceWorkers
		{
			get
			{
				if (this.placeWorkers == null)
				{
					return null;
				}
				this.placeWorkersInstantiatedInt = new List<PlaceWorker>();
				foreach (Type current in this.placeWorkers)
				{
					this.placeWorkersInstantiatedInt.Add((PlaceWorker)Activator.CreateInstance(current));
				}
				return this.placeWorkersInstantiatedInt;
			}
		}
		public override void PostLoad()
		{
			base.PostLoad();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!this.uiIconPath.NullOrEmpty())
				{
					this.uiIcon = ContentFinder<Texture2D>.Get(this.uiIconPath, true);
				}
				else
				{
					if (this.DrawMatSingle != null && this.DrawMatSingle != BaseContent.BadMat)
					{
						this.uiIcon = (Texture2D)this.DrawMatSingle.mainTexture;
					}
				}
			});
		}
		public override void ResolveReferences()
		{
			base.ResolveReferences();
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			BuildableDef.<ConfigErrors>c__Iterator133 <ConfigErrors>c__Iterator = new BuildableDef.<ConfigErrors>c__Iterator133();
			<ConfigErrors>c__Iterator.<>f__this = this;
			BuildableDef.<ConfigErrors>c__Iterator133 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override string ToString()
		{
			return this.defName;
		}
		public override int GetHashCode()
		{
			return this.defName.GetHashCode();
		}
	}
}
