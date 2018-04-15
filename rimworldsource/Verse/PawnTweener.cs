using System;
using UnityEngine;
namespace Verse
{
	public class PawnTweener
	{
		private const float SpringTightness = 0.09f;
		private Pawn pawn;
		private Vector3 springPos = new Vector3(0f, 0f, 0f);
		private Vector3 lastTickSpringPos;
		public Vector3 TweenedPos
		{
			get
			{
				return this.springPos;
			}
		}
		public Vector3 TweenedPosRoot
		{
			get
			{
				if (this.pawn.Destroyed)
				{
					return this.pawn.Position.ToVector3Shifted();
				}
				float num;
				if (this.pawn.pather.Moving)
				{
					if (this.pawn.pather.BuildingBlockingNextPathCell() != null)
					{
						num = 0f;
					}
					else
					{
						if (this.pawn.pather.NextCellDoorToManuallyOpen() != null)
						{
							num = 0f;
						}
						else
						{
							num = 1f - (float)this.pawn.pather.ticksUntilMove / (float)this.pawn.pather.totalMoveDuration;
						}
					}
				}
				else
				{
					num = 0f;
				}
				if (this.pawn.stances.FullBodyBusy)
				{
					num = 0f;
				}
				return this.pawn.pather.nextCell.ToVector3Shifted() * num + this.pawn.Position.ToVector3Shifted() * (1f - num);
			}
		}
		public Vector3 LastTickTweenedVelocity
		{
			get
			{
				return this.TweenedPos - this.lastTickSpringPos;
			}
		}
		public PawnTweener(Pawn pawn)
		{
			this.pawn = pawn;
		}
		public void TweenerTick()
		{
			this.lastTickSpringPos = this.springPos;
			Vector3 a = this.TweenedPosRoot - this.springPos;
			this.springPos += a * 0.09f;
		}
		public void ResetToPosition()
		{
			this.springPos = this.TweenedPosRoot;
		}
	}
}
