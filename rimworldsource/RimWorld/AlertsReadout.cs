using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RimWorld
{
	public class AlertsReadout
	{
		private const int StartTickDelay = 600;
		public const float AlertListWidth = 164f;
		private List<Alert> activeAlerts = new List<Alert>(16);
		private int curAlertIndex;
		private float lastFinalY;
		private readonly List<Alert> AllAlerts = new List<Alert>();
		private static int AlertCycleLength = 20;
		private readonly List<AlertPriority> PriosInDrawOrder;
		public AlertsReadout()
		{
			foreach (Type current in typeof(Alert).AllLeafSubclasses())
			{
				if (current != typeof(Alert_Concept))
				{
					this.AllAlerts.Add((Alert)Activator.CreateInstance(current));
				}
			}
			foreach (ConceptDef current2 in 
				from c in DefDatabase<ConceptDef>.AllDefs
				where c.alertDisplay
				select c)
			{
				this.AllAlerts.Add(new Alert_Concept(current2));
			}
			if (this.PriosInDrawOrder == null)
			{
				this.PriosInDrawOrder = new List<AlertPriority>();
				IEnumerator enumerator3 = Enum.GetValues(typeof(AlertPriority)).GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						AlertPriority item = (AlertPriority)((byte)enumerator3.Current);
						this.PriosInDrawOrder.Add(item);
					}
				}
				finally
				{
					IDisposable disposable = enumerator3 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				this.PriosInDrawOrder.Reverse();
			}
		}
		public void AlertsReadoutUpdate()
		{
			if (Find.TickManager.TicksGame < 600)
			{
				return;
			}
			this.curAlertIndex++;
			if (this.curAlertIndex >= AlertsReadout.AlertCycleLength)
			{
				this.curAlertIndex = 0;
			}
			for (int i = this.curAlertIndex; i < this.AllAlerts.Count; i += AlertsReadout.AlertCycleLength)
			{
				Alert alert = this.AllAlerts[i];
				try
				{
					if (alert.Active)
					{
						if (!this.activeAlerts.Contains(alert))
						{
							this.activeAlerts.Add(alert);
							alert.Notify_Started();
						}
					}
					else
					{
						for (int j = 0; j < this.activeAlerts.Count; j++)
						{
							if (this.activeAlerts[j] == alert)
							{
								this.activeAlerts.RemoveAt(j);
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.ErrorOnce("Exception processing alert " + alert.ToString() + ": " + ex.ToString(), 743575);
					if (this.activeAlerts.Contains(alert))
					{
						this.activeAlerts.Remove(alert);
					}
				}
			}
			for (int k = this.activeAlerts.Count - 1; k >= 0; k--)
			{
				Alert alert2 = this.activeAlerts[k];
				try
				{
					this.activeAlerts[k].AlertActiveUpdate();
				}
				catch (Exception ex2)
				{
					Log.ErrorOnce("Exception updating alert " + alert2.ToString() + ": " + ex2.ToString(), 743575);
					this.activeAlerts.RemoveAt(k);
				}
			}
		}
		public void AlertsReadoutOnGUI()
		{
			if (this.activeAlerts.Count == 0)
			{
				return;
			}
			Alert alert = null;
			AlertPriority alertPriority = AlertPriority.Critical;
			bool flag = false;
			float num = Find.LetterStack.LastTopY - (float)this.activeAlerts.Count * 28f;
			Rect rect = new Rect((float)Screen.width - 154f, num, 154f, this.lastFinalY - num);
			float num2 = GenUI.BackgroundDarkAlphaForText();
			if (num2 > 0.001f)
			{
				GUI.color = new Color(1f, 1f, 1f, num2);
				Widgets.DrawShadowAround(rect);
				GUI.color = Color.white;
			}
			float num3 = num;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			for (int i = 0; i < this.PriosInDrawOrder.Count; i++)
			{
				AlertPriority alertPriority2 = this.PriosInDrawOrder[i];
				for (int j = 0; j < this.activeAlerts.Count; j++)
				{
					Alert alert2 = this.activeAlerts[j];
					if (alert2.Priority == alertPriority2)
					{
						if (!flag)
						{
							alertPriority = alertPriority2;
							flag = true;
						}
						Rect rect2 = alert2.DrawAt(num3, alertPriority2 != alertPriority);
						if (Mouse.IsOver(rect2))
						{
							alert = alert2;
						}
						num3 += rect2.height;
					}
				}
			}
			this.lastFinalY = num3;
			TutorUIHighlighter.HighlightOpportunity("Alerts", rect);
			if (alert != null)
			{
				alert.DrawInfoPane();
			}
		}
	}
}
