using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class Page_Credits : Window
	{
		private const int ColumnWidth = 800;
		private const float AutoScrollDelay = 6f;
		private const float AutoScrollRate = 20f;
		private const float SongStartDelay = 5f;
		private List<CreditsEntry> creds;
		public bool wonGame;
		private float timeUntilAutoScroll = 6f;
		private float scrollPosition;
		private bool playedMusic;
		public float creationRealtime = -1f;
		public override Vector2 InitialWindowSize
		{
			get
			{
				return new Vector2((float)Screen.width, (float)Screen.height);
			}
		}
		protected override float WindowPadding
		{
			get
			{
				return 0f;
			}
		}
		public Page_Credits() : this(string.Empty)
		{
		}
		public Page_Credits(string preCreditsMessage)
		{
			this.doWindowBackground = false;
			this.doCloseButton = false;
			this.doCloseX = false;
			this.closeOnEscapeKey = true;
			this.forcePause = true;
			this.creds = CreditsAssembler.AllCredits().ToList<CreditsEntry>();
			this.creds.Insert(0, new CreditRecord_Space(100f));
			if (!preCreditsMessage.NullOrEmpty())
			{
				this.creds.Insert(1, new CreditRecord_Space(200f));
				this.creds.Insert(2, new CreditRecord_Text(preCreditsMessage, TextAnchor.UpperLeft));
				this.creds.Insert(3, new CreditRecord_Space(50f));
				this.creds.Add(new CreditRecord_Space(300f));
				this.creds.Add(new CreditRecord_Text("ThanksForPlaying".Translate(), TextAnchor.UpperCenter));
			}
		}
		public override void PreOpen()
		{
			base.PreOpen();
			this.creationRealtime = Time.realtimeSinceStartup;
		}
		public override void WindowUpdate()
		{
			base.WindowUpdate();
			if (this.timeUntilAutoScroll > 0f)
			{
				this.timeUntilAutoScroll -= Time.deltaTime;
			}
			else
			{
				this.scrollPosition += 20f * Time.deltaTime;
			}
			if (this.wonGame && !this.playedMusic && Time.realtimeSinceStartup > this.creationRealtime + 5f)
			{
				SongDef named = DefDatabase<SongDef>.GetNamed("EndCreditsSong", true);
				Find.MusicManagerMap.ForceStartSong(named, true);
				this.playedMusic = true;
			}
		}
		public override void DoWindowContents(Rect inRect)
		{
			Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
			GUI.DrawTexture(rect, BaseContent.BlackTex);
			Rect position = new Rect(rect);
			position.yMin += 30f;
			position.yMax -= 30f;
			position.xMin = rect.center.x - 400f;
			position.width = 800f;
			float viewWidth = position.width;
			float num = this.creds.Sum((CreditsEntry c) => c.DrawHeight(viewWidth)) + 200f;
			this.scrollPosition = Mathf.Clamp(this.scrollPosition, 0f, num - 400f);
			GUI.BeginGroup(position);
			Rect position2 = new Rect(0f, 0f, viewWidth, num);
			position2.y -= this.scrollPosition;
			GUI.BeginGroup(position2);
			Text.Font = GameFont.Medium;
			float num2 = 0f;
			foreach (CreditsEntry current in this.creds)
			{
				float num3 = current.DrawHeight(position2.width);
				Rect rect2 = new Rect(0f, num2, position2.width, num3);
				current.Draw(rect2);
				num2 += num3;
			}
			GUI.EndGroup();
			GUI.EndGroup();
			if (Event.current.type == EventType.ScrollWheel)
			{
				this.Scroll(Event.current.delta.y * 25f);
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					this.Scroll(250f);
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.UpArrow)
				{
					this.Scroll(-250f);
					Event.current.Use();
				}
			}
		}
		private void Scroll(float offset)
		{
			this.scrollPosition += offset;
			this.timeUntilAutoScroll = 3f;
		}
	}
}
