#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using TMPro;
using System.Globalization;

namespace MaxyGames.Generated {
	public class b_Conv_24-11 : MaxyGames.RuntimeBehaviour {
		public string[] Files = new string[0];
		public Dictionary<string, string> bd_names = new Dictionary<string, string>();
		public List<string> rows_unparsed = new List<string>();
		public List<int> delimetrs = new List<int>();
		public string NameOfDB = "";
		public List<string> bd_names_array = new List<string>();
		public string Year = "";
		public string Month = "";
		public Dictionary<string, string> n1 = new Dictionary<string, string>();
		public Dictionary<string, string> n2 = new Dictionary<string, string>();
		public Dictionary<string, string> n3 = new Dictionary<string, string>();
		public Dictionary<string, Dictionary<string, string>> listOfSimpleDicTables = new Dictionary<string, Dictionary<string, string>>();
		public Dictionary<string, Dictionary<string, string>> listOfNamedDicTables = new Dictionary<string, Dictionary<string, string>>();
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		private int index2;
		private List<int> delimetrs1 = new List<int>();

		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {
				Debug.Log(n1.ToString());
			}
		}

		public void button() {
			n1 = new Dictionary<string, string>();
			n2 = new Dictionary<string, string>();
			n3 = new Dictionary<string, string>();
			//Собираем названия для бд
			bd_names();
			Files = Directory.GetFiles("Z:\\", "*.txt");
			objectVariable.gameObject.GetComponent<TMPro.TMP_Text>().text = Files.Length.ToString();
			base.StartCoroutine(convert_mainLoop());
		}

		public void bd_names() {
			bd_names.Clear();
			//Список на замену названий с "русского" на Русский
			foreach(string loopObject in File.ReadAllText(Application.streamingAssetsPath + "/" + "tmp.txt").Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				if(!(bd_names.ContainsKey(loopObject.Split(new char[] { '=' })[1].ToLower()))) {
					bd_names.Add(loopObject.Split(new char[] { '=' })[1].ToLower(), loopObject.Split(new char[] { '=' })[0]);
				}
			}
		}

		public System.Collections.IEnumerator convert_mainLoop() {
			string fPath = "";
			string q2 = "";
			string tmp_q = "";
			bd_names_array = new List<string>();
			for(int index = 0; index < Files.Length; index += 1) {
				fPath = Files[index];
				objectVariable1.gameObject.GetComponent<TMPro.TMP_Text>().text = fPath;
				objectVariable2.gameObject.GetComponent<TMPro.TMP_Text>().text = (index + 1).ToString();
				yield return selector_loop(fPath);
				Debug.Log("След файл");
			}
			Debug.Log("Закончена обработка файлов.");
			listOfSimpleDicTables.Clear();
			listOfSimpleDicTables.Add("n1", n1);
			listOfSimpleDicTables.Add("n2", n2);
			listOfNamedDicTables.Clear();
			listOfNamedDicTables.Add("n3", n3);
			//перебор названий постов-файлов бд
			foreach(string loopObject1 in bd_names_array) {
				yield return sql_named(loopObject1);
			}
		}

		public System.Collections.IEnumerator selector_loop(string path) {
			string file_data = "";
			string One_table_data = "";
			List<string> _rowUnparsed = new List<string>();
			List<string> _rowUnparsed0 = new List<string>();
			List<string> tables = new List<string>();
			List<List<string>> Qtable = new List<List<string>>();
			Dictionary<string, string> months = new Dictionary<string, string>() { { "ЯНВАРЬ", "1" }, { "ФЕВРАЛЬ", "2" }, { "МАРТ", "3" }, { "АПРЕЛЬ", "4" }, { "МАЙ", "5" }, { "ИЮНЬ", "6" }, { "ИЮЛЬ", "7" }, { "АВГУСТ", "8" }, { "СЕНТЯБРЬ", "9" }, { "ОКТЯБРЬ", "10" }, { "НОЯБРЬ", "11" }, { "ДЕКАБРЬ", "12" } };
			string N_year_N_month_FromHeader = "";
			string tmp_tblName = "";
			if(Regex.IsMatch(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)")) {
				Year = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$3");
				Month = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$2");
				//название поста, добавить сюда же потом проход через bd_names
				NameOfDB = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$1").Trim();
				//Проверка наличия в списке. если уже есть, то не добавлять, что бы потом лишний раз не бегать
				if(!(bd_names_array.Contains(NameOfDB))) {
					//Добавление имени бд для последующего sql инжекта
					bd_names_array.Add(NameOfDB);
				}
				file_data = File.ReadAllText(path);
				tables = Enumerable.ToList<System.String>(file_data.Trim().Split("Станция", System.StringSplitOptions.RemoveEmptyEntries));
				foreach(string loopObject2 in tables) {
					One_table_data = loopObject2;
					if(One_table_data.Contains("Атм. давление,")) {
						Debug.Log("=n1");
						yield return n1(One_table_data);
					} else if(One_table_data.Contains("С У Т О Ч Н Ы Е   Д А Н Н Ы Е")) {
						Debug.Log("=n2");
						yield return n2(One_table_data);
					} else if(One_table_data.Contains("А Т М О С Ф Е Р Н Ы Е   Я В Л Е Н И Я")) {
						Debug.Log("=n3");
						yield return n3(One_table_data);
					} else if(One_table_data.Contains("М Е С Я Ч Н Ы Е   В Ы В О Д Ы")) {
						Debug.Log("М Е С Я Ч Н Ы Е   В Ы В О Д Ы");
					} else if(One_table_data.Contains("ТЕМПЕРАТУРА ПОЧВЫ НА ГЛУБ. ЗА СУТКИ, град")) {
						Debug.Log("ТЕМПЕРАТУРА ПОЧВЫ НА ГЛУБ. ЗА СУТКИ, град");
					} else if(One_table_data.Contains("СТИХИЙНЫЕ Г/М ЯВЛЕНИЯ, СНЕГОСЪЕМКИ, Г/И ОТЛОЖЕНИЯ")) {
						Debug.Log("СТИХИЙНЫЕ Г/М ЯВЛЕНИЯ, СНЕГОСЪЕМКИ, Г/И ОТЛОЖЕНИЯ");
					}
				}
			}
		}

		/// <summary>
		/// подключение к бд, если не подключено. 
		/// </summary>
		private bool sql_dbExists(string db_name) {
			string path = "";
			SqliteConnection connection3 = default(SqliteConnection);
			path = Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			//Копирование пустой бд в новый файл
			if(!(File.Exists(path))) {
				//Копирование пустой бд в новый файл
				File.Copy(Application.streamingAssetsPath + "/" + "files/" + "_emptyNew" + ".sqlite", path, false);
			}
			return File.Exists(path);
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// </summary>
		private List<string> sql_master_tables(string db_name) {
			List<string> tables1 = new List<string>();
			SqliteCommand cmnd = default(SqliteCommand);
			SqliteDataReader reader = default(SqliteDataReader);
			SqliteConnection connection = new SqliteConnection();
			string path1 = "";
			tables1.Clear();
			path1 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value = new SqliteConnection("URI=file:" + path1)) {
					connection = value;
					connection.Open();
					cmnd = connection.CreateCommand();
					cmnd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
					reader = cmnd.ExecuteReader();
					while(reader.Read()) {
						tables1.Add(reader.GetString(0));
					}
				}
			}
			return tables1;
		}

		/// <summary>
		/// Получение списка заголовков в бд
		/// </summary>
		private List<string> sql_headers(string db_name) {
			SqliteCommand cmnd3 = new SqliteCommand();
			SqliteDataReader reader1 = default(SqliteDataReader);
		}

		public void sql_log(string parameter, string parameter2) {}

		public System.Collections.IEnumerator getDelims(string one_table_data) {
			string delim_row = "";
			int delim_rowCount = 10;
			HashSet<int> delim_hash_ints = new HashSet<int>();
			rows_unparsed.Clear();
			//Распилка
			foreach(string loopObject3 in one_table_data.TrimEnd().Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				rows_unparsed.Add(loopObject3);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed.Count < 10)) {
				delim_rowCount = rows_unparsed.Count;
			}
			for(int index1 = 0; index1 < delim_rowCount; index1 += 1) {
				delim_row = rows_unparsed[index1];
				//Бегает по строке - ищет приключений
				for(index2 = delim_row.IndexOfAny(new char[] { 'I', '|', '║', '│' }); index2 > -1; index2 = delim_row.IndexOfAny(new char[] { 'I', '|', '║', '│' }, (index2 + 1))) {
					delim_hash_ints.Add((index2 + 1));
				}
			}
			delimetrs = Enumerable.ToList<System.Int32>(delim_hash_ints);
			delimetrs.Sort();
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n1(string one_table_data) {
			string row_line = "";
			string row_1th = "";
			string row_2th = "";
			int row_day = 0;
			int row_dayPrev = 0;
			bool row_2th_bool = false;
			string row_cHour = "";
			List<string> row_parsed = new List<string>();
			string row_bd_name = "";
			int row_from = 0;
			int row_length = 0;
			string tmp_4sql_value = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject4 in rows_unparsed) {
				row_line = loopObject4;
				//Получение часа для строк (первая строчка)
				if(Regex.IsMatch(row_line, "поясное.*\\((\\d+)\\D*(\\d+)")) {
					row_1th = Regex.Match(row_line, "поясное.*\\((\\d+)\\D*(\\d+)", RegexOptions.None).Result("$1");
					row_2th = Regex.Match(row_line, "поясное.*\\((\\d+)\\D*(\\d+)", RegexOptions.None).Result("$2");
				} else if(Regex.IsMatch(row_line, "^ *(\\d+)")) {
					row_day = int.Parse(Regex.Match(row_line, "^ *(\\d+)", RegexOptions.None).Result("$1"));
					//поиск текущего часа (первая и вторая половины таблицы)
					if((row_day > row_dayPrev)) {
						row_dayPrev = row_day;
						row_cHour = row_1th;
					} else {
						row_cHour = row_2th;
					}
					//день месяца-первый столбец
					tmp_4sql_value = "" + "y20" + Year + "_m" + Month + "_d" + row_day.ToString() + "_h" + row_cHour;
					//основное тело распарса строки
					for(int index3 = 0; index3 < (delimetrs.Count - 1); index3 += 1) {
						row_from = delimetrs[index3];
						row_length = (delimetrs[(index3 + 1)] - row_from);
						//Проверка на неполную строчку. заполнение @
						if((row_line.Length > row_from)) {
							if((row_line.Length >= (row_from + row_length))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(row_from, row_length).Trim();
							} else {
								//Если нехватает символов в ячейке, но что то есть
								tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(row_from, (row_line.Length - row_from)).Trim();
							}
						} else {
							//если совсем ничего нету
							tmp_4sql_value = tmp_4sql_value + "','" + "@";
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line.Length >= (delimetrs[delimetrs.Count - 1] + (row_line.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim();
						} else {
							//last. если нехватает некоторых символов
							tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim();
						}
					} else {
						//если совсем ничего нету
						tmp_4sql_value = tmp_4sql_value + "','" + "@";
					}
					if(n1.ContainsKey(tmp_4sql_value)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_4sql_value);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						n1.Add(NameOfDB + "&" + Year + "_" + Month + "_" + row_day.ToString() + "_" + row_cHour, tmp_4sql_value);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n2(string one_table_data) {
			string row_line1 = "";
			int row_day1 = 0;
			List<string> row_parsed1 = new List<string>();
			string row_bd_name1 = "";
			int row_from1 = 0;
			int row_length1 = 0;
			string tmp_4sql_value1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject5 in rows_unparsed) {
				row_line1 = loopObject5;
				//break on
				if(row_line1.Contains("Средние")) {
					break;
				} else if(Regex.IsMatch(row_line1, "^ *(\\d+)")) {
					row_day1 = int.Parse(Regex.Match(row_line1, "^ *(\\d+)", RegexOptions.None).Result("$1"));
					//день месяца-первый столбец
					tmp_4sql_value1 = "" + "y20" + Year + "_m" + Month + "_d" + row_day1.ToString();
					//основное тело распарса строки
					for(int index4 = 0; index4 < (delimetrs.Count - 1); index4 += 1) {
						row_from1 = delimetrs[index4];
						row_length1 = (delimetrs[(index4 + 1)] - row_from1);
						//Проверка на неполную строчку. заполнение @
						if((row_line1.Length > row_from1)) {
							if((row_line1.Length >= (row_from1 + row_length1))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(row_from1, row_length1).Trim();
							} else {
								//Если нехватает символов в ячейке, но что то есть
								tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(row_from1, (row_line1.Length - row_from1)).Trim();
							}
						} else {
							//если совсем ничего нету
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + "@";
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line1.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line1.Length >= (delimetrs[delimetrs.Count - 1] + (row_line1.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(delimetrs[delimetrs.Count - 1], (row_line1.Length - delimetrs[delimetrs.Count - 1])).Trim();
						} else {
							//last. если нехватает некоторых символов
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(delimetrs[delimetrs.Count - 1], (row_line1.Length - delimetrs[delimetrs.Count - 1])).Trim();
						}
					} else {
						//если совсем ничего нету
						tmp_4sql_value1 = tmp_4sql_value1 + "','" + "@";
					}
					if(n2.ContainsKey(tmp_4sql_value1)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_4sql_value1);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						n2.Add(NameOfDB + "&" + Year + "_" + Month + "_" + row_day1.ToString(), tmp_4sql_value1);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// sql вставка n1 таблицы в файлы.
		/// </summary>
		private System.Collections.IEnumerator sql_simple(string db_name) {
			int sql_writed = 0;
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteConnection connection1 = new SqliteConnection();
			string path2 = "";
			string q = "";
			int add_new = 0;
			string t_key = "";
			Dictionary<string, string> t_dic = new Dictionary<string, string>();
			string Ntable = "";
			path2 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value1 = new SqliteConnection("URI=file:" + path2)) {
					connection1 = value1;
					connection1.Open();
					//перебор первой особой таблицы
					foreach(KeyValuePair<string, Dictionary<string, string>> loopObject6 in listOfSimpleDicTables) {
						t_dic = loopObject6.Value;
						Ntable = loopObject6.Key;
						sql_writed = 0;
						//перебор первой особой таблицы
						foreach(string loopObject7 in Enumerable.ToList<System.String>(t_dic.Keys)) {
							t_key = loopObject7;
							//поиск совпадений ключей с текущим названием
							if(t_key.Contains(db_name)) {
								q = "INSERT OR IGNORE INTO '" + Ntable + "' " + "" + " VALUES ('" + t_dic[t_key] + "" + "" + "')";
								//Удаляет из библиотеки найденное и совпадённое
								t_dic.Remove(t_key);
								using(SqliteCommand value2 = connection1.CreateCommand()) {
									cmnd1 = value2;
									cmnd1.CommandText = q;
									add_new = cmnd1.ExecuteReader().RecordsAffected;
									sql_writed = (add_new + sql_writed);
								}
								cmnd1.Cancel();
							}
						}
						Debug.Log("Файл=" + t_key + "=" + "Таблица=" + Ntable + "=, внесено=" + sql_writed.ToString());
					}
				}
				//Пишет что база закрыта, но файл удалить не закрывая редактор всё равно не получается, что за фигня..
				Debug.Log(connection1.State.ToString());
				connection1 = null;
				SqliteConnection.ClearAllPools();
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}
		}

		public System.Collections.IEnumerator n3(string one_table_data) {
			string row_line2 = "";
			int row_day2 = 0;
			int row_dayPrev1 = 0;
			string row_bd_name2 = "";
			int row_from2 = 0;
			int row_length2 = 0;
			string tmp_4sql_value2 = "";
			Dictionary<int, string> tmp_Allday = new Dictionary<int, string>();
			string tmp_headers = "";
			string tmp_values = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject8 in rows_unparsed) {
				row_line2 = loopObject8.TrimEnd();
				if(Regex.IsMatch(row_line2, "^ *(\\d+)")) {
					row_day2 = int.Parse(Regex.Match(row_line2, "^ *(\\d+)", RegexOptions.None).Result("$1"));
					row_dayPrev1 = row_day2;
				}
				//Пропуск шапки.
				//В строках без дня, день=предыдущему
				if((row_dayPrev1 != 0)) {
					row_from2 = delimetrs[1];
					row_length2 = row_line2.Length;
					//пропуск строк в которых нету нужного столбца
					if((row_length2 > row_from2)) {
						if(tmp_Allday.ContainsKey(row_day2)) {
							//если 3й столбец не в один ряд, а в несколько
							tmp_Allday[row_day2] = tmp_Allday[row_day2] + " " + row_line2.Substring(row_from2, (row_length2 - row_from2)).Trim();
						} else {
							//если в один ряд
							tmp_Allday.Add(row_day2, row_line2.Substring(row_from2, (row_length2 - row_from2)).Trim());
						}
					}
				}
			}
			//Парс либы дней в конечный формат "почтиSqlite"
			foreach(KeyValuePair<int, string> loopObject9 in tmp_Allday) {
				//Если совсем всё в порядке и вся ячейка что то имеет
				tmp_4sql_value2 = NameOfDB + "&" + Year + "_" + Month + "_" + loopObject9.Key.ToString();
				if(n3.ContainsKey(tmp_4sql_value2)) {
					//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
					Debug.Log("Ключ уже есть:" + tmp_4sql_value2);
				} else {
					//Засовываем сразу почти готовую строчку для sql
					n3.Add(tmp_4sql_value2, "'Year_Month_Day'," + new Regex("([А-Яа-я]+)\\s*(\\d+)\\s*", RegexOptions.None).Replace(loopObject9.Value, "'$1',").TrimEnd(',') + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + loopObject9.Key.ToString() + "'" + "," + new Regex("([А-Яа-я]+)\\s*(\\d+)\\s*", RegexOptions.None).Replace(loopObject9.Value, "'$2',").Replace("00", "00.1").TrimEnd(','));
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// sql вставка n1 таблицы в файлы.
		/// </summary>
		private System.Collections.IEnumerator sql_named(string db_name) {
			int sql_writed1 = 0;
			SqliteCommand cmnd2 = new SqliteCommand();
			SqliteConnection connection2 = new SqliteConnection();
			string path3 = "";
			string q1 = "";
			int add_new1 = 0;
			string t_key1 = "";
			Dictionary<string, string> t_dic1 = new Dictionary<string, string>();
			string Ntable1 = "";
			path3 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value3 = new SqliteConnection("URI=file:" + path3)) {
					connection2 = value3;
					connection2.Open();
					//перебор первой особой таблицы
					foreach(KeyValuePair<string, Dictionary<string, string>> loopObject10 in listOfNamedDicTables) {
						t_dic1 = loopObject10.Value;
						Ntable1 = loopObject10.Key;
						sql_writed1 = 0;
						//перебор первой особой таблицы
						foreach(string loopObject11 in Enumerable.ToList<System.String>(t_dic1.Keys)) {
							t_key1 = loopObject11;
							//поиск совпадений ключей с текущим названием
							if(t_key1.Contains(db_name)) {
								q1 = "INSERT OR IGNORE INTO '" + Ntable1 + "' (" + t_dic1[t_key1].Split(new char[] { '&' })[0] + ") VALUES (" + t_dic1[t_key1].Split(new char[] { '&' })[1] + ")";
								//Удаляет из библиотеки найденное и совпадённое
								t_dic1.Remove(t_key1);
								using(SqliteCommand value4 = connection2.CreateCommand()) {
									cmnd2 = value4;
									cmnd2.CommandText = q1;
									add_new1 = cmnd2.ExecuteReader().RecordsAffected;
									sql_writed1 = (add_new1 + sql_writed1);
								}
								cmnd2.Cancel();
							}
						}
						Debug.Log("Файл=" + t_key1 + "=" + "Таблица=" + Ntable1 + "=, внесено=" + sql_writed1.ToString());
					}
				}
				//Пишет что база закрыта, но файл удалить не закрывая редактор всё равно не получается, что за фигня..
				Debug.Log(connection2.State.ToString());
				connection2 = null;
				SqliteConnection.ClearAllPools();
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}
		}
	}
}
