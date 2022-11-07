#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class _utillz : object {

	public void _2log(string text2log, bool error) {
		string time = "";
		if(error) {
			time = "<color=#ff0000>" + System.DateTime.Now.ToString("HH:mm:ss") + "</color>" + ": ";
		} else {
			time = "<size=80%>" + System.DateTime.Now.ToString("HH:mm:<u>ss<\\/u>") + "</size>" + ": ";
		}
		GameObject.Find("log_window").GetComponent<TMPro.TMP_InputField>().text = time + "" + text2log + System.Environment.NewLine + GameObject.Find("log_window").GetComponent<TMPro.TMP_InputField>().text;
	}

	public string _ConvertStringRusLat(string into) {
		Dictionary<string, string> Letters2Latin = new Dictionary<string, string>() { { "а", "a" }, { "А", "A" }, { "б", "b" }, { "Б", "B" }, { "в", "v" }, { "В", "V" }, { "г", "g" }, { "Г", "G" }, { "д", "d" }, { "Д", "D" }, { "е", "e" }, { "Е", "E" }, { "ё", "yo" }, { "Ё", "Yo" }, { "ж", "j" }, { "Ж", "J" }, { "з", "z" }, { "З", "Z" }, { "и", "i" }, { "И", "I" }, { "й", "y" }, { "Й", "Y" }, { "к", "k" }, { "К", "K" }, { "л", "l" }, { "Л", "L" }, { "м", "m" }, { "М", "M" }, { "н", "n" }, { "Н", "N" }, { "о", "o" }, { "О", "O" }, { "п", "p" }, { "П", "P" }, { "р", "r" }, { "Р", "R" }, { "с", "c" }, { "С", "C" }, { "т", "t" }, { "Т", "T" }, { "у", "u" }, { "У", "U" }, { "ф", "f" }, { "Ф", "F" }, { "х", "x" }, { "Х", "X" }, { "ц", "ts" }, { "Ц", "Ts" }, { "ч", "ch" }, { "Ч", "Ch" }, { "ш", "sh" }, { "Ш", "Sh" }, { "щ", "shch" }, { "Щ", "Shch" }, { "ъ", "_b" }, { "Ъ", "_b" }, { "ы", "bI" }, { "Ы", "bI" }, { "ь", "'" }, { "Ь", "'" }, { "э", "ea" }, { "Э", "Ea" }, { "ю", "yu" }, { "Ю", "Yu" }, { "я", "ya" }, { "Я", "Ya" }, { " ", " " } };
		string out_r = "";
		string value = "";
		for(int index = 0; index < into.Length; index += 1) {
			if(Letters2Latin.TryGetValue(into.Substring(index, 1), out value)) {
				out_r = out_r + value;
			} else {
				out_r = out_r + "~";
			}
		}
		return out_r;
	}
}
