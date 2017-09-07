using System;
using UnityEngine;
namespace Verse
{
	public class Graphic_Mote : Graphic_Single
	{
		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing)
		{
			Mote mote = (Mote)thing;
			ThingDef def = mote.def;
			float moteAge = mote.MoteAge;
			float num = 1f;
			if (moteAge <= (float)def.mote.fadeinDuration && def.mote.fadeinDuration != 0)
			{
				num = moteAge / (float)def.mote.fadeinDuration;
			}
			else
			{
				if (moteAge >= (float)def.mote.ticksBeforeStartFadeout && def.mote.fadeoutDuration != 0)
				{
					num = 1f - (moteAge - (float)def.mote.ticksBeforeStartFadeout) / (float)def.mote.fadeoutDuration;
				}
			}
			Material material = this.MatSingle;
			if (mote.color.r != 1f || mote.color.g != 1f || mote.color.b != 1f || mote.color.a != 1f || num != 1f)
			{
				Color color = new Color(mote.color.r, mote.color.g, mote.color.b, mote.color.a * num);
				material = MaterialPool.MatFrom((Texture2D)material.mainTexture, material.shader, color);
			}
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(mote.DrawPos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), mote.exactScale);
			Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Mote(path=",
				this.path,
				", shader=",
				base.Shader,
				", color=",
				this.color,
				", colorTwo=unsupported)"
			});
		}
	}
}
