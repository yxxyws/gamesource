using RimWorld;
using System;
using UnityEngine;
namespace Verse
{
	public static class VerbUtility
	{
		public static void DrawTargetingGUI_Update(this Verb verb)
		{
			verb.DrawHittableSquares();
			foreach (TargetInfo current in GenUI.TargetsAtMouse(verb.verbProps.targetParams, false))
			{
				if (verb.CanHitTarget(current) || verb.verbProps.MeleeRange)
				{
					GenDraw.DrawTargetHighlight(current);
					ShootLine shootLine;
					if (verb.HighlightFieldRadiusAroundTarget() > 0.2f && verb.TryFindShootLineFromTo(verb.caster.Position, current, out shootLine))
					{
						GenExplosion.RenderPredictedAreaOfEffect(shootLine.Dest, verb.HighlightFieldRadiusAroundTarget());
					}
				}
			}
		}
		public static void DrawTargetingGUI_OnGUI(this Verb verb)
		{
			Vector3 vector = Event.current.mousePosition;
			Texture2D image;
			if (verb.verbProps.MeleeRange || verb.CanHitTarget(Gen.MouseCell()) || !Find.Targeter.targetingVerbAdditionalPawns.NullOrEmpty<Pawn>())
			{
				if (verb.UIIcon != BaseContent.BadTex)
				{
					image = verb.UIIcon;
				}
				else
				{
					image = TexCommand.Attack;
				}
			}
			else
			{
				image = TexCommand.CannotShoot;
			}
			GUI.DrawTexture(new Rect(vector.x + 8f, vector.y + 8f, 32f, 32f), image);
		}
		public static void DrawHittableSquares(this Verb verb)
		{
			if (!verb.verbProps.MeleeRange)
			{
				if (verb.verbProps.minRange < GenRadial.MaxRadialPatternRadius)
				{
					GenDraw.DrawRadiusRing(verb.caster.Position, verb.verbProps.minRange);
				}
				if (verb.verbProps.range < GenRadial.MaxRadialPatternRadius)
				{
					GenDraw.DrawRadiusRing(verb.caster.Position, verb.verbProps.range);
				}
			}
		}
	}
}
