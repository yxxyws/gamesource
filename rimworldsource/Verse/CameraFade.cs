using System;
using UnityEngine;
namespace Verse
{
	public class CameraFade : MonoBehaviour
	{
		private bool fadeTextureDirty = true;
		private GUIStyle backgroundStyle = new GUIStyle();
		private Texture2D fadeTexture;
		private Color currentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);
		private Color targetScreenOverlayColor = new Color(0f, 0f, 0f, 0f);
		private Color deltaColor = new Color(0f, 0f, 0f, 0f);
		private int fadeGUIDepth = -1000;
		private void Awake()
		{
			this.fadeTexture = new Texture2D(1, 1);
			this.fadeTexture.name = "CameraFader";
			this.backgroundStyle.normal.background = this.fadeTexture;
			this.SetScreenOverlayColor(this.currentScreenOverlayColor);
		}
		private void Update()
		{
			if (this.currentScreenOverlayColor != this.targetScreenOverlayColor)
			{
				if (Mathf.Abs(this.currentScreenOverlayColor.a - this.targetScreenOverlayColor.a) < Mathf.Abs(this.deltaColor.a) * Time.deltaTime)
				{
					this.currentScreenOverlayColor = this.targetScreenOverlayColor;
					this.SetScreenOverlayColor(this.currentScreenOverlayColor);
					this.deltaColor = new Color(0f, 0f, 0f, 0f);
				}
				else
				{
					this.SetScreenOverlayColor(this.currentScreenOverlayColor + this.deltaColor * Time.deltaTime);
				}
			}
		}
		private void OnGUI()
		{
			if (this.fadeTextureDirty)
			{
				this.fadeTexture.SetPixel(0, 0, this.currentScreenOverlayColor);
				this.fadeTexture.Apply();
			}
			if (this.currentScreenOverlayColor.a > 0f)
			{
				GUI.depth = this.fadeGUIDepth;
				GUI.Label(new Rect(-10f, -10f, (float)(Screen.width + 10), (float)(Screen.height + 10)), this.fadeTexture, this.backgroundStyle);
			}
		}
		public void SetScreenOverlayColor(Color newScreenOverlayColor)
		{
			this.currentScreenOverlayColor = newScreenOverlayColor;
			this.fadeTextureDirty = true;
		}
		public void StartFade(Color newScreenOverlayColor, float fadeDuration)
		{
			if (fadeDuration <= 0f)
			{
				this.SetScreenOverlayColor(newScreenOverlayColor);
			}
			else
			{
				this.targetScreenOverlayColor = newScreenOverlayColor;
				this.deltaColor = (this.targetScreenOverlayColor - this.currentScreenOverlayColor) / fadeDuration;
			}
		}
	}
}
