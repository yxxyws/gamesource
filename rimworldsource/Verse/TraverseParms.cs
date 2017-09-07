using System;
namespace Verse
{
	public struct TraverseParms : IEquatable<TraverseParms>
	{
		public Pawn pawn;
		public TraverseMode mode;
		public Danger maxDanger;
		public bool canBash;
		public bool canWalkDiagonally;
		public static TraverseParms For(Pawn pawn, Danger maxDanger = Danger.Deadly, TraverseMode mode = TraverseMode.ByPawn, bool canBash = false)
		{
			if (pawn == null)
			{
				Log.Error("TraverseParms for null pawn.");
				return TraverseParms.For(TraverseMode.NoPassClosedDoors, maxDanger, canBash);
			}
			return new TraverseParms
			{
				pawn = pawn,
				maxDanger = maxDanger,
				mode = mode,
				canBash = canBash,
				canWalkDiagonally = true
			};
		}
		public static TraverseParms For(TraverseMode mode, Danger maxDanger = Danger.Deadly, bool canBash = false)
		{
			return new TraverseParms
			{
				pawn = null,
				mode = mode,
				maxDanger = maxDanger,
				canBash = canBash,
				canWalkDiagonally = true
			};
		}
		public void Validate()
		{
			if (this.mode == TraverseMode.ByPawn && this.pawn == null)
			{
				Log.Error("Invalid traverse parameters: IfPawnAllowed but traverser = null.");
			}
		}
		public override bool Equals(object obj)
		{
			return obj is TraverseParms && this.Equals((TraverseParms)obj);
		}
		public bool Equals(TraverseParms other)
		{
			return other.pawn == this.pawn && other.mode == this.mode && other.canBash == this.canBash && other.maxDanger == this.maxDanger && other.canWalkDiagonally == this.canWalkDiagonally;
		}
		public override int GetHashCode()
		{
			int seed = 0;
			if (this.pawn != null)
			{
				seed = Gen.HashCombine<Pawn>(seed, this.pawn);
			}
			else
			{
				seed = Gen.HashCombineStruct<TraverseMode>(seed, this.mode);
			}
			seed = Gen.HashCombineInt(seed, (!this.canBash) ? 0 : 1);
			seed = Gen.HashCombineStruct<Danger>(seed, this.maxDanger);
			return Gen.HashCombineInt(seed, (!this.canWalkDiagonally) ? 0 : 1);
		}
		public override string ToString()
		{
			string text = (!this.canBash) ? string.Empty : " canBash";
			if (this.mode == TraverseMode.ByPawn)
			{
				return string.Concat(new object[]
				{
					"(",
					this.mode,
					" ",
					this.maxDanger,
					" ",
					this.pawn,
					text,
					")"
				});
			}
			return string.Concat(new object[]
			{
				"(",
				this.mode,
				" ",
				this.maxDanger,
				text,
				")"
			});
		}
		public static implicit operator TraverseParms(TraverseMode m)
		{
			if (m == TraverseMode.ByPawn)
			{
				throw new InvalidOperationException("Cannot implicitly convert TraverseMode.ByPawn to RegionTraverseParameters.");
			}
			return TraverseParms.For(m, Danger.Deadly, false);
		}
		public static bool operator ==(TraverseParms a, TraverseParms b)
		{
			return a.pawn == b.pawn && a.mode == b.mode && a.canBash == b.canBash && a.maxDanger == b.maxDanger && a.canWalkDiagonally == b.canWalkDiagonally;
		}
		public static bool operator !=(TraverseParms a, TraverseParms b)
		{
			return a.pawn != b.pawn || a.mode != b.mode || a.canBash != b.canBash || a.maxDanger != b.maxDanger || a.canWalkDiagonally != b.canWalkDiagonally;
		}
	}
}
