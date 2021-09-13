#pragma warning disable
using UnityEngine;
using System.IO;
using System.Data;
using System.Collections.Generic;
using HtmlAgilityPack;
using NPOI.XSSF.UserModel;
using MaxyGames.Generated;

namespace MaxyGames.Generated {
	public class xlsx : MaxyGames.RuntimeBehaviour {
		public bool newTmpFolder = false;
		public string StateSwicth = "";
		public MaxyGames.uNode.uNodeRuntime sqlite = null;

		public void Start() {}

		public void Update() {
			if(Input.GetKeyUp(KeyCode.RightArrow)) {
				base.StartCoroutine(xlsxSet("pogodaiklimat2011", "29838", "2011,2012", true));
			}
		}

		/// <summary>
		/// возвращает ссылку на xlsx файл
		/// </summary>
		private string FileManage(string target, string index, string yearList, bool monoFile) {
			string xlsxPatch = "";
			string tmp_Path = "";
			string xlsxTargetFile = "";
			xlsxPatch = "" + Application.dataPath + "/StreamingAssets/files/" + target + "/xls/";
			//Создавать новые файлы в временых папках, либо перезаписывать ранее созданные в общей.
			if(newTmpFolder) {
				tmp_Path = xlsxPatch + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "/";
			} else {
				tmp_Path = xlsxPatch + index + "/";
			}
			//Существует ли пустой(предзаполненный)  файл-бланк. Влепить потом открытие папки с ним для ручной замены, и\или "автоматическое" обновление его, если был перетащен новый xlsx файл (избыточно?)
			if(!(File.Exists(xlsxPatch + "_blankSomeYear.xlsx"))) {
				Debug.Log("Xlsx бланк отсутствует! ");
			} else {
				Directory.CreateDirectory(tmp_Path);
				xlsxTargetFile = tmp_Path + index + "(" + yearList + ")" + ".xlsx";
				File.Copy(xlsxPatch + "_blankSomeYear.xlsx", xlsxTargetFile, true);
			}
			return xlsxTargetFile;
		}

		public System.Collections.IEnumerator xlsxSet(string target, string index, string yearList, bool monoFile) {
			XSSFWorkbook xslxFile = null;
			List<List<string>> tableFromBD = null;
			List<string> SheetNames = null;
			string year_table_page = "";
			FileStream fs = null;
			using(FileStream value = File.Open(FileManage("pogodaiklimat2011", "29838", "2011,2012", true), FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				fs = value;
				xslxFile = new XSSFWorkbook(fs);
				SheetNames = getSheetNames(xslxFile);
				//Года
				foreach(string loopObject in "2011,2012".Split(new char[] { ',' })) {
					year_table_page = loopObject;
					tableFromBD = sqlite.GetComponent<sqlite>().getData(index, "SELECT * FROM \"" + year_table_page + "\"");
					if(!(SheetNames.Contains(year_table_page))) {
						//Если нужной страницы нету, делаем копию того, что точно должна быть в наличии
						xslxFile.GetSheet("2011").CopySheet(year_table_page);
						Debug.Log("Новая копия, страница: " + year_table_page);
					}
					Debug.Log(year_table_page);
				}
				xslxFile.Write(fs);
				//Закрывашка
				xslxFile.Close();
				yield return xslxFile;
			}
		}

		public List<string> getSheetNames(XSSFWorkbook wb) {
			List<string> Sheets = null;
			Sheets = new List<string>();
			for(int index = 0; index < wb.NumberOfSheets; index += 1) {
				Sheets.Add(wb.GetSheetName(index));
			}
			if(!(Sheets.Contains("2011"))) {
				Debug.Log("В xlsx нету страницы 2011!!!");
			}
			return Sheets;
		}
	}
}
