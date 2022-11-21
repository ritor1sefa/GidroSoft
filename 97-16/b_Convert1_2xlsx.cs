#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using System.Data;
using System.Linq;
using TMPro;
using ClosedXML.Excel;

namespace MaxyGames.Generated {
	public class b_Convert1_2xlsx : MaxyGames.RuntimeBehaviour {
		public Dictionary<string, SqliteConnection> sql_Connections = new Dictionary<string, SqliteConnection>();
		public Dictionary<string, SqliteCommand> sql_cmnds = new Dictionary<string, SqliteCommand>();
		public Dictionary<string, SqliteDataReader> sql_readers = new Dictionary<string, SqliteDataReader>();
		public bool FileNext = true;
		public string currentFile = "";
		public int currentFileN = 0;
		public string currentTable = "";
		public List<string> Files = new List<string>();
		public List<string> Tables = new List<string>();
		public int totalNrows = 0;
		public Dictionary<string, string> bd_names_raw = new Dictionary<string, string>();
		public XLWorkbook wb = new XLWorkbook();
		public IXLWorksheet wSh;
		public int row_int = 0;
		public List<string> row_list = new List<string>();
		public int clmn_int = 0;
		public string raw_value = "";
		public Regex variable6;
		public float cell_float = 0F;
		public int variable8 = 0;
		public string tmp_fileNameNormal = "";

		private void Start() {
			Files = new List<string>();
			Files.Clear();
			Tables = sql_master_tables("_empty");
			foreach(string loopObject in Directory.GetFiles(Application.streamingAssetsPath + "/" + "files/bd/", "*.sqlite")) {
				Files.Add(Path.GetFileNameWithoutExtension(loopObject));
				Files.Remove("_log");
				Files.Remove("_empty");
				Files.Remove("_emptyY");
			}
		}

		/// <summary>
		/// sqlite запрос на выборку столбца данных по году+месяцу
		/// SELECT * FROM "2"  Where "2th" LIKE "%2001" AND "2th" LIKE "02%";
		/// SELECT * FROM "2"  Where "2th" LIKE "%y2001%" AND "2th" LIKE "%m02%";
		/// </summary>
		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {}
		}

		public void button() {
			XLWorkbook variable01 = new XLWorkbook();
			IXLWorksheet variable11 = default(IXLWorksheet);
			base.StartCoroutine(xlsx_mainLoop());
		}

		/// <summary>
		/// подключение к бд, если не подключено. 
		/// </summary>
		private bool sql_connect(string db_name) {
			string path = "";
			SqliteConnection connection = default(SqliteConnection);
			path = Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(!(sql_Connections.ContainsKey(db_name))) {
				if(File.Exists(path)) {
					connection = new SqliteConnection("URI=file:" + path);
					connection.Open();
					//добавление в общий список открытых подключений
					sql_Connections.Add(db_name, connection);
					return true;
				} else {
					Debug.Log("бд нету=" + db_name);
					return false;
				}
			}
			return File.Exists(path);
		}

		/// <summary>
		/// нифига не работает почему то, на большом количестве разных таблиц.
		/// </summary>
		public void sql_close() {
			foreach(KeyValuePair<string, SqliteCommand> loopObject1 in sql_cmnds) {
				loopObject1.Value.Dispose();
			}
			foreach(KeyValuePair<string, SqliteConnection> loopObject2 in sql_Connections) {
				loopObject2.Value.Dispose();
			}
			SqliteConnection.ClearAllPools();
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			SqliteConnection.ClearAllPools();
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			sql_Connections = new Dictionary<string, SqliteConnection>();
			sql_readers = new Dictionary<string, SqliteDataReader>();
			sql_cmnds = new Dictionary<string, SqliteCommand>();
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// Ненужен?
		/// </summary>
		private List<string> sql_master_tables(string db_name) {
			List<string> tables = new List<string>();
			SqliteDataReader reader = default(SqliteDataReader);
			SqliteCommand cmnd = new SqliteCommand();
			tables.Clear();
			if(sql_connect(db_name)) {
				using(SqliteCommand value = sql_Connections[db_name].CreateCommand()) {
					cmnd = value;
					cmnd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
					reader = cmnd.ExecuteReader();
					while(reader.Read()) {
						tables.Add(reader.GetString(0));
					}
				}
			}
			return tables;
		}

		private List<List<string>> sql_getTable(string db_name, string q_Ntable) {
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteDataReader reader1 = default(SqliteDataReader);
			List<string> row = new List<string>();
			List<List<string>> table = new List<List<string>>();
			string tmp_cell = "";
			row.Clear();
			table.Clear();
			table = new List<List<string>>();
			row = new List<string>();
			if(sql_connect(db_name)) {
				using(SqliteCommand value1 = sql_Connections[db_name].CreateCommand()) {
					cmnd1 = value1;
					cmnd1.CommandText = "SELECT * FROM '" + q_Ntable + "'";
					reader1 = cmnd1.ExecuteReader();
					while(reader1.Read()) {
						//добавляем название файла в массив
						row.Add(currentFile);
						for(int index = 0; index < reader1.FieldCount; index += 1) {
							if(reader1.IsDBNull(index)) {
								tmp_cell = "";
							} else {
								tmp_cell = reader1.GetString(index);
							}
							row.Add(tmp_cell);
						}
						table.Add(row);
						row = new List<string>();
						row.Clear();
					}
				}
			} else {
				new _utillz()._2log("sql_insertQ.connect.error", true);
			}
			return table;
		}

		public void sql_log(string parameter, string parameter2) {
			sql_getTable("_log", "INSERT INTO log VALUES('" + parameter + parameter2 + "')");
		}

		/// <summary>
		/// Каждая таблица выбирается из каждого файла. (но не - каждый файл отдельно!)
		/// xlsx формируется потаблично
		/// </summary>
		public System.Collections.IEnumerator xlsx_mainLoop() {
			List<List<string>> finalTable = new List<List<string>>();
			int variable1 = 0;
			//Таблицы старт
			currentTable = "";
			for(int index1 = 0; index1 < Tables.Count; index1 += 1) {
				currentTable = Tables[index1];
				Debug.Log(currentTable);
				yield return new WaitForSeconds(0.05F);
				//Файлы старт
				currentFile = "";
				finalTable.Clear();
				finalTable = new List<List<string>>();
				for(int index2 = 0; index2 < Files.Count; index2 += 1) {
					currentFileN = index2;
					currentFile = Files[index2];
					//склеиваем из файлов в один массив все строки
					finalTable.AddRange(sql_getTable(currentFile, currentTable));
					System.Math.DivRem(finalTable.Count, 10, out variable1);
					if(variable1.Equals(0)) {
						yield return new WaitForEndOfFrame();
					}
				}
				Debug.Log(finalTable.Count);
				totalNrows = finalTable.Count;
				yield return xlsx_save(finalTable);
			}
		}

		public System.Collections.IEnumerator xlsx_save(List<List<string>> finalTable) {
			wb = new XLWorkbook();
			wSh = wb.Worksheets.Add(currentTable);
			for(int index3 = 0; index3 < finalTable.Count; index3 += 1) {
				row_int = (index3 + 1);
				row_list = finalTable[index3];
				for(int index4 = 0; index4 < row_list.Count; index4 += 1) {
					clmn_int = (clmn_int + 1);
					raw_value = row_list[index4];
					if((clmn_int == 1)) {
						//название
						wSh.Cell(row_int, clmn_int).Value = raw_value;
					} else {
						//убираем собак и меняем пробелы на подчерки
						raw_value = new Regex("[@]").Replace(raw_value.Replace("  ", "_").Replace(" ", "_"), "");
						if((clmn_int == 2)) {
							//проверка на дубликатность
							if(Regex.IsMatch(raw_value, "y(\\d+)_m(\\d+)_double")) {
								//повтор есть
								wSh.Cell(row_int, clmn_int).Value = "повтор";
								clmn_int = (clmn_int + 1);
								//год
								wSh.Cell(row_int, clmn_int).Value = Regex.Match(raw_value, "y(\\d+)_m(\\d+)").Result("$1");
								clmn_int = (clmn_int + 1);
								//месяц
								wSh.Cell(row_int, clmn_int).Value = Regex.Match(raw_value, "y(\\d+)_m(\\d+)").Result("$2");
							} else {
								//повтора нету
								wSh.Cell(row_int, clmn_int).Value = "";
								clmn_int = (clmn_int + 1);
								if(Regex.IsMatch(raw_value, "y(\\d+)_m(\\d+)")) {
									//год
									wSh.Cell(row_int, clmn_int).Value = Regex.Match(raw_value, "y(\\d+)_m(\\d+)").Result("$1");
									clmn_int = (clmn_int + 1);
									//месяц
									wSh.Cell(row_int, clmn_int).Value = Regex.Match(raw_value, "y(\\d+)_m(\\d+)").Result("$2");
								} else {
									Debug.Log(raw_value);
								}
							}
						} else if(float.TryParse(raw_value, out cell_float)) {
							wSh.Cell(row_int, clmn_int).SetValue<System.Single>(cell_float);
						} else {
							//название поста
							wSh.Cell(row_int, clmn_int).Value = raw_value;
						}
					}
				}
				clmn_int = 0;
				System.Math.DivRem(row_int, 1000, out variable8);
				if(variable8.Equals(0)) {
					yield return new WaitForEndOfFrame();
					Debug.Log(row_int);
				}
			}
			wb.SaveAs(UnityEngine.Device.Application.streamingAssetsPath + "/files/xlsx/" + "" + currentTable + ".xlsx");
			row_int = 0;
			row_list = new List<string>();
			yield return new WaitForEndOfFrame();
		}
	}
}
