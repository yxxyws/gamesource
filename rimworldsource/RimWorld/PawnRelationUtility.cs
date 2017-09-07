using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public static class PawnRelationUtility
	{
		[DebuggerHidden]
		public static IEnumerable<PawnRelationDef> GetRelations(this Pawn me, Pawn other)
		{
			PawnRelationUtility.<GetRelations>c__Iterator97 <GetRelations>c__Iterator = new PawnRelationUtility.<GetRelations>c__Iterator97();
			<GetRelations>c__Iterator.me = me;
			<GetRelations>c__Iterator.other = other;
			<GetRelations>c__Iterator.<$>me = me;
			<GetRelations>c__Iterator.<$>other = other;
			PawnRelationUtility.<GetRelations>c__Iterator97 expr_23 = <GetRelations>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}
		public static PawnRelationDef GetMostImportantRelation(this Pawn me, Pawn other)
		{
			PawnRelationDef pawnRelationDef = null;
			foreach (PawnRelationDef current in me.GetRelations(other))
			{
				if (pawnRelationDef == null || current.importance > pawnRelationDef.importance)
				{
					pawnRelationDef = current;
				}
			}
			return pawnRelationDef;
		}
		public static void Notify_PawnsSeenByPlayer(IEnumerable<Pawn> seenPawns, string letterHeader, bool informEvenIfSeenBefore = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(letterHeader);
			IEnumerable<Pawn> enumerable = 
				from x in seenPawns
				where x.RaceProps.IsFlesh
				select x;
			IEnumerable<Pawn> enumerable2 = 
				from x in Find.MapPawns.FreeColonistsAndPrisoners
				where x.relations.everSeenByPlayer
				select x;
			if (!informEvenIfSeenBefore)
			{
				enumerable = 
					from x in enumerable
					where !x.relations.everSeenByPlayer
					select x;
			}
			bool flag = false;
			Pawn pawn = null;
			foreach (Pawn current in enumerable)
			{
				bool flag2 = false;
				foreach (Pawn current2 in enumerable2)
				{
					if (current != current2)
					{
						PawnRelationDef mostImportantRelation = current.GetMostImportantRelation(current2);
						if (mostImportantRelation != null)
						{
							if (!flag2)
							{
								flag2 = true;
								stringBuilder.AppendLine();
								stringBuilder.AppendLine(current.KindLabel.CapitalizeFirst() + " " + current.LabelBaseShort + ":");
							}
							if (current.Spawned)
							{
								pawn = current;
							}
							flag = true;
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"  ",
								mostImportantRelation.GetGenderSpecificLabelCap(current2),
								" - ",
								current2.KindLabel,
								" ",
								current2.LabelBaseShort
							}));
							current.relations.everSeenByPlayer = true;
						}
					}
				}
			}
			if (flag)
			{
				if (pawn != null)
				{
					Find.LetterStack.ReceiveLetter("LetterLabelNoticedFamilyMembers".Translate(), stringBuilder.ToString().TrimEndNewlines(), LetterType.Good, pawn, null);
				}
				else
				{
					Find.LetterStack.ReceiveLetter("LetterLabelNoticedFamilyMembers".Translate(), stringBuilder.ToString().TrimEndNewlines(), LetterType.Good, null);
				}
			}
		}
		public static bool TryAppendRelationsWithColonistsInfo(ref string text, Pawn pawn)
		{
			string text2 = null;
			return PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref text2, pawn);
		}
		public static bool TryAppendRelationsWithColonistsInfo(ref string text, ref string title, Pawn pawn)
		{
			Pawn mostImportantColonyRelative = PawnRelationUtility.GetMostImportantColonyRelative(pawn);
			if (mostImportantColonyRelative == null)
			{
				return false;
			}
			if (title != null)
			{
				title = title + " " + "FamilyLetterAppendedSuffix".Translate();
			}
			string genderSpecificLabel = mostImportantColonyRelative.GetMostImportantRelation(pawn).GetGenderSpecificLabel(pawn);
			string text2 = "\n\n";
			if (mostImportantColonyRelative.IsColonist)
			{
				text2 += "FamilyLetterAppendedTextColonist".Translate(new object[]
				{
					mostImportantColonyRelative.LabelBaseShort,
					genderSpecificLabel
				});
			}
			else
			{
				text2 += "FamilyLetterAppendedTextPrisoner".Translate(new object[]
				{
					mostImportantColonyRelative.LabelBaseShort,
					genderSpecificLabel
				});
			}
			text += text2.AdjustedFor(pawn);
			return true;
		}
		public static Pawn GetMostImportantColonyRelative(Pawn pawn)
		{
			IEnumerable<Pawn> enumerable = 
				from x in Find.MapPawns.FreeColonistsAndPrisoners
				where x.relations.everSeenByPlayer
				select x;
			float num = 0f;
			Pawn pawn2 = null;
			foreach (Pawn current in enumerable)
			{
				PawnRelationDef mostImportantRelation = pawn.GetMostImportantRelation(current);
				if (mostImportantRelation != null)
				{
					if (pawn2 == null || mostImportantRelation.importance > num)
					{
						num = mostImportantRelation.importance;
						pawn2 = current;
					}
				}
			}
			return pawn2;
		}
	}
}
