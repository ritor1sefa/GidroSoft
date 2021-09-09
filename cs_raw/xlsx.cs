#pragma warning disable
using UnityEngine;
using System.IO;
using System.Data;
using System.Collections.Generic;
using HtmlAgilityPack;
using NPOI.XSSF.UserModel;
using MaxyGames.Generated;

namespace MaxyGames.Generated {
	public class xlsx : MaxyGames.RuntimeBehaviour {
		public bool newTmpFolder = false;

		public void Start() {}

		public void Update() {
			if(Input.GetKeyUp(KeyCode.RightArrow)) {
				FileManage("pogodaiklimat2011", "29838", "2011,2012", true);
			}
		}

		private void FileManage(string target, string index, string yearList, bool monoFile) {
			string xlsxPatch = "";
			string tmp_Path = "";
			string xlsxTargetFile = "";
			xlsxPatch = "" + Application.dataPath + "/StreamingAssets/files/" + target + "/xls/";
			//Создавать новые файлы в временых папках, либо перезаписывать ранее созданные в общей.
			if(newTmpFolder) {
				tmp_Path = xlsxPatch + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "/";
			} else {
				tmp_Path = xlsxPatch + index + "/";
			}
			//Существует ли пустой(предзаполненный)  файл-бланк. Влепить потом открытие папки с ним для ручной замены, и\или "автоматическое" обновление его, если был перетащен новый xlsx файл (избыточно?)
			if(!(File.Exists(xlsxPatch + "_blankSomeYear.xlsx"))) {
				Debug.Log("Xlsx бланк отсутствует! ");
			} else {
				Directory.CreateDirectory(tmp_Path);
				//В один ли файл все года? (избыточно?)
				if(monoFile) {
					xlsxTargetFile = tmp_Path + index + "(" + yearList + ")" + ".xlsx";
					File.Copy(xlsxPatch + "_blankSomeYear.xlsx", xlsxTargetFile, true);
				}
			}
		}
	}
}
