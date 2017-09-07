using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.Sound;
namespace Verse
{
	public class SoundDef : Def
	{
		[DefaultValue(false), Description("If checked, this sound is a sustainer.\n\nSustainers are used for sounds with a defined beginning and end (as opposed to OneShots, which just fire at a given instant).\n\nThis value must match what the game expects from the SubSoundDef with this name.")]
		public bool sustain;
		[Description("Event names for this sound. \n\nThe code will look up sounds to play them according to their name. If the code finds the event name it wants in this list, it will trigger this sound.\n\nThe Def name is also used as an event name.")]
		public List<string> eventNames = new List<string>();
		[DefaultValue(4), Description("For one-shots, this is the number of individual sounds from this Def than can be playing at a time.\n\n For sustainers, this is the number of sustainers that can be running with this sound (each of which can have sub-sounds). Sustainers can fade in and out as you move the camera or objects move, to keep the nearest ones audible.\n\nThis setting may not work for on-camera sounds.")]
		public int maxVoices = 4;
		[DefaultValue(3), Description("The number of instances of this sound that can play at almost exactly the same moment. Handles cases like six gunners all firing their identical guns at the same time because a target came into view of all of them at the same time. Ordinarily this would make a painfully loud sound, but you can reduce it with this.")]
		public int maxSimultaneous = 3;
		[DefaultValue(VoicePriorityMode.PrioritizeNewest), Description("If the system has to not play some instances of this sound because of maxVoices, this determines which ones are ignored.\n\nYou should use PrioritizeNewest for things like gunshots, so older still-playing samples are overridden by newer, more important ones.\n\nSustained sounds should usually prioritize nearest, so if a new fire starts burning nearby it can override a more distant one.")]
		public VoicePriorityMode priorityMode;
		[DefaultValue(""), Description("The special sound slot this sound takes. If a sound with this slot is playing, new sounds in this slot will not play.\n\nOnly works for on-camera sounds.")]
		public string slot = string.Empty;
		[DefaultValue(""), Description("The name of the SoundDef that will be played when this sustainer starts."), LoadAlias("sustainerStartSound")]
		public string sustainStartSound = string.Empty;
		[DefaultValue(""), Description("The name of the SoundDef that will be played when this sustainer ends."), LoadAlias("sustainerStopSound")]
		public string sustainStopSound = string.Empty;
		[DefaultValue(0f), Description("After a sustainer is ended, the sound will fade out over this many real-time seconds.")]
		public float sustainFadeoutTime;
		[Description("All the sounds that will play when this set is triggered.")]
		public List<SubSoundDef> subSounds = new List<SubSoundDef>();
		[Unsaved]
		public bool isUndefined;
		[Unsaved]
		public Sustainer testSustainer;
		private static Dictionary<string, SoundDef> undefinedSoundDefs = new Dictionary<string, SoundDef>();
		private bool HasSubSoundsOnCamera
		{
			get
			{
				for (int i = 0; i < this.subSounds.Count; i++)
				{
					if (this.subSounds[i].onCamera)
					{
						return true;
					}
				}
				return false;
			}
		}
		public bool HasSubSoundsInWorld
		{
			get
			{
				for (int i = 0; i < this.subSounds.Count; i++)
				{
					if (!this.subSounds[i].onCamera)
					{
						return true;
					}
				}
				return false;
			}
		}
		public int MaxSimultaneousSamples
		{
			get
			{
				return this.maxSimultaneous * this.subSounds.Count;
			}
		}
		public override void ResolveReferences()
		{
			for (int i = 0; i < this.subSounds.Count; i++)
			{
				this.subSounds[i].parentDef = this;
				this.subSounds[i].ResolveReferences();
			}
		}
		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			SoundDef.<ConfigErrors>c__Iterator148 <ConfigErrors>c__Iterator = new SoundDef.<ConfigErrors>c__Iterator148();
			<ConfigErrors>c__Iterator.<>f__this = this;
			SoundDef.<ConfigErrors>c__Iterator148 expr_0E = <ConfigErrors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public void DoEditWidgets(WidgetRow widgetRow)
		{
			if (this.testSustainer == null)
			{
				if (widgetRow.DoIconButton(TexButton.Play, null))
				{
					this.ResolveReferences();
					SoundInfo info;
					if (this.HasSubSoundsInWorld)
					{
						IntVec3 mapPosition = Find.CameraMap.MapPosition;
						info = SoundInfo.InWorld(mapPosition, MaintenanceType.PerFrame);
						for (int i = 0; i < 5; i++)
						{
							MoteThrower.ThrowDustPuff(mapPosition, 1.5f);
						}
					}
					else
					{
						info = SoundInfo.OnCamera(MaintenanceType.PerFrame);
					}
					info.testPlay = true;
					if (this.sustain)
					{
						this.testSustainer = this.TrySpawnSustainer(info);
					}
					else
					{
						this.PlayOneShot(info);
					}
				}
			}
			else
			{
				this.testSustainer.Maintain();
				if (widgetRow.DoIconButton(TexButton.Stop, null))
				{
					this.testSustainer.End();
					this.testSustainer = null;
				}
			}
		}
		public static SoundDef Named(string defName)
		{
			SoundDef namedSilentFail = DefDatabase<SoundDef>.GetNamedSilentFail(defName);
			if (namedSilentFail != null)
			{
				return namedSilentFail;
			}
			if (!Prefs.DevMode && SoundDef.undefinedSoundDefs.ContainsKey(defName))
			{
				return SoundDef.UndefinedDefNamed(defName);
			}
			List<SoundDef> allDefsListForReading = DefDatabase<SoundDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				if (allDefsListForReading[i].eventNames.Count > 0)
				{
					for (int j = 0; j < allDefsListForReading[i].eventNames.Count; j++)
					{
						if (allDefsListForReading[i].eventNames[j] == defName)
						{
							return allDefsListForReading[i];
						}
					}
				}
			}
			if (DefDatabase<SoundDef>.DefCount == 0)
			{
				Log.Warning("Tried to get SoundDef named " + defName + ", but sound defs aren't loaded yet (is it a static variable initialized before play data?).");
			}
			return SoundDef.UndefinedDefNamed(defName);
		}
		private static SoundDef UndefinedDefNamed(string defName)
		{
			SoundDef soundDef;
			if (!SoundDef.undefinedSoundDefs.TryGetValue(defName, out soundDef))
			{
				soundDef = new SoundDef();
				soundDef.isUndefined = true;
				soundDef.defName = defName;
				SoundDef.undefinedSoundDefs.Add(defName, soundDef);
			}
			return soundDef;
		}
	}
}
