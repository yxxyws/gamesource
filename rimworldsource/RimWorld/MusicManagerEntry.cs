using System;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public static class MusicManagerEntry
	{
		private const string SourceGameObjectName = "MusicAudioSourceDummy";
		private static AudioSource audioSource;
		private static SongDef songDef;
		private static float CurVolume
		{
			get
			{
				return Prefs.VolumeMusic * MusicManagerEntry.songDef.volume;
			}
		}
		public static void MusicManagerEntryUpdate()
		{
			if (MusicManagerEntry.audioSource == null || !MusicManagerEntry.audioSource.isPlaying)
			{
				MusicManagerEntry.StartPlaying();
			}
			MusicManagerEntry.audioSource.volume = MusicManagerEntry.CurVolume;
		}
		private static void StartPlaying()
		{
			if (MusicManagerEntry.audioSource != null && !MusicManagerEntry.audioSource.isPlaying)
			{
				MusicManagerEntry.audioSource.Play();
				return;
			}
			if (GameObject.Find("MusicAudioSourceDummy") != null)
			{
				Log.Error("MusicManagerEntry did StartPlaying but there is already a music source GameObject.");
				return;
			}
			MusicManagerEntry.audioSource = new GameObject("MusicAudioSourceDummy")
			{
				transform = 
				{
					parent = Camera.main.transform
				}
			}.AddComponent<AudioSource>();
			MusicManagerEntry.audioSource.bypassEffects = true;
			MusicManagerEntry.audioSource.bypassListenerEffects = true;
			MusicManagerEntry.audioSource.bypassReverbZones = true;
			MusicManagerEntry.audioSource.priority = 0;
			MusicManagerEntry.songDef = DefDatabase<SongDef>.GetNamed("EntrySong", true);
			MusicManagerEntry.audioSource.clip = MusicManagerEntry.songDef.clip;
			MusicManagerEntry.audioSource.volume = MusicManagerEntry.CurVolume;
			MusicManagerEntry.audioSource.loop = true;
			MusicManagerEntry.audioSource.Play();
		}
	}
}
