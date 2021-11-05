#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;
using HtmlAgilityPack;
using NPOI.XSSF.UserModel;
using MaxyGames.Generated;

namespace MaxyGames.Generated {
	public class pogodaiklimat2011 : MaxyGames.RuntimeBehaviour {
		public KeyValuePair<string, string> linkFromConfig = new KeyValuePair<string, string>();
		public string _switch = "";
		public bool Pass = false;
		public MaxyGames.uNode.uNodeRuntime sqlite = null;
		public string db = "pogodaiklimat2011";
		public TMPro.TMP_InputField InputF_Index = null;
		public TMPro.TMP_InputField InputF_Year = null;
		public List<string> sqliteDBs = new List<string>();

		public void Start() {}

		public void StartPogodaklimat2011() {
			if((_tryGetSiteSize("www.google.com") > 0F)) {
				base.StartCoroutine(_function_group());
			} else {
				Debug.Log("Интернет не работает?");
				Debug.Log(_tryGetSiteSize("www.google.com"));
			}
		}

		public void UpdateInInputIndex() {
			//если найдены номера - вывести их. если нет то сделать поиск по списку городов-регионов и вывести их. если совпадений нету - показать имеющиеся бд и сказать что нету совпадений. ели пустая строка-показать имеющиеся.
			if((_indexArray().Count > 0)) {}
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		private List<string> _indexArray() {
			List<string> data_index = new List<string>();
			int variable1 = 0;
			data_index = new List<string>();
			foreach(string loopObject in Regex.Split(InputF_Index.text, "\\D", RegexOptions.Multiline)) {
				if(int.TryParse(loopObject, out variable1)) {
					data_index.Add(loopObject);
				}
			}
			return data_index;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		private List<string> _yearArray() {
			List<string> data_year = null;
			string year_2_parse = "";
			int _yearPlus = 0;
			int _year_from = 0;
			int _year_to = 0;
			string[] _year_splited = new string[0];
			int _year_casual = 0;
			int curYearPlus1 = 0;
			curYearPlus1 = (System.DateTime.Now.Year + 1);
			data_year = new List<string>();
			year_2_parse = InputF_Year.text;
			if(year_2_parse.Contains("+")) {
				if(int.TryParse(Regex.Replace(year_2_parse, ".*([0-9]{4})\\+.*", "$1", RegexOptions.Multiline, System.TimeSpan.FromMilliseconds(500D)), out _yearPlus)) {
					if(((_yearPlus > 2010F) && (_yearPlus < curYearPlus1))) {
						for(int index = _yearPlus; index < curYearPlus1; index += 1) {
							data_year.Add(index.ToString());
							Debug.Log(index);
						}
						if((data_year.Capacity > 0)) {
							return data_year;
						} else {
							Debug.Log("Есть года, но не в том диапазоне");
						}
					} else {
						Debug.Log("Есть +, но нету валидного года рядом с плюсом");
					}
				} else {
					Debug.Log("Есть +, но нету 4значного числа");
				}
			} else if(year_2_parse.Contains("-")) {
				_year_splited = Regex.Replace(year_2_parse, ".*([0-9-]{9}).*", " $1 ").Split(new char[] { '-' });
				_year_from = int.Parse(_year_splited[0]);
				_year_to = int.Parse(_year_splited[1]);
				if((((_year_from > 2010F) && (_year_to < curYearPlus1)) && (_year_to > _year_from))) {
					for(int index1 = _year_from; index1 <= _year_to; index1 += 1) {
						data_year.Add(index1.ToString());
						Debug.Log(index1);
					}
					return data_year;
				} else {
					Debug.Log("from_to_Года не соответствуют: " + year_2_parse);
				}
			} else {
				foreach(string loopObject1 in Regex.Split(year_2_parse, "\\D", RegexOptions.Multiline)) {
					if(((int.TryParse(loopObject1, out _year_casual) && (_year_casual > 2010F)) && (_year_casual < curYearPlus1))) {
						data_year.Add(_year_casual.ToString());
					}
				}
				if((data_year.Capacity > 0F)) {
					data_year.Sort();
					return data_year;
				} else {
					Debug.Log("casual_Года не соответствуют: " + year_2_parse);
				}
			}
			Debug.Log("11111");
			return data_year;
		}

		private System.Collections.IEnumerator _function_group() {
			float timelimit = 0.2F;
			int PassCount = 0;
			Dictionary<string, string> DictLinks = new Dictionary<string, string>();
			base.StartCoroutine(LoadFromConfig());
			yield return new WaitForSeconds(timelimit);
			if(_switch.Equals("LoadFromConfig")) {
				DictLinks = LinkGeneration(true);
				if((DictLinks.Count > 0)) {
					while(!((DictLinks.Count == 0))) {
						Debug.Log("Ссылок на скачивание создано: " + DictLinks.Count.ToString());
						yield return new WaitForSeconds(timelimit);
						base.StartCoroutine(DownLoadData(DictLinks));
						yield return new WaitUntil(() => _switch == "DownLoadData");
						PassCount = (PassCount + 1);
						Debug.Log("итерация");
						yield break;
					}
					Debug.Log("LinkGeneration = 0");
				} else {
					Debug.Log("make xlsx");
				}
			} else {
				Debug.Log("LoadFromConfig > 0.2 sec!!!");
			}
			yield break;
		}

		private System.Collections.IEnumerator LoadFromConfig() {
			linkFromConfig = new KeyValuePair<string, string>();
			foreach(KeyValuePair<string, string> loopObject2 in Ini.Load("config.ini")) {
				if(loopObject2.Key.StartsWith("pogodaiklimat2011")) {
					linkFromConfig = loopObject2;
					break;
				}
			}
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				Debug.Log("pogodaiklimat2011 нету!");
			} else {
				_switch = "LoadFromConfig";
			}
			yield break;
		}

		private Dictionary<string, string> LinkGeneration(bool CheckForMissing) {
			int curYear = 0;
			int curMonth = 0;
			int _monthMin = 1;
			int _monthMax = 13;
			string _link_base = "";
			string _link_Index = "";
			string _link_IndexAndYear = "";
			string _link_Final = "";
			string _FirstDay = "1";
			string _LastDay = "";
			string _t_index = "";
			int _t_year = 2011;
			int _t_month = 0;
			System.DateTime LastDatetimeInDB = new System.DateTime();
			string _t_AllDatetimeClmn = "";
			string _t_dateToCheck = "";
			Dictionary<string, string> DictLinks1 = new Dictionary<string, string>();
			int _t_DaysInMonth = 0;
			string _t_dayToCheck = "";
			int _t_HourInDay = 0;
			bool AddMonth = false;
			curYear = System.DateTime.Now.Year;
			curMonth = System.DateTime.Now.Month;
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				Debug.Log("ссылки из конфига всё таки нету");
			} else {
				_link_base = linkFromConfig.Value;
				//id=index
				foreach(string loopObject3 in _indexArray()) {
					_t_index = loopObject3;
					_link_Index = _link_base.Replace("$id", _t_index);
					//set year
					foreach(string loopObject4 in _yearArray()) {
						_t_AllDatetimeClmn = sqlite.GetComponent<sqlite>().GetAllDatetimeClmn(_t_index, loopObject4);
						if((int.Parse(loopObject4) <= curYear)) {
							_t_year = int.Parse(loopObject4);
							_link_IndexAndYear = _link_Index.Replace("$year", _t_year.ToString());
							if(_t_year.Equals(curYear)) {
								//set month max
								_monthMax = (curMonth + 1);
							}
							//set month links
							for(int index2 = 1; index2 < _monthMax; index2 += 1) {
								_t_month = index2;
								_t_DaysInMonth = System.DateTime.DaysInMonth(_t_year, _t_month);
								//YYYY1231
								_t_dateToCheck = _t_year.ToString() + (_t_month.ToString("D2") as string) + _t_DaysInMonth.ToString();
								if(_t_AllDatetimeClmn.Contains(_t_dateToCheck)) {
									Debug.Log("LinkGeneration. Пропущено т.к. в бд уже есть этого месяца последний день:  " + _t_dateToCheck);
									AddMonth = false;
									if(CheckForMissing) {
										Debug.Log("Однако полная проверочка");
										//проверка на отсутствующие часы в месяце
										if(((_t_DaysInMonth * 8) != (_t_AllDatetimeClmn.Replace(_t_year.ToString() + (_t_month.ToString("D2") as string), _t_year.ToString() + (_t_month.ToString("D2") as string) + "%").Split(new char[] { '%' }).Length - 1))) {
											Debug.Log("чего то не хватает или лишнее");
											//Вероятность что чего то лишнее-крайне мала. Вероятностью что чего то лишнего == недостающего ещё меньше. Так что пренебрегаем
											AddMonth = true;
											//Избыточный поиск недостающего\лишнего
											for(int index3 = 1; index3 < (_t_DaysInMonth + 1); index3 += 1) {
												_t_dayToCheck = _t_year.ToString() + (_t_month.ToString("D2") as string) + index3.ToString("D2");
												//Часов(строк) в этих сутках
												_t_HourInDay = (_t_AllDatetimeClmn.Replace(_t_dayToCheck, _t_dayToCheck + "%").Split(new char[] { '%' }).Length - 1);
												if(!((8 == _t_HourInDay))) {
													Debug.Log("Нехваток\\избыток __" + _t_HourInDay.ToString() + "__ часов в этот день: " + _t_dayToCheck);
												}
											}
										} else {
											Debug.Log("Все часы в месяце на месте (удивительно)");
										}
									}
								} else {
									AddMonth = true;
								}
								//Если чего то всё таки не хватает.
								if(AddMonth) {
									_link_Final = _link_IndexAndYear.Replace("$month", _t_month.ToString());
									//set first day in month
									_link_Final = _link_Final.Replace("$FirstDay", _FirstDay);
									//set last day in month
									if(((curYear == _t_year) && (curMonth == _t_month))) {
										_LastDay = System.DateTime.Now.Day.ToString();
									} else {
										_LastDay = System.DateTime.DaysInMonth(_t_year, _t_month).ToString();
									}
									_link_Final = _link_Final.Replace("$LastDay", _LastDay);
									DictLinks1.Add(_link_Final, _t_index);
									Debug.Log(_link_Final);
								}
							}
						}
					}
				}
			}
			//всё ок, идём дальше
			_switch = "LinkGeneration";
			return DictLinks1;
		}

		private float _tryGetSiteSize(string url) {
			UnityWebRequest conn = null;
			float r = 0F;
			conn = UnityWebRequest.Get(url);
			conn.SetRequestHeader("Accept-Encoding", "gzip, deflate");
			conn.timeout = 5;
			conn.SendWebRequest();
			while(!(conn.isDone)) {
				new WaitForEndOfFrame();
			}
			if(float.TryParse(conn.GetResponseHeader("Content-Length"), out r)) {
				Debug.Log("размер: " + r.ToString());
				//+30% т.к. на сайтах сжатие. А так - чуть ближе к истине
				return ((r / 3.2F) + r);
			} else {
				Debug.Log("Не скачалась шапка. " + "" + "" + url);
			}
			return -1;
		}

		/// <summary>
		/// Тут будет скачивание.
		/// </summary>
		private System.Collections.IEnumerator DownLoadData(Dictionary<string, string> DictLinks) {
			string _index = "";
			string _w_link = "";
			string folder_path = "";
			UnityWebRequest uwr = null;
			int CountTry = 0;
			float site_size = 0F;
			List<string> full_tbl_strArr = new List<string>();
			_switch = "!DownLoadData";
			UnityWebRequest.ClearCookieCache();
			foreach(KeyValuePair<string, string> loopObject5 in DictLinks) {
				_index = loopObject5.Value;
				_w_link = loopObject5.Key;
				using(UnityWebRequest value = UnityWebRequest.Get(_w_link)) {
					uwr = value;
					//wait up to one second to download the image
					uwr.timeout = 10;
					site_size = _tryGetSiteSize(_w_link);
					uwr.SendWebRequest();
					while(!(uwr.isDone)) {
						if((uwr.downloadedBytes > 0UL)) {
							yield return new WaitForEndOfFrame();
						}
					}
					if((uwr.result == UnityWebRequest.Result.Success)) {
						//Парсинг страницы в таблицу
						full_tbl_strArr = parse_(uwr.downloadHandler.text);
						if((full_tbl_strArr.Count > 0)) {
							DBInserter(full_tbl_strArr, _index, _w_link.Substring((_w_link.IndexOf("ayear=") + 6), 4));
						} else {
							Debug.Log("список на вставку в бд пуст: " + full_tbl_strArr.Count.ToString());
						}
					} else {
						Debug.Log(uwr.result.ToString() + ":! " + uwr.error + " | " + _index);
						Pass = true;
						CountTry = (CountTry + 1);
						if((CountTry > 3F)) {
							_switch = "DownLoadData";
							Debug.Log("Много не скачанных файлов");
							yield break;
						}
					}
					yield return new WaitForEndOfFrame();
				}
			}
			_switch = "DownLoadData";
			Debug.Log("DownLoadFiles and at: " + Time.time.ToString());
		}

		private List<string> parse_(string data) {
			HtmlDocument html_doc = null;
			List<string> temp_left_tbl_strArr = null;
			string temp_left_row_str = "";
			string temp_right_row_str = "";
			List<string> full_tbl_strArr1 = null;
			HtmlNodeCollection row_left_nodes = null;
			HtmlNodeCollection row_right_nodes = null;
			string year = "";
			string _t_date = "";
			string _t_data = "";
			html_doc = new HtmlDocument();
			full_tbl_strArr1 = new List<string>();
			temp_left_tbl_strArr = new List<string>();
			html_doc.LoadHtml(data);
			//Парсинг год из заголовка
			year = Regex.Match(html_doc.DocumentNode.SelectSingleNode("//title").InnerText, "\\d{4}").Value;
			if(string.IsNullOrEmpty(year)) {
				Debug.Log("_parse_year из head-tittle не распарсился");
				return null;
			}
			row_left_nodes = html_doc.DocumentNode.SelectNodes("//div[@class='archive-table-left-column']//tr");
			//Обработка строк левой таблицы (время-дата)
			for(int index4 = 0; index4 < row_left_nodes.Count; index4 += 1) {
				temp_left_row_str = "";
				//ячейки
				foreach(HtmlNode loopObject6 in row_left_nodes[index4].SelectNodes("td")) {
					temp_left_row_str = temp_left_row_str + "." + loopObject6.InnerText;
				}
				temp_left_tbl_strArr.Add(temp_left_row_str.Substring(1));
			}
			row_right_nodes = html_doc.DocumentNode.SelectNodes("//div[@class='archive-table-wrap']//tr");
			//контроль на совпадение количества строк в таблицах (а вдруг?) (должно получится 20)
			if((temp_left_tbl_strArr.Count == row_right_nodes.Count)) {
				//Обработка строк правой таблицы-данные
				for(int index5 = 1; index5 < temp_left_tbl_strArr.Count; index5 += 1) {
					temp_right_row_str = "";
					//ячейки
					foreach(HtmlNode loopObject7 in row_right_nodes[index5].SelectNodes("td")) {
						temp_right_row_str = temp_right_row_str + "|" + loopObject7.InnerText;
					}
					//ГодМесяцДеньЧас
					_t_date = year + temp_left_tbl_strArr[index5].Split(new char[] { '.' })[2] + temp_left_tbl_strArr[index5].Split(new char[] { '.' })[1].PadLeft(2, '0') + temp_left_tbl_strArr[index5].Split(new char[] { '.' })[0];
					//Строка целиком
					_t_data = _t_date + "|" + temp_right_row_str.Substring(1);
					full_tbl_strArr1.Add(_t_data);
				}
			} else {
				Debug.Log("Количество строк не совпадает:" + temp_left_tbl_strArr.Count.ToString() + " vs " + row_right_nodes.Count.ToString());
			}
			return full_tbl_strArr1;
		}

		private void DBInserter(List<string> full_tbl_strArr, string indexDB, string table_year) {
			string q = "";
			q = "REPLACE INTO \"" + table_year + "\" (\"date\",\"wind_dir\",\"wind_speed\",\"vis_range\",\"phenomena\",\"cloudy\",\"T\",\"Td\",\"f\",\"Te\",\"Tes\",\"Comfort\",\"P\",\"Po\",\"Tmin\",\"Tmax\",\"R\",\"R24\",\"S\") " + "VALUES (\"" + string.Join<System.String>("\"),(\"", full_tbl_strArr).Replace("|", "\",\"") + "\")";
			Debug.Log(sqlite.GetComponent<sqlite>().InsertQueryTable(indexDB, q));
		}

		public System.Collections.IEnumerator _t_writeToLog(string dataToLog) {
			Stream fs = null;
			string filePatch = "";
			yield break;
		}

		public List<string> _dbSqliteList(bool update) {
			//Обновление списка баз при старте или принудительно
			if((!((sqliteDBs is object)) || update)) {
				//списка баз не было
				sqliteDBs = new List<string>(Directory.EnumerateFiles("" + Application.dataPath + "/StreamingAssets/" + "files/pogodaiklimat2011/bd/", "*.sqlite", SearchOption.AllDirectories));
			}
			return sqliteDBs;
		}

		public void Update() {
			if(Input.GetKeyUp(KeyCode.RightArrow)) {
				_dbSqliteList(false);
			}
		}
	}
}
