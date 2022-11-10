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
			List<string> _rowUnparsed = new List<string>();
			path = Application.streamingAssetsPath + "/" + "tmp.txt";
			file_data = File.ReadAllText(path);
			tables = file_data.Trim().Split("Табли", System.StringSplitOptions.RemoveEmptyEntries);
			new _utillz()._2log("Количесто таблиц в файле: " + tables.Length.ToString(), false);
			foreach(object loopObject in tables) {
				One_table_data = loopObject.ToString();
				if(One_table_data.StartsWith("ца")) {
					//Get Number of table
					N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*\\n", RegexOptions.None).Result("$1");
					if(N_table.Contains("11")) {
						//Get subNumber of table N11
						N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*(\\d+)\\).*\\n", RegexOptions.None).Result("$1_$2");
					}
					cachedValue = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None);
					N_year_N_month = cachedValue.Result("y$2_m$1");
					_rowUnparsed = splitTable(One_table_data, N_table);
					sql_insertTables(parseRow(_alllndexOfDelimeters(_rowUnparsed), _rowUnparsed), N_table, N_year_N_month);
				}
			}
			sql_close();
		}

		/// <summary>
		/// Пилим таблицы, при необходимости
		/// </summary>
		public List<string> splitTable(string One_table_data, string N_table) {
			List<string> _rowsUnparsed = new List<string>();
			List<string> _2thPart = default(List<string>);
			_rowsUnparsed.Clear();
			_rowsUnparsed = new List<string>();
			_2thPart = new List<string>();
			//возможно заменить на свитч?
			if(N_table.Equals("12")) {
				//пилим 12 таблицу пополам
				foreach(string loopObject1 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
					_rowsUnparsed.Add(loopObject1.Substring(0, (loopObject1.Length / 2)));
					_2thPart.Add(loopObject1.Substring((loopObject1.Length / 2), (loopObject1.Length - (loopObject1.Length / 2))));
				}
				//склеиваем таблицы
				_rowsUnparsed.AddRange(_2thPart);
			} else {
				//непилимые таблицы
				foreach(string loopObject2 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
					_rowsUnparsed.Add((loopObject2 as string));
				}
			}
			return _rowsUnparsed;
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		private List<int> _alllndexOfDelimeters(List<string> _rowsUnparsed) {
			List<int> row_indexs_delimeters = new List<int>();
			string row = "";
			string[] table_data_splited = new string[0];
			int count_headers = 20;
			row_indexs_delimeters = new List<int>();
			row_indexs_delimeters.Clear();
			//на случай если строк мало
			//20-с потолка, поправить если что
			if((_rowsUnparsed.Count < 20)) {
				count_headers = _rowsUnparsed.Count;
			}
			//Берёт только первые строки - шапку
			for(int index = 0; index < count_headers; index += 1) {
				row = _rowsUnparsed[index];
				//Бегает по строке - ищет приключений
				for(index1 = row.IndexOfAny(new char[] { '╦', '┬' }); index1 > -1; index1 = row.IndexOfAny(new char[] { '┬', '╦' }, (index1 + 1))) {
					row_indexs_delimeters.Add(index1);
				}
			}
			return row_indexs_delimeters;
		}

		/// <summary>
		/// Пролучаем массив строк-ячеек из таблицы, чистые и обработанные
		/// </summary>
		public List<List<string>> parseRow(List<int> row_indexs_delimeters, List<string> _rowsUnparsed) {
			string tokenToSplitBy = "|";
			int insCount = -1;
			string line = "";
			int from = 0;
			int length = 0;
			int item = 0;
			List<List<string>> _tableParsed = new List<List<string>>();
			List<string> _rowsParsed = default(List<string>);
			row_indexs_delimeters.Sort();
			_tableParsed = new List<List<string>>();
			//построчная обработка
			foreach(string loopObject3 in _rowsUnparsed) {
				line = loopObject3;
				if(Regex.IsMatch(line.Trim(), "^\\d{1,3}\\.")) {
					_rowsParsed = new List<string>();
					//Оставляем только номер, потому что одинаковые названия на "русском" разные.
					_rowsParsed.Add(Regex.Match(line.Substring(0, row_indexs_delimeters[0]), "\\D*(\\d+)\\.\\D*", RegexOptions.None).Result("$1"));
					//1й вариант. вроде чуть быстрее ~15 секунд. против 19ти
					for(int index2 = 0; index2 < (row_indexs_delimeters.Count - 1); index2 += 1) {
						from = row_indexs_delimeters[index2];
						length = (row_indexs_delimeters[(index2 + 1)] - from);
						_rowsParsed.Add(line.TrimStart().Substring(from, length).Trim());
					}
					//последний столбец
					_rowsParsed.Add(line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
					_tableParsed.Add(_rowsParsed);
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

		/// <summary>
		/// нифига не работает почему то, на большом количестве разных таблиц.
		/// </summary>
		public void sql_close() {
			foreach(KeyValuePair<string, SqliteCommand> loopObject4 in sql_cmnds) {
				loopObject4.Value.Dispose();
			}
			foreach(KeyValuePair<string, SqliteConnection> loopObject5 in sql_Connections) {
				loopObject5.Value.Dispose();
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
				using(SqliteCommand value1 = sql_Connections[db_name].CreateCommand()) {
					cmnd = value1;
					cmnd.CommandText = q;
					sql_writed = cmnd.ExecuteReader().RecordsAffected;
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
			string q = "";
			string q_simple = "";
			List<string> _11_2_tmp = default(List<string>);
			foreach(List<string> loopObject6 in q_table) {
				row1 = loopObject6;
				db_name = row1[0];
				//убираем название бд из строки. ненужно
				row1.RemoveAt(0);
				q_simple = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
				//set Q in N11 table
				switch(N_table) {
					case "0": {
					}
					break;
					case "1": {
						q = q_simple;
					}
					break;
					case "2": {
						q = q_simple;
					}
					break;
					case "3": {
					}
					break;
					case "4": {
						q = q_simple;
					}
					break;
					case "5": {
					}
					break;
					case "6": {
						q = q_simple;
					}
					break;
					case "7": {
						q = q_simple;
					}
					break;
					case "8": {
						q = q_simple;
					}
					break;
					case "9": {
					}
					break;
					case "10": {
					}
					break;
					case "11_1": {
						q = "REPLACE INTO '" + "11" + "' " + "('N_year_N_month','dl','dj','mp','ld','jo','c','cl','zc','kc','kl','to','cm','clm','tom','gd','il','r','i','gl','izm','glTs','dm','T','tp')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "11_2": {
						_11_2_tmp = new List<string>();
						for(int index3 = 0; index3 < "tl,tlp,tz,tlz,toc,tzo,tt,tto,mgc,p,mc,mo,mn,mm,mg,pp,pb,pbIL,G,pc,Sh,V,sCh,mJ".Split(",", System.StringSplitOptions.None).Length; index3 += 1) {
							_11_2_tmp.Add(("tl,tlp,tz,tlz,toc,tzo,tt,tto,mgc,p,mc,mo,mn,mm,mg,pp,pb,pbIL,G,pc,Sh,V,sCh,mJ".Split(",", System.StringSplitOptions.None).GetValue(index3) as string) + " = " + "'" + row1[index3] + "'");
						}
						q = "UPDATE '" + "11" + "'" + " SET " + string.Join<System.String>(", ", _11_2_tmp) + " WHERE " + "N_year_N_month='" + N_year_N_month + "'";
					}
					break;
					case "12": {
						q = q_simple;
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					case "": {
					}
					break;
					default: {
						q = "";
					}
					break;
				}
				//двусоставная таблица
				if((q.Length > 10)) {
					//вставка в бд построчно
					sql_insertQ(db_name, q);
					Debug.Log(q);
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
