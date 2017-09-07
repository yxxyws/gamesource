using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Designator_Tame : Designator
	{
		private List<Pawn> justDesignated = new List<Pawn>();
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}
		public Designator_Tame()
		{
			this.defaultLabel = "DesignatorTame".Translate();
			this.defaultDesc = "DesignatorTameDesc".Translate();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Tame", true);
			});
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.DesignateClaim;
			this.hotKey = KeyBindingDefOf.Misc4;
		}
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds())
			{
				return false;
			}
			if (!this.TameablesInCell(c).Any<Pawn>())
			{
				return "MessageMustDesignateTameable".Translate();
			}
			return true;
		}
		public override void DesignateSingleCell(IntVec3 loc)
		{
			foreach (Pawn current in this.TameablesInCell(loc))
			{
				this.DesignateThing(current);
			}
		}
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null && pawn.def.race.Animal && pawn.Faction == null && pawn.RaceProps.wildness < 1f && !pawn.HostileTo(t) && Find.DesignationManager.DesignationOn(pawn, DesignationDefOf.Tame) == null)
			{
				return true;
			}
			return false;
		}
		protected override void FinalizeDesignationSucceeded()
		{
			base.FinalizeDesignationSucceeded();
			foreach (PawnKindDef kind in (
				from p in this.justDesignated
				select p.kindDef).Distinct<PawnKindDef>())
			{
				if (kind.RaceProps.manhunterOnTameFailChance > 0f)
				{
					Messages.Message("MessageAnimalManhuntsOnTameFailed".Translate(new object[]
					{
						kind.label,
						kind.RaceProps.manhunterOnTameFailChance.ToStringPercent("F2")
					}), this.justDesignated.First((Pawn x) => x.kindDef == kind), MessageSound.Standard);
				}
			}
			IEnumerable<Pawn> source = 
				from c in Find.MapPawns.FreeColonistsSpawned
				where c.workSettings.WorkIsActive(WorkTypeDefOf.Handling)
				select c;
			if (!source.Any<Pawn>())
			{
				source = Find.MapPawns.FreeColonistsSpawned;
			}
			if (source.Any<Pawn>())
			{
				Pawn pawn = source.MaxBy((Pawn c) => c.skills.GetSkill(SkillDefOf.Animals).level);
				int level = pawn.skills.GetSkill(SkillDefOf.Animals).level;
				foreach (ThingDef ad in (
					from t in this.justDesignated
					select t.def).Distinct<ThingDef>())
				{
					int num = Mathf.RoundToInt(ad.GetStatValueAbstract(StatDefOf.MinimumHandlingSkill, null));
					if (num > level)
					{
						Messages.Message("MessageNoHandlerSkilledEnough".Translate(new object[]
						{
							ad.label,
							num.ToStringCached(),
							SkillDefOf.Animals.LabelCap,
							pawn.LabelBaseShort,
							level
						}), this.justDesignated.First((Pawn x) => x.def == ad), MessageSound.Negative);
					}
				}
			}
			this.justDesignated.Clear();
		}
		public override void DesignateThing(Thing t)
		{
			Find.DesignationManager.RemoveAllDesignationsOn(t, false);
			Find.DesignationManager.AddDesignation(new Designation(t, DesignationDefOf.Tame));
			this.justDesignated.Add((Pawn)t);
		}
		[DebuggerHidden]
		private IEnumerable<Pawn> TameablesInCell(IntVec3 c)
		{
			Designator_Tame.<TameablesInCell>c__Iterator111 <TameablesInCell>c__Iterator = new Designator_Tame.<TameablesInCell>c__Iterator111();
			<TameablesInCell>c__Iterator.c = c;
			<TameablesInCell>c__Iterator.<$>c = c;
			<TameablesInCell>c__Iterator.<>f__this = this;
			Designator_Tame.<TameablesInCell>c__Iterator111 expr_1C = <TameablesInCell>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
