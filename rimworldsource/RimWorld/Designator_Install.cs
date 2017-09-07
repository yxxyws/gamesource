using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Install : Designator_Place
	{
		private Thing MiniToInstallOrBuildingToReinstall
		{
			get
			{
				Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
				if (singleSelectedThing is MinifiedThing)
				{
					return singleSelectedThing;
				}
				Building building = singleSelectedThing as Building;
				if (building != null && building.def.Minifiable)
				{
					return singleSelectedThing;
				}
				return null;
			}
		}
		private Thing ThingToInstall
		{
			get
			{
				return this.MiniToInstallOrBuildingToReinstall.GetInnerIfMinified();
			}
		}
		protected override bool DoTooltip
		{
			get
			{
				return true;
			}
		}
		public override BuildableDef PlacingDef
		{
			get
			{
				return this.ThingToInstall.def;
			}
		}
		public override string Label
		{
			get
			{
				if (this.MiniToInstallOrBuildingToReinstall is MinifiedThing)
				{
					return "CommandInstall".Translate();
				}
				return "CommandReinstall".Translate();
			}
		}
		public override string Desc
		{
			get
			{
				if (this.MiniToInstallOrBuildingToReinstall is MinifiedThing)
				{
					return "CommandInstallDesc".Translate();
				}
				return "CommandReinstallDesc".Translate();
			}
		}
		protected override Color IconDrawColor
		{
			get
			{
				return Color.white;
			}
		}
		public override bool Visible
		{
			get
			{
				return Find.Selector.SingleSelectedThing != null && base.Visible;
			}
		}
		public Designator_Install()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = TexCommand.Install;
			});
			this.iconProportions = new Vector2(1f, 1f);
		}
		public override bool CanRemainSelected()
		{
			return this.MiniToInstallOrBuildingToReinstall != null;
		}
		public override void ProcessInput(Event ev)
		{
			Thing miniToInstallOrBuildingToReinstall = this.MiniToInstallOrBuildingToReinstall;
			if (miniToInstallOrBuildingToReinstall != null)
			{
				InstallBlueprintUtility.CancelBlueprintsFor(miniToInstallOrBuildingToReinstall);
				if (!((ThingDef)this.PlacingDef).rotatable)
				{
					this.placingRot = Rot4.North;
				}
			}
			base.ProcessInput(ev);
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			return GenConstruct.CanPlaceBlueprintAt(this.PlacingDef, c, this.placingRot, false);
		}
		public override void DesignateSingleCell(IntVec3 c)
		{
			GenSpawn.WipeExistingThings(c, this.placingRot, this.PlacingDef.installBlueprintDef, true);
			MinifiedThing minifiedThing = this.MiniToInstallOrBuildingToReinstall as MinifiedThing;
			if (minifiedThing != null)
			{
				GenConstruct.PlaceBlueprintForInstall(minifiedThing, c, this.placingRot, Faction.OfColony);
			}
			else
			{
				GenConstruct.PlaceBlueprintForReinstall((Building)this.MiniToInstallOrBuildingToReinstall, c, this.placingRot, Faction.OfColony);
			}
			MoteThrower.ThrowMetaPuffs(GenAdj.OccupiedRect(c, this.placingRot, this.PlacingDef.Size));
			DesignatorManager.Deselect();
		}
		protected override void DrawGhost(Color ghostCol)
		{
			Graphic baseGraphic = this.ThingToInstall.Graphic.ExtractInnerGraphicFor(this.ThingToInstall);
			GhostDrawer.DrawGhostThing(Gen.MouseCell(), this.placingRot, (ThingDef)this.PlacingDef, baseGraphic, ghostCol, AltitudeLayer.Blueprint);
		}
	}
}
