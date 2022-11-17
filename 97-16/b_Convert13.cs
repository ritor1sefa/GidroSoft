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

namespace MaxyGames.Generated {
	public class b_Convert13 : MaxyGames.RuntimeBehaviour {
		private string cachedValue;
		public Dictionary<string, SqliteConnection> sql_Connections = new Dictionary<string, SqliteConnection>();
		public Dictionary<string, SqliteCommand> sql_cmnds = new Dictionary<string, SqliteCommand>();
		public Dictionary<string, SqliteDataReader> sql_readers = new Dictionary<string, SqliteDataReader>();
		public string[] Files = new string[0];
		public Dictionary<string, string> bd_names = new Dictionary<string, string>();
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		public GameObject objectVariable3;
		private int index1;
		private int index2;
		private int index6;
		private List<int> delimetrs1 = new List<int>();

		private void Update() {
			string variable0 = "";
		}

		public void button() {
			//Собираем названия для бд
			bd_names();
			Files = Directory.GetFiles("D:\\__job\\2022\\13_юфо_ежемесячники\\txt\\", "*13_13*.txt");
			objectVariable.gameObject.GetComponent<TMPro.TMP_Text>().text = Files.Length.ToString();
			base.StartCoroutine(convert_mainLoop());
		}

		public System.Collections.IEnumerator convert_mainLoop() {
			string fPath = "";
			for(int index = 0; index < Files.Length; index += 1) {
				fPath = Files[index];
				objectVariable1.gameObject.GetComponent<TMPro.TMP_Text>().text = fPath;
				objectVariable2.gameObject.GetComponent<TMPro.TMP_Text>().text = (index + 1).ToString();
				sql_log(fPath, "");
				yield return selector_loop(fPath);
				Debug.Log("След файл");
			}
			Debug.Log("!Закончено!");
		}

		public System.Collections.IEnumerator selector_loop(string path) {
			string file_data = "";
			string One_table_data = "";
			string N_table = "";
			string N_year_N_month1 = "";
			List<string> _rowUnparsed = new List<string>();
			List<string> _rowUnparsed0 = new List<string>();
			List<string> tables = new List<string>();
			List<List<string>> Qtable = new List<List<string>>();
			Dictionary<string, string> months = new Dictionary<string, string>() { { "ЯНВАРЬ", "1" }, { "ФЕВРАЛЬ", "2" }, { "МАРТ", "3" }, { "АПРЕЛЬ", "4" }, { "МАЙ", "5" }, { "ИЮНЬ", "6" }, { "ИЮЛЬ", "7" }, { "АВГУСТ", "8" }, { "СЕНТЯБРЬ", "9" }, { "ОКТЯБРЬ", "10" }, { "НОЯБРЬ", "11" }, { "ДЕКАБРЬ", "12" } };
			string N_year_N_month_FromHeader = "";
			string tmp_tblName = "";
			file_data = File.ReadAllText(path).Replace("", "").Replace("", "");
			tables = Enumerable.ToList<System.String>(file_data.Trim().Split("Табли", System.StringSplitOptions.RemoveEmptyEntries));
			foreach(string loopObject in tables) {
				One_table_data = loopObject;
				//Пропускаем "таблицы" где много точек = меню в начале файла
				if((!(One_table_data.Contains(".....")) && One_table_data.StartsWith("ца"))) {
					//Get Number of table
					N_table = Regex.Match(One_table_data, "ца\\D*(\\w+)", RegexOptions.None).Result("$1");
					//Показываем номер текущей таблицы
					objectVariable3.gameObject.GetComponent<TMPro.TMP_Text>().text = N_table;
					switch(N_table) {
						case "1": {
							yield return n1(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "2": {
						}
						break;
						case "4": {
						}
						break;
						case "6": {
						}
						break;
						case "7": {
						}
						break;
						case "8": {
						}
						break;
						case "11": {
						}
						break;
						case "12": {
						}
						break;
						case "13": {
						}
						break;
						case "14": {
						}
						break;
						case "15": {
						}
						break;
						case "16": {
						}
						break;
						case "17": {
						}
						break;
						case "19": {
						}
						break;
					}
				}
			}
			sql_close();
		}
		/// <summary>
		/// подключение к бд, если не подключено. 
		/// </summary>
		private bool sql_connect(string db_name) {
			string path = "";
			SqliteConnection connection = default(SqliteConnection);
			path = Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_Connections.ContainsKey(db_name)) {
				return true;
			} else {
				//Копирование пустой бд в новый файл
				if(!(File.Exists(path))) {
					//Копирование пустой бд в новый файл
					if(db_name.StartsWith("y_")) {
						File.Copy(Application.streamingAssetsPath + "/" + "files/bd/" + "_emptyY" + ".sqlite", path, false);
					} else {
						File.Copy(Application.streamingAssetsPath + "/" + "files/bd/" + "_empty" + ".sqlite", path, false);
					}
				}
				connection = new SqliteConnection("URI=file:" + path);
				connection.Open();
				//добавление в общий список открытых подключений
				sql_Connections.Add(db_name, connection);
			}
			return File.Exists(path);
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

		public void sql_log(string parameter, string parameter2) {
			sql_insertQ("_log", "INSERT INTO log VALUES('" + parameter + parameter2 + "')");
		}

		public void bd_names() {
			bd_names.Clear();
			//Список на замену названий с "русского" на Русский
			foreach(string loopObject8 in File.ReadAllText(Application.streamingAssetsPath + "/" + "tmp.txt").Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				bd_names.Add(loopObject8.Split(new char[] { '=' })[1], loopObject8.Split(new char[] { '=' })[0]);
			}
		}

		public System.Collections.IEnumerator n1(string one_table_data) {
			string N_year_N_month = "";
			List<string> rows_unparsed = new List<string>();
			string splt_row = "";
			HashSet<int> splt_hash_ints = new HashSet<int>();
			List<int> delimetrs = new List<int>();
			string row_line = "";
			List<string> row_parsed = new List<string>();
			string row_bd_name = "";
			int row_from = 0;
			int row_length = 0;
			List<List<string>> table_parsed = new List<List<string>>();
			List<string> sql_row = new List<string>();
			string sql_bd_name = "";
			string sql_q = "";
			//Возможно вытащить из свитча?
			N_year_N_month = Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("y$1_m13");
			//Распилка
			foreach(string loopObject9 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
				rows_unparsed.Add(loopObject9);
			}
			yield return new WaitForEndOfFrame();
			//Получение позиций разделителей столбцов
			foreach(string loopObject10 in rows_unparsed) {
				//Сокращение количества проходов по таблице, что б не всю првоерять
				if(!(Regex.IsMatch(loopObject10.Trim(), "^ *\\d{1,3}\\."))) {
					splt_row = loopObject10;
					//Бегает по строке - ищет приключений
					for(index6 = splt_row.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index6 > -1; index6 = splt_row.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index6 + 1))) {
						splt_hash_ints.Add(index6);
					}
				}
			}
			delimetrs = Enumerable.ToList<System.Int32>(splt_hash_ints);
			delimetrs.Sort();
			yield return new WaitForEndOfFrame();
			//построчная обработка
			foreach(string loopObject11 in rows_unparsed) {
				row_line = loopObject11;
				//Если в строке есть номер с названием (простой случай)
				if(Regex.IsMatch(row_line.Trim(), "^\\d{1,3}\\.")) {
					//первым добавляем название бд
					if(bd_names.TryGetValue(Regex.Match(row_line.Substring(0, (delimetrs[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_'), out row_bd_name)) {
						//имя бд файла, уже нормализованное
						row_parsed.Add(row_bd_name);
					} else {
						Debug.Log("Нету в списке бд:" + row_bd_name);
					}
					//основное тело распарса строки
					for(int index7 = 0; index7 < (delimetrs.Count - 1); index7 += 1) {
						row_from = delimetrs[index7];
						row_length = (delimetrs[(index7 + 1)] - row_from);
						//Проверка на неполную строчку. заполнение @
						if((row_line.Length > row_from)) {
							if((row_line.Length >= (row_from + row_length))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								row_parsed.Add(row_line.Substring(row_from, row_length).Trim());
							} else {
								//Если нехватает символов в ячейке, но что то есть
								row_parsed.Add(row_line.Substring(row_from, (row_line.Length - row_from)).Trim());
							}
						} else {
							//если совсем ничего нету
							row_parsed.Add("@");
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line.Length >= (delimetrs[delimetrs.Count - 1] + (row_line.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							row_parsed.Add(row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim());
						} else {
							//last. если нехватает некоторых символов
							row_parsed.Add(row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim());
						}
					} else {
						//last. если ячейка совсем пустая
						row_parsed.Add("@");
					}
					table_parsed.Add(row_parsed);
					row_parsed = new List<string>();
				}
			}
			yield return new WaitForEndOfFrame();
			//вставка данных в бд
			foreach(List<string> loopObject12 in table_parsed) {
				sql_row = loopObject12;
				sql_bd_name = "y_" + sql_row[0];
				loopObject12.RemoveAt(0);
				sql_q = "INSERT OR IGNORE INTO '" + "1" + "' " + "VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", sql_row) + "')";
				if(sql_insertQ(sql_bd_name, sql_q).Equals(0)) {
					Debug.Log("В бд:===" + sql_bd_name + " ===уже есть эта строчка===" + sql_q);
				}
			}
		}
	}
}
