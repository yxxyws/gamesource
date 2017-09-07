using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Verse
{
	public static class SavedWorldsDatabase
	{
		public static IEnumerable<FileInfo> AllWorldFiles
		{
			get
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(GenFilePaths.WorldsFolderPath);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				return 
					from f in directoryInfo.GetFiles()
					where f.Extension == ".rww"
					orderby f.LastWriteTime descending
					select f;
			}
		}
	}
}
