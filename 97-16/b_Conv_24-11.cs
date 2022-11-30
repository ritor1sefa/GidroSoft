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
		public Dictionary<string, string> simpleTables = new Dictionary<string, string>();
		public Dictionary<string, string> namedTables = new Dictionary<string, string>();
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		private int index2;
		private List<int> delimetrs1 = new List<int>();

		private void Update() {
			if(Input.GetKeyUp(KeyCode.UpArrow)) {
				//если это убрать, то parseRow2List яростно багует, но не всегда.. 
				Debug.Log(new Regex("[^0-9.-]*").Replace("-0.02 │      │      │      │     -0│     0│    0 │      │    0 │      │ 3.20", ""));
			}
		}

		public void button() {
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
				Debug.Log("=След файл");
			}
			Debug.Log("Закончена обработка файлов.");
			//перебор названий постов-файлов бд
			foreach(string loopObject1 in bd_names_array) {
				yield return sql_simple(loopObject1);
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
				file_data = new Regex("^(.*.*).+$", RegexOptions.Multiline).Replace(File.ReadAllText(path), "");
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
						yield return n4_1(One_table_data.TrimEnd().Substring(0, One_table_data.TrimEnd().IndexOf("Ч и с л о   с л у ч а е в   п о   г р а д а ц и я м")));
						yield return n4_2(One_table_data.TrimEnd().Substring(One_table_data.TrimEnd().IndexOf("Ч и с л о   с л у ч а е в   п о   г р а д а ц и я м"), (One_table_data.TrimEnd().IndexOf("Число дней с осадками по градациям") - One_table_data.TrimEnd().IndexOf("Ч и с л о   с л у ч а е в   п о   г р а д а ц и я м"))));
						yield return n4_3(One_table_data.TrimEnd().Substring(One_table_data.TrimEnd().IndexOf("Число дней с осадками по градациям")));
					} else if(One_table_data.Contains("ТЕМПЕРАТУРА ПОЧВЫ НА ГЛУБ. ЗА СУТКИ, град")) {
						Debug.Log("=20-21=ТЕМПЕРАТУРА ПОЧВЫ НА ГЛУБ. ЗА СУТКИ, град");
						yield return t20_21(One_table_data);
					} else if(One_table_data.Contains("Г/М ЯВЛЕНИЯ")) {
						Debug.Log("Г/М ЯВЛЕНИЯ, СНЕГОСЪЕМКИ, Г/И ОТЛОЖЕНИЯ");
						if(One_table_data.Contains("Вид явления")) {
							Debug.Log("=14=СТИХИЙНЫЕ Г/М ЯВЛЕНИЯ");
							yield return t14(One_table_data.Substring(0, One_table_data.IndexOf("К о н е ц   т а б л и ц ы   с   д а н н ы м и   о б   О Я")));
						} else if(One_table_data.Contains("Маршрут")) {
							Debug.Log("=16=С Н Е Ж Н Ы Й   П О К Р О В");
							yield return t16(One_table_data.Substring(One_table_data.IndexOf("С Н Е Ж Н Ы Й   П О К Р О В"), (One_table_data.IndexOf(" К о н е ц   т а б л и ц ы   с   р е з у л ь т а т а м и   с н е г о с ъ е м о к") - One_table_data.IndexOf("С Н Е Ж Н Ы Й   П О К Р О В"))));
						} else if(One_table_data.Contains("ГОЛОЛЕДНО-ИЗМОРОЗЕВЫМИ  ОТЛОЖЕНИЯМИ")) {
							Debug.Log("=22=ГОЛОЛЕДНО-ИЗМОРОЗЕВЫМИ  ОТЛОЖЕНИЯМИ");
							yield return t22(One_table_data.Substring(One_table_data.IndexOf("ГОЛОЛЕДНО-ИЗМОРОЗЕВЫМИ  ОТЛОЖЕНИЯМИ"), (One_table_data.IndexOf("Конец таблицы с результатами наблюдений за г/и отложениями") - One_table_data.IndexOf("ГОЛОЛЕДНО-ИЗМОРОЗЕВЫМИ  ОТЛОЖЕНИЯМИ"))));
						}
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

		/// <summary>
		/// sql вставка Simple таблиц в файлы.
		/// </summary>
		private System.Collections.IEnumerator sql_simple(string db_name) {
			int sql_writed = 0;
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteConnection connection1 = new SqliteConnection();
			string path2 = "";
			string q = "";
			int add_new = 0;
			string t_key = "";
			string Ntable = "";
			path2 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value1 = new SqliteConnection("URI=file:" + path2)) {
					connection1 = value1;
					connection1.Open();
					sql_writed = 0;
					//перебор первой особой таблицы
					foreach(string loopObject4 in Enumerable.ToList<System.String>(simpleTables.Keys)) {
						t_key = loopObject4;
						Ntable = t_key.Split(new char[] { '&' })[1];
						//поиск совпадений ключей с текущим названием
						if(t_key.Contains(db_name)) {
							q = "INSERT OR IGNORE INTO '" + Ntable + "' " + "" + " VALUES ('" + simpleTables[t_key] + "" + "" + "')";
							//Удаляет из библиотеки найденное и совпадённое
							simpleTables.Remove(t_key);
							using(SqliteCommand value2 = connection1.CreateCommand()) {
								cmnd1 = value2;
								cmnd1.CommandText = q;
								add_new = cmnd1.ExecuteReader().RecordsAffected;
								sql_writed = (add_new + sql_writed);
							}
							cmnd1.Cancel();
						}
					}
					Debug.Log("Файл=" + t_key + "=" + "" + "" + "=внесено=" + sql_writed.ToString());
				}
				//Пишет что база закрыта, но файл удалить не закрывая редактор всё равно не получается, что за фигня..
				Debug.Log(connection1.State.ToString());
				connection1 = null;
				SqliteConnection.ClearAllPools();
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}
		}

		/// <summary>
		/// sql вставка таблиц с выборочными столбцами в файлы.
		/// </summary>
		private System.Collections.IEnumerator sql_named(string db_name) {
			int sql_writed1 = 0;
			SqliteCommand cmnd2 = new SqliteCommand();
			SqliteConnection connection2 = new SqliteConnection();
			string path3 = "";
			string q1 = "";
			int add_new1 = 0;
			string t_key1 = "";
			string Ntable1 = "";
			path3 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value3 = new SqliteConnection("URI=file:" + path3)) {
					connection2 = value3;
					connection2.Open();
					sql_writed1 = 0;
					//перебор первой особой таблицы
					foreach(string loopObject5 in Enumerable.ToList<System.String>(namedTables.Keys)) {
						t_key1 = loopObject5;
						//Получение номера таблицы из ключа
						Ntable1 = t_key1.Split(new char[] { '&' })[1];
						//поиск совпадений ключей с текущим названием
						if(t_key1.Contains(db_name)) {
							q1 = "INSERT OR IGNORE INTO '" + Ntable1 + "' (" + namedTables[t_key1].Split(new char[] { '&' })[0] + ") VALUES (" + namedTables[t_key1].Split(new char[] { '&' })[1] + ")";
							//Удаляет из библиотеки найденное и совпадённое
							namedTables.Remove(t_key1);
							using(SqliteCommand value4 = connection2.CreateCommand()) {
								cmnd2 = value4;
								cmnd2.CommandText = q1;
								add_new1 = cmnd2.ExecuteReader().RecordsAffected;
								sql_writed1 = (add_new1 + sql_writed1);
							}
							cmnd2.Cancel();
						}
					}
					Debug.Log("Файл=" + t_key1 + "=" + "" + "" + "=внесено=" + sql_writed1.ToString());
				}
				//Пишет что база закрыта, но файл удалить не закрывая редактор всё равно не получается, что за фигня..
				Debug.Log(connection2.State.ToString());
				connection2 = null;
				SqliteConnection.ClearAllPools();
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}
		}

		public System.Collections.IEnumerator n1(string one_table_data) {
			string row_line = "";
			string row_1th = "";
			string row_2th = "";
			int row_day = 0;
			int row_dayPrev = 0;
			bool row_2th_bool = false;
			string row_cHour = "";
			int row_from = 0;
			int row_length = 0;
			string tmp_4sql_value = "";
			string tmp_key = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject6 in rows_unparsed) {
				row_line = loopObject6;
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
							tmp_4sql_value = tmp_4sql_value + "','" + "";
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
						tmp_4sql_value = tmp_4sql_value + "','" + "";
					}
					tmp_key = NameOfDB + "&" + "n1" + "&" + "y20" + Year + "_m" + Month + "_d" + row_day.ToString() + "_h" + row_cHour;
					if(simpleTables.ContainsKey(tmp_key)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key, tmp_4sql_value);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n2(string one_table_data) {
			string row_line1 = "";
			int row_day1 = 0;
			int row_from1 = 0;
			int row_length1 = 0;
			string tmp_4sql_value1 = "";
			string tmp_key1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject7 in rows_unparsed) {
				row_line1 = loopObject7;
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
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + "";
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
						tmp_4sql_value1 = tmp_4sql_value1 + "','" + "";
					}
					tmp_key1 = NameOfDB + "&n2&" + Year + "_" + Month + "_" + row_day1.ToString();
					if(simpleTables.ContainsKey(tmp_key1)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key1);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key1, tmp_4sql_value1);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n3(string one_table_data) {
			string row_line2 = "";
			int row_day2 = 0;
			int row_dayPrev1 = 0;
			int row_from2 = 0;
			int row_length2 = 0;
			string tmp_key2 = "";
			Dictionary<int, string> tmp_Allday = new Dictionary<int, string>();
			string tmp_headers = "";
			string tmp_values3 = "";
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
				tmp_key2 = NameOfDB + "&" + "n3" + "&" + Year + "_" + Month + "_" + loopObject9.Key.ToString();
				if(namedTables.ContainsKey(tmp_key2)) {
					//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
					Debug.Log("Ключ уже есть:" + tmp_key2);
				} else {
					//Засовываем сразу почти готовую строчку для sql
					namedTables.Add(tmp_key2, "'Year_Month_Day'," + new Regex("([А-Яа-я]+)\\s*(\\d+)\\s*", RegexOptions.None).Replace(loopObject9.Value, "'$1',").TrimEnd(',') + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + loopObject9.Key.ToString() + "'" + "," + new Regex("([А-Яа-я]+)\\s*(\\d+)\\s*", RegexOptions.None).Replace(loopObject9.Value, "'$2',").Replace("00", "00.1").TrimEnd(','));
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n4_1(string one_table_data) {
			string row_line3 = "";
			int row_from3 = 0;
			int row_length3 = 0;
			string tmp_4sql_value2 = "";
			string tmp_key3 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject10 in rows_unparsed) {
				row_line3 = loopObject10;
				//нужна только одна строчка
				if(row_line3.Contains("Повт")) {
					//день месяца-первый столбец
					tmp_4sql_value2 = "" + "y20" + Year + "_m" + Month;
					//основное тело распарса строки
					for(int index5 = 0; index5 < (delimetrs.Count - 1); index5 += 2) {
						row_from3 = delimetrs[index5];
						row_length3 = (delimetrs[(index5 + 1)] - row_from3);
						//Проверка на неполную строчку. заполнение @
						if((row_line3.Length > row_from3)) {
							if((row_line3.Length >= (row_from3 + row_length3))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								tmp_4sql_value2 = tmp_4sql_value2 + "','" + row_line3.Substring(row_from3, row_length3).Trim();
							} else {
								//Если нехватает символов в ячейке, но что то есть
								tmp_4sql_value2 = tmp_4sql_value2 + "','" + row_line3.Substring(row_from3, (row_line3.Length - row_from3)).Trim();
							}
						} else {
							//если совсем ничего нету
							tmp_4sql_value2 = tmp_4sql_value2 + "','" + "";
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line3.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line3.Length >= (delimetrs[delimetrs.Count - 1] + (row_line3.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							tmp_4sql_value2 = tmp_4sql_value2 + "','" + row_line3.Substring(delimetrs[delimetrs.Count - 1], (row_line3.Length - delimetrs[delimetrs.Count - 1])).Trim();
						} else {
							//last. если нехватает некоторых символов
							tmp_4sql_value2 = tmp_4sql_value2 + "','" + row_line3.Substring(delimetrs[delimetrs.Count - 1], (row_line3.Length - delimetrs[delimetrs.Count - 1])).Trim();
						}
					} else {
						//если совсем ничего нету
						tmp_4sql_value2 = tmp_4sql_value2 + "','" + "";
					}
					tmp_key3 = NameOfDB + "&n4_1&" + Year + "_" + Month;
					if(simpleTables.ContainsKey(tmp_key3)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key3);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key3, tmp_4sql_value2);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n4_2(string one_table_data) {
			string row_line4 = "";
			int row_from4 = 0;
			int row_length4 = 0;
			string tmp_4sql_value3 = "";
			string tmp_key4 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject11 in rows_unparsed) {
				row_line4 = loopObject11;
				//нужна только одна строчка
				if(row_line4.Contains("проц")) {
					//день месяца-первый столбец
					tmp_4sql_value3 = "" + "y20" + Year + "_m" + Month + "','','','";
					//основное тело распарса строки
					for(int index6 = 0; index6 < 15; index6 += 1) {
						row_from4 = delimetrs[index6];
						row_length4 = (delimetrs[(index6 + 1)] - row_from4);
						//Проверка на неполную строчку. заполнение @
						if((row_line4.Length > row_from4)) {
							if((row_line4.Length >= (row_from4 + row_length4))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								tmp_4sql_value3 = tmp_4sql_value3 + "','" + row_line4.Substring(row_from4, row_length4).Trim();
							} else {
								//Если нехватает символов в ячейке, но что то есть
								tmp_4sql_value3 = tmp_4sql_value3 + "','" + row_line4.Substring(row_from4, (row_line4.Length - row_from4)).Trim();
							}
						} else {
							//если совсем ничего нету
							tmp_4sql_value3 = tmp_4sql_value3 + "','" + "";
						}
					}
					tmp_key4 = NameOfDB + "&4&" + Year + "_" + Month;
					if(simpleTables.ContainsKey(tmp_key4)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key4);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key4, tmp_4sql_value3);
					}
					break;
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n4_3(string one_table_data) {
			string row_line5 = "";
			string tmp_key5 = "";
			List<int> tmp_delim = new List<int>() { 7, 13, 19, 25, 32, 38, 42, 46, 50, 54, 58, 62, 66, 70, 74, 80, 87, 92, 99, 106, 113, 116, 119, 122, 125, 34 };
			List<string> tmp_values = new List<string>();
			yield return getDelims(one_table_data);
			tmp_delim.Sort();
			delimetrs = tmp_delim;
			//построчная обработка
			foreach(string loopObject12 in rows_unparsed) {
				row_line5 = loopObject12;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line5, "^ *(\\d+)")) {
					tmp_values = parseRow2List(row_line5);
					//set values for 7 table
					tmp_key5 = NameOfDB + "&" + "7" + "&" + Year + "_" + Month;
					if(simpleTables.ContainsKey(tmp_key5)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key5);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key5, "" + "y20" + Year + "_m" + Month + "','" + string.Join<System.String>("','", tmp_values.GetRange(0, 17)));
					}
					//set values for n5 table
					tmp_key5 = NameOfDB + "&" + "n5" + "&" + Year + "_" + Month;
					if(simpleTables.ContainsKey(tmp_key5)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key5);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key5, "" + "y20" + Year + "_m" + Month + "','" + string.Join<System.String>("','", tmp_values.GetRange(17, 10)));
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator t20_21(string one_table_data) {
			string tmp_topT = "";
			string tmp_bottomT = "";
			string row_line6 = "";
			List<string> tmp_mid = new List<string>();
			List<string> tmp_max = new List<string>();
			List<string> tmp_min = new List<string>();
			List<string> tmp_frz = new List<string>();
			string tmp_key6 = "";
			string tmp_20_line = "";
			string tmp_line_frz = "";
			string tmp_21_line = "";
			tmp_topT = One_table_data.TrimEnd().Substring(0, One_table_data.TrimEnd().IndexOf("Ч и с л о   д н е й   с   м о р о з о м"));
			tmp_bottomT = One_table_data.TrimEnd().Substring(One_table_data.TrimEnd().IndexOf("Ч и с л о   д н е й   с   м о р о з о м"));
			yield return getDelims(tmp_topT);
			//построчная обработка
			foreach(string loopObject13 in rows_unparsed) {
				row_line6 = loopObject13;
				//только строчку с цифрами
				if(row_line6.Contains("Ср.мес")) {
					tmp_mid = parseRow2List(row_line6);
				} else //только строчку с цифрами
				if(row_line6.Contains("Максим")) {
					tmp_max = parseRow2List(row_line6);
				} else //только строчку с цифрами
				if(row_line6.Contains("Миним")) {
					tmp_min = parseRow2List(row_line6);
				}
			}
			yield return getDelims(tmp_bottomT);
			//построчная обработка
			foreach(string loopObject14 in rows_unparsed) {
				row_line6 = loopObject14;
				//только строчку с цифрами
				if(row_line6.Contains("естественным покровом ")) {
					tmp_frz = parseRow2List(row_line6);
				}
			}
			//склеивание списков для 20 таблицы
			for(int index7 = 1; index7 < 5; index7 += 1) {
				tmp_20_line = tmp_20_line + "','" + tmp_mid[index7] + "','" + tmp_max[index7] + "','" + tmp_min[index7];
			}
			//set values for n20 table
			tmp_key6 = NameOfDB + "&" + "20" + "&" + Year + "_" + Month;
			if(simpleTables.ContainsKey(tmp_key6)) {
				//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
				Debug.Log("Ключ уже есть:" + tmp_key6);
			} else {
				simpleTables.Add(tmp_key6, "" + "y20" + Year + "_m" + Month + tmp_20_line);
			}
			//склеивание списков для 21 таблицы
			for(int index8 = 9; index8 < 16; index8 += 1) {
				tmp_21_line = tmp_21_line + "','" + tmp_mid[index8] + "','" + tmp_max[index8] + "','" + tmp_min[index8];
			}
			tmp_21_line = tmp_21_line + "','" + new Regex("[^0-9.,'-]*").Replace(string.Join<System.String>("','", tmp_frz), "");
			//set values for n21 table
			tmp_key6 = NameOfDB + "&" + "21" + "&" + Year + "_" + Month;
			if(simpleTables.ContainsKey(tmp_key6)) {
				//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
				Debug.Log("Ключ уже есть:" + tmp_key6);
			} else {
				simpleTables.Add(tmp_key6, "" + "y20" + Year + "_m" + Month + tmp_21_line);
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator t14(string one_table_data) {
			string row_line7 = "";
			List<string> tmp_values1 = new List<string>();
			string tmp_key7 = "";
			string tmp_key4DB = "";
			string tmp_4sql_value4 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject15 in rows_unparsed) {
				row_line7 = loopObject15;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line7, "^[А-я ]+\\d")) {
					tmp_values1 = parseRow2List(row_line7);
					tmp_4sql_value4 = tmp_values1[1].Trim() + "','" + tmp_values1[2].Trim() + " " + tmp_values1[3].Trim() + "','" + tmp_values1[4].Trim() + "','" + tmp_values1[5].Trim() + " " + tmp_values1[6].Trim() + "','" + tmp_values1[7].Trim() + "','" + tmp_values1[0].Trim() + "','" + tmp_values1[8].Trim() + "','" + tmp_values1[9].Trim();
					tmp_key4DB = new Regex("[',]*").Replace(tmp_4sql_value4, "");
					//set values for n21 table
					tmp_key7 = NameOfDB + "&" + "14" + "&" + Year + "_" + Month + "_k" + tmp_key4DB;
					if(simpleTables.ContainsKey(tmp_key7)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key7);
					} else {
						simpleTables.Add(tmp_key7, "" + "y20" + Year + "_m" + Month + "_k" + tmp_key4DB + "','" + tmp_4sql_value4);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator t16(string one_table_data) {
			string row_line8 = "";
			List<string> tmp_values2 = new List<string>();
			string tmp_key8 = "";
			string tmp_4sql_value5 = "";
			string tmp_key4DB1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject16 in rows_unparsed) {
				row_line8 = loopObject16;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line8, "^[А-я ]+\\d")) {
					tmp_values2 = parseRow2List(row_line8);
					tmp_4sql_value5 = new Regex("[ ]*").Replace("','','','','','" + string.Join<System.String>("','", tmp_values2), "");
					tmp_key4DB1 = new Regex("[',]*").Replace(tmp_4sql_value5, "");
					//set values for 7 table
					tmp_key8 = NameOfDB + "&" + "16" + "&" + Year + "_" + Month + "_k" + tmp_key4DB1;
					if(simpleTables.ContainsKey(tmp_key8)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key8);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key8, "" + "y20" + Year + "_m" + Month + "_k" + tmp_key4DB1 + "','" + tmp_4sql_value5);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator t22(string one_table_data) {
			string c_row_line = "";
			string c_newTable = "";
			List<string> tmp_newTable = new List<string>();
			List<string> row_parsed = new List<string>();
			int cell_tmp = 0;
			int cell_6th = 0;
			int cell_7th = 0;
			List<List<string>> final_row = new List<List<string>>();
			List<string> last_list_row = new List<string>();
			string tmp_key9 = "";
			string tmp_key4DB2 = "";
			string tmp_4sql_value6 = "";
			yield return getDelims(one_table_data);
			tmp_newTable = new List<string>();
			//построчная обработка
			foreach(string loopObject17 in rows_unparsed) {
				c_row_line = loopObject17;
				//новый список строк. чистый
				if(!((((c_row_line.Contains("ГОЛОЛЕДНО") || string.IsNullOrWhiteSpace(c_row_line)) || Regex.IsMatch(c_row_line, "[║╟╦╢├┬┤│|I═=]")) || c_row_line.Contains("Переход")))) {
					tmp_newTable.Add(c_row_line);
				}
			}
			final_row = new List<List<string>>();
			//построчная обработка
			foreach(string loopObject18 in tmp_newTable) {
				row_parsed = parseRow2List(loopObject18);
				if(string.IsNullOrWhiteSpace(row_parsed[0].Trim())) {
					//Если строчка начинается с пустоты=продолжение предыдущей
					if(int.TryParse(row_parsed[6], out cell_tmp)) {
						//6й стобец
						if((cell_6th < cell_tmp)) {
							cell_6th = cell_tmp;
							final_row[final_row.Count - 1][6] = cell_6th.ToString();
						}
					} else {
						Debug.Log("6й столбей не парсанулся! " + row_parsed[6]);
					}
					//Если строчка начинается с пустоты=продолжение предыдущей
					if(int.TryParse(row_parsed[7], out cell_tmp)) {
						//7й столбец
						if((cell_7th < cell_tmp)) {
							cell_7th = cell_tmp;
							final_row[final_row.Count - 1][7] = cell_7th.ToString();
						}
					} else {
						Debug.Log("7й столбей не парсанулся! " + row_parsed[7]);
					}
				} else {
					int.TryParse(row_parsed[6], out cell_6th);
					int.TryParse(row_parsed[7], out cell_7th);
					final_row.Add(row_parsed);
				}
			}
			yield return new WaitForEndOfFrame();
			//формирование строк для/в dict`s
			foreach(List<string> loopObject19 in final_row) {
				last_list_row = loopObject19;
				last_list_row.RemoveAt(0);
				//Поднимаем буквы, потому что в другом наборе данных - они почему то подняты.
				last_list_row[0] = last_list_row[0].ToUpper();
				//магия.. убирания лишних пробелов
				tmp_4sql_value6 = string.Join("','", Enumerable.Select<System.String, System.String>(last_list_row, (string parameterValues) => {
					return parameterValues.Trim();
				}));
				tmp_key4DB2 = new Regex("[',]*").Replace(tmp_4sql_value6, "");
				//set values for n22 table
				tmp_key9 = NameOfDB + "&" + "22" + "&" + Year + "_" + Month + "_k" + tmp_key4DB2;
				if(simpleTables.ContainsKey(tmp_key9)) {
					//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
					Debug.Log("Ключ уже есть:" + tmp_key9);
				} else {
					simpleTables.Add(tmp_key9, "" + "y20" + Year + "_m" + Month + "_k" + tmp_key4DB2 + "','" + tmp_4sql_value6);
				}
			}
			yield return new WaitForEndOfFrame();
		}

		public List<string> parseRow2List(string row) {
			string row_line9 = "";
			int row_from5 = 0;
			int row_length5 = 0;
			List<string> list2retrn = new List<string>();
			Regex regex_clearCell = default(Regex);
			regex_clearCell = new Regex("[║╟╦╢├┬┤│|I═=]*");
			row_line9 = row;
			list2retrn.Add(regex_clearCell.Replace(row_line9.Substring(0, delimetrs[0]), ""));
			//основное тело распарса строки
			for(int index9 = 0; index9 < (delimetrs.Count - 1); index9 += 1) {
				row_from5 = delimetrs[index9];
				row_length5 = (delimetrs[(index9 + 1)] - row_from5);
				//Проверка на неполную строчку. заполнение @
				if((row_line9.Length > row_from5)) {
					if((row_line9.Length >= (row_from5 + row_length5))) {
						list2retrn.Add(regex_clearCell.Replace(row_line9.Substring(row_from5, row_length5), ""));
					} else {
						list2retrn.Add(regex_clearCell.Replace(row_line9.Substring(row_from5, (row_line9.Length - row_from5)), ""));
					}
				} else {
					list2retrn.Add("");
				}
			}
			//Проверка на неполную строчку. заполнение @
			if((row_line9.Length > delimetrs[delimetrs.Count - 1])) {
				if((row_line9.Length >= (delimetrs[delimetrs.Count - 1] + (row_line9.Length - delimetrs[delimetrs.Count - 1])))) {
					list2retrn.Add(regex_clearCell.Replace(row_line9.Substring(delimetrs[delimetrs.Count - 1], (row_line9.Length - delimetrs[delimetrs.Count - 1])), ""));
				} else {
					list2retrn.Add(regex_clearCell.Replace(row_line9.Substring(delimetrs[delimetrs.Count - 1], (row_line9.Length - delimetrs[delimetrs.Count - 1])), ""));
				}
			} else {
				list2retrn.Add("");
			}
			return list2retrn;
		}
	}
}
