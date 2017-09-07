using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
namespace Verse
{
	public class ThingWithComps : Thing
	{
		private List<ThingComp> comps = new List<ThingComp>();
		public List<ThingComp> AllComps
		{
			get
			{
				return this.comps;
			}
		}
		public override Color DrawColor
		{
			get
			{
				CompColorable comp = this.GetComp<CompColorable>();
				if (comp != null && comp.Active)
				{
					return comp.Color;
				}
				return base.DrawColor;
			}
			set
			{
				this.SetColor(value, true);
			}
		}
		public override string LabelBase
		{
			get
			{
				return GenLabel.ThingLabel(this);
			}
		}
		public override void PostMake()
		{
			base.PostMake();
			this.InitializeComps();
		}
		public T GetComp<T>() where T : ThingComp
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				T t = this.comps[i] as T;
				if (t != null)
				{
					return t;
				}
			}
			return (T)((object)null);
		}
		public ThingComp GetCompByDef(CompProperties def)
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				if (this.comps[i].props == def)
				{
					return this.comps[i];
				}
			}
			return null;
		}
		public void InitializeComps()
		{
			for (int i = 0; i < this.def.comps.Count; i++)
			{
				ThingComp thingComp = (ThingComp)Activator.CreateInstance(this.def.comps[i].compClass);
				thingComp.parent = this;
				this.comps.Add(thingComp);
				thingComp.Initialize(this.def.comps[i]);
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				this.InitializeComps();
			}
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostExposeData();
			}
		}
		public void BroadcastCompSignal(string signal)
		{
			this.ReceiveCompSignal(signal);
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].ReceiveCompSignal(signal);
			}
		}
		protected virtual void ReceiveCompSignal(string signal)
		{
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostSpawnSetup();
			}
		}
		public override void DeSpawn()
		{
			base.DeSpawn();
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostDeSpawn();
			}
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			bool spawned = base.Spawned;
			base.Destroy(mode);
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostDestroy(mode, spawned);
			}
		}
		public override void Tick()
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].CompTick();
			}
		}
		public override void TickRare()
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].CompTickRare();
			}
		}
		public override void PreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			base.PreApplyDamage(dinfo, out absorbed);
			if (absorbed)
			{
				return;
			}
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostPreApplyDamage(dinfo, out absorbed);
				if (absorbed)
				{
					return;
				}
			}
		}
		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			base.PostApplyDamage(dinfo, totalDamageDealt);
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostPostApplyDamage(dinfo, totalDamageDealt);
			}
		}
		public override void Draw()
		{
			base.Draw();
			this.Comps_PostDraw();
		}
		protected void Comps_PostDraw()
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostDraw();
			}
		}
		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostDrawExtraSelectionOverlays();
			}
		}
		public override void Print(SectionLayer layer)
		{
			base.Print(layer);
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PostPrintOnto(layer);
			}
		}
		public virtual void PrintForPowerGrid(SectionLayer layer)
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].CompPrintForPowerGrid(layer);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			ThingWithComps.<GetGizmos>c__IteratorC9 <GetGizmos>c__IteratorC = new ThingWithComps.<GetGizmos>c__IteratorC9();
			<GetGizmos>c__IteratorC.<>f__this = this;
			ThingWithComps.<GetGizmos>c__IteratorC9 expr_0E = <GetGizmos>c__IteratorC;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override bool TryAbsorbStack(Thing other, bool respectStackLimit)
		{
			int count;
			if (respectStackLimit)
			{
				count = Mathf.Min(other.stackCount, this.def.stackLimit - this.stackCount);
			}
			else
			{
				count = other.stackCount;
			}
			for (int i = 0; i < this.comps.Count; i++)
			{
				this.comps[i].PreAbsorbStack(other, count);
			}
			return base.TryAbsorbStack(other, respectStackLimit);
		}
		public override Thing SplitOff(int count)
		{
			Thing thing = base.SplitOff(count);
			if (thing != null)
			{
				for (int i = 0; i < this.comps.Count; i++)
				{
					this.comps[i].PostSplitOff(thing);
				}
			}
			return thing;
		}
		public override bool CanStackWith(Thing other)
		{
			if (!base.CanStackWith(other))
			{
				return false;
			}
			for (int i = 0; i < this.comps.Count; i++)
			{
				if (!this.comps[i].AllowStackWith(other))
				{
					return false;
				}
			}
			return true;
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.Append(this.InspectStringPartsFromComps());
			return stringBuilder.ToString();
		}
		protected string InspectStringPartsFromComps()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.comps.Count; i++)
			{
				string text = this.comps[i].CompInspectStringExtra();
				if (!text.NullOrEmpty())
				{
					stringBuilder.AppendLine(text);
				}
			}
			return stringBuilder.ToString();
		}
		public override string GetDescription()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetDescription());
			for (int i = 0; i < this.comps.Count; i++)
			{
				string descriptionPart = this.comps[i].GetDescriptionPart();
				if (!descriptionPart.NullOrEmpty())
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(descriptionPart);
				}
			}
			return stringBuilder.ToString();
		}
		[DebuggerHidden]
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			ThingWithComps.<GetFloatMenuOptions>c__IteratorCA <GetFloatMenuOptions>c__IteratorCA = new ThingWithComps.<GetFloatMenuOptions>c__IteratorCA();
			<GetFloatMenuOptions>c__IteratorCA.selPawn = selPawn;
			<GetFloatMenuOptions>c__IteratorCA.<$>selPawn = selPawn;
			<GetFloatMenuOptions>c__IteratorCA.<>f__this = this;
			ThingWithComps.<GetFloatMenuOptions>c__IteratorCA expr_1C = <GetFloatMenuOptions>c__IteratorCA;
			expr_1C.$PC = -2;
			return expr_1C;
		}
	}
}
