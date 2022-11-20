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
		public string N_table = "";
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		public GameObject objectVariable3;
		private int index4;
		private int index7;
		private int index12;
		private int index17;
		private string row_bd_name3 = "";
		private string sql_bd_name3 = "";
		private int index20;
		private string row_bd_name4 = "";
		private string sql_bd_name4 = "";
		private int index23;
		private List<int> delimetrs6 = new List<int>();

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
			string N_year_N_month6 = "";
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
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "2": {
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "4": {
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "6": {
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "7": {
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "8": {
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "11": {
							//11
							yield return n11_2parts(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "12": {
							//12
							yield return n12_splitted(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "13": {
							yield return n1_simple(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "14": {
							//14&15
							yield return n14_multiline(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "15": {
							//14&15
							yield return n14_multiline(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "16": {
							//14&15
							yield return n16_2partsANDmultiline(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "17": {
							//14&15
							yield return n16_2partsANDmultiline(One_table_data);
							yield return new WaitForEndOfFrame();
						}
						break;
						case "19": {
							yield return n19_2parts(One_table_data);
							yield return new WaitForEndOfFrame();
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
			List<string> row = new List<string>();
			string db_name = "";
			string q = "";
			string q_simple = "";
			List<string> _11_2_tmp = default(List<string>);
			string tmp_21_2 = "";
			foreach(List<string> loopObject3 in q_table) {
				row = loopObject3;
				db_name = row[0];
				//убираем название бд из строки. ненужно
				row.RemoveAt(0);
				q_simple = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row) + "')";
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
						q = "REPLACE INTO '" + "11" + "' " + "('N_year_N_month','dl','dj','mp','ld','jo','c','cl','zc','kc','kl','to','cm','clm','tom','gd','il','r','i','gl','izm','glTs','dm','T','tp')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row) + "')";
					}
					break;
					case "11_2": {
						//N11_2
						_11_2_tmp = new List<string>();
						for(int index1 = 0; index1 < "tl,tlp,tz,tlz,toc,tzo,tt,tto,mgc,p,mc,mo,mn,mm,mg,pp,pb,pbIL,G,pc,Sh,V,sCh,mJ".Split(",", System.StringSplitOptions.None).Length; index1 += 1) {
							_11_2_tmp.Add(("tl,tlp,tz,tlz,toc,tzo,tt,tto,mgc,p,mc,mo,mn,mm,mg,pp,pb,pbIL,G,pc,Sh,V,sCh,mJ".Split(",", System.StringSplitOptions.None).GetValue(index1) as string) + " = " + "'" + row[index1] + "'");
						}
						q = "UPDATE '" + "11" + "'" + " SET " + string.Join<System.String>(", ", _11_2_tmp) + " WHERE " + "N_year_N_month='" + N_year_N_month + "'";
					}
					break;
					case "12": {
						//если новая 12 таблица - то обычный. если старая (короткая) то false
						if((row.Count > 10)) {
							q = q_simple;
						} else {
							//N12
							q = "REPLACE INTO '" + "12" + "' " + "('N_year_N_month','dj','c','cm','tt','izm','gl','mm','gd','G')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row) + "')";
						}
					}
					break;
					case "13": {
						q = q_simple;
					}
					break;
					case "14": {
						//N14&N15
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_d" + row[0] + "_h:m=" + Regex.Replace(row[1], " +", ":") + "','" + string.Join<System.String>("','", row) + "')";
					}
					break;
					case "15": {
						//N14&N15
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_d" + row[0] + "_h:m=" + Regex.Replace(row[1], " +", ":") + "','" + string.Join<System.String>("','", row) + "')";
					}
					break;
					case "16": {
						//N16&N17
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_d" + row[6] + "_trace:" + row[5] + "','" + string.Join<System.String>("','", row) + "')";
					}
					break;
					case "17": {
						//N16&N17
						q = "REPLACE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_d" + row[6] + "_trace:" + row[5] + "','" + string.Join<System.String>("','", row) + "')";
					}
					break;
					case "20": {
						q = q_simple;
					}
					break;
					case "21_2": {
						//N21_2
						q = "REPLACE INTO '" + "21" + "' " + "('N_year_N_month','020_mid', '020_max', '020_min', '040_mid', '040_max', '040_min', '080_mid', '080_max', '080_min', '120_mid', '120_max', '120_min')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row) + "')";
					}
					break;
					case "21_3": {
						cachedValue = "'160_mid', '160_max', '160_min', '240_mid', '240_max', '240_min', '320_mid', '320_max', '320_min', 'dayFrz_002', 'dayFrz_005', 'dayFrz_010', 'dayFrz_015', 'dayFrz_02', 'dayFrz_04', 'dayFrz_08', 'dayFrz_12', 'dayFrz_16', 'dayFrz_24', 'dayFrz_32'";
						//N21_3
						for(int index2 = 0; index2 < cachedValue.Split(",", System.StringSplitOptions.None).Length; index2 += 1) {
							tmp_21_2 = tmp_21_2 + cachedValue.Split(",", System.StringSplitOptions.None)[index2] + " = " + "excluded." + cachedValue.Split(",", System.StringSplitOptions.None)[index2] + ",";
						}
						q = "INSERT INTO '" + "21" + "'('N_year_N_month'," + cachedValue + ") VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row) + "') ON CONFLICT(" + "N_year_N_month" + ") DO UPDATE SET " + tmp_21_2.TrimEnd(',');
					}
					break;
					case "7a": {
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

		public void sql_log(string parameter, string parameter2) {
			sql_insertQ("_log", "INSERT INTO log VALUES('" + parameter + "=>" + parameter2.Replace("'", "_") + "" + "')");
		}

		public void bd_names() {
			bd_names.Clear();
			//Список на замену названий с "русского" на Русский
			foreach(string loopObject4 in File.ReadAllText(Application.streamingAssetsPath + "/" + "tmp.txt").Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				if(!(bd_names.ContainsKey(loopObject4.Split(new char[] { '=' })[1].ToLower()))) {
					bd_names.Add(loopObject4.Split(new char[] { '=' })[1].ToLower(), loopObject4.Split(new char[] { '=' })[0]);
				}
			}
		}

		/// <summary>
		/// Таблицы N1
		/// </summary>
		public System.Collections.IEnumerator n1_simple(string one_table_data) {
			string N_year_N_month = "";
			List<string> rows_unparsed = new List<string>();
			string delim_row = "";
			int delim_rowCount = 10;
			HashSet<int> delim_hash_ints = new HashSet<int>();
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
			foreach(string loopObject5 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
				rows_unparsed.Add(loopObject5);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed.Count < 10)) {
				delim_rowCount = rows_unparsed.Count;
			}
			for(int index3 = 0; index3 < delim_rowCount; index3 += 1) {
				delim_row = rows_unparsed[index3];
				//Бегает по строке - ищет приключений
				for(index4 = delim_row.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index4 > -1; index4 = delim_row.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index4 + 1))) {
					delim_hash_ints.Add((index4 + 1));
				}
			}
			delimetrs = Enumerable.ToList<System.Int32>(delim_hash_ints);
			delimetrs.Sort();
			yield return new WaitForEndOfFrame();
			//построчная обработка
			foreach(string loopObject6 in rows_unparsed) {
				row_line = loopObject6;
				//Если в строке есть номер с названием (простой случай)
				if(Regex.IsMatch(row_line.Trim(), "^\\d{1,3}\\.")) {
					//первым добавляем название бд
					if(bd_names.TryGetValue(Regex.Match(row_line.Substring(0, (delimetrs[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower(), out row_bd_name)) {
						//имя бд файла, уже нормализованное
						row_parsed.Add(row_bd_name);
					} else {
						Debug.Log("Нету в списке бд:" + Regex.Match(row_line.Substring(0, (delimetrs[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower());
						break;
					}
					//основное тело распарса строки
					for(int index5 = 0; index5 < (delimetrs.Count - 1); index5 += 1) {
						row_from = delimetrs[index5];
						row_length = (delimetrs[(index5 + 1)] - row_from);
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
			foreach(List<string> loopObject7 in table_parsed) {
				sql_row = loopObject7;
				sql_bd_name = "y_" + sql_row[0];
				loopObject7.RemoveAt(0);
				sql_q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", sql_row) + "')";
				if(sql_insertQ(sql_bd_name, sql_q).Equals(0)) {
					Debug.Log("бд:" + sql_bd_name + "==" + "Ntable:" + N_table + "===Nyear:" + N_year_N_month);
					//дописываем в ключ о том что это дубликат
					sql_q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + N_year_N_month + "_double','" + string.Join<System.String>("','", sql_row) + "')";
					if(sql_insertQ(sql_bd_name, sql_q).Equals(0)) {
						//еслипрямь и дубликата дубликат есть, жжесть
						Debug.Log("В бд:===" + sql_bd_name + " ===уже есть эта строчка===" + sql_q);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// Таблица N11. из двухчастей
		/// </summary>
		public System.Collections.IEnumerator n11_2parts(string one_table_data) {
			string N_year_N_month1 = "";
			string N_part = "";
			List<string> rows_unparsed1 = new List<string>();
			string delim_row1 = "";
			int delim_rowCount1 = 10;
			HashSet<int> delim_hash_ints1 = new HashSet<int>();
			List<int> delimetrs1 = new List<int>();
			string row_line1 = "";
			List<string> row_parsed1 = new List<string>();
			string row_bd_name1 = "";
			int row_from1 = 0;
			int row_length1 = 0;
			List<List<string>> table_parsed1 = new List<List<string>>();
			List<string> sql_row1 = new List<string>();
			string sql_bd_name1 = "";
			string sql_q1 = "";
			List<string> sql_N2 = new List<string>();
			//Возможно вытащить из свитча?
			N_year_N_month1 = Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("y$1_m13");
			N_part = Regex.Match(one_table_data.ToLower(), "\\(часть\\D+(\\d)\\)", RegexOptions.None).Result("$1");
			//Распилка
			foreach(string loopObject8 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
				rows_unparsed1.Add(loopObject8);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed1.Count < 10)) {
				delim_rowCount1 = rows_unparsed1.Count;
			}
			for(int index6 = 0; index6 < delim_rowCount1; index6 += 1) {
				delim_row1 = rows_unparsed1[index6];
				//Бегает по строке - ищет приключений
				for(index7 = delim_row1.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index7 > -1; index7 = delim_row1.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index7 + 1))) {
					delim_hash_ints1.Add((index7 + 1));
				}
			}
			delimetrs1 = Enumerable.ToList<System.Int32>(delim_hash_ints1);
			delimetrs1.Sort();
			yield return new WaitForEndOfFrame();
			//построчная обработка
			foreach(string loopObject9 in rows_unparsed1) {
				row_line1 = loopObject9;
				//Если в строке есть номер с названием (простой случай)
				if(Regex.IsMatch(row_line1.Trim(), "^\\d{1,3}\\.")) {
					//первым добавляем название бд
					if(bd_names.TryGetValue(Regex.Match(row_line1.Substring(0, (delimetrs1[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower(), out row_bd_name1)) {
						//имя бд файла, уже нормализованное
						row_parsed1.Add(row_bd_name1);
					} else {
						Debug.Log("Нету в списке бд:" + Regex.Match(row_line1.Substring(0, (delimetrs1[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower());
						break;
					}
					//основное тело распарса строки
					for(int index8 = 0; index8 < (delimetrs1.Count - 1); index8 += 1) {
						row_from1 = delimetrs1[index8];
						row_length1 = (delimetrs1[(index8 + 1)] - row_from1);
						//Проверка на неполную строчку. заполнение @
						if((row_line1.Length > row_from1)) {
							if((row_line1.Length >= (row_from1 + row_length1))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								row_parsed1.Add(row_line1.Substring(row_from1, row_length1).Trim());
							} else {
								//Если нехватает символов в ячейке, но что то есть
								row_parsed1.Add(row_line1.Substring(row_from1, (row_line1.Length - row_from1)).Trim());
							}
						} else {
							//если совсем ничего нету
							row_parsed1.Add("@");
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line1.Length > delimetrs1[delimetrs1.Count - 1])) {
						if((row_line1.Length >= (delimetrs1[delimetrs1.Count - 1] + (row_line1.Length - delimetrs1[delimetrs1.Count - 1])))) {
							//last. если все символы на месте.
							row_parsed1.Add(row_line1.Substring(delimetrs1[delimetrs1.Count - 1], (row_line1.Length - delimetrs1[delimetrs1.Count - 1])).Trim());
						} else {
							//last. если нехватает некоторых символов
							row_parsed1.Add(row_line1.Substring(delimetrs1[delimetrs1.Count - 1], (row_line1.Length - delimetrs1[delimetrs1.Count - 1])).Trim());
						}
					} else {
						//last. если ячейка совсем пустая
						row_parsed1.Add("@");
					}
					table_parsed1.Add(row_parsed1);
					row_parsed1 = new List<string>();
				}
			}
			yield return new WaitForEndOfFrame();
			//вставка данных в бд
			foreach(List<string> loopObject10 in table_parsed1) {
				sql_row1 = loopObject10;
				sql_bd_name1 = "y_" + sql_row1[0];
				loopObject10.RemoveAt(0);
				switch(N_part) {
					case "1": {
						sql_q1 = "INSERT OR IGNORE INTO '" + "11" + "' " + "('N_year_N_month','ДЛ','ДЖ','МР','ЛД','ЖО','С','СЛ','ЗС','КС','КЛ','ТО','СМ','СЛМ','ТОМ','ГД','ИЛ','Р','И','ГЛ','ИЗМ','ГЛЦ','ДМ','Т','ТП')" + " VALUES ('" + N_year_N_month1 + "','" + string.Join<System.String>("','", sql_row1) + "')";
					}
					break;
					case "2": {
						//N11_2
						sql_N2 = new List<string>();
						for(int index9 = 0; index9 < "ТЛ,ТЛП,ТЗ,ТЛЗ,ТОС,ТЗО,ТТ,ТТО,МГС,П,МО,МН,ММ,МГ,ПП,ПБ,ПЫЛ,Г,ПС,Ш,В,СЧ,МЖ".Split(",", System.StringSplitOptions.None).Length; index9 += 1) {
							sql_N2.Add(("ТЛ,ТЛП,ТЗ,ТЛЗ,ТОС,ТЗО,ТТ,ТТО,МГС,П,МО,МН,ММ,МГ,ПП,ПБ,ПЫЛ,Г,ПС,Ш,В,СЧ,МЖ".Split(",", System.StringSplitOptions.None).GetValue(index9) as string) + " = " + "'" + sql_row1[index9] + "'");
						}
						sql_q1 = "UPDATE '" + "11" + "'" + " SET " + string.Join<System.String>(", ", sql_N2) + " WHERE " + "N_year_N_month='" + N_year_N_month1 + "'";
					}
					break;
				}
				if(sql_insertQ(sql_bd_name1, sql_q1).Equals(0)) {
					Debug.Log("бд:" + sql_bd_name1 + "=N11==" + N_year_N_month1 + "_double" + "===Npart:" + N_part);
					//дописываем в ключ о том что это дубликат
					switch(N_part) {
						case "1": {
							sql_q1 = "INSERT OR IGNORE INTO '" + "11" + "' " + "('N_year_N_month','ДЛ','ДЖ','МР','ЛД','ЖО','С','СЛ','ЗС','КС','КЛ','ТО','СМ','СЛМ','ТОМ','ГД','ИЛ','Р','И','ГЛ','ИЗМ','ГЛЦ','ДМ','Т','ТП')" + " VALUES ('" + N_year_N_month1 + "_double','" + string.Join<System.String>("','", sql_row1) + "')";
						}
						break;
						case "2": {
							//N11_2
							sql_N2 = new List<string>();
							for(int index10 = 0; index10 < "ТЛ,ТЛП,ТЗ,ТЛЗ,ТОС,ТЗО,ТТ,ТТО,МГС,П,МО,МН,ММ,МГ,ПП,ПБ,ПЫЛ,Г,ПС,Ш,В,СЧ,МЖ".Split(",", System.StringSplitOptions.None).Length; index10 += 1) {
								sql_N2.Add(("ТЛ,ТЛП,ТЗ,ТЛЗ,ТОС,ТЗО,ТТ,ТТО,МГС,П,МО,МН,ММ,МГ,ПП,ПБ,ПЫЛ,Г,ПС,Ш,В,СЧ,МЖ".Split(",", System.StringSplitOptions.None).GetValue(index10) as string) + " = " + "'" + sql_row1[index10] + "'");
							}
							sql_q1 = "UPDATE '" + "11" + "'" + " SET " + string.Join<System.String>(", ", sql_N2) + " WHERE " + "N_year_N_month='" + N_year_N_month1 + "_double'";
						}
						break;
					}
					if(sql_insertQ(sql_bd_name1, sql_q1).Equals(0)) {
						Debug.Log("В бд:===" + sql_bd_name1 + " ===уже есть эта строчка===" + sql_q1);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// Таблица N12, по 2 в одной строке
		/// вставка в N11
		/// </summary>
		public System.Collections.IEnumerator n12_splitted(string one_table_data) {
			string N_year_N_month2 = "";
			List<string> rows_unparsed2 = new List<string>();
			int split_row_maxLenght = 0;
			int split_row_hulfLenght = 0;
			string split_row = "";
			List<string> split_2thPart = new List<string>();
			string delim_row2 = "";
			int delim_rowCount2 = 10;
			HashSet<int> delim_hash_ints2 = new HashSet<int>();
			List<int> delimetrs2 = new List<int>();
			string row_line2 = "";
			List<string> row_parsed2 = new List<string>();
			string row_bd_name2 = "";
			int row_from2 = 0;
			int row_length2 = 0;
			List<List<string>> table_parsed2 = new List<List<string>>();
			List<string> sql_row2 = new List<string>();
			string sql_bd_name2 = "";
			string sql_clmns = "";
			string sql_tmp = "";
			string sql_q2 = "";
			int year = 0;
			//Возможно вытащить из свитча?
			N_year_N_month2 = Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("y$1_m13");
			if(int.TryParse(Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("$1"), out year)) {
				//с 2008 таблицы не распилены и шапка другая
				if((year > 2007)) {
					//Распилка
					foreach(string loopObject11 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
						rows_unparsed2.Add(loopObject11);
					}
					yield return new WaitForEndOfFrame();
				} else {
					//пилим 12 таблицу пополам
					foreach(string loopObject12 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
						if(!(Regex.IsMatch(loopObject12.Trim(), "^ца\\D*(\\d+)\\."))) {
							split_row = loopObject12;
							if((split_row.Length > split_row_maxLenght)) {
								//Тут всегда самая большая длина строки (из шапки)
								split_row_maxLenght = split_row.Length;
								split_row_hulfLenght = (split_row_maxLenght / 2);
							}
							//если первая неполная
							if((split_row_hulfLenght > split_row.Length)) {
								//Если текущая строка меньше половины максимальной
								rows_unparsed2.Add(split_row.Substring(0, split_row.Length));
							} else {
								//Если текущая строка больше половины максимальной
								rows_unparsed2.Add(split_row.Substring(0, split_row_hulfLenght));
								//второй столбец. засовываем оставшуюся строку
								split_2thPart.Add(split_row.Substring(split_row_hulfLenght, (split_row.Length - split_row_hulfLenght)));
							}
						}
					}
					foreach(string loopObject13 in split_2thPart) {
						if(Regex.IsMatch(loopObject13.TrimStart(), "^\\d{1,3}\\.")) {
							//второго столбца добавляем только строки
							rows_unparsed2.Add(loopObject13);
						}
					}
				}
				//Разделители-столбцы
				if((rows_unparsed2.Count < 10)) {
					delim_rowCount2 = rows_unparsed2.Count;
				}
				for(int index11 = 0; index11 < delim_rowCount2; index11 += 1) {
					delim_row2 = rows_unparsed2[index11];
					//Бегает по строке - ищет приключений
					for(index12 = delim_row2.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index12 > -1; index12 = delim_row2.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index12 + 1))) {
						delim_hash_ints2.Add((index12 + 1));
					}
				}
				delimetrs2 = Enumerable.ToList<System.Int32>(delim_hash_ints2);
				delimetrs2.Sort();
				yield return new WaitForEndOfFrame();
				//построчная обработка
				foreach(string loopObject14 in rows_unparsed2) {
					row_line2 = loopObject14.TrimStart();
					//Если в строке есть номер с названием (простой случай)
					if(Regex.IsMatch(row_line2, "^\\d{1,3}\\.")) {
						//первым добавляем название бд
						if(bd_names.TryGetValue(Regex.Match(row_line2.Substring(0, (delimetrs2[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower(), out row_bd_name2)) {
							//имя бд файла, уже нормализованное
							row_parsed2.Add(row_bd_name2);
						} else {
							Debug.Log("Нету в списке бд:" + Regex.Match(row_line2.Substring(0, (delimetrs2[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower());
							break;
						}
						//основное тело распарса строки
						for(int index13 = 0; index13 < (delimetrs2.Count - 1); index13 += 1) {
							row_from2 = delimetrs2[index13];
							row_length2 = (delimetrs2[(index13 + 1)] - row_from2);
							//Проверка на неполную строчку. заполнение @
							if((row_line2.Length > row_from2)) {
								if((row_line2.Length >= (row_from2 + row_length2))) {
									//Если совсем всё в порядке и вся ячейка что то имеет
									row_parsed2.Add(row_line2.Substring(row_from2, row_length2).Trim());
								} else {
									//Если нехватает символов в ячейке, но что то есть
									row_parsed2.Add(row_line2.Substring(row_from2, (row_line2.Length - row_from2)).Trim());
								}
							} else {
								//если совсем ничего нету
								row_parsed2.Add("@");
							}
						}
						//Проверка на неполную строчку. заполнение @
						if((row_line2.Length > delimetrs2[delimetrs2.Count - 1])) {
							if((row_line2.Length >= (delimetrs2[delimetrs2.Count - 1] + (row_line2.Length - delimetrs2[delimetrs2.Count - 1])))) {
								//last. если все символы на месте.
								row_parsed2.Add(row_line2.Substring(delimetrs2[delimetrs2.Count - 1], (row_line2.Length - delimetrs2[delimetrs2.Count - 1])).Trim());
							} else {
								//last. если нехватает некоторых символов
								row_parsed2.Add(row_line2.Substring(delimetrs2[delimetrs2.Count - 1], (row_line2.Length - delimetrs2[delimetrs2.Count - 1])).Trim());
							}
						} else {
							//last. если ячейка совсем пустая
							row_parsed2.Add("@");
						}
						table_parsed2.Add(row_parsed2);
						row_parsed2 = new List<string>();
					}
				}
				yield return new WaitForEndOfFrame();
				//вставка данных в бд
				foreach(List<string> loopObject15 in table_parsed2) {
					sql_row2 = loopObject15;
					sql_bd_name2 = "y_" + sql_row2[0];
					loopObject15.RemoveAt(0);
					sql_tmp = "";
					//с 2008 таблицы не распилены и шапка другая
					if((year > 2007)) {
						sql_clmns = "'ДЖ','С','СМ','ТТ','ИЗМ','ГЛ','ММ','ГД','Г','ДМ','ПМ','ПБ','МГ','СЧ','Ш'";
					} else {
						sql_clmns = "'ДЖ','С','СМ','ТТ','ИЗМ','ГЛ','ММ','ГД','Г'";
					}
					//N21_3
					for(int index14 = 0; index14 < sql_clmns.Split(",", System.StringSplitOptions.None).Length; index14 += 1) {
						sql_tmp = sql_tmp + sql_clmns.Split(",", System.StringSplitOptions.None)[index14] + " = " + "excluded." + sql_clmns.Split(",", System.StringSplitOptions.None)[index14] + ",";
					}
					sql_q2 = "INSERT OR IGNORE INTO '" + "11" + "'('N_year_N_month'," + sql_clmns + ") VALUES ('" + N_year_N_month2 + "','" + string.Join<System.String>("','", sql_row2) + "') ";
					if(sql_insertQ(sql_bd_name2, sql_q2).Equals(0)) {
						Debug.Log("бд:" + sql_bd_name2 + "==" + "Ntable:11" + "" + "===Nyear:" + N_year_N_month2);
						//N21_3
						for(int index15 = 0; index15 < sql_clmns.Split(",", System.StringSplitOptions.None).Length; index15 += 1) {
							sql_tmp = sql_tmp + sql_clmns.Split(",", System.StringSplitOptions.None)[index15] + " = " + "excluded." + sql_clmns.Split(",", System.StringSplitOptions.None)[index15] + ",";
						}
						sql_q2 = "INSERT OR IGNORE INTO '" + "11" + "'('N_year_N_month'," + sql_clmns + ") VALUES ('" + N_year_N_month2 + "_double','" + string.Join<System.String>("','", sql_row2) + "') ";
						if(sql_insertQ(sql_bd_name2, sql_q2).Equals(0)) {
							Debug.Log("В бд:===" + sql_bd_name2 + " ===уже есть эта строчка===" + sql_q2);
						}
					}
				}
				yield return new WaitForEndOfFrame();
			} else {
				Debug.Log("Год не распарсился :\\");
			}
		}

		/// <summary>
		/// Таблица N14,15 - многострочная.
		/// </summary>
		public System.Collections.IEnumerator n14_multiline(string one_table_data) {
			string N_year_N_month3 = "";
			List<string> rows_unparsed3 = new List<string>();
			string delim_row3 = "";
			int delim_rowCount3 = 10;
			HashSet<int> delim_hash_ints3 = new HashSet<int>();
			List<int> delimetrs3 = new List<int>();
			string row_line3 = "";
			List<string> row_parsed3 = new List<string>();
			int row_from3 = 0;
			int row_length3 = 0;
			List<List<string>> table_parsed3 = new List<List<string>>();
			List<string> sql_row3 = new List<string>();
			string sql_q3 = "";
			//Возможно вытащить из свитча?
			N_year_N_month3 = Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("y$1_m13");
			//Распилка
			foreach(string loopObject16 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
				rows_unparsed3.Add(loopObject16);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed3.Count < 10)) {
				delim_rowCount3 = rows_unparsed3.Count;
			}
			for(int index16 = 0; index16 < delim_rowCount3; index16 += 1) {
				delim_row3 = rows_unparsed3[index16];
				//Бегает по строке - ищет приключений
				for(index17 = delim_row3.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index17 > -1; index17 = delim_row3.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index17 + 1))) {
					delim_hash_ints3.Add((index17 + 1));
				}
			}
			delimetrs3 = Enumerable.ToList<System.Int32>(delim_hash_ints3);
			delimetrs3.Sort();
			yield return new WaitForEndOfFrame();
			//построчная обработка
			//выглядит не супер, но что поделать
			foreach(string loopObject17 in rows_unparsed3) {
				row_line3 = loopObject17;
				row_parsed3 = new List<string>();
				//Если в строке есть номер с названием (простой случай)
				if(Regex.IsMatch(row_line3.Trim(), "^\\d{1,3}\\.")) {
					//Если название есть в списке, добавляем в массив
					if(bd_names.TryGetValue(Regex.Match(row_line3.Substring(0, (delimetrs3[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower(), out row_bd_name3)) {
						//имя бд файла, уже нормализованное
						row_parsed3.Add(row_bd_name3);
					} else {
						Debug.Log("Нету в списке бд:" + Regex.Match(row_line3.Substring(0, (delimetrs3[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower());
						break;
					}
				} else //если в строке что то есть, но не название
				if((((((row_line3.Trim().Length > delimetrs3[0]) && !(string.IsNullOrWhiteSpace(row_bd_name3))) && !(Regex.IsMatch(row_line3, "ца\\D*(\\w+)"))) && Regex.IsMatch(row_line3, "^[^║═=|]*$")) && !(Regex.IsMatch(row_line3, "Год\\D*(\\d+)")))) {
					//имя бд файла, уже нормализованное
					row_parsed3.Add(row_bd_name3);
				}
				//Если есть в массиве название
				if((row_parsed3.Count > 0)) {
					//основное тело распарса строки
					for(int index18 = 0; index18 < (delimetrs3.Count - 1); index18 += 1) {
						row_from3 = delimetrs3[index18];
						row_length3 = (delimetrs3[(index18 + 1)] - row_from3);
						//Проверка на неполную строчку. заполнение @
						if((row_line3.Length > row_from3)) {
							if((row_line3.Length >= (row_from3 + row_length3))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								row_parsed3.Add(row_line3.Substring(row_from3, row_length3).Trim());
							} else {
								//Если нехватает символов в ячейке, но что то есть
								row_parsed3.Add(row_line3.Substring(row_from3, (row_line3.Length - row_from3)).Trim());
							}
						} else {
							//если совсем ничего нету
							row_parsed3.Add("@");
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line3.Length > delimetrs3[delimetrs3.Count - 1])) {
						if((row_line3.Length >= (delimetrs3[delimetrs3.Count - 1] + (row_line3.Length - delimetrs3[delimetrs3.Count - 1])))) {
							//last. если все символы на месте.
							row_parsed3.Add(row_line3.Substring(delimetrs3[delimetrs3.Count - 1], (row_line3.Length - delimetrs3[delimetrs3.Count - 1])).Trim());
						} else {
							//last. если нехватает некоторых символов
							row_parsed3.Add(row_line3.Substring(delimetrs3[delimetrs3.Count - 1], (row_line3.Length - delimetrs3[delimetrs3.Count - 1])).Trim());
						}
					} else {
						//last. если ячейка совсем пустая
						row_parsed3.Add("@");
					}
					table_parsed3.Add(row_parsed3);
					row_parsed3 = new List<string>();
				}
			}
			yield return new WaitForEndOfFrame();
			//вставка данных в бд
			foreach(List<string> loopObject18 in table_parsed3) {
				sql_row3 = loopObject18;
				sql_bd_name3 = "y_" + sql_row3[0];
				loopObject18.RemoveAt(0);
				sql_q3 = "INSERT OR IGNORE INTO '" + "14" + "' " + "VALUES ('" + N_year_N_month3 + "_hash:" + string.Join<System.String>("_", sql_row3) + "','" + string.Join<System.String>("','", sql_row3) + "')";
				if(sql_insertQ(sql_bd_name3, sql_q3).Equals(0)) {
					Debug.Log("бд:" + sql_bd_name3 + "==" + "Ntable:" + "14" + "===Nyear:" + N_year_N_month3);
					sql_q3 = "INSERT OR IGNORE INTO '" + "14" + "' " + "VALUES ('" + N_year_N_month3 + "_double_hash:" + string.Join<System.String>("_", sql_row3) + "','" + string.Join<System.String>("','", sql_row3) + "')";
					if(sql_insertQ(sql_bd_name3, sql_q3).Equals(0)) {
						Debug.Log("В бд:===" + sql_bd_name3 + " ===уже есть эта строчка===" + sql_q3);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// Таблицы N16.17 - многострочные и из 2 частей
		/// </summary>
		public System.Collections.IEnumerator n16_2partsANDmultiline(string one_table_data) {
			string N_year_N_month4 = "";
			string N_part1 = "";
			List<string> rows_unparsed4 = new List<string>();
			string delim_row4 = "";
			int delim_rowCount4 = 10;
			HashSet<int> delim_hash_ints4 = new HashSet<int>();
			List<int> delimetrs4 = new List<int>();
			string row_line4 = "";
			List<string> row_parsed4 = new List<string>();
			int row_from4 = 0;
			int row_length4 = 0;
			List<List<string>> table_parsed4 = new List<List<string>>();
			List<string> sql_row4 = new List<string>();
			string sql_q4 = "";
			List<string> sql_N22 = new List<string>();
			//Возможно вытащить из свитча?
			N_year_N_month4 = Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("y$1_m13");
			N_part1 = Regex.Match(one_table_data.ToLower(), "\\(часть\\D+(\\d)\\)", RegexOptions.None).Result("$1");
			//Распилка
			foreach(string loopObject19 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
				rows_unparsed4.Add(loopObject19);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed4.Count < 10)) {
				delim_rowCount4 = rows_unparsed4.Count;
			}
			for(int index19 = 0; index19 < delim_rowCount4; index19 += 1) {
				delim_row4 = rows_unparsed4[index19];
				//Бегает по строке - ищет приключений
				for(index20 = delim_row4.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index20 > -1; index20 = delim_row4.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index20 + 1))) {
					delim_hash_ints4.Add((index20 + 1));
				}
			}
			delimetrs4 = Enumerable.ToList<System.Int32>(delim_hash_ints4);
			delimetrs4.Sort();
			yield return new WaitForEndOfFrame();
			//построчная обработка
			//выглядит не супер, но что поделать
			foreach(string loopObject20 in rows_unparsed4) {
				row_line4 = loopObject20;
				row_parsed4 = new List<string>();
				//Если в строке есть номер с названием (простой случай)
				if(Regex.IsMatch(row_line4.Trim(), "^\\d{1,3}\\.")) {
					//Если название есть в списке, добавляем в массив
					if(bd_names.TryGetValue(Regex.Match(row_line4.Substring(0, (delimetrs4[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower(), out row_bd_name4)) {
						//имя бд файла, уже нормализованное
						row_parsed4.Add(row_bd_name4);
					} else {
						Debug.Log("Нету в списке бд:" + Regex.Match(row_line4.Substring(0, (delimetrs4[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower());
						break;
					}
				} else //если в строке что то есть, но не название
				if((((((row_line4.Trim().Length > delimetrs4[0]) && !(string.IsNullOrWhiteSpace(row_bd_name4))) && !(Regex.IsMatch(row_line4, "ца\\D*(\\w+)"))) && Regex.IsMatch(row_line4, "^[^║═=|]*$")) && !(Regex.IsMatch(row_line4, "Год\\D*(\\d+)")))) {
					//имя бд файла, уже нормализованное
					row_parsed4.Add(row_bd_name4);
				}
				//Если есть в массиве название
				if((row_parsed4.Count > 0)) {
					//основное тело распарса строки
					for(int index21 = 0; index21 < (delimetrs4.Count - 1); index21 += 1) {
						row_from4 = delimetrs4[index21];
						row_length4 = (delimetrs4[(index21 + 1)] - row_from4);
						//Проверка на неполную строчку. заполнение @
						if((row_line4.Length > row_from4)) {
							if((row_line4.Length >= (row_from4 + row_length4))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								row_parsed4.Add(row_line4.Substring(row_from4, row_length4).Trim());
							} else {
								//Если нехватает символов в ячейке, но что то есть
								row_parsed4.Add(row_line4.Substring(row_from4, (row_line4.Length - row_from4)).Trim());
							}
						} else {
							//если совсем ничего нету
							row_parsed4.Add("@");
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line4.Length > delimetrs4[delimetrs4.Count - 1])) {
						if((row_line4.Length >= (delimetrs4[delimetrs4.Count - 1] + (row_line4.Length - delimetrs4[delimetrs4.Count - 1])))) {
							//last. если все символы на месте.
							row_parsed4.Add(row_line4.Substring(delimetrs4[delimetrs4.Count - 1], (row_line4.Length - delimetrs4[delimetrs4.Count - 1])).Trim());
						} else {
							//last. если нехватает некоторых символов
							row_parsed4.Add(row_line4.Substring(delimetrs4[delimetrs4.Count - 1], (row_line4.Length - delimetrs4[delimetrs4.Count - 1])).Trim());
						}
					} else {
						//last. если ячейка совсем пустая
						row_parsed4.Add("@");
					}
					table_parsed4.Add(row_parsed4);
					row_parsed4 = new List<string>();
				}
			}
			yield return new WaitForEndOfFrame();
			//вставка данных в бд
			foreach(List<string> loopObject21 in table_parsed4) {
				sql_row4 = loopObject21;
				sql_bd_name4 = "y_" + sql_row4[0];
				loopObject21.RemoveAt(0);
				//пихаем в разные строчки, пока - пойдёт. если что исправим
				switch(N_part1) {
					case "1": {
						sql_q4 = "INSERT OR IGNORE INTO '" + "16" + "' " + "('N_year_N_month_N_day_trace','1_ежД_типУч','1_ежД_разрСнП_день','1_ежД_ПервПослСнег_день','1_ежД_днейСоСн','1_дСн_маршрут','1_дСн_числоСнегоСъёмок','1_дСн_высотаСн_максИзСр','1_дСн_высотаСн_максИзСр_дата','1_дСн_высотаСн_абсМакс','1_дСн_высотаСн_абсМакс_дата','1_дСн_максЗапВод_вСнеге_','1_дСн_максЗапВод_вСнеге_дата','1_дСн_максЗапВод_общий_','1_дСн_максЗапВод_общий_дата')" + " VALUES ('" + N_year_N_month4 + "_hash:" + string.Join<System.String>("_", sql_row4) + "','" + string.Join<System.String>("','", sql_row4) + "')";
					}
					break;
					case "2": {
						sql_q4 = "INSERT OR IGNORE INTO '" + "16" + "' " + "('N_year_N_month_N_day_trace','2_ежД_типУч','2_ежД_разрСнП_день','2_ежД_ПервПослСнег_день','2_ежД_днейСоСн','2_дСн_маршрут','2_дСн_числоСнегоСъёмок','2_дСн_высотаСн_максИзСр','2_дСн_высотаСн_максИзСр_дата','2_дСн_высотаСн_абсМакс','2_дСн_высотаСн_абсМакс_дата','2_дСн_максЗапВод_вСнеге_','2_дСн_максЗапВод_вСнеге_дата','2_дСн_максЗапВод_общий_','2_дСн_максЗапВод_общий_дата')" + " VALUES ('" + N_year_N_month4 + "_hash:" + string.Join<System.String>("_", sql_row4) + "','" + string.Join<System.String>("','", sql_row4) + "')";
					}
					break;
				}
				if(sql_insertQ(sql_bd_name4, sql_q4).Equals(0)) {
					Debug.Log("бд:" + sql_bd_name4 + "==" + "Ntable:" + "16" + "===Nyear:" + N_year_N_month4);
					//пихаем в разные строчки, пока - пойдёт. если что исправим
					switch(N_part1) {
						case "1": {
							sql_q4 = "INSERT OR IGNORE INTO '" + "16" + "' " + "('N_year_N_month_N_day_trace','1_ежД_типУч','1_ежД_разрСнП_день','1_ежД_ПервПослСнег_день','1_ежД_днейСоСн','1_дСн_маршрут','1_дСн_числоСнегоСъёмок','1_дСн_высотаСн_максИзСр','1_дСн_высотаСн_максИзСр_дата','1_дСн_высотаСн_абсМакс','1_дСн_высотаСн_абсМакс_дата','1_дСн_максЗапВод_вСнеге_','1_дСн_максЗапВод_вСнеге_дата','1_дСн_максЗапВод_общий_','1_дСн_максЗапВод_общий_дата')" + " VALUES ('" + N_year_N_month4 + "_double_hash:" + string.Join<System.String>("_", sql_row4) + "','" + string.Join<System.String>("','", sql_row4) + "')";
						}
						break;
						case "2": {
							sql_q4 = "INSERT OR IGNORE INTO '" + "16" + "' " + "('N_year_N_month_N_day_trace','2_ежД_типУч','2_ежД_разрСнП_день','2_ежД_ПервПослСнег_день','2_ежД_днейСоСн','2_дСн_маршрут','2_дСн_числоСнегоСъёмок','2_дСн_высотаСн_максИзСр','2_дСн_высотаСн_максИзСр_дата','2_дСн_высотаСн_абсМакс','2_дСн_высотаСн_абсМакс_дата','2_дСн_максЗапВод_вСнеге_','2_дСн_максЗапВод_вСнеге_дата','2_дСн_максЗапВод_общий_','2_дСн_максЗапВод_общий_дата')" + " VALUES ('" + N_year_N_month4 + "_double_hash:" + string.Join<System.String>("_", sql_row4) + "','" + string.Join<System.String>("','", sql_row4) + "')";
						}
						break;
					}
					if(sql_insertQ(sql_bd_name4, sql_q4).Equals(0)) {
						Debug.Log("В бд:===" + sql_bd_name4 + " ===уже есть эта строчка===" + sql_q4);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// Таблица N19. из двухчастей
		/// </summary>
		public System.Collections.IEnumerator n19_2parts(string one_table_data) {
			string N_year_N_month5 = "";
			string N_part2 = "";
			List<string> rows_unparsed5 = new List<string>();
			string delim_row5 = "";
			int delim_rowCount5 = 10;
			HashSet<int> delim_hash_ints5 = new HashSet<int>();
			List<int> delimetrs5 = new List<int>();
			string row_line5 = "";
			List<string> row_parsed5 = new List<string>();
			string row_bd_name5 = "";
			int row_from5 = 0;
			int row_length5 = 0;
			List<List<string>> table_parsed5 = new List<List<string>>();
			List<string> sql_row5 = new List<string>();
			string sql_bd_name5 = "";
			string sql_q5 = "";
			List<string> sql_N21 = new List<string>();
			//Возможно вытащить из свитча?
			N_year_N_month5 = Regex.Match(one_table_data, "Год\\D*(\\d+)", RegexOptions.None).Result("y$1_m13");
			N_part2 = Regex.Match(one_table_data.ToLower(), "\\(часть\\D+(\\d)\\)", RegexOptions.None).Result("$1");
			//Распилка
			foreach(string loopObject22 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
				rows_unparsed5.Add(loopObject22);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed5.Count < 10)) {
				delim_rowCount5 = rows_unparsed5.Count;
			}
			for(int index22 = 0; index22 < delim_rowCount5; index22 += 1) {
				delim_row5 = rows_unparsed5[index22];
				//Бегает по строке - ищет приключений
				for(index23 = delim_row5.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index23 > -1; index23 = delim_row5.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index23 + 1))) {
					delim_hash_ints5.Add((index23 + 1));
				}
			}
			delimetrs5 = Enumerable.ToList<System.Int32>(delim_hash_ints5);
			delimetrs5.Sort();
			yield return new WaitForEndOfFrame();
			//построчная обработка
			foreach(string loopObject23 in rows_unparsed5) {
				row_line5 = loopObject23;
				//Если в строке есть номер с названием (простой случай)
				if(Regex.IsMatch(row_line5.Trim(), "^\\d{1,3}\\.")) {
					//первым добавляем название бд
					if(bd_names.TryGetValue(Regex.Match(row_line5.Substring(0, (delimetrs5[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower(), out row_bd_name5)) {
						//имя бд файла, уже нормализованное
						row_parsed5.Add(row_bd_name5);
					} else {
						Debug.Log("Нету в списке бд:" + Regex.Match(row_line5.Substring(0, (delimetrs5[0] - 1)), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower());
						break;
					}
					//основное тело распарса строки
					for(int index24 = 0; index24 < (delimetrs5.Count - 1); index24 += 1) {
						row_from5 = delimetrs5[index24];
						row_length5 = (delimetrs5[(index24 + 1)] - row_from5);
						//Проверка на неполную строчку. заполнение @
						if((row_line5.Length > row_from5)) {
							if((row_line5.Length >= (row_from5 + row_length5))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								row_parsed5.Add(row_line5.Substring(row_from5, row_length5).Trim());
							} else {
								//Если нехватает символов в ячейке, но что то есть
								row_parsed5.Add(row_line5.Substring(row_from5, (row_line5.Length - row_from5)).Trim());
							}
						} else {
							//если совсем ничего нету
							row_parsed5.Add("@");
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line5.Length > delimetrs5[delimetrs5.Count - 1])) {
						if((row_line5.Length >= (delimetrs5[delimetrs5.Count - 1] + (row_line5.Length - delimetrs5[delimetrs5.Count - 1])))) {
							//last. если все символы на месте.
							row_parsed5.Add(row_line5.Substring(delimetrs5[delimetrs5.Count - 1], (row_line5.Length - delimetrs5[delimetrs5.Count - 1])).Trim());
						} else {
							//last. если нехватает некоторых символов
							row_parsed5.Add(row_line5.Substring(delimetrs5[delimetrs5.Count - 1], (row_line5.Length - delimetrs5[delimetrs5.Count - 1])).Trim());
						}
					} else {
						//last. если ячейка совсем пустая
						row_parsed5.Add("@");
					}
					table_parsed5.Add(row_parsed5);
					row_parsed5 = new List<string>();
				}
			}
			yield return new WaitForEndOfFrame();
			//вставка данных в бд
			foreach(List<string> loopObject24 in table_parsed5) {
				sql_row5 = loopObject24;
				sql_bd_name5 = "y_" + sql_row5[0];
				loopObject24.RemoveAt(0);
				switch(N_part2) {
					case "2": {
						sql_q5 = "INSERT OR IGNORE INTO '" + "19" + "' " + "('N_year_N_month','020_mid','020_max','020_min','040_mid','040_max','040_min','080_mid','080_max','080_min','120_mid','120_max','120_min')" + " VALUES ('" + N_year_N_month5 + "','" + string.Join<System.String>("','", sql_row5) + "')";
					}
					break;
					case "3": {
						//N11_2
						sql_N21 = new List<string>();
						for(int index25 = 0; index25 < "'160_mid','160_max','160_min','240_mid','240_max','240_min','320_mid','320_max','320_min','dayFrz_002','dayFrz_005','dayFrz_010','dayFrz_015','dayFrz_02','dayFrz_04','dayFrz_08','dayFrz_12','dayFrz_16','dayFrz_24','dayFrz_32'".Split(",", System.StringSplitOptions.None).Length; index25 += 1) {
							sql_N21.Add(("'160_mid','160_max','160_min','240_mid','240_max','240_min','320_mid','320_max','320_min','dayFrz_002','dayFrz_005','dayFrz_010','dayFrz_015','dayFrz_02','dayFrz_04','dayFrz_08','dayFrz_12','dayFrz_16','dayFrz_24','dayFrz_32'".Split(",", System.StringSplitOptions.None).GetValue(index25) as string) + " = " + "'" + sql_row5[index25] + "'");
						}
						sql_q5 = "UPDATE '" + "19" + "'" + " SET " + string.Join<System.String>(", ", sql_N21) + " WHERE " + "N_year_N_month='" + N_year_N_month5 + "'";
					}
					break;
				}
				if(sql_insertQ(sql_bd_name5, sql_q5).Equals(0)) {
					Debug.Log("бд:" + sql_bd_name5 + "==" + "Ntable:" + "19" + "===Nyear:" + N_year_N_month5);
					switch(N_part2) {
						case "2": {
							sql_q5 = "INSERT OR IGNORE INTO '" + "19" + "' " + "('N_year_N_month','020_mid','020_max','020_min','040_mid','040_max','040_min','080_mid','080_max','080_min','120_mid','120_max','120_min')" + " VALUES ('" + N_year_N_month5 + "_double','" + string.Join<System.String>("','", sql_row5) + "')";
						}
						break;
						case "3": {
							//N11_2
							sql_N21 = new List<string>();
							for(int index26 = 0; index26 < "'160_mid','160_max','160_min','240_mid','240_max','240_min','320_mid','320_max','320_min','dayFrz_002','dayFrz_005','dayFrz_010','dayFrz_015','dayFrz_02','dayFrz_04','dayFrz_08','dayFrz_12','dayFrz_16','dayFrz_24','dayFrz_32'".Split(",", System.StringSplitOptions.None).Length; index26 += 1) {
								sql_N21.Add(("'160_mid','160_max','160_min','240_mid','240_max','240_min','320_mid','320_max','320_min','dayFrz_002','dayFrz_005','dayFrz_010','dayFrz_015','dayFrz_02','dayFrz_04','dayFrz_08','dayFrz_12','dayFrz_16','dayFrz_24','dayFrz_32'".Split(",", System.StringSplitOptions.None).GetValue(index26) as string) + " = " + "'" + sql_row5[index26] + "'");
							}
							sql_q5 = "UPDATE '" + "19" + "'" + " SET " + string.Join<System.String>(", ", sql_N21) + " WHERE " + "N_year_N_month='" + N_year_N_month5 + "_double'";
						}
						break;
					}
					if(sql_insertQ(sql_bd_name5, sql_q5).Equals(0)) {
						Debug.Log("В бд:===" + sql_bd_name5 + " ===уже есть эта строчка===" + sql_q5);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
