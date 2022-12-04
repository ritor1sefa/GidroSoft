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
		public string wet_cor_m = "";
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
			yield return new WaitForEndOfFrame();
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
				//Если есть - то это короткий файл, и пост к тому же
				if(file_data.Contains("Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й       Т М П")) {
					Debug.Log("Т М П");
					wet_cor_m = "";
					//"ВСЕМИРНОЕ"==длинный вариант ТМН. Если нет == короткий
					if(file_data.Contains("ВСЕМИРНОЕ")) {
						if(Regex.IsMatch(file_data, "участка:.+\\|\\s+(\\S+)\\s+\\|")) {
							wet_cor_m = new Regex("участка:.+\\|\\s+(\\S+)\\s+\\|", RegexOptions.Multiline).Match(file_data).Result("$1");
						} else {
							Debug.Log("2Поправки на смачивание нету!");
						}
						//1 С У Т О Ч Н Ы Е   Д А Н Н Ы Е, Осадки, температура
						yield return TMN_L_n2_1(file_data.Substring(file_data.IndexOf("Стр. 2"), (file_data.IndexOf("Стр. 3") - file_data.IndexOf("Стр. 2"))));
						//2С У Т О Ч Н Ы Е   Д А Н Н Ы Е
						yield return TMN_L_n2_2_n3(file_data.Substring(file_data.IndexOf("Стр. 3"), (file_data.IndexOf("Стр. 4") - file_data.IndexOf("Стр. 3"))));
						//М Е С Я Ч Н Ы Е   В Ы В О Д Ы
						yield return TMN_L_t1_t7_1_t16(file_data.Substring(file_data.IndexOf("Стр. 4"), (file_data.IndexOf("ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯМ, НЕ МЕНЕЕ (ММ)") - file_data.IndexOf("Стр. 4"))));
						//ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯМ, НЕ МЕНЕЕ (ММ)
						yield return TMN_L_t7_2_t11(file_data.Substring((file_data.IndexOf("ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯМ, НЕ МЕНЕЕ (ММ)") - 20)));
					} else {
						if(Regex.IsMatch(file_data, "Месяц\\s*(\\S+)\\s*мм")) {
							wet_cor_m = new Regex("Месяц\\s*(\\S+)\\s*мм", RegexOptions.Multiline).Match(file_data).Result("$1");
						} else {
							Debug.Log("1Поправки на смачивание нету!");
						}
						//Склеивание таблицы в один столбец
						One_table_data = mergeTable(file_data.Substring(file_data.IndexOfAny(new char[] { '═', '=' }), (file_data.IndexOf("УСЛОВНЫЕ ОБОЗНАЧЕНИЯ АТМОСФЕРНЫХ ЯВЛЕНИЙ") - file_data.IndexOfAny(new char[] { '═', '=' }))));
						//Смешанная таблица (та что слева)
						yield return TMN_S_n2_n3(One_table_data.Substring(0, One_table_data.IndexOf("Температура воздуха, градусы")));
						//Температура воздуха, градусы
						yield return TMN_S_t1_t16(One_table_data.Substring((One_table_data.IndexOf("Температура воздуха, градусы") - 20), (One_table_data.IndexOf("КОЛИЧЕСТВО ОСАДКОВ, ММ") - One_table_data.IndexOf("Температура воздуха, градусы"))));
						//КОЛИЧЕСТВО ОСАДКОВ, ММ
						yield return TMN_S_t7_1(One_table_data.Substring(One_table_data.IndexOf("КОЛИЧЕСТВО ОСАДКОВ, ММ"), (One_table_data.IndexOf("ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯ") - One_table_data.IndexOf("КОЛИЧЕСТВО ОСАДКОВ, ММ"))));
						//ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯ
						yield return TMN_S_t7_2(One_table_data.Substring(One_table_data.IndexOf("ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯ"), (One_table_data.IndexOf("С атмосферными явлениями") - One_table_data.IndexOf("ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯ"))));
						//С атмосферными явлениями
						yield return TMN_S_t11(One_table_data.Substring((One_table_data.IndexOf("С атмосферными явлениями") - 20)));
						if(file_data.Contains("О П А С Н Ы Е   Г И Д Р О М Е Т Е О Р О Л О Г И Ч Е С К И Е   Я В Л Е Н И Я")) {
							//ОЯ, снег
							One_table_data = file_data.Substring(file_data.IndexOf("О П А С Н Ы Е   Г И Д Р О М Е Т Е О Р О Л О Г И Ч Е С К И Е   Я В Л Е Н И Я"), (file_data.Length - file_data.IndexOf("О П А С Н Ы Е   Г И Д Р О М Е Т Е О Р О Л О Г И Ч Е С К И Е   Я В Л Е Н И Я")));
							if(One_table_data.Contains("Г/М ЯВЛЕНИЯ")) {
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
				} else {
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

		/// <summary>
		/// split2Rows
		/// </summary>
		private List<string> split2Rows(string data2SplittingROWS) {
			rows_unparsed = new List<string>();
			//Распилка
			foreach(string loopObject3 in data2SplittingROWS.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				rows_unparsed.Add(loopObject3);
			}
			return rows_unparsed;
		}

		/// <summary>
		/// getDelims
		/// </summary>
		public System.Collections.IEnumerator getDelims(string one_table_data) {
			string delim_row = "";
			int delim_rowCount = 10;
			HashSet<int> delim_hash_ints = new HashSet<int>();
			rows_unparsed.Clear();
			yield return split2Rows(one_table_data);
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
		/// mergeTable
		/// </summary>
		public string mergeTable(string toSplitingCLMNS) {
			int _1thLenght = 0;
			string row = "";
			int rowL = 0;
			int tmp_hulf = 0;
			string clmn_1th = "";
			string clmn_2th = "";
			if(Regex.IsMatch(toSplitingCLMNS, "(^\\s*={3,}|^\\s*═{3,})(\\s*={3,}|\\s*═{3,})", RegexOptions.Multiline)) {
				_1thLenght = Regex.Match(toSplitingCLMNS, "(^\\s*={3,}|^\\s*═{3,})(\\s*={3,}|\\s*═{3,})", RegexOptions.Multiline).Result("$1").Length;
				clmn_1th = "";
				clmn_2th = "";
				foreach(string loopObject4 in split2Rows(toSplitingCLMNS)) {
					row = loopObject4;
					rowL = row.Length;
					if(!(string.IsNullOrWhiteSpace(row))) {
						if((rowL > _1thLenght)) {
							tmp_hulf = _1thLenght;
							clmn_2th = clmn_2th + System.Environment.NewLine + row.Substring(tmp_hulf, (rowL - tmp_hulf));
						} else {
							tmp_hulf = rowL;
						}
						clmn_1th = clmn_1th + System.Environment.NewLine + row.Substring(0, tmp_hulf);
						tmp_hulf = _1thLenght;
					}
				}
			} else {
				Debug.Log("Нет таблицы для распилки!");
			}
			return clmn_1th + "" + clmn_2th;
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
					foreach(string loopObject5 in Enumerable.ToList<System.String>(simpleTables.Keys)) {
						t_key = loopObject5;
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
								if((add_new == 0)) {
									//воткнуть проверку - на "такие же значения как в бд?"
									Debug.Log("Повтор:" + db_name + "=" + q);
								}
							}
							cmnd1.Cancel();
						}
					}
					Debug.Log("Simple" + "=внесено=" + sql_writed.ToString());
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
					foreach(string loopObject6 in Enumerable.ToList<System.String>(namedTables.Keys)) {
						t_key1 = loopObject6;
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
								if((add_new1 == 0)) {
									//воткнуть проверку - на "такие же значения как в бд?"
									Debug.Log("Повтор:" + db_name + "=" + q1);
								}
							}
							cmnd2.Cancel();
						}
					}
					Debug.Log("Named" + "=внесено=" + sql_writed1.ToString());
				}
				//Пишет что база закрыта, но файл удалить не закрывая редактор всё равно не получается, что за фигня..
				Debug.Log(connection2.State.ToString());
				connection2 = null;
				SqliteConnection.ClearAllPools();
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}
		}

		/// <summary>
		/// n1
		/// </summary>
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
			foreach(string loopObject7 in rows_unparsed) {
				row_line = loopObject7;
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

		/// <summary>
		/// n2
		/// </summary>
		public System.Collections.IEnumerator n2(string one_table_data) {
			string row_line1 = "";
			int row_day1 = 0;
			int row_from1 = 0;
			int row_length1 = 0;
			string tmp_4sql_value1 = "";
			string tmp_key1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject8 in rows_unparsed) {
				row_line1 = loopObject8;
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
					if(namedTables.ContainsKey(tmp_key1)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key1);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						namedTables.Add(tmp_key1, "'Year_Month_Day'," + "'t_Возд_сред','t_Возд_макс','t_Возд_мин','t_Пов.Почвы_сред','t_Пов.Почвы_макс','t_Пов.Почвы_мин','t_ТочкиРосы_мин','Парц.Давл.вод.п_гПа_сред','Отн.Вл_сред','Отн.Вл_мин','Деф.Насыщ_гПа_сред','Деф.Насыщ_гПа_макс','Атм.Дав_наУр.Станции','Атм.Дав_наУр.Моря','Характ.облачн_Ш_О','Характ.облачн_Ш_Н','Ветер_сред','Ветер_макс_из8срок','Ветер_макс_абсМакс','ОсадСутки_Сумма','Сост.поверхн.почвы_Ш','СнПокров_ст.покр','СнПокров_высота_см'" + "&" + "'" + tmp_4sql_value1 + "'");
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// n3
		/// </summary>
		public System.Collections.IEnumerator n3(string one_table_data) {
			string row_line2 = "";
			int row_day2 = 0;
			int row_dayPrev1 = 0;
			int row_from2 = 0;
			int row_length2 = 0;
			string tmp_key2 = "";
			Dictionary<int, string> tmp_Allday = new Dictionary<int, string>();
			string tmp_headers = "";
			string tmp_values12 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject9 in rows_unparsed) {
				row_line2 = loopObject9.TrimEnd();
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
			foreach(KeyValuePair<int, string> loopObject10 in tmp_Allday) {
				//Если совсем всё в порядке и вся ячейка что то имеет
				tmp_key2 = NameOfDB + "&" + "n3" + "&" + Year + "_" + Month + "_" + loopObject10.Key.ToString();
				if(namedTables.ContainsKey(tmp_key2)) {
					//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
					Debug.Log("Ключ уже есть:" + tmp_key2);
				} else {
					//Засовываем сразу почти готовую строчку для sql
					namedTables.Add(tmp_key2, "'Year_Month_Day'," + new Regex("([А-Яа-я]+)\\s*(\\d+)\\s*", RegexOptions.None).Replace(loopObject10.Value, "'$1',").TrimEnd(',').ToUpper() + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + loopObject10.Key.ToString() + "'" + "," + new Regex("([А-Яа-я]+)\\s*(\\d+)\\s*", RegexOptions.None).Replace(loopObject10.Value, "'$2',").Replace("00", "00.1").TrimEnd(','));
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// n4_1
		/// </summary>
		public System.Collections.IEnumerator n4_1(string one_table_data) {
			string row_line3 = "";
			int row_from3 = 0;
			int row_length3 = 0;
			string tmp_4sql_value2 = "";
			string tmp_key3 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject11 in rows_unparsed) {
				row_line3 = loopObject11;
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

		/// <summary>
		/// n4_2
		/// </summary>
		public System.Collections.IEnumerator n4_2(string one_table_data) {
			string row_line4 = "";
			int row_from4 = 0;
			int row_length4 = 0;
			string tmp_4sql_value3 = "";
			string tmp_key4 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject12 in rows_unparsed) {
				row_line4 = loopObject12;
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

		/// <summary>
		/// n4_3
		/// </summary>
		public System.Collections.IEnumerator n4_3(string one_table_data) {
			string row_line5 = "";
			string tmp_key5 = "";
			List<int> tmp_delim = new List<int>() { 7, 13, 19, 25, 32, 38, 42, 46, 50, 54, 58, 62, 66, 70, 74, 80, 87, 92, 99, 106, 113, 116, 119, 122, 125, 34 };
			List<string> tmp_values = new List<string>();
			yield return getDelims(one_table_data);
			tmp_delim.Sort();
			delimetrs = tmp_delim;
			//построчная обработка
			foreach(string loopObject13 in rows_unparsed) {
				row_line5 = loopObject13;
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

		/// <summary>
		/// t20_21
		/// </summary>
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
			foreach(string loopObject14 in rows_unparsed) {
				row_line6 = loopObject14;
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
			foreach(string loopObject15 in rows_unparsed) {
				row_line6 = loopObject15;
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

		/// <summary>
		/// t14
		/// </summary>
		public System.Collections.IEnumerator t14(string one_table_data) {
			string row_line7 = "";
			List<string> tmp_values1 = new List<string>();
			string tmp_key7 = "";
			string tmp_key4DB = "";
			string tmp_4sql_value4 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject16 in rows_unparsed) {
				row_line7 = loopObject16;
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

		/// <summary>
		/// t16
		/// </summary>
		public System.Collections.IEnumerator t16(string one_table_data) {
			string row_line8 = "";
			List<string> tmp_values2 = new List<string>();
			string tmp_key8 = "";
			string tmp_4sql_value5 = "";
			string tmp_key4DB1 = "";
			string tmp_TMName = "";
			int countWithSn = 0;
			string tmp_TMN_key = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject17 in rows_unparsed) {
				row_line8 = loopObject17;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line8, "^[А-я ]+\\d")) {
					tmp_values2 = parseRow2List(row_line8);
					tmp_4sql_value5 = new Regex("[ ]*").Replace("','','','','','" + string.Join<System.String>("','", tmp_values2), "");
					tmp_key4DB1 = new Regex("[',]*").Replace(tmp_4sql_value5, "");
					tmp_TMName = NameOfDB + "&" + "16" + "&" + Year + "_" + Month + "_null";
					if(namedTables.ContainsKey(tmp_TMName)) {
						foreach(string loopObject18 in namedTables.Keys) {
							tmp_TMN_key = loopObject18;
							//Считаем количество дней со снегом
							if(tmp_TMN_key.Contains(NameOfDB + "&" + "n2" + "&" + Year + "_" + Month)) {
								//Считаем количество дней со снегом
								if(int.TryParse(namedTables[tmp_TMN_key].Split(new char[] { '&' })[1].Split(new char[] { ',' })[6].Trim('''), out int _)) {
									countWithSn = (countWithSn + 1);
								}
							}
						}
						tmp_4sql_value5 = namedTables[tmp_TMName].Split(new char[] { '&' })[1].Replace("'y20" + Year + "_m" + Month + "_null',", "") + ",'" + countWithSn.ToString() + "','" + tmp_4sql_value5.Substring(15);
						namedTables.Remove(tmp_TMName);
					}
					//set values for 7 table
					tmp_key8 = NameOfDB + "&" + "16" + "&" + Year + "_" + Month + "_k" + tmp_key4DB1;
					if(simpleTables.ContainsKey(tmp_key8)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key8);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						simpleTables.Add(tmp_key8, "" + "y20" + Year + "_m" + Month + "_k" + tmp_key4DB1 + "'," + tmp_4sql_value5);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// t22
		/// </summary>
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
			foreach(string loopObject19 in rows_unparsed) {
				c_row_line = loopObject19;
				//новый список строк. чистый
				if(!((((c_row_line.Contains("ГОЛОЛЕДНО") || string.IsNullOrWhiteSpace(c_row_line)) || Regex.IsMatch(c_row_line, "[║╟╦╢├┬┤│|I═=]")) || c_row_line.Contains("Переход")))) {
					tmp_newTable.Add(c_row_line);
				}
			}
			final_row = new List<List<string>>();
			//построчная обработка
			foreach(string loopObject20 in tmp_newTable) {
				row_parsed = parseRow2List(loopObject20);
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
			foreach(List<string> loopObject21 in final_row) {
				last_list_row = loopObject21;
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

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// Для таблиц n2 & n3
		/// </summary>
		public System.Collections.IEnumerator TMN_S_n2_n3(string one_table_data) {
			string row_line9 = "";
			List<string> tmp_values3 = new List<string>();
			string tmp_key10 = "";
			string toN2TableValue = "";
			string toN3TableValue = "";
			string day = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject22 in rows_unparsed) {
				row_line9 = loopObject22;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line9, "^ *(\\d+)")) {
					tmp_values3 = parseRow2List(row_line9);
					day = tmp_values3[0].Trim();
					toN2TableValue = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values3.GetRange(1, 7), (string parameterValues1) => {
						return parameterValues1.Trim();
					})) + "'";
					toN3TableValue = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values3.GetRange(8, 9), (string parameterValues2) => {
						return parameterValues2.Trim();
					})) + "'";
					//set values for n2 table
					tmp_key10 = NameOfDB + "&" + "n2" + "&" + Year + "_" + Month + "_" + day;
					if(namedTables.ContainsKey(tmp_key10)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key10);
					} else {
						namedTables.Add(tmp_key10, "'Year_Month_Day'," + "'t_Возд_макс','t_Возд_мин','ОсадСутки_Ночь','ОсадСутки_День','ОсадСутки_Сумма','СнПокров_ст.покр','СнПокров_высота_см'" + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + day + "'," + toN2TableValue);
					}
					//set values for n3 table
					tmp_key10 = NameOfDB + "&" + "n3" + "&" + Year + "_" + Month + "_" + day;
					if(namedTables.ContainsKey(tmp_key10)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key10);
					} else {
						namedTables.Add(tmp_key10, "'Year_Month_Day'," + "'дж','с','см','тт','изм','гл','мм','гд','г'".ToUpper() + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + day + "'," + toN3TableValue);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// Для таблиц t1 & t16
		/// </summary>
		public System.Collections.IEnumerator TMN_S_t1_t16(string one_table_data) {
			string row_line10 = "";
			List<string> tmp_values4 = new List<string>();
			string tmp_key11 = "";
			string toT1TableValue = "";
			string toT16TableValue = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject23 in rows_unparsed) {
				row_line10 = loopObject23;
				if(!((row_line10.IndexOfAny(new char[] { '=', '═' }) > -1))) {
					tmp_values4 = parseRow2List(row_line10);
					//только строчку с цифрами
					if(Regex.IsMatch(row_line10, "^ *(\\d+)")) {
						switch(int.Parse(tmp_values4[0])) {
							case 1: {
								toT16TableValue = "'ИзменитьВручнуюТип','" + tmp_values4[7].Trim() + "','";
							}
							break;
							case 2: {
								toT16TableValue = toT16TableValue + tmp_values4[7].Trim() + "','";
							}
							break;
							case 3: {
								toT16TableValue = toT16TableValue + tmp_values4[7].Trim() + "'";
							}
							break;
						}
					} else {
						//строчка с месяцем - для первой таблицы
						if(row_line10.Contains("Мес")) {
							toT1TableValue = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values4.GetRange(1, 6), (string parameterValues3) => {
								return parameterValues3.Trim();
							})) + "'";
							//4t1
							//set values for t1 table
							tmp_key11 = NameOfDB + "&" + "1" + "&" + Year + "_" + Month;
							if(namedTables.ContainsKey(tmp_key11)) {
								//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
								Debug.Log("Ключ уже есть:" + tmp_key11);
							} else {
								namedTables.Add(tmp_key11, "'N_year_N_month'," + "'tV_mid_max','tV_mid_min','tV_abs_max','tV_abs_max_day','tV_abs_min','tV_abs_min_day'" + "&" + "'" + "y20" + Year + "_m" + Month + "'," + toT1TableValue);
							}
						}
					}
				}
			}
			if((toT16TableValue.Length > 0)) {
				//set values for t16 table
				tmp_key11 = NameOfDB + "&" + "16" + "&" + Year + "_" + Month + "_null";
				if(namedTables.ContainsKey(tmp_key11)) {
					//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
					Debug.Log("Ключ уже есть:" + tmp_key11);
				} else {
					namedTables.Add(tmp_key11, "'N_year_N_month_N_day_trace'," + "'e_type','e_1dec','e_2dec','e_3dec'".ToUpper() + "&" + "'" + "y20" + Year + "_m" + Month + "_null" + "'," + toT16TableValue);
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// t7=первая половина
		/// </summary>
		public System.Collections.IEnumerator TMN_S_t7_1(string one_table_data) {
			string row_line11 = "";
			List<string> tmp_values5 = new List<string>();
			string tmp_key12 = "";
			string toT7TableValue = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject24 in rows_unparsed) {
				row_line11 = loopObject24;
				//игнорим шапку
				if(!((row_line11.IndexOfAny(new char[] { '|', 'н' }) > -1))) {
					//только строчку с цифрами
					if(Regex.IsMatch(row_line11, "^ *(\\d+)")) {
						tmp_values5 = parseRow2List(row_line11);
						//Добавляем поправку на смачивание, полученную сильно выше
						toT7TableValue = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values5.GetRange(3, 5), (string parameterValues4) => {
							return parameterValues4.Trim();
						})) + "','" + wet_cor_m + "'";
						//set values for t7_1 table
						tmp_key12 = NameOfDB + "&" + "7" + "&" + Year + "_" + Month;
						if(namedTables.ContainsKey(tmp_key12)) {
							//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
							Debug.Log("Ключ уже есть:" + tmp_key12);
						} else {
							//засовываем полный список столбцов. вторую половину данных в следующей функции-таблице
							namedTables.Add(tmp_key12, "'N_year_N_month'," + "'rnfl_nightTime','rnfl_dayTime','rnfl_summ','rnfl_maxByDay','rnfl_day','tt_wet_corr','RD_00','RD_01','RD_05','RD_1','RD_5','RD_10','RD_20','RD_30','RD_50','RD_80','RD_120'" + "&" + "'" + "y20" + Year + "_m" + Month + "'," + toT7TableValue);
						}
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// t7=вторая половина
		/// </summary>
		public System.Collections.IEnumerator TMN_S_t7_2(string one_table_data) {
			string row_line12 = "";
			List<string> tmp_values6 = new List<string>();
			string tmp_key13 = "";
			string toT7TableValue1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject25 in rows_unparsed) {
				row_line12 = loopObject25;
				//игнорим шапку
				if(!((row_line12.IndexOfAny(new char[] { '|', 'н' }) > -1))) {
					//только строчку с цифрами
					if(Regex.IsMatch(row_line12, "^ *(\\d+)")) {
						tmp_values6 = parseRow2List(row_line12);
						toT7TableValue1 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values6, (string parameterValues5) => {
							return parameterValues5.Trim();
						})) + "'";
						//set values for t7_2 table
						tmp_key13 = NameOfDB + "&" + "7" + "&" + Year + "_" + Month;
						if(namedTables.ContainsKey(tmp_key13)) {
							namedTables[tmp_key13] = namedTables[tmp_key13] + "," + toT7TableValue1;
						} else {
							//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
							Debug.Log("Первой половины 7й таблицы нету! О_о" + tmp_key13);
						}
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// Для таблицы11
		/// АТМОСФЕРНЫЕ  ЯВЛЕНИЯ,  ЧИСЛО  ДНЕЙ 
		/// </summary>
		public System.Collections.IEnumerator TMN_S_t11(string one_table_data) {
			string row_line13 = "";
			List<string> tmp_values7 = new List<string>();
			string tmp_key14 = "";
			string toT11TableValue = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject26 in rows_unparsed) {
				row_line13 = loopObject26;
				//игнорим шапку
				if(!((row_line13.IndexOfAny(new char[] { '|', 'д', '=', '-' }) > -1))) {
					//только строчку с цифрами
					if(Regex.IsMatch(row_line13, "^ *(\\d+)")) {
						tmp_values7 = parseRow2List(row_line13);
						toT11TableValue = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values7.GetRange(0, 9), (string parameterValues6) => {
							return parameterValues6.Trim();
						})) + "'";
						//set values for t11 table
						tmp_key14 = NameOfDB + "&" + "11" + "&" + Year + "_" + Month;
						if(namedTables.ContainsKey(tmp_key14)) {
							//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
							Debug.Log("Ключ уже есть:" + tmp_key14);
						} else {
							//засовываем полный список столбцов. вторую половину данных в следующей функции-таблице
							namedTables.Add(tmp_key14, "'Year_Month_Day'," + "'ДЖ','С','СМ','ТТ','ИЗМ','ГЛ','ММ','ГД','Г'" + "&" + "'" + "y20" + Year + "_m" + Month + "'," + toT11TableValue);
						}
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// Для таблицы n2
		/// 1 С У Т О Ч Н Ы Е   Д А Н Н Ы Е, Осадки, температура
		/// </summary>
		public System.Collections.IEnumerator TMN_L_n2_1(string one_table_data) {
			string row_line14 = "";
			List<string> tmp_values8 = new List<string>();
			string tmp_key15 = "";
			string toN2TableValue1 = "";
			string day1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject27 in rows_unparsed) {
				row_line14 = loopObject27;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line14, "^ *(\\d+)")) {
					tmp_values8 = parseRow2List(row_line14);
					day1 = tmp_values8[1].Trim();
					//Температура воздуха
					toN2TableValue1 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values8.GetRange(5, 2), (string parameterValues7) => {
						return parameterValues7.Trim();
					})) + "'";
					//Осадки
					toN2TableValue1 = toN2TableValue1 + ",'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values8.GetRange(2, 3), (string parameterValues8) => {
						return parameterValues8.Trim();
					})) + "'";
					//set values for n2 table
					tmp_key15 = NameOfDB + "&" + "n2" + "&" + Year + "_" + Month + "_" + day1;
					if(namedTables.ContainsKey(tmp_key15)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key15);
					} else {
						//Добавлены названия столбцов снега для следующей таблицы
						namedTables.Add(tmp_key15, "'Year_Month_Day'," + "'t_Возд_макс','t_Возд_мин','ОсадСутки_Ночь','ОсадСутки_День','ОсадСутки_Сумма','СнПокров_ст.покр','СнПокров_высота_см'" + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + day1 + "'," + toN2TableValue1);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// n2=вторая половина, снег
		/// n3=Атмосферные явления, продолжительность
		/// </summary>
		public System.Collections.IEnumerator TMN_L_n2_2_n3(string one_table_data) {
			string row_line15 = "";
			List<string> tmp_values9 = new List<string>();
			string day2 = "";
			string tmp_key16 = "";
			string toN2TableValue2 = "";
			string toN3TableValue1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject28 in rows_unparsed) {
				row_line15 = loopObject28;
				//только строчку с цифрами
				if(Regex.IsMatch(row_line15, "^ *(\\d+)")) {
					tmp_values9 = parseRow2List(row_line15);
					day2 = tmp_values9[1].Trim();
					//снег
					toN2TableValue2 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values9.GetRange(2, 2), (string parameterValues9) => {
						return parameterValues9.Trim();
					})) + "'";
					toN3TableValue1 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values9.GetRange(4, 15), (string parameterValues10) => {
						return parameterValues10.Trim();
					})) + "'";
					//set values for n2_2 table
					tmp_key16 = NameOfDB + "&" + "n2" + "&" + Year + "_" + Month + "_" + day2;
					if(namedTables.ContainsKey(tmp_key16)) {
						namedTables[tmp_key16] = namedTables[tmp_key16] + "," + toN2TableValue2;
					} else {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Первой половины n2й таблицы нету! О_о" + tmp_key16);
					}
					//set values for n3 table
					tmp_key16 = NameOfDB + "&" + "n3" + "&" + Year + "_" + Month + "_" + day2;
					if(namedTables.ContainsKey(tmp_key16)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_key16);
					} else {
						namedTables.Add(tmp_key16, "'Year_Month_Day'," + "'ДЖ','С','СМ','ТТ','ИЗМ','ГЛ','ММ','ГД','Г','ДМ','ПМ','ПБ','МГ','СЧ','Ш'".ToUpper() + "&" + "'" + "y20" + Year + "_m" + Month + "_d" + day2 + "'," + toN3TableValue1);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// t1=температуры за месяц
		/// t7=осадки за месяц
		/// t16=снег-декады и число дней с ним
		/// </summary>
		public System.Collections.IEnumerator TMN_L_t1_t7_1_t16(string one_table_data) {
			string row_line16 = "";
			List<string> tmp_values10 = new List<string>();
			string tmp_key17 = "";
			string toT1TableValue1 = "";
			string toT16TableValue1 = "";
			string toT7TableValue0 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject29 in rows_unparsed) {
				row_line16 = loopObject29;
				if(!((row_line16.IndexOfAny(new char[] { '=', '═', '|' }) > -1))) {
					tmp_values10 = parseRow2List(row_line16);
					//только строчку с цифрами
					if(Regex.IsMatch(row_line16, "^ *(\\d+)")) {
						//декады
						switch(tmp_values10[0].Trim()) {
							case "1-я": {
								toT16TableValue1 = "'ИзменитьВручнуюТип','" + tmp_values10[14].Trim() + "','";
							}
							break;
							case "2-я": {
								toT16TableValue1 = toT16TableValue1 + tmp_values10[14].Trim() + "','";
							}
							break;
							case "3-я": {
								toT16TableValue1 = toT16TableValue1 + tmp_values10[14].Trim() + "'";
							}
							break;
						}
					} else {
						//строчка с месяцем - для первой таблицы
						if(row_line16.Contains("Мес")) {
							//температуры за месяц
							toT1TableValue1 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values10.GetRange(2, 6), (string parameterValues11) => {
								return parameterValues11.Trim();
							})) + "'";
							//Добавляем поправку на смачивание, полученную сильно выше
							toT7TableValue0 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values10.GetRange(8, 5), (string parameterValues12) => {
								return parameterValues12.Trim();
							})) + "','" + wet_cor_m + "'";
							//set values for t1 table
							tmp_key17 = NameOfDB + "&" + "1" + "&" + Year + "_" + Month;
							if(namedTables.ContainsKey(tmp_key17)) {
								//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
								Debug.Log("Ключ уже есть:" + tmp_key17);
							} else {
								namedTables.Add(tmp_key17, "'N_year_N_month'," + "'tV_mid_max','tV_mid_min','tV_abs_max','tV_abs_max_day','tV_abs_min','tV_abs_min_day'" + "&" + "'" + "y20" + Year + "_m" + Month + "'," + toT1TableValue1);
							}
							//set values for t7 table
							tmp_key17 = NameOfDB + "&" + "7" + "&" + Year + "_" + Month;
							if(namedTables.ContainsKey(tmp_key17)) {
								//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
								Debug.Log("Ключ уже есть:" + tmp_key17);
							} else {
								//засовываем полный список столбцов. вторую половину данных в следующей функции-таблице
								namedTables.Add(tmp_key17, "'N_year_N_month'," + "'rnfl_nightTime','rnfl_dayTime','rnfl_summ','rnfl_maxByDay','rnfl_day','tt_wet_corr','RD_00','RD_01','RD_05','RD_1','RD_5','RD_10','RD_20','RD_30','RD_50','RD_80','RD_120'" + "&" + "'" + "y20" + Year + "_m" + Month + "'," + toT7TableValue0);
							}
						}
					}
				}
			}
			if((toT16TableValue1.Length > 0)) {
				//4t16
				//set values for t16 table
				tmp_key17 = NameOfDB + "&" + "16" + "&" + Year + "_" + Month + "_null";
				if(namedTables.ContainsKey(tmp_key17)) {
					//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
					Debug.Log("Ключ уже есть:" + tmp_key17);
				} else {
					namedTables.Add(tmp_key17, "'N_year_N_month_N_day_trace'," + "'e_type','e_1dec','e_2dec','e_3dec'".ToUpper() + "&" + "'" + "y20" + Year + "_m" + Month + "_null" + "'," + toT16TableValue1);
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// TMN =Т А Б Л И Ц Ы    М Е Т Е О Р О Л О Г И Ч Е С К И Х   Н А Б Л Ю Д Е Н И Й 
		/// t7=вторая половина
		/// t11=число дней с АЯ
		/// </summary>
		public System.Collections.IEnumerator TMN_L_t7_2_t11(string one_table_data) {
			string row_line17 = "";
			List<string> tmp_values11 = new List<string>();
			string tmp_key18 = "";
			string toT7TableValue2 = "";
			string toT11TableValue1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject30 in rows_unparsed) {
				row_line17 = loopObject30;
				//игнорим шапку
				if(!((row_line17.IndexOfAny(new char[] { '|', 'н' }) > -1))) {
					//только строчку с цифрами
					if(Regex.IsMatch(row_line17, "^ *(\\d+)")) {
						tmp_values11 = parseRow2List(row_line17);
						//ЧИСЛО ДНЕЙ С ОСАДКАМИ ПО ГРАДАЦИЯМ, НЕ МЕНЕЕ (ММ
						toT7TableValue2 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values11.GetRange(1, 11), (string parameterValues13) => {
							return parameterValues13.Trim();
						})) + "'";
						//ЧИСЛО ДНЕЙ С АТМОСФЕРНЫМИ ЯВЛЕНИЯМИ
						toT11TableValue1 = "'" + string.Join("','", Enumerable.Select<System.String, System.String>(tmp_values11.GetRange(12, 15), (string parameterValues14) => {
							return parameterValues14.Trim();
						})) + "'";
						//set values for t7_2 table
						tmp_key18 = NameOfDB + "&" + "7" + "&" + Year + "_" + Month;
						if(namedTables.ContainsKey(tmp_key18)) {
							namedTables[tmp_key18] = namedTables[tmp_key18] + "," + toT7TableValue2;
						} else {
							//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
							Debug.Log("Первой половины 7й таблицы нету! О_о" + tmp_key18);
						}
						//set values for t11 table
						tmp_key18 = NameOfDB + "&" + "11" + "&" + Year + "_" + Month;
						if(namedTables.ContainsKey(tmp_key18)) {
							//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
							Debug.Log("Ключ уже есть:" + tmp_key18);
						} else {
							//засовываем полный список столбцов. вторую половину данных в следующей функции-таблице
							namedTables.Add(tmp_key18, "'Year_Month_Day'," + "'ДЖ','С','СМ','ТТ','ИЗ','ГЛ','ММ','ГД','Г','ДМ','ПМ','ПБ','МГ','СЧ','Ш'" + "&" + "'" + "y20" + Year + "_m" + Month + "'," + toT11TableValue1);
						}
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// parseRow2List
		/// </summary>
		public List<string> parseRow2List(string row) {
			string row_line18 = "";
			int row_from5 = 0;
			int row_length5 = 0;
			List<string> list2retrn = new List<string>();
			Regex regex_clearCell = default(Regex);
			regex_clearCell = new Regex("[║╟╦╢├┬┤│|I═=]*");
			row_line18 = row;
			list2retrn.Add(regex_clearCell.Replace(row_line18.Substring(0, delimetrs[0]), ""));
			//основное тело распарса строки
			for(int index9 = 0; index9 < (delimetrs.Count - 1); index9 += 1) {
				row_from5 = delimetrs[index9];
				row_length5 = (delimetrs[(index9 + 1)] - row_from5);
				//Проверка на неполную строчку. заполнение @
				if((row_line18.Length > row_from5)) {
					if((row_line18.Length >= (row_from5 + row_length5))) {
						list2retrn.Add(regex_clearCell.Replace(row_line18.Substring(row_from5, row_length5), ""));
					} else {
						list2retrn.Add(regex_clearCell.Replace(row_line18.Substring(row_from5, (row_line18.Length - row_from5)), ""));
					}
				} else {
					list2retrn.Add("");
				}
			}
			//Проверка на неполную строчку. заполнение @
			if((row_line18.Length > delimetrs[delimetrs.Count - 1])) {
				if((row_line18.Length >= (delimetrs[delimetrs.Count - 1] + (row_line18.Length - delimetrs[delimetrs.Count - 1])))) {
					list2retrn.Add(regex_clearCell.Replace(row_line18.Substring(delimetrs[delimetrs.Count - 1], (row_line18.Length - delimetrs[delimetrs.Count - 1])), ""));
				} else {
					list2retrn.Add(regex_clearCell.Replace(row_line18.Substring(delimetrs[delimetrs.Count - 1], (row_line18.Length - delimetrs[delimetrs.Count - 1])), ""));
				}
			} else {
				list2retrn.Add("");
			}
			return list2retrn;
		}
	}
}
