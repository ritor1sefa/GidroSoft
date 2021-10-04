#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using MaxyGames.Generated;
using NPOI.XSSF.UserModel;
using HtmlAgilityPack;
using System.Data;
using System.IO;

namespace MaxyGames.Generated {
	public class _xlsx : MaxyGames.RuntimeBehaviour {
		public MaxyGames.uNode.uNodeRuntime _sqlite = null;
		public bool newTmpFolder = false;

		public void Update() {
			if(Input.GetKeyUp(KeyCode.RightArrow)) {
				base.StartCoroutine(xlsxSet("pogodaiklimat2011", "37036", "2011,2012", true));
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

		public string FileManage(string target, string index_id, string yearList, bool monoFile) {
			string xlsxPatch = "";
			string tmp_Path = "";
			string xlsxTargetFile = "";
			xlsxPatch = Application.dataPath + "/StreamingAssets/files/" + target + "/xls/";
			//Создавать новые файлы в временых папках, либо перезаписывать ранее созданные в общей.
			if(newTmpFolder) {
				tmp_Path = xlsxPatch + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "/";
			} else {
				tmp_Path = xlsxPatch + index_id + "/";
			}
			//Существует ли пустой(предзаполненный)  файл-бланк. Влепить потом открытие папки с ним для ручной замены, и\или "автоматическое" обновление его, если был перетащен новый xlsx файл (избыточно?)
			if(!(File.Exists(xlsxPatch + "_blankSomeYear.xlsx"))) {
				Debug.Log("Xlsx бланк отсутствует! ");
			} else {
				Directory.CreateDirectory(tmp_Path);
				xlsxTargetFile = tmp_Path + index_id + "(" + yearList + ")" + ".xlsx";
				File.Copy(xlsxPatch + "_blankSomeYear.xlsx", xlsxTargetFile, true);
			}
			return xlsxTargetFile;
		}

		public System.Collections.IEnumerator xlsxSet(string target, string index_id, string yearList, bool monof) {
			XSSFWorkbook xslxFile = null;
			List<List<string>> tableFromBD = new List<List<string>>();
			List<string> SheetNames = new List<string>();
			string year_table_page = "";
			FileStream fsO = null;
			string Path = "";
			NPOI.SS.UserModel.ISheet currentSheet = null;
			int _RowStart = 0;
			int _CellStart = 0;
			int c_Row = 2;
			int c_Cell = 0;
			NPOI.SS.UserModel.IRow c_iRow = null;
			NPOI.SS.UserModel.ICell c_iCell = null;
			float cell_num_value = 0F;
			Path = FileManage(target, index_id, yearList, monof);
			using(FileStream value = File.Open(Path, FileMode.Open)) {
				xslxFile = new XSSFWorkbook(value);
				SheetNames = getSheetNames(xslxFile);
				//Года
				foreach(string loopObject in "2015".Split(new char[] { ',' })) {
					year_table_page = loopObject;
					tableFromBD = _sqlite.GetComponent<sqlite>().getData(index_id, "SELECT * FROM \"" + year_table_page + "\" ORDER BY \"date\"");
					if(!(SheetNames.Contains(year_table_page))) {
						//Если нужной страницы нету, делаем копию того, что точно должна быть в наличии
						xslxFile.GetSheet("2011").CopySheet(year_table_page);
						Debug.Log("Новая копия, страница: " + year_table_page);
					}
					Debug.Log(year_table_page);
					currentSheet = xslxFile.GetSheet(year_table_page);
					foreach(List<string> loopObject1 in tableFromBD) {
						c_Row = (c_Row + 1);
						try {
							c_iRow = currentSheet.GetRow(c_Row);
						}
						catch {
							c_iRow = currentSheet.CreateRow(c_Row);
							Debug.Log("row created");
						}
						finally {
							foreach(string loopObject2 in loopObject1) {
								c_Cell = (c_Cell + 1);
								if((c_Cell == 1)) {
									c_iRow.GetCell(1, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(System.DateTime.ParseExact(loopObject2, "yyyyMMddHH", null).Hour);
									c_iRow.GetCell(2, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(System.Convert.ToDouble(System.DateTime.ParseExact(loopObject2, "yyyyMMddHH", null).ToString("dd.MM")));
									c_Cell = 2;
								} else {
									c_iCell = c_iRow.GetCell(c_Cell, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK);
									if(float.TryParse(loopObject2, out cell_num_value)) {
										c_iCell.SetCellValue(System.Convert.ToDouble(loopObject2));
									} else {
										c_iCell.SetCellValue(loopObject2);
									}
								}
							}
							c_Cell = 0;
						}
					}
					c_Row = 2;
				}
			}
			using(FileStream value1 = new FileStream(Path, FileMode.Create, FileAccess.Write)) {
				//Сохраняем
				xslxFile.Write(value1);
			}
			yield break;
		}
	}
}
