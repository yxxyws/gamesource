using System;
using UnityEngine;
namespace Verse
{
	public class RootGameObject : MonoBehaviour
	{
		private void Start()
		{
			RootManager.Notify_LoadedLevelChanged();
			Find.RootRoot.Init();
		}
		private void Update()
		{
			Find.RootRoot.RootUpdate();
		}
		private void OnGUI()
		{
			Find.RootRoot.RootOnGUI();
		}
	}
}
