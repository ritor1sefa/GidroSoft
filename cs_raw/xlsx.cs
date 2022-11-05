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
using System.Globalization;
using TMPro;
using System.Diagnostics;

namespace MaxyGames.Generated {
	public class _xlsx_un : MaxyGames.RuntimeBehaviour {
		public bool newTmpFolder = false;
		public GameObject pogoda2011 = null;
		public TMP_InputField InputF_right_indexs = null;

		public void Update() {
			float variable0 = 0F;
			if(Input.GetKeyUp(KeyCode.UpArrow)) {
				UnityEngine.Debug.Log(System.Convert.ToDouble("+0.3", CultureInfo.InvariantCulture).GetType());
			}
		}

		public List<string> getSheetNames(NPOI.XSSF.UserModel.XSSFWorkbook wb) {
			List<string> Sheets = null;
			Sheets = new List<string>();
			for(int index = 0; index < wb.NumberOfSheets; index += 1) {
				Sheets.Add(wb.GetSheetName(index));
			}
			if(!(Sheets.Contains("2011"))) {
				UnityEngine.Debug.Log("В xlsx нету страницы 2011!!!");
			}
			return Sheets;
		}

		public string FileManage(string target, string index_id, List<string> yearList, bool monoFile) {
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
				UnityEngine.Debug.Log("Xlsx бланк отсутствует! ");
			} else {
				Directory.CreateDirectory(tmp_Path);
				xlsxTargetFile = tmp_Path + index_id + "(" + yearList[0] + "-" + yearList[(yearList.Count - 1)] + ")" + ".xlsx";
				File.Copy(xlsxPatch + "_blankSomeYear.xlsx", xlsxTargetFile, true);
			}
			return xlsxTargetFile;
		}

		public System.Collections.IEnumerator xlsxSet(string target, string index_id, string yearList, bool monof) {
			NPOI.XSSF.UserModel.XSSFWorkbook xslxFile = null;
			List<List<string>> tableFromBD1 = new List<List<string>>();
			List<string> SheetNames = new List<string>();
			string year_table_page1 = "";
			FileStream fsO = null;
			string Path1 = "";
			NPOI.SS.UserModel.ISheet currentSheet = null;
			int _RowStart1 = 0;
			int _CellStart = 0;
			int c_Row1 = 2;
			int c_Cell1 = 0;
			NPOI.SS.UserModel.IRow c_iRow = null;
			NPOI.SS.UserModel.ICell c_iCell = null;
			float cell_num_value = 0F;
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

		public System.Collections.IEnumerator xlsxSet_ClosedXML(string target, string index_id, List<string> yearList, bool monof) {
			NPOI.XSSF.UserModel.XSSFWorkbook xslxFile1 = null;
			List<List<string>> tableFromBD = new List<List<string>>();
			List<string> SheetNames1 = new List<string>();
			string year_table_page = "";
			string Path = "";
			int _RowStart = 4;
			int _CellStart1 = 2;
			int c_Row = 2;
			int c_Cell = 0;
			float cell_num_value1 = 0F;
			IXLWorksheet IXLcurrentSheet = null;
			XLWorkbook XLWB = null;
			List<string> c_row_list = new List<string>();
			int c_row_in_BD = 0;
			int c_item_in_list = 0;
			string c_item_value = "";
			IXLCell c_ixl_cell = null;
			NPOI.XSSF.UserModel.XSSFWorkbook variable17 = null;
			double cell_num_value_double = 0D;
			Path = FileManage(target, index_id, yearList, monof);
			new _Log()._2log(index_id + ": открытие", false);
			yield return new WaitForEndOfFrame();
			using(XLWorkbook value = new XLWorkbook(Path, XLEventTracking.Disabled)) {
				XLWB = value;
				new _Log()._2log(index_id + ": старт", false);
				yield return new WaitForEndOfFrame();
				//Года
				foreach(string loopObject in yearList) {
					year_table_page = loopObject;
					tableFromBD = new sqlite() { variable4 = null }.getDataUniversalLstLst(index_id, "SELECT * FROM \"" + year_table_page + "\" ORDER BY \"date\"");
					yield return new WaitForEndOfFrame();
					if(!(XLWB.Worksheets.Contains(year_table_page))) {
						XLWB.Worksheet("2011").CopyTo(year_table_page);
						new _Log()._2log(index_id + ": " + "Новая копия, страница: " + year_table_page, true);
						yield return new WaitForEndOfFrame();
					}
					IXLcurrentSheet = XLWB.Worksheet(year_table_page);
					for(int index1 = 0; index1 < tableFromBD.Count; index1 += 1) {
						c_row_in_BD = index1;
						c_row_list = tableFromBD[c_row_in_BD];
						//нужен ли?
						c_Row = (_RowStart + c_row_in_BD);
						for(int index2 = 0; index2 < c_row_list.Count; index2 += 1) {
							c_item_in_list = index2;
							c_item_value = c_row_list[c_item_in_list];
							if((c_item_in_list == 0)) {
								IXLcurrentSheet.Cell(c_Row, 2).SetValue<System.Double>(System.DateTime.ParseExact(c_item_value, "yyyyMMddHH", null).Hour);
								IXLcurrentSheet.Cell(c_Row, 3).SetValue<System.Double>(System.Convert.ToDouble(System.DateTime.ParseExact(c_item_value, "yyyyMMddHH", null).ToString("dd.MM"), CultureInfo.InvariantCulture));
								c_Cell = 4;
							} else {
								c_ixl_cell = IXLcurrentSheet.Cell(c_Row, c_Cell);
								if(double.TryParse(c_item_value, NumberStyles.Float, CultureInfo.InvariantCulture, out cell_num_value_double)) {
									c_ixl_cell.SetValue<System.Double>(cell_num_value_double);
								} else {
									c_ixl_cell.SetValue<System.String>(c_item_value);
								}
								c_Cell = (c_Cell + 1);
							}
							new WaitForEndOfFrame();
						}
					}
					yield return new WaitForEndOfFrame();
					yield return new WaitForEndOfFrame();
				}
				new _Log()._2log(index_id + ": сохранение", false);
				yield return new WaitForEndOfFrame();
				XLWB.Save(false, false);
				yield return new WaitForEndOfFrame();
				new _Log()._2log(index_id + ": cохранено", true);
			}
			yield break;
		}

		public void createXlsx_Pogodaklimat2011() {
			foreach(string loopObject1 in InputF_right_indexs.text.Split("|", System.StringSplitOptions.RemoveEmptyEntries)) {
				base.StartCoroutine(xlsxSet_ClosedXML("pogodaiklimat2011", loopObject1, pogoda2011.GetComponent<MaxyGames.Generated.pogodaiklimat2011_un>()._f_yearArray(), true));
			}
		}

		public void openfolder() {
			Process.Start(Application.dataPath + "/StreamingAssets/files/" + "pogodaiklimat2011/" + "xls/");
		}
	}
}
