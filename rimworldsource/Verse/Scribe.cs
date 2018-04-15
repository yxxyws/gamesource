using System;
using System.IO;
using System.Xml;
namespace Verse
{
	public static class Scribe
	{
		public static LoadSaveMode mode = LoadSaveMode.Inactive;
		public static XmlNode curParent = null;
		public static bool writingForDebug = false;
		private static Stream saveStream = null;
		private static XmlWriter writer = null;
		private static LoadSaveMode oldMode = Scribe.mode;
		private static XmlNode oldParent = Scribe.curParent;
		public static void SaveState()
		{
			Scribe.oldMode = Scribe.mode;
			Scribe.oldParent = Scribe.curParent;
		}
		public static void RestoreState()
		{
			Scribe.curParent = Scribe.oldParent;
			Scribe.mode = Scribe.oldMode;
		}
		public static void EnterNode(string elementName)
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				Scribe.writer.WriteStartElement(elementName);
			}
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				Scribe.curParent = Scribe.curParent[elementName];
			}
		}
		public static void ExitNode()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				Scribe.writer.WriteEndElement();
			}
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				Scribe.curParent = Scribe.curParent.ParentNode;
			}
		}
		public static void InitLoading(string filePath)
		{
			using (StreamReader streamReader = new StreamReader(filePath))
			{
				using (XmlTextReader xmlTextReader = new XmlTextReader(streamReader))
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(xmlTextReader);
					Scribe.curParent = xmlDocument.DocumentElement;
				}
			}
			Scribe.mode = LoadSaveMode.LoadingVars;
		}
		public static void FinalizeLoading()
		{
			Scribe.ExitNode();
		}
		public static void InitWriting(string filePath, string documentElementName)
		{
			DebugLoadIDsSavingErrorsChecker.Clear();
			Scribe.mode = LoadSaveMode.Saving;
			Scribe.saveStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "\t";
			Scribe.writer = XmlWriter.Create(Scribe.saveStream, xmlWriterSettings);
			Scribe.writer.WriteStartDocument();
			Scribe.EnterNode(documentElementName);
		}
		public static void FinalizeWriting()
		{
			if (Scribe.writer != null)
			{
				Scribe.ExitNode();
				Scribe.writer.WriteEndDocument();
				Scribe.writer.Close();
				Scribe.writer = null;
			}
			if (Scribe.saveStream != null)
			{
				Scribe.saveStream.Close();
				Scribe.saveStream = null;
			}
			Scribe.mode = LoadSaveMode.Inactive;
			DebugLoadIDsSavingErrorsChecker.CheckForErrorsAndClear();
		}
		public static void WriteElement(string elementName, string value)
		{
			Scribe.writer.WriteElementString(elementName, value);
		}
		public static void WriteAttribute(string attributeName, string value)
		{
			Scribe.writer.WriteAttributeString(attributeName, value);
		}
		public static string DebugOutputFor(IExposable saveable)
		{
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (Scribe.writer = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "  ",
					OmitXmlDeclaration = true
				}))
				{
					LoadSaveMode loadSaveMode = Scribe.mode;
					DebugLoadIDsSavingErrorsChecker.Clear();
					Scribe.mode = LoadSaveMode.Saving;
					Scribe.writingForDebug = true;
					Scribe_Deep.LookDeep<IExposable>(ref saveable, "Saveable", new object[0]);
					Scribe.mode = loadSaveMode;
					DebugLoadIDsSavingErrorsChecker.CheckForErrorsAndClear();
					Scribe.writingForDebug = false;
					result = stringWriter.ToString();
				}
			}
			return result;
		}
	}
}
