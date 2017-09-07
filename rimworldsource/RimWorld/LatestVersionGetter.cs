using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class LatestVersionGetter : MonoBehaviour
	{
		public int latestPublicBuild = -1;
		private WWW www;
		private bool Errored
		{
			get
			{
				return this.www.error != null && !(this.www.error == string.Empty);
			}
		}
		[DebuggerHidden]
		private IEnumerator Start()
		{
			LatestVersionGetter.<Start>c__Iterator11D <Start>c__Iterator11D = new LatestVersionGetter.<Start>c__Iterator11D();
			<Start>c__Iterator11D.<>f__this = this;
			return <Start>c__Iterator11D;
		}
		public void DrawAt(Rect rect)
		{
			if (this.Errored)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				Widgets.Label(rect, "ErrorGettingVersionInfo".Translate(new object[]
				{
					this.www.error
				}));
			}
			else
			{
				if (!this.www.isDone)
				{
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
					Widgets.Label(rect, "LoadingVersionInfo".Translate());
				}
				else
				{
					if (this.latestPublicBuild > VersionControl.CurrentBuild)
					{
						GUI.color = Color.yellow;
						Widgets.Label(rect, "BuildNowAvailable".Translate(new object[]
						{
							this.www.text
						}));
					}
					else
					{
						if (this.latestPublicBuild == VersionControl.CurrentBuild)
						{
							GUI.color = new Color(1f, 1f, 1f, 0.5f);
							Widgets.Label(rect, "BuildUpToDate".Translate());
						}
						else
						{
							if (this.latestPublicBuild < VersionControl.CurrentBuild)
							{
								GUI.color = new Color(1f, 1f, 1f, 0.5f);
								Widgets.Label(rect, "BuildUpToDate".Translate());
							}
						}
					}
				}
			}
			GUI.color = Color.white;
		}
	}
}
