using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
namespace Verse
{
	public static class PlayDataLoader
	{
		public static bool loaded;
		public static void LoadAllPlayData(bool recovering = false)
		{
			if (PlayDataLoader.loaded)
			{
				Log.Error("Loading play data when already loaded. Call ClearAllPlayData first.");
				return;
			}
			DeepProfiler.Start("LoadAllPlayData");
			try
			{
				PlayDataLoader.DoPlayLoad();
			}
			catch (Exception arg)
			{
				if (!Prefs.ResetModsConfigOnCrash)
				{
					throw;
				}
				if (recovering)
				{
					Log.Warning("Could not recover from errors loading play data. Giving up.");
					throw;
				}
				IEnumerable<InstalledMod> activeMods = ModsConfig.ActiveMods;
				if (activeMods.Count<InstalledMod>() == 1 && activeMods.First<InstalledMod>().IsCoreMod)
				{
					throw;
				}
				Log.Warning("Caught exception while loading play data but there are active mods other than Core. Resetting mods config and trying again.\nThe exception was: " + arg);
				try
				{
					PlayDataLoader.ClearAllPlayData();
				}
				catch
				{
					Log.Warning("Caught exception while recovering from errors and trying to clear all play data. Ignoring it.\nThe exception was: " + arg);
				}
				ModsConfig.Reset();
				CrossRefLoader.Clear();
				PostLoadInitter.Clear();
				PlayDataLoader.LoadAllPlayData(true);
				return;
			}
			finally
			{
				DeepProfiler.End();
			}
			PlayDataLoader.loaded = true;
			if (recovering)
			{
				Log.Message("Successfully recovered from errors and loaded play data.");
				DelayedErrorWindowRequest.Add("RecoveredFromErrorsText".Translate(), "RecoveredFromErrorsDialogTitle".Translate());
			}
		}
		private static void DoPlayLoad()
		{
			GraphicDatabase.Clear();
			DeepProfiler.Start("Load all active mods.");
			try
			{
				LoadedModManager.LoadAllActiveMods();
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Load language metadata.");
			try
			{
				LanguageDatabase.LoadAllMetadata();
			}
			finally
			{
				DeepProfiler.End();
			}
			LongEventHandler.SetCurrentEventText("LoadingDefs".Translate());
			DeepProfiler.Start("Copy all Defs from mods to global databases.");
			try
			{
				foreach (Type current in typeof(Def).AllSubclasses())
				{
					GenGeneric.InvokeStaticMethodOnGenericType(typeof(DefDatabase<>), current, "AddAllInMods");
				}
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Resolve cross-references between non-implied Defs.");
			try
			{
				CrossRefLoader.ResolveAllWantedCrossReferences(FailMode.Silent);
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Rebind defs (early).");
			try
			{
				DefOfHelper.RebindAllDefOfs(true);
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Generate implied Defs (pre-resolve).");
			try
			{
				DefGenerator.GenerateImpliedDefs_PreResolve();
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Resolve cross-references between Defs made by the implied defs.");
			try
			{
				CrossRefLoader.ResolveAllWantedCrossReferences(FailMode.LogErrors);
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Rebind DefOfs (final).");
			try
			{
				DefOfHelper.RebindAllDefOfs(false);
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Other def binding and short hash giving.");
			try
			{
				ConceptDatabase.ReloadAndRebind();
				ConceptDecider.Reset();
				ShortHashGiver.Reset();
				CostListCalculator.Reset();
				PawnApparelGenerator.Reset();
				RestUtility.Reset();
				PawnWeaponGenerator.Reset();
				ThinkTreeKeyAssigner.Reset();
				ThingCategoryNodeDatabase.FinalizeInit();
				TrainableUtility.Reset();
				HaulAIUtility.Reset();
				WorkGiver_InteractAnimal.Reset();
				MedicalCareUtility.Reset();
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Resolve references.");
			try
			{
				foreach (Type current2 in typeof(Def).AllSubclasses())
				{
					if (current2 != typeof(ThingDef))
					{
						GenGeneric.InvokeStaticMethodOnGenericType(typeof(DefDatabase<>), current2, "ResolveAllReferences");
					}
				}
				DefDatabase<ThingDef>.ResolveAllReferences();
			}
			finally
			{
				DeepProfiler.End();
			}
			DeepProfiler.Start("Generate implied Defs (post-resolve).");
			try
			{
				DefGenerator.GenerateImpliedDefs_PostResolve();
			}
			finally
			{
				DeepProfiler.End();
			}
			if (Prefs.DevMode)
			{
				DeepProfiler.Start("Error check all defs.");
				try
				{
					foreach (Type current3 in typeof(Def).AllSubclasses())
					{
						GenGeneric.InvokeStaticMethodOnGenericType(typeof(DefDatabase<>), current3, "ErrorCheckAllDefs");
					}
				}
				finally
				{
					DeepProfiler.End();
				}
			}
			LongEventHandler.SetCurrentEventText("Initializing".Translate());
			DeepProfiler.Start("Load keyboard preferences.");
			try
			{
				KeyPrefs.Init();
			}
			finally
			{
				DeepProfiler.End();
			}
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				DeepProfiler.Start("Load backstories and bios.");
				try
				{
					BackstoryDatabase.ReloadAllBackstories();
					SolidBioDatabase.LoadAllBios();
				}
				finally
				{
					DeepProfiler.End();
				}
			});
			DeepProfiler.Start("Inject selected language data into defs.");
			try
			{
				LanguageDatabase.activeLanguage.InjectIntoDefs();
				GenLabel.ClearCache();
			}
			finally
			{
				DeepProfiler.End();
			}
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				StaticConstructorOnStartupUtility.CallAll();
			});
		}
		public static void ClearAllPlayData()
		{
			LanguageDatabase.Clear();
			LoadedModManager.ClearDestroy();
			foreach (Type current in typeof(Def).AllSubclasses())
			{
				GenGeneric.InvokeStaticMethodOnGenericType(typeof(DefDatabase<>), current, "Clear");
			}
			ThingCategoryNodeDatabase.Clear();
			BackstoryDatabase.Clear();
			SolidBioDatabase.Clear();
			PlayDataLoader.loaded = false;
		}
	}
}
