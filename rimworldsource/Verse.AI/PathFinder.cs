using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI.Group;
namespace Verse.AI
{
	public static class PathFinder
	{
		internal struct PathFinderNodeFast
		{
			public int knownCost;
			public int totalCostEstimate;
			public ushort parentX;
			public ushort parentZ;
			public ushort status;
		}
		private struct PathFinderNode
		{
			public IntVec3 position;
			public IntVec3 parentPosition;
		}
		internal class PathFinderNodeFastCostComparer : IComparer<int>
		{
			private PathFinder.PathFinderNodeFast[] grid;
			public PathFinderNodeFastCostComparer(PathFinder.PathFinderNodeFast[] grid)
			{
				this.grid = grid;
			}
			public int Compare(int a, int b)
			{
				if (this.grid[a].totalCostEstimate > this.grid[b].totalCostEstimate)
				{
					return 1;
				}
				if (this.grid[a].totalCostEstimate < this.grid[b].totalCostEstimate)
				{
					return -1;
				}
				return 0;
			}
		}
		public const int DefaultMoveTicksCardinal = 13;
		private const int DefaultMoveTicksDiagonal = 18;
		private const int SearchLimit = 160000;
		private const int HeuristicStrengthAnimal = 23;
		private const int HeuristicStrengthShort = 8;
		private const int HeuristicStrengthMedium = 13;
		private const int HeuristicStrengthLong = 19;
		private const int HeuristicStrengthExtreme = 22;
		private const int Cost_DoorToBash = 300;
		private const int Cost_BlockedWall = 60;
		private const float Cost_BlockedWallPerHitPoint = 0.1f;
		private const int Cost_OutsideAllowedArea = 600;
		private static FastPriorityQueue<int> openList = null;
		private static PathFinder.PathFinderNodeFast[] calcGrid = null;
		private static ushort statusOpenValue = 1;
		private static ushort statusClosedValue = 2;
		private static int mapSizePowTwo;
		private static ushort gridSizeX = 0;
		private static ushort gridSizeZ = 0;
		private static ushort gridSizeXMinus1 = 0;
		private static ushort gridSizeZLog2 = 0;
		private static int mapSizeX;
		private static int mapSizeZ;
		private static PathGrid pathGrid;
		private static int[] pathGridDirect;
		private static Building[] edificeGrid;
		private static PawnPath newPath = null;
		private static int moveTicksCardinal;
		private static int moveTicksDiagonal;
		private static int curIndex = 0;
		private static ushort curX = 0;
		private static ushort curZ = 0;
		private static IntVec3 curIntVec3 = default(IntVec3);
		private static int neighIndex = 0;
		private static ushort neighX = 0;
		private static ushort neighZ = 0;
		private static int neighCostThroughCur = 0;
		private static int neighCost = 0;
		private static int h = 0;
		private static int closedCellCount = 0;
		private static int destinationIndex = 0;
		private static int destinationX = -1;
		private static int destinationZ = -1;
		private static CellRect destinationRect;
		private static bool destinationIsOneCell;
		private static int heuristicStrength;
		private static bool debug_pathFailMessaged = false;
		private static int debug_totalOpenListCount = 0;
		private static int debug_openCellsPopped = 0;
		private static readonly sbyte[] Directions = new sbyte[]
		{
			0,
			1,
			0,
			-1,
			1,
			1,
			-1,
			-1,
			-1,
			0,
			1,
			0,
			-1,
			1,
			1,
			-1
		};
		public static void Reinit()
		{
			PathFinder.mapSizePowTwo = Find.Map.info.PowerOfTwoOverMapSize;
			PathFinder.gridSizeX = (ushort)PathFinder.mapSizePowTwo;
			PathFinder.gridSizeZ = (ushort)PathFinder.mapSizePowTwo;
			PathFinder.gridSizeXMinus1 = PathFinder.gridSizeX - 1;
			PathFinder.gridSizeZLog2 = (ushort)Math.Log((double)PathFinder.gridSizeZ, 2.0);
			PathFinder.mapSizeX = Find.Map.Size.x;
			PathFinder.mapSizeZ = Find.Map.Size.z;
			PathFinder.calcGrid = new PathFinder.PathFinderNodeFast[(int)(PathFinder.gridSizeX * PathFinder.gridSizeZ)];
			PathFinder.openList = new FastPriorityQueue<int>(new PathFinder.PathFinderNodeFastCostComparer(PathFinder.calcGrid));
		}
		public static PawnPath FindPath(IntVec3 start, TargetInfo dest, Pawn pawn, PathEndMode peMode = PathEndMode.OnCell)
		{
			bool flag = false;
			if (pawn != null && pawn.CurJob != null && pawn.CurJob.canBash)
			{
				flag = true;
			}
			bool canBash = flag;
			return PathFinder.FindPath(start, dest, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, canBash), peMode);
		}
		public static PawnPath FindPath(IntVec3 start, TargetInfo dest, TraverseParms traverseParms, PathEndMode peMode = PathEndMode.OnCell)
		{
			if (DebugSettings.pathThroughWalls)
			{
				traverseParms.mode = TraverseMode.PassAnything;
			}
			Pawn pawn = traverseParms.pawn;
			bool flag = traverseParms.mode == TraverseMode.PassAnything;
			if (!start.IsValid)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to FindPath with invalid start ",
					start,
					", pawn= ",
					pawn
				}));
				return PawnPath.NotFound;
			}
			if (!dest.IsValid)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to FindPath with invalid dest ",
					dest,
					", pawn= ",
					pawn
				}));
				return PawnPath.NotFound;
			}
			if (!flag)
			{
				if (traverseParms.mode == TraverseMode.ByPawn)
				{
					if (!pawn.CanReach(dest, peMode, Danger.Deadly, traverseParms.canBash, traverseParms.mode))
					{
						return PawnPath.NotFound;
					}
				}
				else
				{
					if (!start.CanReach(dest, peMode, traverseParms))
					{
						return PawnPath.NotFound;
					}
				}
			}
			ByteGrid byteGrid = (pawn == null) ? null : pawn.GetAvoidGrid();
			PathFinder.destinationX = dest.Cell.x;
			PathFinder.destinationZ = dest.Cell.z;
			PathFinder.curIndex = CellIndices.CellToIndex(start);
			PathFinder.destinationIndex = CellIndices.CellToIndex(dest.Cell);
			if (!dest.HasThing || peMode == PathEndMode.OnCell)
			{
				PathFinder.destinationRect = CellRect.SingleCell(dest.Cell);
			}
			else
			{
				PathFinder.destinationRect = dest.Thing.OccupiedRect();
			}
			if (peMode == PathEndMode.Touch)
			{
				PathFinder.destinationRect = PathFinder.destinationRect.ExpandedBy(1);
			}
			PathFinder.destinationIsOneCell = (PathFinder.destinationRect.Width == 1 && PathFinder.destinationRect.Height == 1);
			PathFinder.pathGrid = Find.PathGrid;
			PathFinder.pathGridDirect = Find.PathGrid.pathGrid;
			PathFinder.edificeGrid = Find.EdificeGrid.InnerArray;
			PathFinder.statusOpenValue += 2;
			PathFinder.statusClosedValue += 2;
			if (PathFinder.statusClosedValue >= 65435)
			{
				PathFinder.ResetStatuses();
			}
			if (pawn != null && pawn.RaceProps.Animal)
			{
				PathFinder.heuristicStrength = 23;
			}
			else
			{
				float lengthHorizontal = (start - dest.Cell).LengthHorizontal;
				if (lengthHorizontal < 40f)
				{
					PathFinder.heuristicStrength = 8;
				}
				else
				{
					if (lengthHorizontal < 80f)
					{
						PathFinder.heuristicStrength = 13;
					}
					else
					{
						if (lengthHorizontal < 130f)
						{
							PathFinder.heuristicStrength = 19;
						}
						else
						{
							PathFinder.heuristicStrength = 22;
						}
					}
				}
			}
			PathFinder.closedCellCount = 0;
			PathFinder.openList.Clear();
			PathFinder.debug_pathFailMessaged = false;
			PathFinder.debug_totalOpenListCount = 0;
			PathFinder.debug_openCellsPopped = 0;
			if (pawn != null)
			{
				PathFinder.moveTicksCardinal = pawn.TicksPerMoveCardinal;
				PathFinder.moveTicksDiagonal = pawn.TicksPerMoveDiagonal;
			}
			else
			{
				PathFinder.moveTicksCardinal = 13;
				PathFinder.moveTicksDiagonal = 18;
			}
			PathFinder.calcGrid[PathFinder.curIndex].knownCost = 0;
			PathFinder.calcGrid[PathFinder.curIndex].totalCostEstimate = 0;
			PathFinder.calcGrid[PathFinder.curIndex].parentX = (ushort)start.x;
			PathFinder.calcGrid[PathFinder.curIndex].parentZ = (ushort)start.z;
			PathFinder.calcGrid[PathFinder.curIndex].status = PathFinder.statusOpenValue;
			PathFinder.openList.Push(PathFinder.curIndex);
			Area area = null;
			if (pawn != null && pawn.playerSettings != null)
			{
				area = pawn.playerSettings.AreaRestriction;
			}
			while (PathFinder.openList.Count > 0)
			{
				PathFinder.debug_totalOpenListCount += PathFinder.openList.Count;
				PathFinder.debug_openCellsPopped++;
				PathFinder.curIndex = PathFinder.openList.Pop();
				if (PathFinder.calcGrid[PathFinder.curIndex].status != PathFinder.statusClosedValue)
				{
					PathFinder.curIntVec3 = CellIndices.IndexToCell(PathFinder.curIndex);
					PathFinder.curX = (ushort)PathFinder.curIntVec3.x;
					PathFinder.curZ = (ushort)PathFinder.curIntVec3.z;
					PathFinder.DebugFlash(PathFinder.curIntVec3, (float)PathFinder.calcGrid[PathFinder.curIndex].knownCost / 1500f, PathFinder.calcGrid[PathFinder.curIndex].knownCost.ToString());
					if (PathFinder.destinationIsOneCell)
					{
						if (PathFinder.curIndex == PathFinder.destinationIndex)
						{
							return PathFinder.FinalizedPath();
						}
					}
					else
					{
						if (PathFinder.destinationRect.Contains(PathFinder.curIntVec3))
						{
							return PathFinder.FinalizedPath();
						}
					}
					if (PathFinder.closedCellCount > 160000)
					{
						Log.Warning(string.Concat(new object[]
						{
							pawn,
							" pathing from ",
							start,
							" to ",
							dest,
							" hit search limit of ",
							160000,
							" cells."
						}));
						PathFinder.DebugDrawRichData();
						return PawnPath.NotFound;
					}
					for (int i = 0; i < 8; i++)
					{
						PathFinder.neighX = (ushort)((int)PathFinder.curX + (int)PathFinder.Directions[i]);
						PathFinder.neighZ = (ushort)((int)PathFinder.curZ + (int)PathFinder.Directions[i + 8]);
						IntVec3 intVec = new IntVec3((int)PathFinder.neighX, 0, (int)PathFinder.neighZ);
						PathFinder.neighIndex = CellIndices.CellToIndex((int)PathFinder.neighX, (int)PathFinder.neighZ);
						if ((int)PathFinder.neighX >= PathFinder.mapSizeX || (int)PathFinder.neighZ >= PathFinder.mapSizeZ)
						{
							PathFinder.DebugFlash(intVec, 0.75f, "oob");
						}
						else
						{
							if (PathFinder.calcGrid[PathFinder.neighIndex].status != PathFinder.statusClosedValue)
							{
								int num = 0;
								bool flag2 = false;
								if (!PathFinder.pathGrid.WalkableFast(intVec))
								{
									if (!flag)
									{
										PathFinder.DebugFlash(intVec, 0.22f, "walk");
										goto IL_D1C;
									}
									flag2 = true;
									num += 60;
									Thing edifice = intVec.GetEdifice();
									if (edifice != null)
									{
										if (!edifice.def.useHitPoints)
										{
											goto IL_D1C;
										}
										num += (int)((float)edifice.HitPoints * 0.1f);
									}
								}
								if (i > 3)
								{
									switch (i)
									{
									case 4:
										if (!PathFinder.pathGrid.WalkableFast((int)PathFinder.curX, (int)(PathFinder.curZ - 1)))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)PathFinder.curX, 0, (int)(PathFinder.curZ - 1)), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										if (!PathFinder.pathGrid.WalkableFast((int)(PathFinder.curX + 1), (int)PathFinder.curZ))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)(PathFinder.curX + 1), 0, (int)PathFinder.curZ), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										break;
									case 5:
										if (!PathFinder.pathGrid.WalkableFast((int)PathFinder.curX, (int)(PathFinder.curZ + 1)))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)PathFinder.curX, 0, (int)(PathFinder.curZ + 1)), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										if (!PathFinder.pathGrid.WalkableFast((int)(PathFinder.curX + 1), (int)PathFinder.curZ))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)(PathFinder.curX + 1), 0, (int)PathFinder.curZ), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										break;
									case 6:
										if (!PathFinder.pathGrid.WalkableFast((int)PathFinder.curX, (int)(PathFinder.curZ + 1)))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)PathFinder.curX, 0, (int)(PathFinder.curZ + 1)), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										if (!PathFinder.pathGrid.WalkableFast((int)(PathFinder.curX - 1), (int)PathFinder.curZ))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)(PathFinder.curX - 1), 0, (int)PathFinder.curZ), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										break;
									case 7:
										if (!PathFinder.pathGrid.WalkableFast((int)PathFinder.curX, (int)(PathFinder.curZ - 1)))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)PathFinder.curX, 0, (int)(PathFinder.curZ - 1)), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										if (!PathFinder.pathGrid.WalkableFast((int)(PathFinder.curX - 1), (int)PathFinder.curZ))
										{
											if (!flag || !traverseParms.canWalkDiagonally)
											{
												PathFinder.DebugFlash(new IntVec3((int)(PathFinder.curX - 1), 0, (int)PathFinder.curZ), 0.9f, "corn");
												goto IL_D1C;
											}
											num += 60;
										}
										break;
									}
								}
								if (i > 3)
								{
									PathFinder.neighCost = PathFinder.moveTicksDiagonal;
								}
								else
								{
									PathFinder.neighCost = PathFinder.moveTicksCardinal;
								}
								PathFinder.neighCost += num;
								if (!flag2)
								{
									PathFinder.neighCost += PathFinder.pathGridDirect[PathFinder.neighIndex];
								}
								if (byteGrid != null)
								{
									PathFinder.neighCost += (int)(byteGrid[PathFinder.neighIndex] * 8);
								}
								if (area != null && !area[intVec])
								{
									PathFinder.neighCost += 600;
								}
								Building building = PathFinder.edificeGrid[CellIndices.CellToIndex((int)PathFinder.neighX, (int)PathFinder.neighZ)];
								if (building != null)
								{
									Building_Door building_Door = building as Building_Door;
									if (building_Door != null)
									{
										switch (traverseParms.mode)
										{
										case TraverseMode.ByPawn:
											if (!traverseParms.canBash && building_Door.IsForbiddenToPass(pawn))
											{
												if (DebugViewSettings.drawPaths)
												{
													PathFinder.DebugFlash(building.Position, 0.77f, "forbid");
												}
												goto IL_D1C;
											}
											if (!building_Door.FreePassage)
											{
												if (building_Door.PawnCanOpen(pawn))
												{
													PathFinder.neighCost += building_Door.TicksToOpenNow;
												}
												else
												{
													if (!traverseParms.canBash)
													{
														if (DebugViewSettings.drawPaths)
														{
															PathFinder.DebugFlash(building.Position, 0.34f, "cant pass");
														}
														goto IL_D1C;
													}
													PathFinder.neighCost += 300;
												}
											}
											break;
										case TraverseMode.NoPassClosedDoors:
											if (!building_Door.FreePassage)
											{
												goto IL_D1C;
											}
											break;
										}
									}
									else
									{
										if (pawn != null)
										{
											PathFinder.neighCost += (int)building.PathFindCostFor(pawn);
										}
									}
								}
								PathFinder.neighCostThroughCur = PathFinder.neighCost + PathFinder.calcGrid[PathFinder.curIndex].knownCost;
								if ((PathFinder.calcGrid[PathFinder.neighIndex].status != PathFinder.statusClosedValue && PathFinder.calcGrid[PathFinder.neighIndex].status != PathFinder.statusOpenValue) || PathFinder.calcGrid[PathFinder.neighIndex].knownCost > PathFinder.neighCostThroughCur)
								{
									int status = (int)PathFinder.calcGrid[PathFinder.neighIndex].status;
									PathFinder.calcGrid[PathFinder.neighIndex].parentX = PathFinder.curX;
									PathFinder.calcGrid[PathFinder.neighIndex].parentZ = PathFinder.curZ;
									PathFinder.calcGrid[PathFinder.neighIndex].knownCost = PathFinder.neighCostThroughCur;
									PathFinder.calcGrid[PathFinder.neighIndex].status = PathFinder.statusOpenValue;
									PathFinder.h = PathFinder.heuristicStrength * (Mathf.Abs((int)PathFinder.neighX - PathFinder.destinationX) + Mathf.Abs((int)PathFinder.neighZ - PathFinder.destinationZ));
									PathFinder.calcGrid[PathFinder.neighIndex].totalCostEstimate = PathFinder.neighCostThroughCur + PathFinder.h;
									if (status != (int)PathFinder.statusOpenValue)
									{
										PathFinder.openList.Push(PathFinder.neighIndex);
									}
								}
							}
						}
						IL_D1C:;
					}
					PathFinder.closedCellCount++;
					PathFinder.calcGrid[PathFinder.curIndex].status = PathFinder.statusClosedValue;
				}
			}
			if (!PathFinder.debug_pathFailMessaged)
			{
				Log.Warning(string.Concat(new object[]
				{
					pawn.ThingID,
					" aka ",
					pawn.LabelCap,
					" pathing from ",
					start,
					" to ",
					dest,
					" ran out of cells to process.\nJob:",
					pawn.jobs.curJob,
					"\nFaction: ",
					pawn.Faction,
					"\n\nThis will be the last message to avoid spam."
				}));
				PathFinder.debug_pathFailMessaged = true;
			}
			PathFinder.DebugDrawRichData();
			return PawnPath.NotFound;
		}
		private static void DebugFlash(IntVec3 c, float colorPct, string str)
		{
			if (DebugViewSettings.drawPaths)
			{
				Find.DebugDrawer.FlashCell(c, colorPct, str);
			}
		}
		private static PawnPath FinalizedPath()
		{
			PathFinder.newPath = PawnPathPool.GetEmptyPawnPath();
			IntVec3 parentPosition = new IntVec3((int)PathFinder.curX, 0, (int)PathFinder.curZ);
			while (true)
			{
				PathFinder.PathFinderNodeFast pathFinderNodeFast = PathFinder.calcGrid[CellIndices.CellToIndex(parentPosition)];
				PathFinder.PathFinderNode pathFinderNode;
				pathFinderNode.parentPosition = new IntVec3((int)pathFinderNodeFast.parentX, 0, (int)pathFinderNodeFast.parentZ);
				pathFinderNode.position = parentPosition;
				PathFinder.newPath.AddNode(pathFinderNode.position);
				if (pathFinderNode.position == pathFinderNode.parentPosition)
				{
					break;
				}
				parentPosition = pathFinderNode.parentPosition;
			}
			PathFinder.newPath.SetupFound((float)PathFinder.calcGrid[PathFinder.curIndex].knownCost);
			return PathFinder.newPath;
		}
		private static void ResetStatuses()
		{
			int num = PathFinder.calcGrid.Length;
			for (int i = 0; i < num; i++)
			{
				PathFinder.calcGrid[i].status = 0;
			}
			PathFinder.statusOpenValue = 1;
			PathFinder.statusClosedValue = 2;
		}
		private static void DebugDrawRichData()
		{
			if (DebugViewSettings.drawPaths)
			{
				while (PathFinder.openList.Count > 0)
				{
					int num = PathFinder.openList.Pop();
					IntVec3 c = new IntVec3(num & (int)PathFinder.gridSizeXMinus1, 0, num >> (int)PathFinder.gridSizeZLog2);
					Find.DebugDrawer.FlashCell(c, 0f, "open");
				}
			}
		}
	}
}
