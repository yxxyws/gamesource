using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Verse
{
	public class Corpse : ThingWithComps, IBillGiver, IThoughtGiver, IStrippable
	{
		private const int VanishAfterTicks = 3000000;
		public Pawn innerPawn;
		private int timeOfDeath = -1000;
		private int vanishAfterTimestamp = 3000000;
		private BillStack operationsBillStack;
		public int Age
		{
			get
			{
				return Find.TickManager.TicksGame - this.timeOfDeath;
			}
			set
			{
				this.timeOfDeath = Find.TickManager.TicksGame - value;
			}
		}
		public override string Label
		{
			get
			{
				return "DeadLabel".Translate(new object[]
				{
					this.innerPawn.LabelCap
				});
			}
		}
		public override bool IngestibleNow
		{
			get
			{
				if (!this.innerPawn.RaceProps.IsFlesh)
				{
					return false;
				}
				CompRottable comp = base.GetComp<CompRottable>();
				return comp == null || comp.Stage != RotStage.Dessicated;
			}
		}
		private bool ShouldVanish
		{
			get
			{
				return this.innerPawn.RaceProps.Animal && this.vanishAfterTimestamp > 0 && this.Age >= this.vanishAfterTimestamp && this.GetRoom().TouchesMapEdge && !Find.RoofGrid.Roofed(base.Position);
			}
		}
		public BillStack BillStack
		{
			get
			{
				return this.operationsBillStack;
			}
		}
		public IEnumerable<IntVec3> IngredientStackCells
		{
			get
			{
				Corpse.<>c__Iterator189 <>c__Iterator = new Corpse.<>c__Iterator189();
				<>c__Iterator.<>f__this = this;
				Corpse.<>c__Iterator189 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}
		public Corpse()
		{
			this.operationsBillStack = new BillStack(this);
		}
		public bool CurrentlyUsable()
		{
			return this.InteractionCell.IsValid;
		}
		public bool AnythingToStrip()
		{
			return this.innerPawn.AnythingToStrip();
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			if (this.timeOfDeath < 0)
			{
				this.timeOfDeath = Find.TickManager.TicksGame;
			}
			this.innerPawn.Rotation = Rot4.South;
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			if (this.innerPawn.ownership != null)
			{
				this.innerPawn.ownership.UnclaimAll();
			}
			if (this.innerPawn.equipment != null)
			{
				this.innerPawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
			}
			this.innerPawn.inventory.DestroyAll(DestroyMode.Vanish);
			if (this.innerPawn.apparel != null)
			{
				this.innerPawn.apparel.DestroyAll(DestroyMode.Vanish);
			}
			if (!Find.WorldPawns.Contains(this.innerPawn))
			{
				Find.WorldPawns.PassToWorld(this.innerPawn, PawnDiscardDecideMode.Decide);
			}
			base.Destroy(mode);
		}
		public override void TickRare()
		{
			base.TickRare();
			this.innerPawn.TickRare();
			CompRottable comp = base.GetComp<CompRottable>();
			if (this.vanishAfterTimestamp < 0 || (comp != null && comp.Stage != RotStage.Dessicated))
			{
				this.vanishAfterTimestamp = this.Age + 3000000;
			}
			if (this.ShouldVanish)
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}
		public override void Ingested(Pawn ingester, float nutritionWanted)
		{
			BodyPartRecord bestBodyPartToEat = this.GetBestBodyPartToEat(ingester, nutritionWanted);
			if (bestBodyPartToEat != null)
			{
				float bodyPartNutrition = FoodUtility.GetBodyPartNutrition(this.innerPawn, bestBodyPartToEat);
				ingester.needs.food.CurLevel += bodyPartNutrition;
				ingester.records.AddTo(RecordDefOf.NutritionEaten, bodyPartNutrition);
				if (ingester.needs.mood != null)
				{
					bool flag = ingester.story.traits.HasTrait(TraitDefOf.Cannibal);
					if (!flag)
					{
						ingester.needs.mood.thoughts.TryGainThought(ThoughtDefOf.AteCorpse);
					}
					if (ingester.RaceProps.Humanlike && this.innerPawn.RaceProps.Humanlike)
					{
						if (!flag)
						{
							ingester.needs.mood.thoughts.TryGainThought(ThoughtDefOf.AteHumanlikeMeatDirect);
						}
						else
						{
							ingester.needs.mood.thoughts.TryGainThought(ThoughtDefOf.AteHumanlikeMeatDirectCannibal);
						}
					}
				}
				float num = 0f;
				if (!ingester.RaceProps.Animal)
				{
					num = 0.04f;
				}
				if (Rand.Value < num)
				{
					ingester.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.FoodPoisoning, ingester, null), null, null);
				}
			}
			if (bestBodyPartToEat == this.innerPawn.RaceProps.body.corePart || bestBodyPartToEat == null)
			{
				if (PawnUtility.ShouldSendNotificationAbout(this.innerPawn))
				{
					Messages.Message("MessageEatenByPredator".Translate(new object[]
					{
						this.innerPawn.LabelBaseShort,
						ingester.LabelIndefinite()
					}).CapitalizeFirst(), ingester, MessageSound.Negative);
				}
				this.Destroy(DestroyMode.Vanish);
			}
			else
			{
				Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, this.innerPawn, bestBodyPartToEat);
				hediff_MissingPart.lastInjury = HediffDefOf.Bite;
				hediff_MissingPart.IsFresh = true;
				this.innerPawn.health.AddHediff(hediff_MissingPart, null, null);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Thing> ButcherProducts(Pawn butcher, float efficiency)
		{
			Corpse.<ButcherProducts>c__Iterator18A <ButcherProducts>c__Iterator18A = new Corpse.<ButcherProducts>c__Iterator18A();
			<ButcherProducts>c__Iterator18A.butcher = butcher;
			<ButcherProducts>c__Iterator18A.efficiency = efficiency;
			<ButcherProducts>c__Iterator18A.<$>butcher = butcher;
			<ButcherProducts>c__Iterator18A.<$>efficiency = efficiency;
			<ButcherProducts>c__Iterator18A.<>f__this = this;
			Corpse.<ButcherProducts>c__Iterator18A expr_2A = <ButcherProducts>c__Iterator18A;
			expr_2A.$PC = -2;
			return expr_2A;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.timeOfDeath, "timeOfDeath", 0, false);
			Scribe_Values.LookValue<int>(ref this.vanishAfterTimestamp, "vanishAfterTimestamp", 0, false);
			Scribe_Deep.LookDeep<BillStack>(ref this.operationsBillStack, "operationsBillStack", new object[]
			{
				this
			});
			Scribe_Deep.LookDeep<Pawn>(ref this.innerPawn, true, "innerPawn", new object[0]);
		}
		public void Strip()
		{
			if (this.innerPawn.equipment != null)
			{
				this.innerPawn.equipment.DropAllEquipment(base.Position, false);
			}
			if (this.innerPawn.apparel != null)
			{
				this.innerPawn.apparel.DropAll(base.Position, false);
			}
			if (this.innerPawn.inventory != null)
			{
				this.innerPawn.inventory.DropAllNearPawn(base.Position, false);
			}
		}
		public override void DrawAt(Vector3 drawLoc)
		{
			Building building = this.StoringBuilding();
			if (building != null && building.def == ThingDefOf.Grave)
			{
				return;
			}
			RotDrawMode bodyDrawType = RotDrawMode.Fresh;
			CompRottable comp = base.GetComp<CompRottable>();
			if (comp != null)
			{
				if (comp.Stage == RotStage.Rotting)
				{
					bodyDrawType = RotDrawMode.Rotting;
				}
				else
				{
					if (comp.Stage == RotStage.Dessicated)
					{
						bodyDrawType = RotDrawMode.Dessicated;
					}
				}
			}
			this.innerPawn.Drawer.renderer.RenderPawnAt(drawLoc, bodyDrawType);
		}
		public Thought GiveObservedThought()
		{
			if (!this.innerPawn.RaceProps.Humanlike)
			{
				return null;
			}
			if (this.StoringBuilding() == null)
			{
				bool flag = false;
				CompRottable comp = base.GetComp<CompRottable>();
				if (comp != null && comp.Stage != RotStage.Fresh)
				{
					flag = true;
				}
				Thought_Observation thought_Observation;
				if (flag)
				{
					thought_Observation = (Thought_Observation)ThoughtMaker.MakeThought(ThoughtDefOf.ObservedLayingRottingCorpse);
				}
				else
				{
					thought_Observation = (Thought_Observation)ThoughtMaker.MakeThought(ThoughtDefOf.ObservedLayingCorpse);
				}
				thought_Observation.Target = this;
				return thought_Observation;
			}
			return null;
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.innerPawn.Faction != null)
			{
				stringBuilder.AppendLine("Faction".Translate() + ": " + this.innerPawn.Faction);
			}
			stringBuilder.AppendLine("DeadTime".Translate(new object[]
			{
				this.Age.TicksToPeriodString(false)
			}));
			float num = 1f - this.innerPawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(this.innerPawn.RaceProps.body.corePart);
			if (num != 0f)
			{
				stringBuilder.AppendLine("CorpsePercentMissing".Translate() + ": " + num.ToStringPercent());
			}
			stringBuilder.AppendLine(base.GetInspectString());
			return stringBuilder.ToString();
		}
		private BodyPartRecord GetBestBodyPartToEat(Pawn ingester, float nutritionWanted)
		{
			IEnumerable<BodyPartRecord> source = 
				from x in this.innerPawn.health.hediffSet.GetNotMissingParts(null, null)
				where x.depth == BodyPartDepth.Outside && FoodUtility.GetBodyPartNutrition(this.innerPawn, x) > 0.001f
				select x;
			if (!source.Any<BodyPartRecord>())
			{
				return null;
			}
			return source.MinBy((BodyPartRecord x) => Mathf.Abs(FoodUtility.GetBodyPartNutrition(this.innerPawn, x) - nutritionWanted));
		}
	}
}
