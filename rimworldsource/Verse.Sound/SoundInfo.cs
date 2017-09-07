using System;
using System.Collections.Generic;
namespace Verse.Sound
{
	public struct SoundInfo
	{
		private Dictionary<string, float> parameters;
		public float volumeFactor;
		public float pitchFactor;
		public bool testPlay;
		public bool IsOnCamera
		{
			get;
			private set;
		}
		public TargetInfo Maker
		{
			get;
			private set;
		}
		public MaintenanceType Maintenance
		{
			get;
			private set;
		}
		public IEnumerable<KeyValuePair<string, float>> DefinedParameters
		{
			get
			{
				SoundInfo.<>c__Iterator187 <>c__Iterator = new SoundInfo.<>c__Iterator187();
				<>c__Iterator.<>f__this = this;
				SoundInfo.<>c__Iterator187 expr_13 = <>c__Iterator;
				expr_13.$PC = -2;
				return expr_13;
			}
		}
		public static SoundInfo OnCamera(MaintenanceType maint = MaintenanceType.None)
		{
			SoundInfo result = default(SoundInfo);
			result.IsOnCamera = true;
			result.Maintenance = maint;
			result.Maker = TargetInfo.Invalid;
			result.testPlay = false;
			result.volumeFactor = (result.pitchFactor = 1f);
			return result;
		}
		public static SoundInfo InWorld(TargetInfo maker, MaintenanceType maint = MaintenanceType.None)
		{
			SoundInfo result = default(SoundInfo);
			result.IsOnCamera = false;
			result.Maintenance = maint;
			result.Maker = maker;
			result.testPlay = false;
			result.volumeFactor = (result.pitchFactor = 1f);
			return result;
		}
		public void SetParameter(string key, float value)
		{
			if (this.parameters == null)
			{
				this.parameters = new Dictionary<string, float>();
			}
			this.parameters[key] = value;
		}
		public override string ToString()
		{
			string text = null;
			if (this.parameters != null && this.parameters.Count > 0)
			{
				text = "parameters=";
				foreach (KeyValuePair<string, float> current in this.parameters)
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						current.Key.ToString(),
						"-",
						current.Value.ToString(),
						" "
					});
				}
			}
			string text3 = null;
			if (this.Maker.HasThing || this.Maker.Cell.IsValid)
			{
				text3 = this.Maker.ToString();
			}
			string text4 = null;
			if (this.Maintenance != MaintenanceType.None)
			{
				text4 = ", Maint=" + this.Maintenance;
			}
			return string.Concat(new string[]
			{
				"(",
				(!this.IsOnCamera) ? "World from " : "Camera",
				text3,
				text,
				text4,
				")"
			});
		}
		public static implicit operator SoundInfo(IntVec3 sourceLoc)
		{
			return SoundInfo.InWorld(sourceLoc, MaintenanceType.None);
		}
		public static implicit operator SoundInfo(Thing sourceThing)
		{
			return SoundInfo.InWorld(sourceThing, MaintenanceType.None);
		}
	}
}
