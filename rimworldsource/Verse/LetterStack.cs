using System;
using System.Collections.Generic;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public sealed class LetterStack : IExposable
	{
		private const float LettersBottomY = 350f;
		public const float LetterSpacing = 12f;
		private List<Letter> letters = new List<Letter>();
		private float lastTopYInt;
		public float LastTopY
		{
			get
			{
				return this.lastTopYInt;
			}
		}
		public void ReceiveLetter(string label, string text, LetterType type, TargetInfo letterLookTarget, string debugText = null)
		{
			Letter let = new Letter(label, text, type, letterLookTarget);
			this.ReceiveLetter(let, debugText);
		}
		public void ReceiveLetter(string label, string text, LetterType type, string debugText = null)
		{
			Letter let = new Letter(label, text, type, TargetInfo.Invalid);
			this.ReceiveLetter(let, debugText);
		}
		public void ReceiveLetter(Letter let, string debugText = null)
		{
			SoundDef soundDef;
			if (let.LetterType == LetterType.BadUrgent)
			{
				soundDef = SoundDef.Named("LetterArriveBadUrgent");
			}
			else
			{
				soundDef = SoundDef.Named("LetterArrive");
			}
			soundDef.PlayOneShotOnCamera();
			this.letters.Add(let);
			let.arrivalTime = Time.time;
		}
		public void RemoveLetter(Letter let)
		{
			this.letters.Remove(let);
		}
		public void LettersOnGUI(float baseY)
		{
			float num = baseY - 30f;
			for (int i = this.letters.Count - 1; i >= 0; i--)
			{
				this.letters[i].DrawButtonAt(num);
				num -= 42f;
			}
			this.lastTopYInt = num;
			num = baseY - 30f;
			for (int j = this.letters.Count - 1; j >= 0; j--)
			{
				this.letters[j].CheckForMouseOverTextAt(num);
				num -= 42f;
			}
		}
		public void LettersUpdate()
		{
			this.letters.RemoveAll((Letter l) => !l.Valid);
		}
		public void ExposeData()
		{
			Scribe_Collections.LookList<Letter>(ref this.letters, "letters", LookMode.Deep, new object[0]);
		}
	}
}
