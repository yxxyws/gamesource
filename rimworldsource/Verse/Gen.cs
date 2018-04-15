using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Verse
{
	public static class Gen
	{
		public const float BigEpsilon = 1E-07f;
		public static readonly int[,] EightWayRoundPattern = new int[,]
		{

			{
				0,
				-1
			},

			{
				0,
				1
			},

			{
				1,
				0
			},

			{
				-1,
				0
			},

			{
				1,
				-1
			},

			{
				1,
				1
			},

			{
				-1,
				1
			},

			{
				-1,
				-1
			}
		};
		public static Vector3 TrueCenter(this Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				return pawn.Drawer.DrawPos;
			}
			return Gen.TrueCenter(t.Position, t.Rotation, t.def.size, t.def.Altitude);
		}
		public static Vector3 TrueCenter(IntVec3 loc, Rot4 rotation, IntVec2 thingSize, float altitude)
		{
			Vector3 result = loc.ToVector3ShiftedWithAltitude(altitude);
			if (thingSize.x != 1 || thingSize.z != 1)
			{
				if (rotation.IsHorizontal)
				{
					int x = thingSize.x;
					thingSize.x = thingSize.z;
					thingSize.z = x;
				}
				switch (rotation.AsInt)
				{
				case 0:
					if (thingSize.x % 2 == 0)
					{
						result.x += 0.5f;
					}
					if (thingSize.z % 2 == 0)
					{
						result.z += 0.5f;
					}
					break;
				case 1:
					if (thingSize.x % 2 == 0)
					{
						result.x += 0.5f;
					}
					if (thingSize.z % 2 == 0)
					{
						result.z -= 0.5f;
					}
					break;
				case 2:
					if (thingSize.x % 2 == 0)
					{
						result.x -= 0.5f;
					}
					if (thingSize.z % 2 == 0)
					{
						result.z -= 0.5f;
					}
					break;
				case 3:
					if (thingSize.x % 2 == 0)
					{
						result.x -= 0.5f;
					}
					if (thingSize.z % 2 == 0)
					{
						result.z += 0.5f;
					}
					break;
				}
			}
			return result;
		}
		public static T RandomEnumValue<T>(bool disallowFirstValue)
		{
			int min = (!disallowFirstValue) ? 0 : 1;
			int num = Rand.Range(min, Enum.GetValues(typeof(T)).Length);
			return (T)((object)num);
		}
		public static bool WithinBoxRadius(IntVec3 A, IntVec3 B, float BoxRad)
		{
			int num = A.x - B.x;
			int num2 = A.z - B.z;
			return (float)num <= BoxRad && (float)num >= -BoxRad && (float)num2 <= BoxRad && (float)num2 >= -BoxRad;
		}
		public static IntVec3 NeighborAtAngle(float angle)
		{
			if (angle < 0f || angle > 360f)
			{
				Log.Error("Angle out of range: + " + angle);
			}
			if (angle < 22.5f || angle > 337.5f)
			{
				return GenAdj.AdjacentCellsAround[0];
			}
			return GenAdj.AdjacentCellsAround[(int)Math.Floor((double)((angle - 22.5f) / 45f)) + 1];
		}
		public static float AngleReversed(float angle)
		{
			if (angle < 0f || angle > 360f)
			{
				Log.Error("Angle out of range: + " + angle);
			}
			angle += 180f;
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return angle;
		}
		public static Vector3 RandomHorizontalVector(float max)
		{
			return new Vector3(Rand.Range(-max, max), 0f, Rand.Range(-max, max));
		}
		public static int GetBitCountOf(long lValue)
		{
			int num = 0;
			while (lValue != 0L)
			{
				lValue &= lValue - 1L;
				num++;
			}
			return num;
		}
		public static int HashCombine<T>(int seed, T obj)
		{
			int num = (obj != null) ? obj.GetHashCode() : 0;
			return (int)((long)seed ^ (long)num + (long)((ulong)-1640531527) + (long)((long)seed << 6) + (long)(seed >> 2));
		}
		public static int HashCombineStruct<T>(int seed, T obj) where T : struct
		{
			return (int)((long)seed ^ (long)obj.GetHashCode() + (long)((ulong)-1640531527) + (long)((long)seed << 6) + (long)(seed >> 2));
		}
		public static int HashCombineInt(int seed, int value)
		{
			return (int)((long)seed ^ (long)value + (long)((ulong)-1640531527) + (long)((long)seed << 6) + (long)(seed >> 2));
		}
		public static int HashOffset(this int baseInt)
		{
			return Gen.HashCombineInt(baseInt, 169495093);
		}
		public static int HashOffset(this Thing t)
		{
			return t.thingIDNumber.HashOffset();
		}
		public static bool IsHashIntervalTick(this Thing t, int interval)
		{
			return (Find.TickManager.TicksGame + t.thingIDNumber.HashOffset()) % interval == 0;
		}
		public static float HashRandForHour(this Thing t, int salt)
		{
			int seed = t.HashOffset();
			seed = Gen.HashCombineInt(seed, GenDate.HourOfDay);
			seed = Gen.HashCombineInt(seed, salt);
			Rand.PushSeed();
			Rand.Seed = seed;
			float value = Rand.Value;
			Rand.PopSeed();
			return value;
		}
		[DebuggerHidden]
		public static IEnumerable<T> GetAllSelectedItems<T>(this Enum value)
		{
			Gen.<GetAllSelectedItems>c__Iterator199<T> <GetAllSelectedItems>c__Iterator = new Gen.<GetAllSelectedItems>c__Iterator199<T>();
			<GetAllSelectedItems>c__Iterator.value = value;
			<GetAllSelectedItems>c__Iterator.<$>value = value;
			Gen.<GetAllSelectedItems>c__Iterator199<T> expr_15 = <GetAllSelectedItems>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
		[DebuggerHidden]
		public static IEnumerable<T> YieldSingle<T>(T val)
		{
			Gen.<YieldSingle>c__Iterator19A<T> <YieldSingle>c__Iterator19A = new Gen.<YieldSingle>c__Iterator19A<T>();
			<YieldSingle>c__Iterator19A.val = val;
			<YieldSingle>c__Iterator19A.<$>val = val;
			Gen.<YieldSingle>c__Iterator19A<T> expr_15 = <YieldSingle>c__Iterator19A;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static Vector3 ScreenToWorldPoint(Vector2 screenLoc)
		{
			Ray ray = Find.CameraMapGameObject.camera.ScreenPointToRay(screenLoc);
			Vector3 result = new Vector3(ray.origin.x, 0f, ray.origin.z);
			return result;
		}
		public static Vector3 InvertedWorldToScreen(Vector3 worldLoc)
		{
			return Find.CameraMap.InvertedWorldToScreenPoint(worldLoc);
		}
		public static Vector3 MouseMapPosVector3()
		{
			return Gen.ScreenToWorldPoint(Input.mousePosition);
		}
		public static IntVec3 MouseCell()
		{
			return Gen.ScreenToWorldPoint(Input.mousePosition).ToIntVec3();
		}
		public static Vector2 RotatedBy(this Vector2 v, float angle)
		{
			Vector3 point = new Vector3(v.x, 0f, v.y);
			point = Quaternion.AngleAxis(angle, Vector3.up) * point;
			return new Vector2(point.x, point.z);
		}
		public static Vector3 RotatedBy(this Vector3 v3, float angle)
		{
			return Quaternion.AngleAxis(angle, Vector3.up) * v3;
		}
		public static IntVec3 RotatedBy(this IntVec3 orig, Rot4 rot)
		{
			switch (rot.AsInt)
			{
			case 0:
				return orig;
			case 1:
				return new IntVec3(orig.z, orig.y, -orig.x);
			case 2:
				return new IntVec3(-orig.x, orig.y, -orig.z);
			case 3:
				return new IntVec3(-orig.z, orig.y, orig.x);
			default:
				return orig;
			}
		}
		public static int ManhattanDistanceFlat(IntVec3 A, IntVec3 B)
		{
			return Math.Abs(A.x - B.x) + Math.Abs(A.z - B.z);
		}
		public static Quaternion ToQuat(this float ang)
		{
			return Quaternion.AngleAxis(ang, Vector3.up);
		}
		public static int Clamp(int value, int min, int max)
		{
			return (value >= min) ? ((value <= max) ? value : max) : min;
		}
	}
}
