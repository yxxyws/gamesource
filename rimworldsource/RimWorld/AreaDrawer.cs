using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RimWorld
{
	internal class AreaDrawer
	{
		private const float Opacity = 0.33f;
		private const int MaxCellsPerMesh = 16383;
		public Area area;
		private bool wantDraw;
		private Material material;
		private bool dirty = true;
		private List<Mesh> meshes = new List<Mesh>();
		private static List<Vector3> verts = new List<Vector3>();
		private static List<int> tris = new List<int>();
		public AreaDrawer(Area area)
		{
			this.area = area;
			this.material = SolidColorMaterials.SimpleSolidColorMaterial(new Color(area.Color.r, area.Color.g, area.Color.b, 0.33f));
			this.material.renderQueue = 3600;
		}
		public void MarkForDraw()
		{
			this.wantDraw = true;
		}
		public void AreaDrawerUpdate()
		{
			if (this.wantDraw)
			{
				this.ActuallyDraw();
				this.wantDraw = false;
			}
		}
		private void ActuallyDraw()
		{
			if (this.dirty)
			{
				this.RegenerateMesh();
			}
			for (int i = 0; i < this.meshes.Count; i++)
			{
				Graphics.DrawMesh(this.meshes[i], Vector3.zero, Quaternion.identity, this.material, 0);
			}
		}
		public void SetDirty()
		{
			this.dirty = true;
		}
		public void RegenerateMesh()
		{
			for (int i = 0; i < this.meshes.Count; i++)
			{
				this.meshes[i].Clear();
			}
			int num = 0;
			int num2 = 0;
			if (this.meshes.Count < num + 1)
			{
				this.meshes.Add(new Mesh());
			}
			Mesh mesh = this.meshes[num];
			CellRect wholeMap = CellRect.WholeMap;
			float y = Altitudes.AltitudeFor(AltitudeLayer.WorldDataOverlay);
			for (int j = wholeMap.minX; j <= wholeMap.maxX; j++)
			{
				for (int k = wholeMap.minZ; k <= wholeMap.maxZ; k++)
				{
					if (this.area[CellIndices.CellToIndex(j, k)])
					{
						AreaDrawer.verts.Add(new Vector3((float)j, y, (float)k));
						AreaDrawer.verts.Add(new Vector3((float)j, y, (float)(k + 1)));
						AreaDrawer.verts.Add(new Vector3((float)(j + 1), y, (float)(k + 1)));
						AreaDrawer.verts.Add(new Vector3((float)(j + 1), y, (float)k));
						int count = AreaDrawer.verts.Count;
						AreaDrawer.tris.Add(count - 4);
						AreaDrawer.tris.Add(count - 3);
						AreaDrawer.tris.Add(count - 2);
						AreaDrawer.tris.Add(count - 4);
						AreaDrawer.tris.Add(count - 2);
						AreaDrawer.tris.Add(count - 1);
						num2++;
						if (num2 >= 16383)
						{
							this.FinalizeWorkingDataIntoMesh(mesh);
							num++;
							if (this.meshes.Count < num + 1)
							{
								this.meshes.Add(new Mesh());
							}
							mesh = this.meshes[num];
							num2 = 0;
						}
					}
				}
			}
			this.FinalizeWorkingDataIntoMesh(mesh);
			this.dirty = false;
		}
		private void FinalizeWorkingDataIntoMesh(Mesh mesh)
		{
			if (AreaDrawer.verts.Count > 0)
			{
				mesh.vertices = AreaDrawer.verts.ToArray();
				AreaDrawer.verts.Clear();
				mesh.SetTriangles(AreaDrawer.tris.ToArray(), 0);
				AreaDrawer.tris.Clear();
			}
		}
	}
}
