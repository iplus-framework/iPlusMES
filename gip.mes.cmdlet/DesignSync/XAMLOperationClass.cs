using gip.core.datamodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.cmdlet.DesignSync
{
    public class XAMLOperationClass
    {
		#region 
		public  void DownloadACClassAllDesings(Database db, string rootFolder, string[] classACIdentifiers)
		{
			foreach (var classACIdentifier in classACIdentifiers)
			{
				DownloadACClassAllDesings(db, rootFolder, classACIdentifier);
			}
		}

		public  void DownloadACClassAllDesings(Database db, string rootFolder, string classACIdentifier)
		{
			string classFolder = Path.Combine(rootFolder, classACIdentifier);
			if (!Directory.Exists(classFolder))
				Directory.CreateDirectory(classFolder);
			ACClass acClass = db.ACClass.FirstOrDefault(p => p.ACIdentifier == classACIdentifier);
			if (acClass != null)
			{
				List<ACClassDesign> designs = acClass.ACClassDesign_ACClass.ToList();
				List<DefineDesignTitle> translations =
				designs
				.Select(c => new DefineDesignTitle()
				{
					ACIdentifier = c.ACIdentifier,
					CaptionTuple = c.ACCaptionTranslation
				})
				.ToList();
				WriteTranslation(classFolder, translations);
				designs.ForEach(p => DownloadACClassOneDesing(db, classFolder, classACIdentifier, p.ACIdentifier));
			}
		}

		public  void DownloadACClassOneDesing(Database db, string rootFolder, string classACIdentifier, string designACIdentifier, bool writeTranslation = false)
		{
			ACClassDesign design = db.ACClassDesign.FirstOrDefault(c => c.ACClass.ACIdentifier == classACIdentifier && c.ACIdentifier == designACIdentifier);
			DownloadACClassOneDesing(rootFolder, design, writeTranslation);
		}

		public  void DownloadACClassOneDesing(string rootFolder, ACClassDesign design, bool writeTranslation = false)
		{
			string designFileName = Path.Combine(rootFolder, design.ACIdentifier + ".xaml");
			if (File.Exists(designFileName))
			{
				File.Delete(designFileName);
			}
			File.WriteAllText(designFileName, design.XMLDesign, Encoding.UTF8);
		}

		#endregion

		#region Upload

		public  void UploadACClassAllDesings(Database db, string rootFolder, string[] classACIdentifiers)
		{
			foreach (var classACIdentifier in classACIdentifiers)
			{
				UploadACClassAllDesings(db, rootFolder, classACIdentifier);
			}
		}

		public  void UploadACClassAllDesings(Database db, string rootFolder, string classACIdentifier)
		{
			string fullFolder = rootFolder + @"\" + classACIdentifier;
			List<string> fileNames = new DirectoryInfo(fullFolder).GetFiles("*.xaml").Select(c => c.Name).ToList();
			List<DefineDesignTitle> translations = ReadTranslation(rootFolder, classACIdentifier);
			fileNames.ForEach(p => UploadACClassOneDesing(db, rootFolder, classACIdentifier, p.Replace(".xaml", ""), translations));
		}


		public  void UploadACClassOneDesing(Database db, string rootFolder, string classACIdentifier, string designACIdentifier, List<DefineDesignTitle> translations = null)
		{
			DefineDesignTitle translation = null;
			if (translations == null)
				translations = ReadTranslation(rootFolder, classACIdentifier);
			string designFileName = rootFolder + @"\" + classACIdentifier + @"\" + designACIdentifier + ".xaml";
			if (File.Exists(designFileName))
			{

				ACClassDesign design = db.ACClassDesign.FirstOrDefault(c => c.ACClass.ACIdentifier == classACIdentifier && c.ACIdentifier == designACIdentifier);
				if (design == null)
				{

					ACClass cls = db.ACClass.FirstOrDefault(c => c.ACIdentifier == classACIdentifier);
					ACClass vbDesignClass = db.ACClass.FirstOrDefault(c => c.ACIdentifier == @"VBDesign");
					design = new ACClassDesign();
					design.ACClassDesignID = Guid.NewGuid();
					design.ACClassID = cls.ACClassID;
					design.ACIdentifier = designACIdentifier;
					design.ACCaptionTranslation = string.Format(@"en{{'0'}}de{{'0'}}", designACIdentifier);
					// design.DesignNo = "";
					design.ValueTypeACClassID = vbDesignClass.ACClassID;
					design.ACKindIndex = 12010;
					design.ACUsageIndex = (designACIdentifier.ToLower() == "mainlayout") ? (short)4200 : (short)4210;
					design.SortIndex = 999;
					design.IsRightmanagement = false;
					design.Comment = "";
					//				design.IsDefault = "";
					//				design.IsResourceStyle = "";
					//				design.VisualHeight = "";
					//				design.VisualWidth = "";
					//				design.XMLConfig = "";
					design.BranchNo = 0;
					design.InsertName = "SUP";
					design.InsertDate = DateTime.Now;
					db.ACClassDesign.Add(design);
				}
				design.XMLDesign = File.ReadAllText(designFileName, Encoding.UTF8);
				design.UpdateDate = DateTime.Now;
				design.UpdateName = "SUP";

				if (translations != null)
				{
					translation = translations.FirstOrDefault(c => c.ACIdentifier == design.ACIdentifier);
					if (translation != null)
						design.ACCaptionTranslation = translation.ToString();
				}


				db.ACSaveChanges();
			}
		}

		/*
			Tuple<string,string> => ACClass.ACIdentifier, 
		*/
		public  void UploadComposite(Database db, string rootFolder, string compositeString)
		{
			List<Tuple<string, string>> inputValues =
			compositeString.Split(Environment.NewLine.ToCharArray())
			.Where(c => !string.IsNullOrEmpty(c))
			.Select(c =>

			{
				string[] tmp = c.Split('\\').ToArray();
				return new Tuple<string, string>(tmp[0], tmp[1]);

			})
			.ToList();
			foreach (var element in inputValues)
			{
				UploadACClassOneDesing(db, rootFolder, element.Item1, element.Item2);
			}
		}
		#endregion

		#region Translation

		public  List<DefineDesignTitle> ReadTranslation(string rootFolder, string classACIdentifier)
		{
			List<DefineDesignTitle> translationList = null;
			string fullFileName = Path.Combine(rootFolder, classACIdentifier, "Translation.json");
			if (File.Exists(fullFileName))
			{
				string content = File.ReadAllText(fullFileName, Encoding.UTF8);
				translationList = JsonConvert.DeserializeObject<List<DefineDesignTitle>>(content);
			}
			return translationList;
		}

		public  void WriteTranslation(string rootFolder, List<DefineDesignTitle> translations)
		{
			string fullFileName = Path.Combine(rootFolder, "Translation.json");
			string content = JsonConvert.SerializeObject(translations, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(fullFileName, content, Encoding.UTF8);
		}

		#endregion
	}
}
