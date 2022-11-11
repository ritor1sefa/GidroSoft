#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using System.Data;
using System.Linq;

namespace MaxyGames.Generated {
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		private Match cachedValue;
		public Dictionary<string, SqliteConnection> sql_Connections = new Dictionary<string, SqliteConnection>();
		public Dictionary<string, SqliteCommand> sql_cmnds = new Dictionary<string, SqliteCommand>();
		public Dictionary<string, SqliteDataReader> sql_readers = new Dictionary<string, SqliteDataReader>();
		private int index;

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
					//Переделать на свитч?
					if("11." + "21.".Contains(N_table)) {
						//Get subNumber of table N11&N21
						N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*(\\d+)\\).*\\n", RegexOptions.None).Result("$1_$2");
					}
					cachedValue = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None);
					N_year_N_month = cachedValue.Result("y$2_m$1");
					_rowUnparsed = splitTable(One_table_data, N_table);
					sql_insertTables(parseRow(_alllndexOfDelimeters(_rowUnparsed), _rowUnparsed, N_table), N_table, N_year_N_month);
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
			switch(N_table) {
				case "12": {
					//пилим 12 таблицу пополам
					foreach(string loopObject1 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
						_rowsUnparsed.Add(loopObject1.Substring(0, (loopObject1.Length / 2)));
						_2thPart.Add(loopObject1.Substring((loopObject1.Length / 2), (loopObject1.Length - (loopObject1.Length / 2))));
					}
					//склеиваем таблицы
					_rowsUnparsed.AddRange(_2thPart);
				}
				break;
				default: {
					//непилимые таблицы
					foreach(string loopObject2 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
						_rowsUnparsed.Add((loopObject2 as string));
					}
				}
				break;
			}
			return _rowsUnparsed;
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		private List<int> _alllndexOfDelimeters(List<string> _rowsUnparsed) {
			string row = "";
			string[] table_data_splited = new string[0];
			HashSet<int> tmp_hash_ints = new HashSet<int>();
			foreach(string loopObject3 in _rowsUnparsed) {
				if(Regex.IsMatch(loopObject3.Trim(), "^\\d{1,3}\\.")) {
					return Enumerable.ToList<System.Int32>(tmp_hash_ints);
				} else {
					row = loopObject3;
					//Бегает по строке - ищет приключений
					for(index = row.IndexOfAny(new char[] { '╦', '┬', '|' }); index > -1; index = row.IndexOfAny(new char[] { '┬', '╦', '|' }, (index + 1))) {
						tmp_hash_ints.Add(index);
					}
				}
			}
			return Enumerable.ToList<System.Int32>(tmp_hash_ints);
		}

		/// <summary>
		/// Пролучаем массив строк-ячеек из таблицы, чистые и обработанные
		/// </summary>
		public List<List<string>> parseRow(List<int> row_indexs_delimeters, List<string> _rowsUnparsed, string N_table) {
			string tokenToSplitBy = "|";
			int insCount = -1;
			string line = "";
			int from = 0;
			int length = 0;
			int item = 0;
			List<List<string>> _tableParsed = new List<List<string>>();
			List<string> _rowsParsed = default(List<string>);
			bool headerSkiped = false;
			string tmp_db_name = "";
			int tmp_startLine = 0;
			row_indexs_delimeters.Sort();
			_tableParsed = new List<List<string>>();
			//Для таблиц с несколькими строчками. 14+
			headerSkiped = false;
			//построчная обработка
			foreach(string loopObject4 in _rowsUnparsed) {
				line = loopObject4;
				if(Regex.IsMatch(line, "^ +═")) {
					//Сдвиг строки для кривой таблицы N12, второй её половины
					tmp_startLine = (line.Length - line.TrimStart().Length);
				}
				if((Regex.IsMatch(line.Trim(), "^\\d{1,3}\\.") || headerSkiped)) {
					headerSkiped = true;
					_rowsParsed = new List<string>();
					//Проверка на конец таблицы
					if(((row_indexs_delimeters[0] > line.Length) || string.IsNullOrEmpty(line.Substring(row_indexs_delimeters[0], (line.Length - row_indexs_delimeters[0])).Trim()))) {
						headerSkiped = false;
					} else {
						if(string.IsNullOrEmpty(line.Substring(0, row_indexs_delimeters[0]).Trim())) {
							//добавляем данные в первый столбец
							line = tmp_db_name + line.Substring(tmp_db_name.Length, (line.Length - tmp_db_name.Length));
						} else {
							//сохранение имени бд, на случай пустой следующей строки
							tmp_db_name = Regex.Match(line.Substring(0, row_indexs_delimeters[0]), "^\\D*(\\d+)\\.", RegexOptions.None).Result("$1");
						}
						//Оставляем только номер, потому что одинаковые названия на "русском" разные.
						_rowsParsed.Add(tmp_db_name);
						for(int index1 = 0; index1 < (row_indexs_delimeters.Count - 1); index1 += 1) {
							from = row_indexs_delimeters[index1];
							length = (row_indexs_delimeters[(index1 + 1)] - from);
							_rowsParsed.Add(line.Substring((from + tmp_startLine), length).Trim());
						}
						//последний столбец
						_rowsParsed.Add(line.Substring((row_indexs_delimeters[row_indexs_delimeters.Count - 1] + tmp_startLine), (line.Length - (row_indexs_delimeters[row_indexs_delimeters.Count - 1] + tmp_startLine))).Trim());
						_tableParsed.Add(_rowsParsed);
					}
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
			foreach(KeyValuePair<string, SqliteCommand> loopObject5 in sql_cmnds) {
				loopObject5.Value.Dispose();
			}
			foreach(KeyValuePair<string, SqliteConnection> loopObject6 in sql_Connections) {
				loopObject6.Value.Dispose();
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
			List<string> _21_2_tmp = default(List<string>);
			foreach(List<string> loopObject7 in q_table) {
				row1 = loopObject7;
				db_name = row1[0];
				//убираем название бд из строки. ненужно
				row1.RemoveAt(0);
				Debug.Log("файл:" + db_name);
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
						//N11_1
						q = "REPLACE INTO '" + "11" + "' " + "('N_year_N_month','dl','dj','mp','ld','jo','c','cl','zc','kc','kl','to','cm','clm','tom','gd','il','r','i','gl','izm','glTs','dm','T','tp')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "11_2": {
						//N11_2
						_11_2_tmp = new List<string>();
						for(int index2 = 0; index2 < "tl,tlp,tz,tlz,toc,tzo,tt,tto,mgc,p,mc,mo,mn,mm,mg,pp,pb,pbIL,G,pc,Sh,V,sCh,mJ".Split(",", System.StringSplitOptions.None).Length; index2 += 1) {
							_11_2_tmp.Add(("tl,tlp,tz,tlz,toc,tzo,tt,tto,mgc,p,mc,mo,mn,mm,mg,pp,pb,pbIL,G,pc,Sh,V,sCh,mJ".Split(",", System.StringSplitOptions.None).GetValue(index2) as string) + " = " + "'" + row1[index2] + "'");
						}
						q = "UPDATE '" + "11" + "'" + " SET " + string.Join<System.String>(", ", _11_2_tmp) + " WHERE " + "N_year_N_month='" + N_year_N_month + "'";
					}
					break;
					case "12": {
						//N12
						q = "REPLACE INTO '" + "12" + "' " + "('N_year_N_month','dj','c','cm','tt','izm','gl','mm','gd','G')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "13": {
						q = q_simple;
					}
					break;
					case "14": {
						//N14&N15
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_h:m=" + Regex.Replace(row1[1], " +", ":") + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "15": {
						//N14&N15
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_h:m=" + Regex.Replace(row1[1], " +", ":") + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "16": {
						//N16&N17
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_d" + row1[6] + "_trace:" + row1[5] + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "17": {
						//N16&N17
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_d" + row1[6] + "_trace:" + row1[5] + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "20": {
						q = q_simple;
					}
					break;
					case "21_1": {
						//N21_1
						q = "REPLACE INTO '" + "21" + "' " + "('N_year_N_month','020_mid', '020_max', '020_min', '040_mid', '040_max', '040_min', '080_mid', '080_max', '080_min', '120_mid', '120_max', '120_min')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "21_2": {
						//N21_2
						_21_2_tmp = new List<string>();
						for(int index3 = 0; index3 < "160_mid, 160_max, 160_min, 240_mid, 240_max, 240_min, 320_mid, 320_max, 320_min, dayFrz_002, dayFrz_005, dayFrz_010, dayFrz_015, dayFrz_02, dayFrz_04, dayFrz_08, dayFrz_12, dayFrz_16, dayFrz_24, dayFrz_32".Split(",", System.StringSplitOptions.None).Length; index3 += 1) {
							_21_2_tmp.Add(("160_mid, 160_max, 160_min, 240_mid, 240_max, 240_min, 320_mid, 320_max, 320_min, dayFrz_002, dayFrz_005, dayFrz_010, dayFrz_015, dayFrz_02, dayFrz_04, dayFrz_08, dayFrz_12, dayFrz_16, dayFrz_24, dayFrz_32".Split(",", System.StringSplitOptions.None).GetValue(index3) as string) + " = " + "'" + row1[index3] + "'");
						}
						q = "UPDATE '" + "11" + "'" + " SET " + string.Join<System.String>(", ", _21_2_tmp) + " WHERE " + "N_year_N_month='" + N_year_N_month + "'";
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
