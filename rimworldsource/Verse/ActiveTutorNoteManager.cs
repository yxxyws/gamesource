using RimWorld;
using System;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public static class ActiveTutorNoteManager
	{
		public static TutorNote activeNote;
		public static float activeNoteStartRealTime = -1f;
		public static void CloseAll()
		{
			ActiveTutorNoteManager.activeNote = null;
		}
		public static void StartShowing(TutorItem item)
		{
			if (!Prefs.AdaptiveTrainingEnabled)
			{
				return;
			}
			if (!item.Relevant)
			{
				return;
			}
			TutorNote tutorNote = item as TutorNote;
			if (tutorNote != null)
			{
				SoundDefOf.TutorMessageAppear.PlayOneShotOnCamera();
				if (ActiveTutorNoteManager.activeNote != null)
				{
					tutorNote.doFadeIn = false;
				}
				ActiveTutorNoteManager.activeNote = tutorNote;
				ActiveTutorNoteManager.activeNoteStartRealTime = Time.time;
			}
		}
		public static void ActiveLessonManagerOnGUI()
		{
			if (Find.WindowStack.WindowsPreventDrawTutor)
			{
				return;
			}
			if (ActiveTutorNoteManager.activeNote != null)
			{
				if (!ActiveTutorNoteManager.activeNote.Relevant)
				{
					ActiveTutorNoteManager.activeNote = null;
					return;
				}
				ActiveTutorNoteManager.activeNote.TutorItemOnGUI();
			}
		}
	}
}
