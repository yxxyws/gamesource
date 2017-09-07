using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Verse.AI
{
	[StaticConstructorOnStartup]
	public sealed class ReservationManager : IExposable
	{
		private List<Reservation> reservations = new List<Reservation>();
		private static readonly Material DebugReservedThingIcon = MaterialPool.MatFrom("UI/Overlays/ReservedForWork", ShaderDatabase.Cutout);
		public void ExposeData()
		{
			Scribe_Collections.LookList<Reservation>(ref this.reservations, "reservations", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				for (int i = this.reservations.Count - 1; i >= 0; i--)
				{
					Reservation reservation = this.reservations[i];
					if (reservation.Target.Thing != null && reservation.Target.Thing.Destroyed)
					{
						Log.Error("Loaded reservation with destroyed target: " + reservation + ". Deleting it...");
						this.reservations.Remove(reservation);
					}
					if (reservation.Claimant != null && reservation.Claimant.Destroyed)
					{
						Log.Error("Loaded reservation with destroyed claimant: " + reservation + ". Deleting it...");
						this.reservations.Remove(reservation);
					}
					if (reservation.Claimant == null)
					{
						Log.Error("Loaded reservation with null claimant: " + reservation + ". Deleting it...");
						this.reservations.Remove(reservation);
					}
				}
			}
		}
		public bool CanReserve(Pawn claimant, TargetInfo target, int maxPawns = 1)
		{
			if (claimant == null)
			{
				Log.Error("CanReserve with claimant==null");
				return false;
			}
			int num = 0;
			int count = this.reservations.Count;
			for (int i = 0; i < count; i++)
			{
				Reservation reservation = this.reservations[i];
				if (!(reservation.Target != target))
				{
					if (ReservationManager.RespectsReservationsOf(claimant, reservation.Claimant))
					{
						if (reservation.Claimant == claimant)
						{
							return true;
						}
						if (reservation.MaxPawns != maxPawns)
						{
							return false;
						}
						num++;
						if (num >= maxPawns)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public bool Reserve(Pawn claimant, TargetInfo target, int maxPawns = 1)
		{
			if (this.FirstReserverOf(target, claimant.Faction) == claimant)
			{
				return true;
			}
			if (target.ThingDestroyed)
			{
				Log.Warning(string.Concat(new object[]
				{
					claimant,
					" tried to reserve destroyed thing ",
					target,
					" for maxPawns ",
					maxPawns,
					" doing job ",
					(claimant.CurJob == null) ? "null" : claimant.CurJob.ToString()
				}));
				return false;
			}
			if (!this.CanReserve(claimant, target, maxPawns))
			{
				Pawn pawn = this.FirstReserverOf(target, claimant.Faction);
				Log.Error(string.Concat(new object[]
				{
					"Could not reserve ",
					target,
					" for ",
					claimant,
					" doing job ",
					(claimant.CurJob == null) ? "null" : claimant.CurJob.ToString(),
					" for maxPawns ",
					maxPawns,
					".Existing reserver: ",
					pawn,
					" doing job ",
					(claimant.CurJob == null) ? "null" : claimant.CurJob.ToString()
				}));
				return false;
			}
			this.reservations.Add(new Reservation(claimant, maxPawns, target));
			return true;
		}
		public void Release(TargetInfo target, Pawn claimant)
		{
			if (target.ThingDestroyed)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Releasing destroyed thing ",
					target,
					" for ",
					claimant
				}));
			}
			Reservation reservation = null;
			for (int i = 0; i < this.reservations.Count; i++)
			{
				Reservation reservation2 = this.reservations[i];
				if (reservation2.Target == target && reservation2.Claimant == claimant)
				{
					reservation = reservation2;
					break;
				}
			}
			if (reservation == null && !target.ThingDestroyed)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to release ",
					target,
					" that wasn't reserved by ",
					claimant,
					"."
				}));
				return;
			}
			this.reservations.Remove(reservation);
		}
		public void ReleaseAllForTarget(Thing t)
		{
			this.reservations.RemoveAll((Reservation r) => r.Target.Thing != null && r.Target.Thing == t);
		}
		public void ReleaseAllClaimedBy(Pawn claimant)
		{
			this.reservations.RemoveAll((Reservation r) => r.Claimant == claimant);
		}
		public bool IsReserved(TargetInfo target, Faction faction)
		{
			return this.FirstReserverOf(target, faction) != null;
		}
		public Pawn FirstReserverOf(TargetInfo target, Faction faction)
		{
			int count = this.reservations.Count;
			for (int i = 0; i < count; i++)
			{
				Reservation reservation = this.reservations[i];
				if (reservation.Target == target && reservation.Faction == faction)
				{
					return reservation.Claimant;
				}
			}
			return null;
		}
		public IEnumerable<Thing> AllReservedThings()
		{
			return 
				from res in this.reservations
				select res.Target.Thing;
		}
		private static bool RespectsReservationsOf(Pawn newClaimant, Pawn oldClaimant)
		{
			if (newClaimant.Faction == null || oldClaimant.Faction == null)
			{
				return false;
			}
			if (newClaimant.Faction == oldClaimant.Faction)
			{
				return true;
			}
			if (!newClaimant.Faction.HostileTo(oldClaimant.Faction))
			{
				return true;
			}
			if (oldClaimant.HostFaction != null && oldClaimant.HostFaction == newClaimant.HostFaction)
			{
				return true;
			}
			if (newClaimant.HostFaction != null)
			{
				if (oldClaimant.HostFaction != null)
				{
					return true;
				}
				if (newClaimant.HostFaction == oldClaimant.Faction)
				{
					return true;
				}
			}
			return false;
		}
		internal string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("All reservation in ReservationManager:");
			for (int i = 0; i < this.reservations.Count; i++)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"[",
					i,
					"] ",
					this.reservations[i].ToString()
				}));
			}
			return stringBuilder.ToString();
		}
		internal void DebugDrawReservations()
		{
			for (int i = 0; i < this.reservations.Count; i++)
			{
				Reservation reservation = this.reservations[i];
				if (reservation.Target.Thing != null)
				{
					if (reservation.Target.Thing.Spawned)
					{
						Thing thing = reservation.Target.Thing;
						Vector3 s = new Vector3((float)thing.RotatedSize.x, 1f, (float)thing.RotatedSize.z);
						Matrix4x4 matrix = default(Matrix4x4);
						matrix.SetTRS(thing.DrawPos + Vector3.up * 0.1f, Quaternion.identity, s);
						Graphics.DrawMesh(MeshPool.plane10, matrix, ReservationManager.DebugReservedThingIcon, 0);
						GenDraw.DrawLineBetween(reservation.Claimant.DrawPos, reservation.Target.Thing.DrawPos);
					}
					else
					{
						Graphics.DrawMesh(MeshPool.plane03, reservation.Claimant.DrawPos + Vector3.up + new Vector3(0.5f, 0f, 0.5f), Quaternion.identity, ReservationManager.DebugReservedThingIcon, 0);
					}
				}
				else
				{
					Graphics.DrawMesh(MeshPool.plane10, reservation.Target.Cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays), Quaternion.identity, ReservationManager.DebugReservedThingIcon, 0);
					GenDraw.DrawLineBetween(reservation.Claimant.DrawPos, reservation.Target.Cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays));
				}
			}
		}
	}
}
