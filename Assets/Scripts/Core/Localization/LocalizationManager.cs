using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Localization manager.
/// </summary>
public static class LocalizationManager
{
	public const string c_DefaultSpreadSheet	= "Data.csv";
	public const string c_Spreadsheet			= "Spreadsheet";
	public const string c_KeyHeader				= "Key";

	public const string	c_DefaultKeyValue		= "";

	public const string	c_DefaultLanguage		= "en-US";
		
	private static readonly Dictionary<string, Dictionary<string, string>>	m_Dictionary = new Dictionary<string, Dictionary<string, string>>();
	private static string													m_Language = c_DefaultLanguage;
	private static string													m_DeafultSpreadsheetFolder;
		
	/// <summary>
	/// Get or set language.
	/// </summary>
	public static string Language
	{
		get => m_Language;
		set
		{
			if (m_Dictionary.ContainsKey(value))
				m_Language = value; 
			else
				m_Language = c_DefaultLanguage;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public enum ChangeLocalizationKeyMode
	{
		None,

		Block,
		Override,
		Inherit,
		Switch
	}

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Set default language.
	/// </summary>
	public static void AutoLanguage()
	{
		Language = CultureInfo.CurrentCulture.Name;
	}

	/// <summary>
	/// Read localization spreadsheets.
	/// </summary>
	public static void Read(string path = "Localization", bool clearDictionary = false)
	{
		// clear dictionary if flag set
		if (clearDictionary)
			m_Dictionary.Clear();

		var textAssets = Resources.LoadAll<TextAsset>(path);
		
		if (textAssets.Length == 0)
			return;

		// parse spreadsheets
		foreach (var textAsset in textAssets)
		{
			// read text with CsvHelper
			using (var memoryStream = new MemoryStream(textAsset.bytes))
			using (var stream = new StreamReader(memoryStream, Encoding.UTF8))
			using (var csv = new CsvReader(stream))
			{
				// set options
				csv.Configuration.Delimiter = ",";
				csv.Configuration.HasHeaderRecord = true;

				// read header
				csv.Read();
				csv.ReadHeader();

				var languageList = csv.Context.HeaderRecord.Except(new []{ c_KeyHeader, c_Spreadsheet }).ToList();
					
				// validate languages from headers
				foreach (var language in languageList)	
					if (m_Dictionary.TryGetValue(language, out var localizationData) == false)
					{
						localizationData = new Dictionary<string, string>();
						m_Dictionary.Add(language, localizationData);
					}
#if UNITY_EDITOR	// register spreadsheet
				if (m_Dictionary.ContainsKey(c_Spreadsheet) == false)
					m_Dictionary[c_Spreadsheet] = new Dictionary<string, string>();
#endif

				// parse spreadsheet
				while (csv.Read())
				{
					var key = csv.GetField(c_KeyHeader);
					// try to read all presented languages
					foreach (var language in languageList)
					{
						var localizationData = m_Dictionary[language];
						// read data or set default
						if (csv.TryGetField<string>(language, out var data))
							localizationData[key] = data;
						else
							localizationData[key] = c_DefaultKeyValue;
					}
#if UNITY_EDITOR		// add key to spreadsheet
					m_Dictionary[c_Spreadsheet][key] = AssetDatabase.GetAssetPath(textAsset);//System.IO.Path.Combine(path, textAsset.name);
#endif
				}
			}
		}

#if UNITY_EDITOR
		// add default text file
		m_DeafultSpreadsheetFolder = Path.Combine(Path.GetDirectoryName(m_Dictionary[c_Spreadsheet].First().Value), c_DefaultSpreadSheet).Replace('\\', '/');
#endif

		AutoLanguage();
	}

	public static void Save(string spreadsheetName = null)
	{
		//Debug.Log("Save");
		var languageList = m_Dictionary.Keys.Except(new []{c_Spreadsheet}).ToList();

		var dictionaryList = m_Dictionary.Where(n => n.Key != c_Spreadsheet).ToList();

		var headerList = new List<string>();
		headerList.Add(c_KeyHeader);
		headerList.AddRange(languageList);


		foreach (var spreadsheet in m_Dictionary[c_Spreadsheet].ToLookup(key => key.Value, value => value.Key))
		{
			if (spreadsheetName == null || spreadsheet.Key == spreadsheetName)
				using (var fileStream = File.Create(spreadsheet.Key))
				using (var stream = new StreamWriter(fileStream, Encoding.UTF8))
				using (var csv = new CsvWriter(stream))
				{
					csv.Configuration.Delimiter = ",";
					csv.Configuration.HasHeaderRecord = false;

					// write header
					foreach (var n in headerList)
						csv.WriteField(n);
					csv.NextRecord();

					// write keys
					foreach (var key in spreadsheet)
					{
						// write key
						csv.WriteField(key);
						// get value from dictionary or write default
						foreach (var dic in dictionaryList)
							if (dic.Value.TryGetValue(key, out var value))
								csv.WriteField(value);
							else
								csv.WriteField(c_DefaultKeyValue);

						csv.NextRecord();
					}
				}
		}
	}

	public static string GenerateNewLocalizationKey(string key = "", string value = c_DefaultKeyValue)
	{
		// read dictionary
		if (m_Dictionary.ContainsKey(Language) == false)
			Read();

		var localizationKey = "None";
		if (string.IsNullOrEmpty(key) == false)
			localizationKey = key;

		if (m_Dictionary[Language].ContainsKey(localizationKey) == false)
		{
			// add to dictionary
			m_Dictionary[Language].Add(localizationKey, value);

			// add to spreadsheet
			m_Dictionary[c_Spreadsheet].Add(localizationKey, m_DeafultSpreadsheetFolder);
				
			// save changes
			Save(implGetSpreadsheetOfKey(localizationKey));
            Read();
		}

		return localizationKey;
	}

	public static bool ChangeLocalizationKey(string localizationKey, string newLocalizationKey, ChangeLocalizationKeyMode mode)
	{
		// read dictionary
		if (m_Dictionary.ContainsKey(Language) == false)
			Read();

		// do nothing if mode none
		if (mode == ChangeLocalizationKeyMode.None)
			return true;

		// for all languages
		foreach (var dic in m_Dictionary.Values)
		{
			// is key can be changed
			if (dic.TryGetValue(localizationKey, out var localizedData))
			{
				switch (mode)
				{
					case ChangeLocalizationKeyMode.None:
						return true;
					case ChangeLocalizationKeyMode.Block:
					{
						if (dic.ContainsKey(newLocalizationKey) == false)
						{
							// remove changed key
							dic.Remove(localizationKey);
							// add new
							dic.Add(newLocalizationKey, localizedData);
							// save changes
							Save(implGetSpreadsheetOfKey(localizationKey));
							//Save();
							return true;
						}
						else
						{
							return false;
						}
					}
					case ChangeLocalizationKeyMode.Override:
					{
						if (dic.ContainsKey(newLocalizationKey))
						{
							// override existed key value
							dic[newLocalizationKey] = dic[localizationKey];
							dic.Remove(localizationKey);
						}
						else
						{
							// rename key
							dic.Remove(localizationKey);
							dic.Add(newLocalizationKey, localizedData);
						}
					}	break;
					case ChangeLocalizationKeyMode.Inherit:
					{
						if (dic.ContainsKey(newLocalizationKey))
						{
							// inherit existed key value
							dic.Remove(localizationKey);
						}
						else
						{
							// rename key
							dic.Remove(localizationKey);
							dic.Add(newLocalizationKey, localizedData);
						}
					}	break;
					case ChangeLocalizationKeyMode.Switch:
					{
						if (dic.ContainsKey(newLocalizationKey))
						{
							// don't delete previous key
						}
						else
						{
							// rename key
							dic.Remove(localizationKey);
							dic.Add(newLocalizationKey, localizedData);
						}
					}	break;
					default:
						throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
				}
			}
		}
			
		// save changes
		Save(implGetSpreadsheetOfKey(localizationKey));
		//Save();
		return true;
	}

	public static void ChangeLocalizationKeyValue(string localizationKey, string value)
	{
		m_Dictionary[Language][localizationKey] = value;
			
		//m_Dictionary[c_Spreadsheet].Add(localizationKey, m_DeafultSpreadsheetFolder);
		Save(implGetSpreadsheetOfKey(localizationKey));
		//Save();
	}
	
	private static string implGetSpreadsheetOfKey(string key)
	{
		if (m_Dictionary[c_Spreadsheet].TryGetValue(key, out string value))
			return value;

		return null;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Get localized value by localization key.
	/// </summary>
	public static string Localize(string localizationKey)
	{
/*#if DEBUG
			if (m_Dictionary.ContainsKey(Language) == false)
				Read();

			if (m_Dictionary[Language].ContainsKey(localizationKey) == false)
				m_Dictionary[Language].Add(localizationKey, "None");

			//if (!m_Dictionary.ContainsKey(Language)) throw new KeyNotFoundException("Language not found: " + Language);
			//if (!m_Dictionary[Language].ContainsKey(localizationKey)) throw new KeyNotFoundException("Translation not found: " + localizationKey);
#endif*/

		if (m_Dictionary[Language].TryGetValue(localizationKey, out var data))
			return data;

		return c_DefaultKeyValue;
	}

	/// <summary>
	/// Get localized value by localization key.
	/// </summary>
	public static string Localize(string localizationKey, params object[] args)
	{
		var pattern = Localize(localizationKey);

		return string.Format(pattern, args);
	}

	public static List<string> GetLanguages()
	{
		var result = m_Dictionary.Keys.ToList();
#if UNITY_EDITOR
		result.Remove(c_Spreadsheet);
#endif
		return result;
	}

	public static bool ContainsKey(string key)
	{
		if (m_Dictionary.Count == 0)
			Read();
		return m_Dictionary[c_Spreadsheet].ContainsKey(key);
	}
}