using RimWorld;
using System;
using UnityEngine;
namespace Verse
{
	public class CameraMap
	{
		private const float MaxDeltaTime = 0.025f;
		private const float ScreenDollyEdgeWidth = 20f;
		private const float ScreenDollyEdgeWidth_BottomFullscreen = 6f;
		private const float MinDurationForMouseToTouchScreenBottomEdgeToDolly = 0.28f;
		private const float MapEdgeClampMarginCells = -2f;
		private const float StartingSize = 24f;
		private const float MinSize = 11f;
		private const float MaxSize = 60f;
		private const float ZoomSpeed = 2.6f;
		private const float ZoomTightness = 0.4f;
		private const float ZoomScaleFromAltDenominator = 35f;
		private const float PageKeyZoomRate = 4f;
		private const float ScrollWheelZoomRate = 0.35f;
		private const float MinAltitude = 15f;
		private const float MaxAltitude = 65f;
		public CameraShaker shaker = new CameraShaker();
		public CameraMapConfig config = new CameraMapConfig_Normal();
		public Vector3 currentRealPosition;
		private Vector3 camVelocity;
		private Vector3 camRootPos;
		public float camRootSize;
		private float desiredSize;
		private Vector2 mouseDragVect = Vector2.zero;
		private Vector2 dollyVect = Vector2.zero;
		private bool mouseCoveredByUI;
		private float mouseTouchingScreenBottomEdgeStartTime = -1f;
		private static int lastViewRectGetFrame = -1;
		private static CellRect lastViewRect;
		private float ScreenDollyEdgeWidthBottom
		{
			get
			{
				if (Screen.fullScreen)
				{
					return 6f;
				}
				return 20f;
			}
		}
		public CameraZoomRange CurrentZoom
		{
			get
			{
				if (this.camRootSize < 12f)
				{
					return CameraZoomRange.Closest;
				}
				if (this.camRootSize < 13.8f)
				{
					return CameraZoomRange.Close;
				}
				if (this.camRootSize < 42f)
				{
					return CameraZoomRange.Middle;
				}
				if (this.camRootSize < 57f)
				{
					return CameraZoomRange.Far;
				}
				return CameraZoomRange.Furthest;
			}
		}
		public IntVec3 MapPosition
		{
			get
			{
				IntVec3 result = this.currentRealPosition.ToIntVec3();
				result.y = 0;
				return result;
			}
		}
		public CellRect CurrentViewRect
		{
			get
			{
				if (Time.frameCount != CameraMap.lastViewRectGetFrame)
				{
					CameraMap.lastViewRect = default(CellRect);
					float num = (float)Screen.width / (float)Screen.height;
					CameraMap.lastViewRect.minX = Mathf.FloorToInt(this.currentRealPosition.x - this.camRootSize * num - 1f);
					CameraMap.lastViewRect.maxX = Mathf.CeilToInt(this.currentRealPosition.x + this.camRootSize * num);
					CameraMap.lastViewRect.minZ = Mathf.FloorToInt(this.currentRealPosition.z - this.camRootSize - 1f);
					CameraMap.lastViewRect.maxZ = Mathf.CeilToInt(this.currentRealPosition.z + this.camRootSize);
					CameraMap.lastViewRectGetFrame = Time.frameCount;
				}
				return CameraMap.lastViewRect;
			}
		}
		private float HitchReduceFactor
		{
			get
			{
				float result = 1f;
				if (Time.deltaTime > 0.025f)
				{
					result = 0.025f / Time.deltaTime;
				}
				return result;
			}
		}
		public CameraMap()
		{
			this.ResetSize();
		}
		private Vector2 UpdateAndGetCurInputDollyVect()
		{
			Vector2 vector = this.dollyVect;
			bool flag = false;
			if ((Game.isEditor || Screen.fullScreen) && Prefs.EdgeScreenScroll && !this.mouseCoveredByUI)
			{
				Vector2 vector2 = Input.mousePosition;
				Vector2 point = vector2;
				point.y = (float)Screen.height - point.y;
				Rect rect = new Rect(0f, 0f, 200f, 200f);
				Rect rect2 = new Rect((float)(Screen.width - 250), 0f, 255f, 255f);
				Rect rect3 = new Rect(0f, (float)(Screen.height - 250), 225f, 255f);
				Rect rect4 = new Rect((float)(Screen.width - 250), (float)(Screen.height - 250), 255f, 255f);
				MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainTabDefOf.Inspect.Window;
				if (Find.MainTabsRoot.OpenTab == MainTabDefOf.Inspect && mainTabWindow_Inspect.CurDrawHeight > rect3.height)
				{
					rect3.yMin = (float)Screen.height - mainTabWindow_Inspect.CurDrawHeight;
				}
				if (!rect.Contains(point) && !rect3.Contains(point) && !rect2.Contains(point) && !rect4.Contains(point))
				{
					Vector2 b = new Vector2(0f, 0f);
					if (vector2.x >= 0f && vector2.x < 20f)
					{
						b.x -= this.config.dollyRateScreenEdge;
					}
					if (vector2.x <= (float)Screen.width && vector2.x > (float)Screen.width - 20f)
					{
						b.x += this.config.dollyRateScreenEdge;
					}
					if (vector2.y <= (float)Screen.height && vector2.y > (float)Screen.height - 20f)
					{
						b.y += this.config.dollyRateScreenEdge;
					}
					if (vector2.y >= 0f && vector2.y < this.ScreenDollyEdgeWidthBottom)
					{
						if (this.mouseTouchingScreenBottomEdgeStartTime < 0f)
						{
							this.mouseTouchingScreenBottomEdgeStartTime = Time.realtimeSinceStartup;
						}
						if (Time.realtimeSinceStartup - this.mouseTouchingScreenBottomEdgeStartTime >= 0.28f)
						{
							b.y -= this.config.dollyRateScreenEdge;
						}
						flag = true;
					}
					vector += b;
				}
			}
			if (!flag)
			{
				this.mouseTouchingScreenBottomEdgeStartTime = -1f;
			}
			if (Input.GetKey(KeyCode.LeftShift))
			{
				vector *= 2.4f;
			}
			return vector;
		}
		public void ResetSize()
		{
			this.desiredSize = 24f;
			this.camRootSize = this.desiredSize;
		}
		public void Expose()
		{
			Scribe.EnterNode("cameraMap");
			Scribe_Values.LookValue<Vector3>(ref this.camRootPos, "camRootPos", default(Vector3), false);
			Scribe_Values.LookValue<float>(ref this.desiredSize, "desiredSize", 0f, false);
			this.camRootSize = this.desiredSize;
			Scribe.ExitNode();
		}
		public void CameraOnGUI()
		{
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			this.mouseCoveredByUI = false;
			if (Find.WindowStack.GetWindowAt(GenUI.AbsMousePosition()) != null)
			{
				this.mouseCoveredByUI = true;
			}
			if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
			{
				this.mouseDragVect = Event.current.delta;
				Event.current.Use();
			}
			float num = 0f;
			if (Event.current.type == EventType.ScrollWheel)
			{
				num -= Event.current.delta.y * 0.35f;
				ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraZoom, KnowledgeAmount.TinyInteraction);
			}
			if (KeyBindingDefOf.MapZoomIn.KeyDownEvent)
			{
				num -= 4f;
				ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraZoom, KnowledgeAmount.SmallInteraction);
			}
			if (KeyBindingDefOf.MapZoomOut.KeyDownEvent)
			{
				num += 4f;
				ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraZoom, KnowledgeAmount.SmallInteraction);
			}
			this.desiredSize -= num * 2.6f * this.camRootSize / 35f;
			if (this.desiredSize < 11f)
			{
				this.desiredSize = 11f;
			}
			if (this.desiredSize > 60f)
			{
				this.desiredSize = 60f;
			}
			this.dollyVect = Vector3.zero;
			if (KeyBindingDefOf.MapDollyLeft.IsDown)
			{
				this.dollyVect.x = -this.config.dollyRateKeys;
			}
			if (KeyBindingDefOf.MapDollyRight.IsDown)
			{
				this.dollyVect.x = this.config.dollyRateKeys;
			}
			if (KeyBindingDefOf.MapDollyUp.IsDown)
			{
				this.dollyVect.y = this.config.dollyRateKeys;
			}
			if (KeyBindingDefOf.MapDollyDown.IsDown)
			{
				this.dollyVect.y = -this.config.dollyRateKeys;
			}
			if (this.mouseDragVect != Vector2.zero)
			{
				this.mouseDragVect *= this.HitchReduceFactor;
				this.mouseDragVect.x = this.mouseDragVect.x * -1f;
				this.dollyVect += this.mouseDragVect * this.config.dollyRateMouseDrag;
				this.mouseDragVect = Vector2.zero;
			}
		}
		public void CameraUpdate()
		{
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			Vector2 lhs = this.UpdateAndGetCurInputDollyVect();
			if (lhs != Vector2.zero)
			{
				float d = (this.camRootSize - 11f) / 49f * 0.7f + 0.3f;
				this.camVelocity = new Vector3(lhs.x, 0f, lhs.y) * d;
				ConceptDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraDolly, KnowledgeAmount.FrameInteraction);
			}
			if (!Find.WindowStack.WindowsPreventCameraMotion)
			{
				float d2 = Time.deltaTime * this.HitchReduceFactor;
				this.camRootPos += this.camVelocity * d2 * this.config.moveSpeedScale;
				if (this.camRootPos.x > (float)Find.Map.Size.x + -2f)
				{
					this.camRootPos.x = (float)Find.Map.Size.x + -2f;
				}
				if (this.camRootPos.z > (float)Find.Map.Size.z + -2f)
				{
					this.camRootPos.z = (float)Find.Map.Size.z + -2f;
				}
				if (this.camRootPos.x < 2f)
				{
					this.camRootPos.x = 2f;
				}
				if (this.camRootPos.z < 2f)
				{
					this.camRootPos.z = 2f;
				}
			}
			if (this.camVelocity != Vector3.zero)
			{
				this.camVelocity *= this.config.camSpeedDecayFactor;
				if (this.camVelocity.magnitude < 0.1f)
				{
					this.camVelocity = Vector3.zero;
				}
			}
			float num = this.desiredSize - this.camRootSize;
			this.camRootSize += num * 0.4f;
			this.camRootPos.y = 15f + (this.camRootSize - 11f) / 49f * 50f;
			this.shaker.Update();
			this.currentRealPosition = this.camRootPos + this.shaker.ShakeOffset;
		}
		public void JumpTo(Vector3 newLookAt)
		{
			this.camRootPos = new Vector3(newLookAt.x, this.camRootPos.y, newLookAt.z);
		}
		public void JumpTo(IntVec3 IntLoc)
		{
			this.JumpTo(IntLoc.ToVector3Shifted());
		}
		public float CellSizePixels()
		{
			return (float)Screen.height / (this.camRootSize * 2f);
		}
		public Vector2 InvertedWorldToScreenPoint(Vector3 worldLoc)
		{
			Vector3 vector = Find.CameraMapGameObject.camera.WorldToScreenPoint(worldLoc);
			vector.y = (float)Screen.height - vector.y;
			return new Vector2(vector.x, vector.y);
		}
	}
}
