using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class StorytellerDef : Def
	{
		public string quotation = "No quotation.";
		public int listOrder = 9999;
		private string portraitLarge;
		private string portraitTiny;
		[Unsaved]
		public Texture2D portraitLargeTex;
		[Unsaved]
		public Texture2D portraitTinyTex;
		public List<Type> incidentMakers;
		public float threatCycleLength = 7f;
		public float minDaysBetweenThreatBigs = 1.5f;
		public float classic_RandomEventMTBDays;
		public float classic_ThreatBigMTBDays;
		public float classic_ThreatSmallMTBDays;
		public float random_AverageIncidentsPerDay;
		public float random_GeneralWeight;
		public float random_ThreatSmallWeight;
		public float random_LargeThreatWeight;
		public float desiredPopulationMin = 3f;
		public float desiredPopulationMax = 10f;
		public float desiredPopulationCritical = 13f;
		public int desiredPopulationGainIntervalMinDays = 3;
		public int desiredPopulationGainIntervalMaxDays = 10;
		public int random_MaxThreatBigIntervalDays = 11;
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.portraitTinyTex = ContentFinder<Texture2D>.Get(this.portraitTiny, true);
				this.portraitLargeTex = ContentFinder<Texture2D>.Get(this.portraitLarge, true);
			});
		}
	}
}
