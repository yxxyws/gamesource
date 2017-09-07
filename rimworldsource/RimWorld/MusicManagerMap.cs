using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public sealed class MusicManagerMap
	{
		private enum MusicManagerState
		{
			Normal,
			Fadeout
		}
		private const float FadeoutDuration = 10f;
		private AudioSource audioSource;
		private MusicManagerMap.MusicManagerState state;
		private float fadeoutFactor = 1f;
		private float nextSongStartTime = 12f;
		private SongDef lastStartedSong;
		private Queue<SongDef> recentSongs = new Queue<SongDef>();
		public bool disabled;
		private SongDef forcedNextSong;
		private bool songWasForced;
		private bool ignorePrefsVolumeThisSong;
		public float subtleAmbienceSoundVolumeMultiplier = 1f;
		private static readonly FloatRange SongIntervalRelax = new FloatRange(85f, 105f);
		private static readonly FloatRange SongIntervalTension = new FloatRange(2f, 5f);
		private float CurTime
		{
			get
			{
				return Time.time;
			}
		}
		private bool DangerMusicMode
		{
			get
			{
				return Find.StoryWatcher.watcherDanger.DangerRating == StoryDanger.High;
			}
		}
		public float CurVolume
		{
			get
			{
				float num = (!this.ignorePrefsVolumeThisSong) ? Prefs.VolumeMusic : 1f;
				if (this.lastStartedSong == null)
				{
					return num;
				}
				return this.lastStartedSong.volume * num * this.fadeoutFactor;
			}
		}
		public bool IsPlaying
		{
			get
			{
				return this.audioSource.isPlaying;
			}
		}
		public string DebugString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("MusicManagerMap");
				stringBuilder.AppendLine("state: " + this.state);
				stringBuilder.AppendLine("lastStartedSong: " + this.lastStartedSong);
				stringBuilder.AppendLine("fadeoutFactor: " + this.fadeoutFactor);
				stringBuilder.AppendLine("nextSongStartTime: " + this.nextSongStartTime);
				stringBuilder.AppendLine("CurTime: " + this.CurTime);
				stringBuilder.AppendLine("recentSongs: " + GenText.ToCommaList(
					from s in this.recentSongs
					select s.defName));
				stringBuilder.AppendLine("disabled: " + this.disabled);
				return stringBuilder.ToString();
			}
		}
		public MusicManagerMap()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.audioSource = new GameObject("MusicAudioSourceDummy")
				{
					transform = 
					{
						parent = Find.RootRoot.soundRoot.sourcePool.sourcePoolCamera.cameraSourcesContainer.transform
					}
				}.AddComponent<AudioSource>();
				this.audioSource.bypassEffects = true;
				this.audioSource.bypassListenerEffects = true;
				this.audioSource.bypassReverbZones = true;
				this.audioSource.priority = 0;
			});
		}
		public void ForceSilenceFor(float time)
		{
			this.nextSongStartTime = this.CurTime + time;
		}
		public void MusicUpdate()
		{
			this.UpdateSubtleAmbienceSoundVolumeMultiplier();
			if (this.disabled)
			{
				return;
			}
			if (this.songWasForced)
			{
				this.state = MusicManagerMap.MusicManagerState.Normal;
				this.fadeoutFactor = 1f;
			}
			if (this.audioSource.isPlaying && !this.songWasForced && ((this.DangerMusicMode && !this.lastStartedSong.tense) || (!this.DangerMusicMode && this.lastStartedSong.tense)))
			{
				this.state = MusicManagerMap.MusicManagerState.Fadeout;
			}
			this.audioSource.volume = this.CurVolume;
			if (this.audioSource.isPlaying)
			{
				if (this.state == MusicManagerMap.MusicManagerState.Fadeout)
				{
					this.fadeoutFactor -= Time.deltaTime / 10f;
					if (this.fadeoutFactor <= 0f)
					{
						this.audioSource.Stop();
						this.ignorePrefsVolumeThisSong = false;
						this.state = MusicManagerMap.MusicManagerState.Normal;
						this.fadeoutFactor = 1f;
					}
				}
			}
			else
			{
				if (this.DangerMusicMode && this.nextSongStartTime > this.CurTime + MusicManagerMap.SongIntervalTension.max)
				{
					this.nextSongStartTime = this.CurTime + MusicManagerMap.SongIntervalTension.RandomInRange;
				}
				if (this.nextSongStartTime < this.CurTime - 5f)
				{
					float randomInRange;
					if (this.DangerMusicMode)
					{
						randomInRange = MusicManagerMap.SongIntervalTension.RandomInRange;
					}
					else
					{
						randomInRange = MusicManagerMap.SongIntervalRelax.RandomInRange;
					}
					this.nextSongStartTime = this.CurTime + randomInRange;
				}
				if (this.CurTime >= this.nextSongStartTime)
				{
					this.StartNewSong();
				}
			}
		}
		private void UpdateSubtleAmbienceSoundVolumeMultiplier()
		{
			if (this.IsPlaying && this.CurVolume > 0.001f)
			{
				this.subtleAmbienceSoundVolumeMultiplier -= Time.deltaTime * 0.1f;
			}
			else
			{
				this.subtleAmbienceSoundVolumeMultiplier += Time.deltaTime * 0.1f;
			}
			this.subtleAmbienceSoundVolumeMultiplier = Mathf.Clamp01(this.subtleAmbienceSoundVolumeMultiplier);
		}
		private void StartNewSong()
		{
			this.lastStartedSong = this.ChooseNextSong();
			this.audioSource.clip = this.lastStartedSong.clip;
			this.audioSource.volume = this.CurVolume;
			this.audioSource.Play();
			this.recentSongs.Enqueue(this.lastStartedSong);
		}
		public void ForceStartSong(SongDef song, bool ignorePrefsVolume)
		{
			this.forcedNextSong = song;
			this.ignorePrefsVolumeThisSong = ignorePrefsVolume;
			this.StartNewSong();
		}
		private SongDef ChooseNextSong()
		{
			this.songWasForced = false;
			if (this.forcedNextSong != null)
			{
				SongDef result = this.forcedNextSong;
				this.forcedNextSong = null;
				this.songWasForced = true;
				return result;
			}
			IEnumerable<SongDef> source = 
				from song in DefDatabase<SongDef>.AllDefs
				where this.AppropriateNow(song)
				select song;
			while (this.recentSongs.Count > 7)
			{
				this.recentSongs.Dequeue();
			}
			while (!source.Any<SongDef>() && this.recentSongs.Count > 0)
			{
				this.recentSongs.Dequeue();
			}
			if (!source.Any<SongDef>())
			{
				Log.Error("Could not get any appropriate song. Getting random and logging song selection data.");
				this.LogSongSelectionData();
				return DefDatabase<SongDef>.GetRandom();
			}
			return source.RandomElementByWeight((SongDef s) => s.commonality);
		}
		private bool AppropriateNow(SongDef song)
		{
			if (!song.playOnMap)
			{
				return false;
			}
			if (this.DangerMusicMode)
			{
				if (!song.tense)
				{
					return false;
				}
			}
			else
			{
				if (song.tense)
				{
					return false;
				}
			}
			if (song.allowedTimeOfDay == TimeOfDay.Any)
			{
				return (song.allowedSeasons.NullOrEmpty<Season>() || song.allowedSeasons.Contains(GenDate.CurrentSeason)) && !this.recentSongs.Contains(song);
			}
			if (song.allowedTimeOfDay == TimeOfDay.Night)
			{
				return GenDate.CurrentDayPercent < 0.2f || GenDate.CurrentDayPercent > 0.7f;
			}
			return GenDate.CurrentDayPercent > 0.2f && GenDate.CurrentDayPercent < 0.7f;
		}
		public void LogSongSelectionData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Most recent song: " + ((this.lastStartedSong == null) ? "None" : this.lastStartedSong.defName));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Songs appropriate to play now:");
			foreach (SongDef current in 
				from s in DefDatabase<SongDef>.AllDefs
				where this.AppropriateNow(s)
				select s)
			{
				stringBuilder.AppendLine("   " + current.defName);
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Recently played songs:");
			foreach (SongDef current2 in this.recentSongs)
			{
				stringBuilder.AppendLine("   " + current2.defName);
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
