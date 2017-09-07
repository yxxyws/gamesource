using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
namespace Verse
{
	public static class ConceptDatabase
	{
		private class ConceptKnowledge
		{
			public Dictionary<ConceptDef, float> knowledge = new Dictionary<ConceptDef, float>();
			public ConceptKnowledge()
			{
				foreach (ConceptDef current in DefDatabase<ConceptDef>.AllDefs)
				{
					this.knowledge.Add(current, 0f);
				}
			}
		}
		private static ConceptDatabase.ConceptKnowledge data;
		static ConceptDatabase()
		{
			ConceptDatabase.ReloadAndRebind();
		}
		public static void ReloadAndRebind()
		{
			ConceptDatabase.data = XmlLoader.ItemFromXmlFile<ConceptDatabase.ConceptKnowledge>(GenFilePaths.ConceptKnowledgeFilePath, true);
			foreach (ConceptDef current in DefDatabase<ConceptDef>.AllDefs)
			{
				if (!ConceptDatabase.data.knowledge.ContainsKey(current))
				{
					Log.Warning("Knowledge data was missing key " + current + ". Adding it...");
					ConceptDatabase.data.knowledge.Add(current, 0f);
				}
			}
		}
		public static void ResetPersistent()
		{
			FileInfo fileInfo = new FileInfo(GenFilePaths.ConceptKnowledgeFilePath);
			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
			ConceptDatabase.data = new ConceptDatabase.ConceptKnowledge();
		}
		public static void Save()
		{
			try
			{
				XDocument xDocument = new XDocument();
				XElement content = XmlSaver.XElementFromObject(ConceptDatabase.data, typeof(ConceptDatabase.ConceptKnowledge));
				xDocument.Add(content);
				xDocument.Save(GenFilePaths.ConceptKnowledgeFilePath);
			}
			catch (Exception ex)
			{
				GenUI.ErrorDialog("ProblemSavingFile".Translate(new object[]
				{
					GenFilePaths.ConceptKnowledgeFilePath,
					ex.ToString()
				}));
				Log.Error("Exception saving knowledge database: " + ex);
			}
		}
		public static float GetKnowledge(ConceptDef conc)
		{
			if (conc == null)
			{
				Log.Error("Cannot get knowledge for null ConceptDef.");
				return 100f;
			}
			return ConceptDatabase.data.knowledge[conc];
		}
		public static void SetKnowledge(ConceptDef conc, float know)
		{
			ConceptDatabase.data.knowledge[conc] = know;
		}
		public static bool IsComplete(ConceptDef conc)
		{
			return ConceptDatabase.data.knowledge[conc] > 99.9f;
		}
		public static void KnowledgeDemonstrated(ConceptDef conc, KnowledgeAmount know)
		{
			float num = 1000f;
			switch (know)
			{
			case KnowledgeAmount.GuiFrame:
				num = 0.15f;
				break;
			case KnowledgeAmount.FrameInteraction:
				num = 0.8f;
				break;
			case KnowledgeAmount.TinyInteraction:
				num = 3f;
				break;
			case KnowledgeAmount.SmallInteraction:
				num = 10f;
				break;
			case KnowledgeAmount.SpecificInteraction:
				num = 40f;
				break;
			case KnowledgeAmount.Total:
				num = 100f;
				break;
			case KnowledgeAmount.NoteClosed:
				num = 50f;
				break;
			case KnowledgeAmount.NoteTaught:
				num = 100f;
				break;
			default:
				Log.Error("Missing knowledge amount for " + know);
				break;
			}
			float num2 = ConceptDatabase.GetKnowledge(conc);
			num2 += num;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			ConceptDatabase.SetKnowledge(conc, num2);
			ConceptDecider.Notify_KnowledgeDemonstrated(conc);
		}
	}
}
