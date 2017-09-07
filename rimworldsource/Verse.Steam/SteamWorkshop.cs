using RimWorld;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse.Sound;
namespace Verse.Steam
{
	public static class SteamWorkshop
	{
		private static InstalledMod curMod;
		private static UGCUpdateHandle_t curUpdateHandle;
		private static WorkshopInteractStage curStage = WorkshopInteractStage.None;
		private static List<PublishedFileId_t> downloadingMods = new List<PublishedFileId_t>();
		private static CallResult<SubmitItemUpdateResult_t> submitResult;
		private static CallResult<CreateItemResult_t> createResult;
		private static Callback<ItemInstalled_t> itemInstalledCallback;
		private static Callback<RemoteStoragePublishedFileSubscribed_t> itemSubscribedCallback;
		private static Callback<RemoteStoragePublishedFileUnsubscribed_t> itemUnsubscribedCallback;
		public static List<PublishedFileId_t> DownloadingMods = new List<PublishedFileId_t>();
		public static WorkshopInteractStage CurStage
		{
			get
			{
				return SteamWorkshop.curStage;
			}
		}
		internal static void Init()
		{
			SteamWorkshop.itemInstalledCallback = Callback<ItemInstalled_t>.Create(new Callback<ItemInstalled_t>.DispatchDelegate(SteamWorkshop.OnItemInstalled));
			SteamWorkshop.itemSubscribedCallback = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(new Callback<RemoteStoragePublishedFileSubscribed_t>.DispatchDelegate(SteamWorkshop.OnItemSubscribed));
			SteamWorkshop.itemUnsubscribedCallback = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(new Callback<RemoteStoragePublishedFileUnsubscribed_t>.DispatchDelegate(SteamWorkshop.OnItemUnsubscribed));
		}
		internal static void OnItemInstalled(ItemInstalled_t result)
		{
			if (result.m_unAppID != SteamUtils.GetAppID())
			{
				return;
			}
			Log.Message("Workshop: Item installed: " + result.m_nPublishedFileId);
			InstalledModLister.RebuildModList();
			if (SteamWorkshop.downloadingMods.Contains(result.m_nPublishedFileId))
			{
				SteamWorkshop.downloadingMods.Remove(result.m_nPublishedFileId);
			}
		}
		internal static void OnItemSubscribed(RemoteStoragePublishedFileSubscribed_t result)
		{
			if (result.m_nAppID != SteamUtils.GetAppID())
			{
				return;
			}
			Log.Message("Workshop: Item subscribed: " + result.m_nPublishedFileId);
			InstalledModLister.RebuildModList();
			if (!InstalledModLister.AllInstalledMods.Any((InstalledMod m) => m.PublishedFileId == result.m_nPublishedFileId))
			{
				SteamWorkshop.downloadingMods.Add(result.m_nPublishedFileId);
			}
		}
		internal static void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t result)
		{
			if (result.m_nAppID != SteamUtils.GetAppID())
			{
				return;
			}
			Log.Message("Workshop: Item unsubscribed: " + result.m_nPublishedFileId);
			InstalledModLister.RebuildModList();
			if (SteamWorkshop.downloadingMods.Contains(result.m_nPublishedFileId))
			{
				SteamWorkshop.downloadingMods.Remove(result.m_nPublishedFileId);
			}
		}
		internal static void Upload(InstalledMod mod)
		{
			if (SteamWorkshop.curStage != WorkshopInteractStage.None)
			{
				Messages.Message("UploadAlreadyInProgress".Translate(), MessageSound.RejectInput);
				return;
			}
			SoundDefOf.TickHigh.PlayOneShotOnCamera();
			SteamWorkshop.curMod = mod;
			if (mod.PublishedFileId != PublishedFileId_t.Invalid)
			{
				Log.Message(string.Concat(new object[]
				{
					"Workshop: Starting item update for mod ",
					mod.Name,
					" in folder ",
					mod.Identifier,
					" with PublishedFileId ",
					mod.PublishedFileId
				}));
				SteamWorkshop.curUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), mod.PublishedFileId);
				SteamWorkshop.curStage = WorkshopInteractStage.SubmittingItem;
				SteamWorkshop.SetItemData();
				SteamWorkshop.submitResult = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(SteamWorkshop.OnItemSubmitted));
				SteamAPICall_t hAPICall = SteamUGC.SubmitItemUpdate(SteamWorkshop.curUpdateHandle, "[Auto-generated text]: Update on " + DateTime.Now.ToString() + ".");
				SteamWorkshop.submitResult.Set(hAPICall, null);
			}
			else
			{
				Log.Message("Workshop: Starting item creation for mod " + mod.Name + " in folder " + mod.Identifier);
				SteamWorkshop.curStage = WorkshopInteractStage.CreatingItem;
				SteamWorkshop.createResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(SteamWorkshop.OnItemCreated));
				SteamAPICall_t hAPICall2 = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst);
				SteamWorkshop.createResult.Set(hAPICall2, null);
			}
			Find.WindowStack.Add(new Dialog_WorkshopOperation());
		}
		internal static void Unsubscribe(InstalledMod mod)
		{
			SteamUGC.UnsubscribeItem(mod.PublishedFileId);
		}
		private static void OnItemCreated(CreateItemResult_t result, bool IOFailure)
		{
			if (IOFailure)
			{
				string text = "Workshop: OnCreateItem IOFailure. Result: " + result.m_eResult.GetLabel();
				Log.Error(text);
				Find.WindowStack.Add(new Dialog_Message(text, null));
				return;
			}
			SteamWorkshop.curMod.PublishedFileId = result.m_nPublishedFileId;
			Log.Message("Workshop: Item created. PublishedFileId: " + SteamWorkshop.curMod.PublishedFileId);
			SteamWorkshop.curUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), SteamWorkshop.curMod.PublishedFileId);
			SteamWorkshop.SetItemData();
			SteamWorkshop.curStage = WorkshopInteractStage.SubmittingItem;
			Log.Message("Workshop: Submitting item.");
			SteamWorkshop.submitResult = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(SteamWorkshop.OnItemSubmitted));
			SteamAPICall_t hAPICall = SteamUGC.SubmitItemUpdate(SteamWorkshop.curUpdateHandle, "[Auto-generated text]: Initial upload.");
			SteamWorkshop.submitResult.Set(hAPICall, null);
			SteamWorkshop.createResult = null;
		}
		private static void OnItemSubmitted(SubmitItemUpdateResult_t result, bool IOFailure)
		{
			if (IOFailure)
			{
				string text = "Workshop: OnSubmitItem IOFailure. Result: " + result.m_eResult.GetLabel();
				Log.Error(text);
				Find.WindowStack.Add(new Dialog_Message(text, null));
				return;
			}
			Log.Message("Workshop: Item submit result: " + result.m_eResult);
			SteamWorkshop.curStage = WorkshopInteractStage.None;
			if (result.m_eResult != EResult.k_EResultOK)
			{
				Find.WindowStack.Add(new Dialog_Message("WorkshopSubmissionFailed".Translate(new object[]
				{
					GenText.SplitCamelCase(result.m_eResult.GetLabel())
				}), null));
			}
			else
			{
				SteamUtility.OpenUrlSteamIfPossible(SteamWorkshop.curMod.SteamWorkshopPageUrl);
				Messages.Message("WorkshopUploadSucceeded".Translate(new object[]
				{
					SteamWorkshop.curMod.Name
				}), MessageSound.Benefit);
			}
			SteamWorkshop.submitResult = null;
		}
		private static void SetItemData()
		{
			SteamUGC.SetItemTitle(SteamWorkshop.curUpdateHandle, SteamWorkshop.curMod.Name);
			SteamUGC.SetItemContent(SteamWorkshop.curUpdateHandle, SteamWorkshop.curMod.Directory.FullName);
			SteamUGC.SetItemDescription(SteamWorkshop.curUpdateHandle, SteamWorkshop.curMod.Description);
			SteamUGC.SetItemPreview(SteamWorkshop.curUpdateHandle, SteamWorkshop.curMod.PreviewImagePath);
		}
		public static void GetUpdateStatus(out EItemUpdateStatus updateStatus, out float progPercent)
		{
			ulong num;
			ulong num2;
			updateStatus = SteamUGC.GetItemUpdateProgress(SteamWorkshop.curUpdateHandle, out num, out num2);
			progPercent = num / num2;
		}
		[DebuggerHidden]
		public static IEnumerable<PublishedFileId_t> AllSubscribedItems()
		{
			SteamWorkshop.<AllSubscribedItems>c__Iterator1AF <AllSubscribedItems>c__Iterator1AF = new SteamWorkshop.<AllSubscribedItems>c__Iterator1AF();
			SteamWorkshop.<AllSubscribedItems>c__Iterator1AF expr_07 = <AllSubscribedItems>c__Iterator1AF;
			expr_07.$PC = -2;
			return expr_07;
		}
		[DebuggerHidden]
		public static IEnumerable<string> WorkshopModsFolders()
		{
			SteamWorkshop.<WorkshopModsFolders>c__Iterator1B0 <WorkshopModsFolders>c__Iterator1B = new SteamWorkshop.<WorkshopModsFolders>c__Iterator1B0();
			SteamWorkshop.<WorkshopModsFolders>c__Iterator1B0 expr_07 = <WorkshopModsFolders>c__Iterator1B;
			expr_07.$PC = -2;
			return expr_07;
		}
		public static void LogStatus()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("All subscribed items (there are " + SteamUGC.GetNumSubscribedItems() + "):");
			foreach (PublishedFileId_t current in SteamWorkshop.AllSubscribedItems())
			{
				stringBuilder.AppendLine(SteamWorkshop.ItemStatusString(current));
			}
			stringBuilder.AppendLine("----------------------");
			stringBuilder.AppendLine("All extant mods:");
			foreach (InstalledMod current2 in InstalledModLister.AllInstalledMods)
			{
				stringBuilder.AppendLine(current2.Identifier + ": " + SteamWorkshop.ItemStatusString(current2.PublishedFileId));
			}
			Log.Message(stringBuilder.ToString());
		}
		private static string ItemStatusString(PublishedFileId_t pfid)
		{
			if (pfid == PublishedFileId_t.Invalid)
			{
				return "[unpublished]";
			}
			string text = "[" + pfid + "] ";
			ulong num;
			string str;
			bool flag;
			if (SteamUGC.GetItemInstallInfo(pfid, out num, out str, 1024u, out flag))
			{
				text += "\n   installed";
				text = text + "\n   folder=" + str;
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"\n   sizeOnDisk=",
					num / 1024uL,
					"Kb"
				});
			}
			else
			{
				text += "\n   not installed";
			}
			return text;
		}
	}
}
