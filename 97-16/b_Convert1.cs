#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using System.Data;

namespace MaxyGames.Generated {
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		private Match cachedValue;
		public Dictionary<string, SqliteConnection> sql_Connections = new Dictionary<string, SqliteConnection>();
		public Dictionary<string, SqliteCommand> sql_cmnds = new Dictionary<string, SqliteCommand>();
		public Dictionary<string, SqliteDataReader> sql_readers = new Dictionary<string, SqliteDataReader>();
		private int index1;

		/// <summary>
		/// sqlite запрос на выборку столбца данных по году+месяцу
		/// SELECT * FROM "2"  Where "2th" LIKE "%2001" AND "2th" LIKE "02%";
		/// SELECT * FROM "2"  Where "2th" LIKE "%y2001%" AND "2th" LIKE "%m02%";
		/// </summary>
		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {}
		}

		public void loadFromFiles() {
			string path = "tmp.txt";
			string file_data = "";
			System.Array tables = new string[0];
			string One_table_data = "";
			string N_table = "";
			string N_year_N_month = "";
			path = Application.streamingAssetsPath + "/" + "tmp.txt";
			file_data = File.ReadAllText(path);
			tables = file_data.Trim().Split("Табли", System.StringSplitOptions.RemoveEmptyEntries);
			new _utillz()._2log("Количесто таблиц в файле: " + tables.Length.ToString(), false);
			foreach(object loopObject in tables) {
				One_table_data = loopObject.ToString();
				//Get Number of table
				N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\.\\D*\\n", RegexOptions.None).Result("$1");
				cachedValue = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None);
				N_year_N_month = cachedValue.Result("y$2_m$1");
				sql_insertTables(parseRow(One_table_data, _alllndexOfDelimeters(One_table_data)), N_table, N_year_N_month);
				sql_close();
			}
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		private List<int> _alllndexOfDelimeters(string table_data) {
			List<int> row_indexs_delimeters = new List<int>();
			string row = "";
			string[] table_data_splited = new string[0];
			int count_headers = 20;
			row_indexs_delimeters = new List<int>();
			table_data_splited = table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries);
			//на случай если строк мало
			//20-с потолка, поправить если что
			if((table_data_splited.Length < 20)) {
				count_headers = table_data_splited.Length;
			}
			//Берёт только первые строки - шапку
			for(int index = 0; index < count_headers; index += 1) {
				row = (table_data_splited.GetValue(index) as string);
				//Бегает по строке - ищет приключений
				for(index1 = row.IndexOfAny(new char[] { '╦', '┬' }); index1 > -1; index1 = row.IndexOfAny(new char[] { '┬', '╦' }, (index1 + 1))) {
					row_indexs_delimeters.Add(index1);
				}
			}
			return row_indexs_delimeters;
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		public List<List<string>> parseRow(string table_data, List<int> row_indexs_delimeters) {
			string tokenToSplitBy = "|";
			int insCount = -1;
			string line = "";
			int from = 0;
			int length = 0;
			int item = 0;
			List<string> _rowParsed = new List<string>();
			List<List<string>> _tableParsed = new List<List<string>>();
			row_indexs_delimeters.Sort();
			_tableParsed.Clear();
			foreach(string loopObject1 in table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				line = loopObject1;
				if(Regex.IsMatch(line.Trim(), "^\\d{1,3}\\.")) {
					_rowParsed = new List<string>();
					//Оставляем только номер, потому что названия на "русском" разные.
					_rowParsed.Add(Regex.Match(line.Substring(0, row_indexs_delimeters[0]), "\\D*(\\d+)\\D*", RegexOptions.None).Result("$1"));
					//1й вариант. вроде чуть быстрее ~15 секунд. против 19ти
					for(int index2 = 0; index2 < (row_indexs_delimeters.Count - 1); index2 += 1) {
						from = row_indexs_delimeters[index2];
						length = (row_indexs_delimeters[(index2 + 1)] - from);
						_rowParsed.Add(line.Substring(from, length).Trim());
					}
					//последний столбец
					_rowParsed.Add(line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
					_tableParsed.Add(_rowParsed);
				}
			}
			return _tableParsed;
		}

		/// <summary>
		/// подключение к бд, если не подключено. 
		/// </summary>
		private bool sql_connect(string db_name) {
			string path1 = "";
			SqliteConnection connection = default(SqliteConnection);
			path1 = Application.streamingAssetsPath + "/" + "files/" + db_name + ".sqlite";
			if(sql_Connections.ContainsKey(db_name)) {
				return true;
			} else {
				//Копирование пустой бд в новый файл
				if(!(File.Exists(path1))) {
					File.Copy(Application.streamingAssetsPath + "/" + "files/" + "_empty" + ".sqlite", path1, false);
				}
				connection = new SqliteConnection("URI=file:" + path1);
				connection.Open();
				//добавление в общий список открытых подключений
				sql_Connections.Add(db_name, connection);
			}
			return File.Exists(path1);
		}

		public void sql_close() {
			foreach(KeyValuePair<string, SqliteCommand> loopObject2 in sql_cmnds) {
				loopObject2.Value.Dispose();
			}
			foreach(KeyValuePair<string, SqliteConnection> loopObject3 in sql_Connections) {
				loopObject3.Value.Dispose();
			}
			sql_Connections = new Dictionary<string, SqliteConnection>();
			sql_readers = new Dictionary<string, SqliteDataReader>();
			sql_cmnds = new Dictionary<string, SqliteCommand>();
			SqliteConnection.ClearAllPools();
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// Ненужен?
		/// </summary>
		private List<string> sql_master_tables(string db_name) {
			List<string> tables1 = new List<string>();
			tables1.Clear();
			if(sql_connect(db_name)) {
				using(SqliteCommand value = sql_Connections[db_name].CreateCommand()) {
					sql_cmnds.Add(db_name, value);
					sql_cmnds[db_name].CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
					sql_readers.Add(db_name, sql_cmnds[db_name].ExecuteReader());
					while(sql_readers[db_name].Read()) {
						tables1.Add(sql_readers[db_name].GetString(0));
					}
				}
			}
			return tables1;
		}

		/// <summary>
		/// Выполняет одну вставку
		/// </summary>
		private int sql_insertQ(string db_name, string q) {
			int sql_writed = 0;
			SqliteCommand cmnd = new SqliteCommand();
			if(sql_connect(db_name)) {
				cmnd = sql_Connections[db_name].CreateCommand();
				cmnd.CommandText = q;
				using(SqliteDataReader value1 = cmnd.ExecuteReader()) {
					sql_writed = value1.RecordsAffected;
				}
			} else {
				new _utillz()._2log("sql_insertQ.connect.error", true);
			}
			return sql_writed;
		}

		/// <summary>
		/// Основной вставщик Таблиц в бд.
		/// Сделать под разные таблицы тут? => пробую зафигачить безшапочный вариант
		/// </summary>
		public int sql_insertTables(List<List<string>> q_table, string N_table, string N_year_N_month) {
			List<string> row1 = new List<string>();
			string db_name = "";
			foreach(List<string> loopObject4 in q_table) {
				row1 = loopObject4;
				db_name = row1[0];
				//убираем название бд из строки. ненужно
				row1.RemoveAt(0);
				if(N_table.Equals("1")) {
					//вставка в бд построчно
					sql_insertQ(db_name, "REPLACE INTO '" + N_table + "'" + "VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')");
					Debug.Log(N_table);
				} else {
					Debug.Log("не первая таблица");
				}
			}
			return 0;
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// not need?
		/// </summary>
		private List<string> sql_headers(string db_name) {
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteDataReader reader = default(SqliteDataReader);
			if(sql_connect(db_name)) {
				cmnd1 = sql_Connections[db_name].CreateCommand();
				cmnd1.CommandText = "PRAGMA table_info (1) ";
				reader = cmnd1.ExecuteReader();
				while(reader.Read()) {
					new _utillz()._2log((reader.GetValue(1) as string), false);
				}
			}
			return new List<string>();
		}
	}
}
