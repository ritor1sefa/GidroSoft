#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;
using HtmlAgilityPack;
using NPOI.XSSF.UserModel;
using MaxyGames.Generated;
using TMPro;

namespace MaxyGames.Generated {
	/// <summary>
	/// http://digitalnativestudios.com/textmeshpro/docs/rich-text/
	/// </summary>
	public class pogodaiklimat2011_un : MaxyGames.RuntimeBehaviour {
		public KeyValuePair<string, string> linkFromConfig = new KeyValuePair<string, string>();
		public string _switch = "";
		public bool Pass = false;
		public string db = "pogodaiklimat2011";
		public TMP_InputField InputF_Index = null;
		public TMP_InputField InputF_Year = null;
		public List<string> sqliteDBs = new List<string>();
		public Dictionary<string, string> sqliteIdNamesType = new Dictionary<string, string>() { { "123", "Value=999" } };
		public Transform parentOfLeft = null;
		public Transform parentOfRight = null;
		public TMP_InputField InputF_right_indexs = null;
		public GameObject objectVariable;

		public void Start() {
			_f_dbSqliteList(false);
			UpdateInInputIndex();
			//clean
			for(int index = 0; index < parentOfRight.childCount; index += 1) {
				Object.Destroy(parentOfRight.GetChild(index).gameObject);
			}
		}

		public void StartPogodaklimat2011() {
			if((_tryGetSiteSize("http://www.google.com") > 0F)) {
				if((_tryGetSiteSize("http://www.pogodaiklimat.ru") > 0F)) {
					new _Log()._2log("start", false);
					base.StartCoroutine(_function_group());
				} else {
					new _Log()._2log("Гугл работает, сайт - похоже что нет" + _tryGetSiteSize("http://www.pogodaiklimat.ru").ToString(), true);
				}
			} else {
				new _Log()._2log("Интернет не работает?" + _tryGetSiteSize("http://www.google.com").ToString(), true);
			}
		}

		/// <summary>
		/// ппоказывает список слева
		/// </summary>
		public void UpdateInInputIndex() {
			List<string> _inputArrayList = null;
			List<string> _dbSqliteList = null;
			GameObject GO_current = null;
			string index_cur = "";
			UnityEngine.UI.ColorBlock _Colorblock = new UnityEngine.UI.ColorBlock();
			_Colorblock = InputF_Index.colors;
			_inputArrayList = _f_indexArray();
			_dbSqliteList = _f_dbSqliteList(false);
			//если найдены номера - вывести их. если нет то сделать поиск по списку городов-регионов и вывести их. если совпадений нету - показать имеющиеся бд и сказать что нету совпадений. ели пустая строка-показать имеющиеся.
			if((_inputArrayList.Count > 0)) {
				//clean
				for(int index1 = 0; index1 < parentOfLeft.childCount; index1 += 1) {
					Object.Destroy(parentOfLeft.GetChild(index1).gameObject);
				}
				foreach(string loopObject in _inputArrayList) {
					index_cur = loopObject;
					//если найдены номера - вывести их. если нет то сделать поиск по списку городов-регионов и вывести их. если совпадений нету - показать имеющиеся бд и сказать что нету совпадений. ели пустая строка-показать имеющиеся.
					if(_dbSqliteList.Contains(index_cur)) {
						GO_current = Object.Instantiate<UnityEngine.GameObject>(objectVariable, parentOfLeft);
						//set name of GameObject
						GO_current.name = index_cur;
						//именование батона
						GO_current.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = new sqlite() { variable4 = null }.getDataUniversalStr("2011_names", "SELECT name FROM 'ru' WHERE id_index='" + index_cur + "' ORDER BY id_index DESC");
					}
				}
				_Colorblock.normalColor = new Color() { r = 0.8156863F, g = 1F, b = 0.8156863F, a = 1F };
				_Colorblock.highlightedColor = new Color() { r = 0.8156863F, g = 1F, b = 0.8156863F, a = 1F };
				_Colorblock.selectedColor = new Color() { r = 0.8156863F, g = 1F, b = 0.8156863F, a = 1F };
				_Colorblock.pressedColor = new Color() { r = 1F, g = 0.972549F, b = 0.4862745F, a = 1F };
			} else {
				Debug.Log("совпадений нету");
				_Colorblock.normalColor = new Color() { r = 1F, g = 0.8156863F, b = 0.8156863F, a = 1F };
				_Colorblock.highlightedColor = new Color() { r = 1F, g = 0.8156863F, b = 0.8156863F, a = 1F };
				_Colorblock.selectedColor = new Color() { r = 1F, g = 0.8156863F, b = 0.8156863F, a = 1F };
				_Colorblock.pressedColor = new Color() { r = 1F, g = 0.972549F, b = 0.4862745F, a = 1F };
			}
			InputF_Index.colors = _Colorblock;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _f_indexArray() {
			List<string> data_index = new List<string>();
			int _tmpint = 0;
			data_index = new List<string>();
			foreach(string loopObject1 in Regex.Split(InputF_Index.text, "[^a-zA-Z0-9А-Яа-я]", RegexOptions.Multiline)) {
				if(int.TryParse(loopObject1, out _tmpint)) {
					data_index.Add(loopObject1);
				}
			}
			return data_index;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _f_yearArray() {
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
						for(int index2 = _yearPlus; index2 < curYearPlus1; index2 += 1) {
							data_year.Add(index2.ToString());
							new _Log()._2log(index2.ToString(), true);
						}
						if((data_year.Capacity > 0)) {
							return data_year;
						} else {
							new _Log()._2log("Есть года, но не в том диапазоне", true);
						}
					} else {
						new _Log()._2log("Есть +, но нету валидного года рядом с плюсом", true);
					}
				} else {
					new _Log()._2log("Есть +, но нету 4значного числа", true);
				}
			} else if(year_2_parse.Contains("-")) {
				_year_splited = Regex.Replace(year_2_parse, ".*([0-9-]{9}).*", " $1 ").Split(new char[] { '-' });
				_year_from = int.Parse(_year_splited[0]);
				_year_to = int.Parse(_year_splited[1]);
				if((((_year_from > 2010F) && (_year_to < curYearPlus1)) && (_year_to > _year_from))) {
					for(int index3 = _year_from; index3 <= _year_to; index3 += 1) {
						data_year.Add(index3.ToString());
						new _Log()._2log(index3.ToString(), false);
					}
					return data_year;
				} else {
					new _Log()._2log("from_to_Года не соответствуют: " + year_2_parse, true);
				}
			} else {
				foreach(string loopObject2 in Regex.Split(year_2_parse, "\\D", RegexOptions.Multiline)) {
					if(((int.TryParse(loopObject2, out _year_casual) && (_year_casual > 2010F)) && (_year_casual < curYearPlus1))) {
						data_year.Add(_year_casual.ToString());
					}
				}
				if((data_year.Capacity > 0F)) {
					data_year.Sort();
					return data_year;
				} else {
					new _Log()._2log("casual_Года не соответствуют: " + year_2_parse, false);
				}
			}
			new _Log()._2log("_f_yearArray_All+", true);
			return data_year;
		}

		/// <summary>
		/// получение названий файлов бд
		/// </summary>
		public List<string> _f_dbSqliteList(bool update) {
			//Обновление списка баз при старте или принудительно
			if(((sqliteDBs.Count == 0) || update)) {
				sqliteDBs.Clear();
				foreach(string loopObject3 in Directory.EnumerateFiles("" + Application.dataPath + "/StreamingAssets/" + "files/pogodaiklimat2011/bd/", "*.sqlite", SearchOption.AllDirectories)) {
					sqliteDBs.Add(loopObject3.Remove(loopObject3.LastIndexOf(".")).Substring((loopObject3.Remove(loopObject3.LastIndexOf(".")).LastIndexOf("/") + 1)));
				}
			}
			return sqliteDBs;
		}

		private System.Collections.IEnumerator _function_group() {
			float timelimit = 0.2F;
			int PassCount = 0;
			Dictionary<string, string> DictLinks = new Dictionary<string, string>();
			base.StartCoroutine(LoadFromConfig());
			yield return new WaitForSeconds(timelimit);
			if(_switch.Equals("LoadFromConfig")) {
				DictLinks = LinkGeneration(false);
				if((DictLinks.Count > 0)) {
					//стрёмный кусок. объеденить с предыдущим Ифом?
					while(!((DictLinks.Count == 0))) {
						new _Log()._2log("Ссылок на скачивание создано: " + DictLinks.Count.ToString(), false);
						yield return new WaitForSeconds(timelimit);
						base.StartCoroutine(DownLoadData(DictLinks));
						yield return new WaitUntil(() => _switch == "DownLoadData");
						PassCount = (PassCount + 1);
						new _Log()._2log("закончено?", true);
						yield break;
					}
					new _Log()._2log("LinkGeneration = 0", true);
				} else {
					new _Log()._2log("ссылок нету(?)", true);
				}
			} else {
				new _Log()._2log("LoadFromConfig > 0.2 sec!!!", true);
			}
			yield break;
		}

		private System.Collections.IEnumerator LoadFromConfig() {
			linkFromConfig = new KeyValuePair<string, string>();
			foreach(KeyValuePair<string, string> loopObject4 in Ini.Load("config.ini")) {
				if(loopObject4.Key.StartsWith("pogodaiklimat2011")) {
					linkFromConfig = loopObject4;
					break;
				}
			}
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				new _Log()._2log("pogodaiklimat2011 нету!", true);
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
				new _Log()._2log("ссылки из конфига всё таки нету", true);
			} else {
				_link_base = linkFromConfig.Value;
				//id=index
				foreach(string loopObject5 in _f_indexArrayRight()) {
					_t_index = loopObject5;
					_link_Index = _link_base.Replace("$id", _t_index);
					//set year
					foreach(string loopObject6 in _f_yearArray()) {
						_t_AllDatetimeClmn = new sqlite() { variable4 = null }.GetAllDatetimeClmn(_t_index, loopObject6);
						if((int.Parse(loopObject6) <= curYear)) {
							_t_year = int.Parse(loopObject6);
							_link_IndexAndYear = _link_Index.Replace("$year", _t_year.ToString());
							if(_t_year.Equals(curYear)) {
								//set month max
								_monthMax = (curMonth + 1);
							}
							//set month links
							for(int index4 = 1; index4 < _monthMax; index4 += 1) {
								_t_month = index4;
								_t_DaysInMonth = System.DateTime.DaysInMonth(_t_year, _t_month);
								//YYYY1231
								_t_dateToCheck = _t_year.ToString() + (_t_month.ToString("D2") as string) + _t_DaysInMonth.ToString();
								if(_t_AllDatetimeClmn.Contains(_t_dateToCheck)) {
									new _Log()._2log("LinkGeneration. Пропущено т.к. в бд уже есть этого месяца последний день:  " + _t_dateToCheck, false);
									AddMonth = false;
									if(CheckForMissing) {
										new _Log()._2log("Однако полная проверочка", true);
										//проверка на отсутствующие часы в месяце
										if(((_t_DaysInMonth * 8) != (_t_AllDatetimeClmn.Replace(_t_year.ToString() + (_t_month.ToString("D2") as string), _t_year.ToString() + (_t_month.ToString("D2") as string) + "%").Split(new char[] { '%' }).Length - 1))) {
											new _Log()._2log("чего то не хватает или лишнее", true);
											//Вероятность что чего то лишнее-крайне мала. Вероятностью что чего то лишнего == недостающего ещё меньше. Так что пренебрегаем
											AddMonth = true;
											//Избыточный поиск недостающего\лишнего
											for(int index5 = 1; index5 < (_t_DaysInMonth + 1); index5 += 1) {
												_t_dayToCheck = _t_year.ToString() + (_t_month.ToString("D2") as string) + index5.ToString("D2");
												//Часов(строк) в этих сутках
												_t_HourInDay = (_t_AllDatetimeClmn.Replace(_t_dayToCheck, _t_dayToCheck + "%").Split(new char[] { '%' }).Length - 1);
												if(!((8 == _t_HourInDay))) {
													new _Log()._2log("Нехваток\\избыток __" + _t_HourInDay.ToString() + "__ часов в этот день: " + _t_dayToCheck, true);
												}
											}
										} else {
											new _Log()._2log("Все часы в месяце на месте (удивительно)", true);
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
				new _Log()._2log(r.ToString() + "| размер: " + url, false);
				//+30% т.к. на сайтах сжатие. А так - чуть ближе к истине
				return ((r / 3.2F) + r);
			} else {
				new _Log()._2log("Не скачалась шапка. " + "" + "" + url, true);
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
			foreach(KeyValuePair<string, string> loopObject7 in DictLinks) {
				_index = loopObject7.Value;
				_w_link = loopObject7.Key;
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
							new _Log()._2log("список на вставку в бд пуст: " + full_tbl_strArr.Count.ToString(), true);
						}
					} else {
						new _Log()._2log(uwr.result.ToString() + ":! " + uwr.error + " | " + _index, true);
						Pass = true;
						CountTry = (CountTry + 1);
						if((CountTry > 3F)) {
							_switch = "DownLoadData";
							new _Log()._2log("Много не скачанных файлов" + CountTry.ToString(), true);
						}
					}
					yield return new WaitForEndOfFrame();
				}
			}
			_switch = "DownLoadData";
			new _Log()._2log("DownLoadFiles End", true);
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
				new _Log()._2log("_parse_year из head-tittle не распарсился", true);
				return null;
			}
			row_left_nodes = html_doc.DocumentNode.SelectNodes("//div[@class='archive-table-left-column']//tr");
			//Обработка строк левой таблицы (время-дата)
			for(int index6 = 0; index6 < row_left_nodes.Count; index6 += 1) {
				temp_left_row_str = "";
				//ячейки
				foreach(HtmlNode loopObject8 in row_left_nodes[index6].SelectNodes("td")) {
					temp_left_row_str = temp_left_row_str + "." + loopObject8.InnerText;
				}
				temp_left_tbl_strArr.Add(temp_left_row_str.Substring(1));
			}
			row_right_nodes = html_doc.DocumentNode.SelectNodes("//div[@class='archive-table-wrap']//tr");
			//контроль на совпадение количества строк в таблицах (а вдруг?) (должно получится 20)
			if((temp_left_tbl_strArr.Count == row_right_nodes.Count)) {
				//Обработка строк правой таблицы-данные
				for(int index7 = 1; index7 < temp_left_tbl_strArr.Count; index7 += 1) {
					temp_right_row_str = "";
					//ячейки
					foreach(HtmlNode loopObject9 in row_right_nodes[index7].SelectNodes("td")) {
						temp_right_row_str = temp_right_row_str + "|" + loopObject9.InnerText;
					}
					//ГодМесяцДеньЧас
					_t_date = year + temp_left_tbl_strArr[index7].Split(new char[] { '.' })[2] + temp_left_tbl_strArr[index7].Split(new char[] { '.' })[1].PadLeft(2, '0') + temp_left_tbl_strArr[index7].Split(new char[] { '.' })[0];
					//Строка целиком
					_t_data = _t_date + "|" + temp_right_row_str.Substring(1);
					full_tbl_strArr1.Add(_t_data);
				}
			} else {
				new _Log()._2log("Количество строк не совпадает:" + temp_left_tbl_strArr.Count.ToString() + " vs " + row_right_nodes.Count.ToString(), true);
			}
			return full_tbl_strArr1;
		}

		private void DBInserter(List<string> full_tbl_strArr, string indexDB, string table_year) {
			string q = "";
			string ans = "";
			q = "REPLACE INTO \"" + table_year + "\" (\"date\",\"wind_dir\",\"wind_speed\",\"vis_range\",\"phenomena\",\"cloudy\",\"T\",\"Td\",\"f\",\"Te\",\"Tes\",\"Comfort\",\"P\",\"Po\",\"Tmin\",\"Tmax\",\"R\",\"R24\",\"S\") " + "VALUES (\"" + string.Join<System.String>("\"),(\"", full_tbl_strArr).Replace("|", "\",\"") + "\")";
			ans = "Записей в Бд вставлено: " + new sqlite() { variable4 = null }.InsertQueryTable(indexDB, q).ToString();
			new _Log()._2log(ans, false);
		}

		public void Update() {
			if(Input.GetKeyUp(KeyCode.RightArrow)) {
				new _Log()._2log(sqliteIdNamesType.ContainsValue("=99").ToString(), true);
			}
		}

		public void addToRight() {
			for(int index8 = 0; index8 < parentOfLeft.childCount; index8 += 1) {
				if(!(InputF_right_indexs.text.Contains(parentOfLeft.GetChild(index8).gameObject.name))) {
					InputF_right_indexs.text = InputF_right_indexs.text + "|" + parentOfLeft.GetChild(index8).gameObject.name;
					parentOfLeft.GetChild(index8).SetParent(parentOfRight);
				}
			}
			UpdateInInputIndex();
		}

		public void clearRight() {
			//clean
			for(int index9 = 0; index9 < parentOfRight.childCount; index9 += 1) {
				Object.Destroy(parentOfRight.GetChild(index9).gameObject);
			}
			InputF_right_indexs.text = "";
			UpdateInInputIndex();
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _f_indexArrayRight() {
			List<string> data_index1 = new List<string>();
			int _tmpint1 = 0;
			data_index1 = new List<string>();
			foreach(string loopObject10 in InputF_right_indexs.text.Split("|", System.StringSplitOptions.RemoveEmptyEntries)) {
				if(int.TryParse(loopObject10, out _tmpint1)) {
					data_index1.Add(loopObject10);
				}
			}
			return data_index1;
		}
	}
}
