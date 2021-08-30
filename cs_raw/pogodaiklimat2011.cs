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
		public UnityEngine.UI.InputField _index = null;
		public UnityEngine.UI.InputField _year = null;
		public string _switch = "";
		public bool TryAgain = false;
		public MaxyGames.uNode.uNodeRuntime sqlite = null;
		public string db = "pogodaiklimat2011";
		public Dictionary<string, string> linksToDownload = new Dictionary<string, string>();
		private int curYear = 0;
		private int curMonth = 0;
		private string _link_base = "";

		public void StartPogodaklimat2011() {
			if((_tryGetSiteSize("www.google.com") > 0F)) {
				base.StartCoroutine(_function_group());
			} else {
				Debug.Log("Интернет не работает?");
			}
		}

		public System.Collections.IEnumerator _function_group() {
			float timelimit = 0.1F;
			WaitForSeconds variable11 = null;
			int variable2 = 0;
			base.StartCoroutine(LoadFromConfig());
			yield return new WaitForSeconds(timelimit);
			if(_switch.Equals("LoadFromConfig")) {
				Debug.Log("LinkGen start at: " + Time.realtimeSinceStartup.ToString());
				base.StartCoroutine(LinkGeneration());
				yield return new WaitForSeconds(timelimit);
				Debug.Log("LinkGen end at: " + Time.realtimeSinceStartup.ToString());
				if(_switch.Equals("LinkGeneration")) {
					base.StartCoroutine(DownLoadData());
					yield return new WaitUntil(() => _switch == "DownLoadData");
					variable2 = (variable2 + 1);
					Debug.Log("итерация");
				} else {
					Debug.Log("LinkGeneration -> завис??");
				}
			} else {
				Debug.Log("LoadFromConfig > 0.2 sec!!!");
			}
			yield break;
		}

		public System.Collections.IEnumerator LoadFromConfig() {
			linkFromConfig = new KeyValuePair<string, string>();
			foreach(KeyValuePair<string, string> loopObject in Ini.Load("config.ini")) {
				if(loopObject.Key.StartsWith("pogodaiklimat2011")) {
					linkFromConfig = loopObject;
					break;
				}
			}
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				Debug.Log("pogodaiklimat2011 нету!");
			} else {
				Debug.Log(linkFromConfig);
				_switch = "LoadFromConfig";
			}
			yield break;
		}

		public System.Collections.IEnumerator LinkGeneration() {
			int _monthMin = 1;
			int _monthMax = 13;
			string _link_Index = "";
			string _link_IndexAndYear = "";
			string _link_Final = "";
			string _FirstDay = "1";
			string _LastDay = "";
			string _t_index = "";
			int _t_year = 2011;
			int _t_month = 0;
			System.DateTime LastDatetimeInDB = new System.DateTime();
			linksToDownload.Clear();
			curYear = System.DateTime.Now.Year;
			curMonth = System.DateTime.Now.Month;
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				_switch = "LinkGeneration";
				yield break;
			} else {
				_link_base = linkFromConfig.Value;
				//id=index
				foreach(string loopObject1 in _indexArray()) {
					_t_index = loopObject1;
					_link_Index = _link_base.Replace("$id", _t_index);
					LastDatetimeInDB = sqlite.GetComponent<sqlite>().GetLastDate(db, _t_index);
					//set year
					foreach(string loopObject2 in _yearArray()) {
						if((int.Parse(loopObject2) <= curYear)) {
							//set year (from last in bd to selected)
							for(int index = LastDatetimeInDB.Year; index <= int.Parse(loopObject2); index += 1) {
								_t_year = index;
								_link_IndexAndYear = _link_Index.Replace("$year", _t_year.ToString());
								//set month max
								if(_t_year.Equals(curYear)) {
									_monthMax = (curMonth + 1);
								}
								//set month min (based on data in bd)
								if((LastDatetimeInDB.Year == _t_year)) {
									_monthMin = LastDatetimeInDB.Month;
								}
								//set month links
								for(int index1 = _monthMin; index1 < _monthMax; index1 += 1) {
									_t_month = index1;
									_link_Final = _link_IndexAndYear.Replace("$month", _t_month.ToString());
									//set first day in month
									if(((LastDatetimeInDB.Year == _t_year) && (LastDatetimeInDB.Month == _t_month))) {
										_FirstDay = LastDatetimeInDB.Day.ToString();
									}
									_link_Final = _link_Final.Replace("$FirstDay", _FirstDay);
									//set last day in month
									if(((curYear == _t_year) && (curMonth == _t_month))) {
										_LastDay = System.DateTime.Now.Day.ToString();
									} else {
										_LastDay = System.DateTime.DaysInMonth(_t_year, _t_month).ToString();
									}
									_link_Final = _link_Final.Replace("$LastDay", _LastDay);
									linksToDownload.Add(_link_Final, _t_index);
								}
							}
						}
					}
				}
			}
			_switch = "LinkGeneration";
			yield break;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _indexArray() {
			List<string> data_index = new List<string>();
			int variable1 = 0;
			data_index = new List<string>();
			foreach(string loopObject3 in Regex.Split(_index.text, "\\D", RegexOptions.Multiline)) {
				if(int.TryParse(loopObject3, out variable1)) {
					data_index.Add(loopObject3);
				}
			}
			return data_index;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _yearArray() {
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
			year_2_parse = _year.text;
			if(year_2_parse.Contains("+")) {
				if(int.TryParse(Regex.Replace(year_2_parse, ".*([0-9]{4})\\+.*", "$1", RegexOptions.Multiline, System.TimeSpan.FromMilliseconds(500D)), out _yearPlus)) {
					if(((_yearPlus > 2010F) && (_yearPlus < curYearPlus1))) {
						for(int index2 = _yearPlus; index2 < curYearPlus1; index2 += 1) {
							data_year.Add(index2.ToString());
							Debug.Log(index2);
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
					for(int index3 = _year_from; index3 <= _year_to; index3 += 1) {
						data_year.Add(index3.ToString());
						Debug.Log(index3);
					}
					return data_year;
				} else {
					Debug.Log("from_to_Года не соответствуют: " + year_2_parse);
				}
			} else {
				foreach(string loopObject4 in Regex.Split(year_2_parse, "\\D", RegexOptions.Multiline)) {
					if(((int.TryParse(loopObject4, out _year_casual) && (_year_casual > 2010F)) && (_year_casual < curYearPlus1))) {
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

		public float _tryGetSiteSize(string url) {
			UnityWebRequest conn = null;
			float r = 0F;
			conn = UnityWebRequest.Get(url);
			conn.SetRequestHeader("Accept-Encoding", "gzip, deflate");
			conn.timeout = 3;
			conn.SendWebRequest();
			while(!(conn.isDone)) {
				new WaitForEndOfFrame();
			}
			if(float.TryParse(conn.GetResponseHeader("Content-Length"), out r)) {
				//+30% т.к. на сайтах сжатие. А так - чуть ближе к истине
				return ((r / 3.2F) + r);
			} else {
				Debug.Log("tryGetFileSize: " + conn.GetResponseHeader("Content-Length") + "_" + url);
			}
			return -1;
		}

		/// <summary>
		/// Тут будет скачивание.
		/// </summary>
		public System.Collections.IEnumerator DownLoadData() {
			string _index1 = "";
			string _w_link = "";
			string folder_path = "";
			UnityWebRequest uwr = null;
			string file_path = "";
			Stream _file = null;
			int CountTry = 0;
			float site_size = 0F;
			List<string> full_tbl_strArr = new List<string>();
			TryAgain = false;
			UnityWebRequest.ClearCookieCache();
			foreach(KeyValuePair<string, string> loopObject5 in linksToDownload) {
				_index1 = loopObject5.Value;
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
					yield return new WaitForSeconds(0.1F);
					if((uwr.result == UnityWebRequest.Result.Success)) {
						Debug.Log("старт парса at: " + Time.realtimeSinceStartup.ToString());
						//Парсинг страницы в таблицу
						full_tbl_strArr = parse_(uwr.downloadHandler.text);
						Debug.Log("конец парсинга at: " + Time.realtimeSinceStartup.ToString());
						if((full_tbl_strArr.Count > 0)) {
							DBInserter(full_tbl_strArr, _index1);
						}
					} else {
						Debug.Log(uwr.result.ToString() + ":! " + uwr.error + " | " + _index1);
						TryAgain = true;
						CountTry = (CountTry + 1);
						if((CountTry > 3F)) {
							_switch = "DownLoadData";
							Debug.Log("Много не скачанных файлов");
							yield break;
						}
					}
				}
			}
			_switch = "DownLoadFiles";
			Debug.Log("DownLoadFiles and at: " + Time.time.ToString());
			yield break;
		}

		public List<string> parse_(string data) {
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

		public void DBInserter(List<string> full_tbl_strArr, string index) {
			Debug.Log(sqlite.GetComponent<sqlite>().InsertQueryTable("pogodaiklimat2011", "INSERT INTO \"" + index + "\" (\"date\",\"wind_dir\",\"wind_speed\",\"vis_range\",\"phenomena\",\"cloudy\",\"T\",\"Td\",\"f\",\"Te\",\"Tes\",\"Comfort\",\"P\",\"Po\",\"Tmin\",\"Tmax\",\"R\",\"R24\",\"S\") " + "VALUES (\"" + string.Join<System.String>("\"),(\"", full_tbl_strArr).Replace("|", "\",\"") + "\")", index));
		}
	}
}
