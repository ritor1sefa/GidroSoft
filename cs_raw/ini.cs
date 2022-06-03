using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Ini
{
	/// <summary>
	/// Функция сохраняет данные в ini файл
	/// </summary>
	/// <param name="fileName">имя файла в директории Data приложения (в редакторе используется папка Assets)</param>
	/// <param name="data">данные в виде словаря</param>
	public static void Save(string fileName, Dictionary<string, string> data)
	{
		// используем построитель строк для получения полного текста файла
		StringBuilder sb = new StringBuilder();
		
		// для каждой пары ключ-значение добавляем строку
		foreach (KeyValuePair<string, string> keyValuePair in data)
		{
			sb.AppendLine(keyValuePair.Key + "=" + keyValuePair.Value);
		}
		
		// получаем полный путь
		string path = GetFullPath(fileName);
		
		// сохраняем
		File.WriteAllText(path, sb.ToString());
	}
	
	/// <summary>
	/// Функция читает данные из ini файла
	/// </summary>
	/// <param name="fileName">имя файла в директории Data приложения (в редакторе используется папка Assets)</param>
	/// <returns></returns>
	public static Dictionary<string, string> Load(string fileName)
	{
		// создаем словарь
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		// получаем полный путь к файлу
		string path = GetFullPath(fileName);
		
		// читаем файл в массив строк
		string[] lines = File.ReadAllLines(path);
		
		// выполняем получение данных из кадой строки
		foreach (string line in lines)
		{
			string dataString = line.Trim();
			
			// пустые строки пропускаем
			if (string.IsNullOrEmpty(dataString)) continue;
			
			// пропускаем комментарии
			if (dataString.StartsWith(";")) continue;
			
			// так же пропускаем строки, не содержащие равно
			if (dataString.Contains("="))
			{
				// находим позицию первого равно
				int pos = dataString.IndexOf("=");
				
				// получаем данные
				string key = dataString.Substring(0, pos).Trim();
				string value = "";
				if ((pos + 1) < dataString.Length)
				{
					value = dataString.Substring(pos + 1, dataString.Length - pos - 1).Trim();
				}
				// сохраняем данные в коллекцию
				data.Add(key, value);
			}			
			/*
			// Sections
			if (dataString.Contains("["))
			{
				int pos1 = dataString.IndexOf("[");
				int pos2 = dataString.IndexOf("]");
				
				// получаем данные
				string key = "Section";
				string value = dataString.Substring(pos1, pos2).Trim();
				// сохраняем данные в коллекцию
				data.Add(key, value);
			}
			*/
		}
		return data;
	}
	
	/// <summary>
	/// Функция получаения полного пути в папке Data приложения (Assets в режиме редактора)
	/// </summary>
	/// <param name="localPath">Относительный путь к файлу</param>
	/// <returns>Полный путь</returns>
	public static string GetFullPath(string localPath)
	{
		/*%userprofile%\AppData\LocalLow\<companyname>\<productname>*/
		////string basePath = Application.persistentDataPath.Trim(); 

		/*Unity Editor: <path to project folder>/Assets
		Win/Linux player: <path to executablename_Data folder> (note that most Linux installations will be case-sensitive!) */
		////string basePath = Application.dataPath.Trim(); 

		/*StreamingAssets folder*/
		string basePath = Application.streamingAssetsPath.Trim();
		if (!(basePath.EndsWith("/") || basePath.EndsWith("\\")))
		{
			basePath += "/";
		}
		
		localPath = localPath.Trim();
		if (localPath.StartsWith("/") || localPath.StartsWith("\\"))
		{
			localPath = localPath.Substring(1);
		}
		
		return basePath + localPath;
	}


}

