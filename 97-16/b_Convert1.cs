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
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		private Match cachedValue;
		private string cachedValue1;
		public Dictionary<string, SqliteConnection> sql_Connections = new Dictionary<string, SqliteConnection>();
		public Dictionary<string, SqliteCommand> sql_cmnds = new Dictionary<string, SqliteCommand>();
		public Dictionary<string, SqliteDataReader> sql_readers = new Dictionary<string, SqliteDataReader>();
		public string[] Files = new string[0];
		public bool FileNext = true;
		public int currentFile = 0;
		public bool TableNext = true;
		public int currentTable = 0;
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		public GameObject objectVariable3;
		private List<int> delimetrs = new List<int>();
		private int index;
		public GameObject objectVariable4;

		/// <summary>
		/// sqlite запрос на выборку столбца данных по году+месяцу
		/// SELECT * FROM "2"  Where "2th" LIKE "%2001" AND "2th" LIKE "02%";
		/// SELECT * FROM "2"  Where "2th" LIKE "%y2001%" AND "2th" LIKE "%m02%";
		/// </summary>
		private void Update() {
			string variable0 = "";
			objectVariable.gameObject.GetComponent<TMPro.TMP_Text>().text = currentFile.ToString();
			objectVariable1.gameObject.GetComponent<TMPro.TMP_Text>().text = currentTable.ToString();
			objectVariable2.gameObject.GetComponent<TMPro.TMP_Text>().text = ((System.GC.GetTotalMemory(false) / 1024L) / 1024L).ToString();
			if((Files.Length > currentFile)) {
				objectVariable3.gameObject.GetComponent<TMPro.TMP_Text>().text = Files[currentFile];
				base.StartCoroutine(NewFunction());
			}
			if(Input.GetKeyUp(KeyCode.UpArrow)) {
				TableNext = true;
				base.StartCoroutine(loadFromFiles());
			}
		}

		public System.Collections.IEnumerator loadFromFiles() {
			string path = "tmp.txt";
			string file_data = "";
			string One_table_data = "";
			string N_table = "";
			string N_year_N_month = "";
			List<string> _rowUnparsed = new List<string>();
			List<string> table = new List<string>();
			List<List<string>> Qtable = new List<List<string>>();
			if((Files.Length > currentFile)) {
				path = Files[currentFile];
				sql_log(path, "");
				currentFile = (currentFile + 1);
				file_data = File.ReadAllText(path).Replace("", "").Replace("", "");
				table = Enumerable.ToList<System.String>(file_data.Trim().Split("Табли", System.StringSplitOptions.RemoveEmptyEntries));
				while(TableNext) {
					TableNext = false;
					//что б не зависало.
					yield return new WaitForSeconds(0.02F);
					if((table.Count > currentTable)) {
						One_table_data = table[currentTable];
						//Пропускаем "таблицы" где много точек = меню в начале файла
						if((!(One_table_data.Contains(".....")) && One_table_data.StartsWith("ца"))) {
							//Get Number of table
							N_table = Regex.Match(One_table_data, "^ца\\D*(\\d+.*)\\..*\\n", RegexOptions.None).Result("$1");
							switch(N_table) {
								case "11": {
									//Get subNumber of table N11&N21
									N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*(\\d+)\\).*\\n", RegexOptions.None).Result("$1_$2");
								}
								break;
								case "21": {
									//Get subNumber of table N11&N21
									N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*(\\d+)\\).*\\n", RegexOptions.None).Result("$1_$2");
								}
								break;
							}
							//Пропускаемые таблицы
							switch(N_table) {
								case "3": {
								}
								break;
								case "5": {
								}
								break;
								case "9": {
								}
								break;
								case "10": {
								}
								break;
								case "18": {
								}
								break;
								case "19": {
								}
								break;
								case "22": {
								}
								break;
								case "4a": {
								}
								break;
								default: {
									cachedValue = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None);
									N_year_N_month = cachedValue.Result("y$2_m$1");
									_rowUnparsed = splitTable(One_table_data, N_table);
									new WaitForEndOfFrame();
									delimetrs = _alllndexOfDelimeters(_rowUnparsed);
									new WaitForEndOfFrame();
									Qtable = parseRow(delimetrs, _rowUnparsed, N_table, N_year_N_month);
									new WaitForEndOfFrame();
									sql_insertTables(Qtable, N_table, N_year_N_month);
								}
								break;
							}
						}
						currentTable = (currentTable + 1);
						TableNext = true;
					}
				}
				Debug.Log("След файл");
				sql_close();
				FileNext = true;
				currentTable = 0;
			}
		}

		/// <summary>
		/// Пилим таблицы, при необходимости
		/// </summary>
		public List<string> splitTable(string One_table_data, string N_table) {
			List<string> _rowsUnparsed = new List<string>();
			List<string> _2thPart = default(List<string>);
			string tmp_row_raw = "";
			string tmp_ifNextMonth = "";
			int tmp_row_maxLenght = 0;
			int tmp_row_hulfLenght = 0;
			_rowsUnparsed.Clear();
			_rowsUnparsed = new List<string>();
			_2thPart = new List<string>();
			tmp_row_raw = "";
			tmp_row_maxLenght = 0;
			switch(N_table) {
				case "12": {
					//Если новые 12 таблицы делить не надо
					if((One_table_data.IndexOfAny(new char[] { '=', '|' }) > 0)) {
						One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None);
					} else {
						//пилим 12 таблицу пополам
						foreach(string loopObject in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
							if(!(Regex.IsMatch(loopObject.Trim(), "^ца\\D*(\\d+)\\."))) {
								tmp_row_raw = loopObject;
								if((tmp_row_raw.Length > tmp_row_maxLenght)) {
									//Тут всегда самая большая длина строки (из шапки)
									tmp_row_maxLenght = tmp_row_raw.Length;
									tmp_row_hulfLenght = (tmp_row_maxLenght / 2);
								} else //если первая неполная
								if((tmp_row_hulfLenght > tmp_row_raw.Length)) {
									//Если текущая строка меньше половины максимальной
									_rowsUnparsed.Add(tmp_row_raw.Substring(0, tmp_row_raw.Length));
								} else {
									//Если текущая строка больше половины максимальной
									_rowsUnparsed.Add(tmp_row_raw.Substring(0, tmp_row_hulfLenght));
									//второй столбец. засовываем оставшуюся строку
									_2thPart.Add(tmp_row_raw.Substring(tmp_row_hulfLenght, (tmp_row_raw.Length - tmp_row_hulfLenght)));
								}
							}
						}
						foreach(string loopObject1 in _2thPart) {
							if(Regex.IsMatch(loopObject1.TrimStart(), "^\\d{1,3}\\.")) {
								//второго столбца добавляем только строки
								_rowsUnparsed.Add(loopObject1);
							}
						}
					}
				}
				break;
				default: {
					tmp_ifNextMonth = "";
					//непилимые таблицы
					foreach(string loopObject2 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
						tmp_row_raw = loopObject2;
						if(loopObject2.Trim().ToLower().Contains("Переход".ToLower())) {
							//если есть в строке "Переход на следующий месяц"
							tmp_ifNextMonth = "_";
							sql_log("Переход на следующий месяц", "");
						}
						_rowsUnparsed.Add(tmp_ifNextMonth + tmp_row_raw);
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
					for(index = row.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index > -1; index = row.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index + 1))) {
						tmp_hash_ints.Add(index);
					}
				}
			}
			foreach(int loopObject4 in tmp_hash_ints) {
				Debug.Log(loopObject4);
			}
			return Enumerable.ToList<System.Int32>(tmp_hash_ints);
		}

		/// <summary>
		/// Пролучаем массив строк-ячеек из таблицы, чистые и обработанные
		/// </summary>
		public List<List<string>> parseRow(List<int> row_indexs_delimeters, List<string> _rowsUnparsed, string N_table, string N_year_N_month) {
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
			string tmp_line = "";
			string tmp_name = "";
			row_indexs_delimeters.Sort();
			_tableParsed = new List<List<string>>();
			//Для таблиц с несколькими строчками. 14+
			headerSkiped = false;
			//построчная обработка
			foreach(string loopObject5 in _rowsUnparsed) {
				line = loopObject5;
				if(Regex.IsMatch(line, "^ +═")) {
					//Сдвиг строки для кривой таблицы N12, второй её половины
					tmp_startLine = (line.Length - line.TrimStart().Length);
				}
				if((Regex.IsMatch(line.Trim(), "^\\d{1,3}\\.") || headerSkiped)) {
					headerSkiped = true;
					_rowsParsed = new List<string>();
					//Проверка на конец таблицы
					if(((row_indexs_delimeters[0] > line.Length) || string.IsNullOrEmpty(line.Substring(row_indexs_delimeters[0], (line.Length - row_indexs_delimeters[0])).Trim()))) {
						//пустота под новой шапкой?
						if(!((tmp_line.IndexOfAny(new char[] { '|', '=' }) > 0))) {
							headerSkiped = false;
						}
					} else if((row_indexs_delimeters[0] >= line.Trim().Length)) {
						sql_log("BD=" + line.Trim() + "==" + "Table=" + N_table + "==YM=" + N_year_N_month, "= Только название или вообще пустая строчка");
					} else {
						if(Regex.IsMatch(line.Substring(0, (row_indexs_delimeters[0] - 1)), "_\\D*\\d+\\.(.+)")) {
							//если начинается с подчёркивания="Переход на следующий месяц"
							tmp_name = "_" + Regex.Match(" " + line.Substring(0, (row_indexs_delimeters[0] - 1)).TrimStart(), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1");
						}
						if(string.IsNullOrEmpty(line.Substring(0, (row_indexs_delimeters[0] - 1)).TrimStart())) {
							//добавляем данные в первый столбец
							tmp_line = tmp_db_name + line.Trim().Substring(tmp_db_name.Length, (line.Length - tmp_db_name.Length));
						} else {
							//если не "Переход на следующий месяц", т.е. обычный
							tmp_name = Regex.Match(" " + line.Substring(0, (row_indexs_delimeters[0] - 1)).TrimStart(), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1");
							//сохранение имени бд, на случай пустой следующей строки
							tmp_db_name = NickBuhro.Translit.Transliteration.CyrillicToLatin(tmp_name.Trim(), NickBuhro.Translit.Language.Russian).ToLower().Replace(",", "_");
							tmp_line = line.Trim();
						}
						sql_log("BD=" + tmp_db_name + "==" + "Table=" + N_table + "==YM=" + N_year_N_month, "");
						_rowsParsed.Add(tmp_db_name);
						for(int index1 = 0; index1 < (row_indexs_delimeters.Count - 1); index1 += 1) {
							from = (row_indexs_delimeters[index1] + tmp_startLine);
							length = (row_indexs_delimeters[(index1 + 1)] - from);
							//Проверка на неполную строчку. заполнение @
							if((tmp_line.Length > from)) {
								if((tmp_line.Length >= (from + length))) {
									//Если совсем всё в порядке и вся ячейка что то имеет
									_rowsParsed.Add(tmp_line.Substring(from, length).Trim());
								} else {
									//Если нехватает символов в ячейке, но что то есть
									_rowsParsed.Add(tmp_line.Substring(from, (tmp_line.Length - from)).Trim());
								}
							} else {
								//если совсем ничего нету
								_rowsParsed.Add("@");
							}
						}
						//Проверка на неполную строчку. заполнение @
						if((tmp_line.Length > row_indexs_delimeters[row_indexs_delimeters.Count - 1])) {
							if((tmp_line.Length >= (row_indexs_delimeters[row_indexs_delimeters.Count - 1] + (tmp_line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])))) {
								//last. если все символы на месте.
								_rowsParsed.Add(tmp_line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (tmp_line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
							} else {
								//last. если нехватает некоторых символов
								_rowsParsed.Add(tmp_line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (tmp_line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
							}
						} else {
							//last. если ячейка совсем пустая
							_rowsParsed.Add("@");
							sql_log(N_table + "=" + line + "=" + "Строка не полная. в бд вместо недостающих ячеек внесена собака", "");
						}
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
			path1 = Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_Connections.ContainsKey(db_name)) {
				return true;
			} else {
				//Копирование пустой бд в новый файл
				if(!(File.Exists(path1))) {
					File.Copy(Application.streamingAssetsPath + "/" + "files/bd/" + "_empty" + ".sqlite", path1, false);
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
			foreach(KeyValuePair<string, SqliteCommand> loopObject6 in sql_cmnds) {
				loopObject6.Value.Dispose();
			}
			foreach(KeyValuePair<string, SqliteConnection> loopObject7 in sql_Connections) {
				loopObject7.Value.Dispose();
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
			tables.Clear();
			if(sql_connect(db_name)) {
				using(SqliteCommand value = sql_Connections[db_name].CreateCommand()) {
					sql_cmnds.Add(db_name, value);
					sql_cmnds[db_name].CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
					sql_readers.Add(db_name, sql_cmnds[db_name].ExecuteReader());
					while(sql_readers[db_name].Read()) {
						tables.Add(sql_readers[db_name].GetString(0));
					}
				}
			}
			return tables;
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
			string tmp_21_2 = "";
			foreach(List<string> loopObject8 in q_table) {
				row1 = loopObject8;
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
						//если новая 12 таблица - то обычный. если старая (короткая) то false
						if((row1.Count > 10)) {
							q = q_simple;
						} else {
							//N12
							q = "REPLACE INTO '" + "12" + "' " + "('N_year_N_month','dj','c','cm','tt','izm','gl','mm','gd','G')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
						}
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
					case "21_2": {
						//N21_2
						q = "REPLACE INTO '" + "21" + "' " + "('N_year_N_month','020_mid', '020_max', '020_min', '040_mid', '040_max', '040_min', '080_mid', '080_max', '080_min', '120_mid', '120_max', '120_min')" + " VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "21_3": {
						cachedValue1 = "'160_mid', '160_max', '160_min', '240_mid', '240_max', '240_min', '320_mid', '320_max', '320_min', 'dayFrz_002', 'dayFrz_005', 'dayFrz_010', 'dayFrz_015', 'dayFrz_02', 'dayFrz_04', 'dayFrz_08', 'dayFrz_12', 'dayFrz_16', 'dayFrz_24', 'dayFrz_32'";
						//N21_3
						for(int index3 = 0; index3 < cachedValue1.Split(",", System.StringSplitOptions.None).Length; index3 += 1) {
							tmp_21_2 = tmp_21_2 + cachedValue1.Split(",", System.StringSplitOptions.None)[index3] + " = " + "excluded." + cachedValue1.Split(",", System.StringSplitOptions.None)[index3] + ",";
						}
						q = "INSERT INTO '" + "21" + "'('N_year_N_month'," + cachedValue1 + ") VALUES ('" + N_year_N_month + "','" + string.Join<System.String>("','", row1) + "') ON CONFLICT(" + "N_year_N_month" + ") DO UPDATE SET " + tmp_21_2.TrimEnd(',');
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

		private void Start() {
			Files = Directory.GetFiles("D:\\__job\\2022\\13_юфо_ежемесячники\\txt\\", "*.txt");
			objectVariable4.gameObject.GetComponent<TMPro.TMP_Text>().text = Files.Length.ToString();
		}

		public System.Collections.IEnumerator NewFunction() {
			while(FileNext) {
				FileNext = false;
				TableNext = true;
				yield return new WaitForSeconds(0.5F);
				base.StartCoroutine(loadFromFiles());
			}
		}

		public void button() {
			base.StartCoroutine(NewFunction());
		}

		public void sql_log(string parameter, string parameter2) {
			sql_insertQ("_log", "INSERT INTO log VALUES('" + parameter + parameter2 + "')");
		}
	}
}
