#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using MaxyGames.Generated;
using HtmlAgilityPack;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using ClosedXML;
using DocumentFormat.OpenXml.Office2013.Excel;
using Aspose.Cells;

namespace MaxyGames.Generated {
	public class _xlsx : MaxyGames.RuntimeBehaviour {
		public MaxyGames.uNode.uNodeRuntime _sqlite = null;
		public bool newTmpFolder = false;

		public void Update() {}

		public List<string> getSheetNames(NPOI.XSSF.UserModel.XSSFWorkbook wb) {
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
			NPOI.XSSF.UserModel.XSSFWorkbook xslxFile = null;
			List<List<string>> tableFromBD = new List<List<string>>();
			List<string> SheetNames = new List<string>();
			string year_table_page = "";
			FileStream fsO = null;
			string Path = "";
			NPOI.SS.UserModel.ISheet currentSheet = null;
			int _RowStart1 = 0;
			int _CellStart = 0;
			int c_Row = 2;
			int c_Cell = 0;
			NPOI.SS.UserModel.IRow c_iRow = null;
			NPOI.SS.UserModel.ICell c_iCell = null;
			float cell_num_value = 0F;
			Path = FileManage(target, index_id, yearList, monof);
			using(FileStream value = File.Open(Path, FileMode.Open)) {
				xslxFile = new NPOI.XSSF.UserModel.XSSFWorkbook(value);
				SheetNames = getSheetNames(xslxFile);
				//Года
				foreach(string loopObject in "2015".Split(new char[] { ',' })) {
					year_table_page = loopObject;
					tableFromBD = new List<List<string>>();
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

		public System.Collections.IEnumerator NewFunction() {
			double double_num = 3.82D;
			XLWorkbook wb__blankSomeYear = null;
			string path = "";
			IXLWorksheet _2011 = null;
			IXLWorksheet _2012 = null;
			yield return null;
		}

		public System.Collections.IEnumerator xlsxSet_ClosedXML(string target, string index_id, string yearList, bool monof) {
			NPOI.XSSF.UserModel.XSSFWorkbook xslxFile1 = null;
			List<List<string>> tableFromBD1 = new List<List<string>>();
			List<string> SheetNames1 = new List<string>();
			string year_table_page1 = "";
			string Path1 = "";
			int _RowStart = 4;
			int _CellStart1 = 2;
			int c_Row1 = 2;
			int c_Cell1 = 0;
			float cell_num_value1 = 0F;
			IXLWorksheet IXLcurrentSheet = null;
			XLWorkbook XLWB = null;
			List<string> c_row_list = new List<string>();
			int c_row_in_BD = 0;
			int c_item_in_list = 0;
			string c_item_value = "";
			IXLCell c_ixl_cell = null;
			NPOI.XSSF.UserModel.XSSFWorkbook variable17 = null;
			Path1 = FileManage(target, index_id, yearList, monof);
			Debug.Log("открытие");
			yield return new WaitForEndOfFrame();
			using(XLWorkbook value2 = new XLWorkbook(Path1, XLEventTracking.Disabled)) {
				XLWB = value2;
				Debug.Log("цикл");
				yield return new WaitForEndOfFrame();
				//Года
				foreach(string loopObject3 in "2011,2012,2013,2014,2015,2016,2017,2018,2019,2020,2021".Split(new char[] { ',' })) {
					year_table_page1 = loopObject3;
					tableFromBD1 = _sqlite.GetComponent<sqlite>().getData(index_id, "SELECT * FROM \"" + year_table_page1 + "\" ORDER BY \"date\"");
					if(!(XLWB.Worksheets.Contains(year_table_page1))) {
						XLWB.Worksheet("2011").CopyTo(year_table_page1);
						Debug.Log("Новая копия, страница: " + year_table_page1);
					}
					Debug.Log(year_table_page1);
					IXLcurrentSheet = XLWB.Worksheet(year_table_page1);
					for(int index1 = 0; index1 < tableFromBD1.Count; index1 += 1) {
						c_row_in_BD = index1;
						c_row_list = tableFromBD1[c_row_in_BD];
						//нужен ли?
						c_Row1 = (_RowStart + c_row_in_BD);
						for(int index2 = 0; index2 < c_row_list.Count; index2 += 1) {
							c_item_in_list = index2;
							c_item_value = c_row_list[c_item_in_list];
							if((c_item_in_list == 0)) {
								IXLcurrentSheet.Cell(c_Row1, 2).SetValue<System.Double>(System.DateTime.ParseExact(c_item_value, "yyyyMMddHH", null).Hour);
								IXLcurrentSheet.Cell(c_Row1, 3).SetValue<System.Double>(System.Convert.ToDouble(System.DateTime.ParseExact(c_item_value, "yyyyMMddHH", null).ToString("dd.MM")));
								c_Cell1 = 4;
							} else {
								c_ixl_cell = IXLcurrentSheet.Cell(c_Row1, c_Cell1);
								if(float.TryParse(c_item_value, out cell_num_value1)) {
									c_ixl_cell.SetValue<System.Double>(System.Convert.ToDouble(c_item_value));
								} else {
									c_ixl_cell.SetValue<System.String>(c_item_value);
								}
								c_Cell1 = (c_Cell1 + 1);
							}
						}
					}
					yield return new WaitForEndOfFrame();
				}
				Debug.Log("Сохранение");
				yield return new WaitForEndOfFrame();
				XLWB.Save(false, false);
				yield return new WaitForEndOfFrame();
				Debug.Log("сохранено");
			}
			yield break;
		}
	}
}
