using System;
using UnityEngine;
namespace Verse
{
	public class CameraMapGameObject : MonoBehaviour
	{
		private const float ReverbDummyAltitude = 65f;
		private GameObject reverbDummy;
		private void Awake()
		{
			this.reverbDummy = GameObject.Find("ReverbZoneDummy");
		}
		private void OnPreCull()
		{
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			Find.WeatherManager.DrawAllWeather();
		}
		private void OnGUI()
		{
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			Find.CameraMap.CameraOnGUI();
		}
		private void Update()
		{
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			CameraMap cameraMap = Find.CameraMap;
			cameraMap.CameraUpdate();
			base.camera.orthographicSize = cameraMap.camRootSize;
			base.camera.transform.position = cameraMap.currentRealPosition;
			Vector3 position = base.transform.position;
			position.y = 65f;
			this.reverbDummy.transform.position = position;
		}
	}
}
