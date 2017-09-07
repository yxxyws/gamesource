using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public class SimpleCurve : IEnumerable, IEnumerable<CurvePoint>
	{
		private List<CurvePoint> points = new List<CurvePoint>();
		[Unsaved]
		private SimpleCurveView view;
		public int PointsCount
		{
			get
			{
				return this.points.Count;
			}
		}
		public IEnumerable<CurvePoint> AllPoints
		{
			get
			{
				return this.points;
			}
		}
		public bool HasView
		{
			get
			{
				return this.view != null;
			}
		}
		public SimpleCurveView View
		{
			get
			{
				if (this.view == null)
				{
					this.view = new SimpleCurveView();
					this.view.SetViewRectAround(this);
				}
				return this.view;
			}
		}
		public CurvePoint this[int i]
		{
			get
			{
				return this.points[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		[DebuggerHidden]
		public IEnumerator<CurvePoint> GetEnumerator()
		{
			SimpleCurve.<GetEnumerator>c__Iterator197 <GetEnumerator>c__Iterator = new SimpleCurve.<GetEnumerator>c__Iterator197();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}
		public void SetPoints(IEnumerable<CurvePoint> newPoints)
		{
			this.points.Clear();
			foreach (CurvePoint current in newPoints)
			{
				this.points.Add(current);
			}
			this.SortPoints();
		}
		public void Add(float x, float y)
		{
			CurvePoint newPoint = new CurvePoint(x, y);
			this.Add(newPoint);
		}
		public void Add(CurvePoint newPoint)
		{
			this.points.Add(newPoint);
			this.SortPoints();
		}
		public void SortPoints()
		{
			Comparison<CurvePoint> comparison = delegate(CurvePoint a, CurvePoint b)
			{
				if (a.x < b.x)
				{
					return -1;
				}
				if (b.x < a.x)
				{
					return 1;
				}
				return 0;
			};
			this.points.Sort(comparison);
		}
		public void RemovePointNear(CurvePoint point)
		{
			for (int i = 0; i < this.points.Count; i++)
			{
				if ((this.points[i].loc - point.loc).sqrMagnitude < 0.001f)
				{
					this.points.RemoveAt(i);
					return;
				}
			}
		}
		public float Evaluate(float x)
		{
			if (this.points.Count == 0)
			{
				Log.Error("Evaluating a SimpleCurve with no points.");
				return 0f;
			}
			if (x <= this.points[0].x)
			{
				return this.points[0].y;
			}
			if (x >= this.points[this.points.Count - 1].x)
			{
				return this.points[this.points.Count - 1].y;
			}
			CurvePoint curvePoint = this.points[this.points.Count - 1];
			CurvePoint curvePoint2 = null;
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.points[i].x >= x)
				{
					curvePoint = this.points[i];
					if (i > 0)
					{
						curvePoint2 = this.points[i - 1];
					}
					else
					{
						curvePoint2 = this.points[i];
					}
					break;
				}
			}
			float t = (x - curvePoint2.x) / (curvePoint.x - curvePoint2.x);
			return Mathf.Lerp(curvePoint2.y, curvePoint.y, t);
		}
		public float PeriodProbabilityFromCumulative(float startX, float span)
		{
			if (this.points.Count < 2)
			{
				return 0f;
			}
			if (this.points[0].y != 0f)
			{
				Log.Warning("PeriodProbabilityFromCumulative should only run on curves whose first point is 0.");
			}
			float num = this.Evaluate(startX + span) - this.Evaluate(startX);
			if (num < 0f)
			{
				Log.Error("PeriodicProbability got negative probability from " + this + ": slope should never be negative.");
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			return num;
		}
		[DebuggerHidden]
		public IEnumerable<string> ConfigErrors(string prefix)
		{
			SimpleCurve.<ConfigErrors>c__Iterator198 <ConfigErrors>c__Iterator = new SimpleCurve.<ConfigErrors>c__Iterator198();
			<ConfigErrors>c__Iterator.prefix = prefix;
			<ConfigErrors>c__Iterator.<$>prefix = prefix;
			<ConfigErrors>c__Iterator.<>f__this = this;
			SimpleCurve.<ConfigErrors>c__Iterator198 expr_1C = <ConfigErrors>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
