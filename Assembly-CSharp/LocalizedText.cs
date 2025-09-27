// Decompiled with JetBrains decompiler
// Type: LocalizedText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

#nullable disable
[ExecuteAlways]
public class LocalizedText : MonoBehaviour
{
  private static Dictionary<string, List<string>> MAIN_TABLE;
  private static Dictionary<string, List<string>> TABLE_PC = new Dictionary<string, List<string>>();
  private static Dictionary<string, List<string>> TABLE_SW = new Dictionary<string, List<string>>();
  private static Dictionary<string, List<string>> TABLE_XB = new Dictionary<string, List<string>>();
  private static Dictionary<string, List<string>> TABLE_PS = new Dictionary<string, List<string>>();
  private static Dictionary<string, List<string>> DIALOGUE_TABLE;
  public static Action OnLangugageChanged;
  public const string MAIN_PATH = "Localization/Localized_Text";
  public const string FULL_PATH = "Assets/Resources/Localization/Localized_Text.csv";
  public const string UNLOCALIZED_PATH = "Localization/Unlocalized_Text";
  public const string FULL_PATH_UNLOCALIZED = "Assets/Resources/Localization/Unlocalized_Text.csv";
  public const string SERIALIZED_TERMS_PATH = "Localization/SerializedTermsData";
  public const string SERIALIZED_DIALOGUE_PATH = "Localization/SerializedDialogueData";
  public const string SERIALIZED_TERMS_PATH_FULL = "Assets/Resources/Localization/SerializedTermsData.txt";
  public const string SERIALIZED_DIALOGUE_PATH_FULL = "Assets/Resources/Localization/SerializedDialogueData.txt";
  public const int LANGUAGE_COUNT = 14;
  public static LocalizedText.Language CURRENT_LANGUAGE = LocalizedText.Language.Japanese;
  public string index;
  public TMP_Text tmp;
  public bool autoSet = true;
  private int row;
  [SerializeField]
  private string currentText;
  public bool useDebugLanguage;
  public LocalizedText.Language debugLanguage;
  public bool tripleIt;
  private const string defaultHeaderName = "Muli";
  private static int lineLength;
  public string addendum;
  public LocalizedText.FontStyle fontStyle;
  public const char UNBREAKABLE_SPACE = ' ';
  private static List<char> unbreakableSpaceRequiredChars = new List<char>()
  {
    '?',
    '!',
    ':',
    ';',
    '"',
    '%'
  };

  public static Dictionary<string, List<string>> mainTable
  {
    get
    {
      if (LocalizedText.MAIN_TABLE == null)
        LocalizedText.LoadMainTable();
      return LocalizedText.MAIN_TABLE;
    }
  }

  public static void TryInitTables()
  {
    if (LocalizedText.MAIN_TABLE == null)
      LocalizedText.LoadMainTable();
    if (LocalizedText.DIALOGUE_TABLE != null)
      return;
    LocalizedText.InitDialogueTable();
  }

  public static Dictionary<string, List<string>> dialogueTable
  {
    get
    {
      if (LocalizedText.DIALOGUE_TABLE == null)
        LocalizedText.InitDialogueTable();
      return LocalizedText.DIALOGUE_TABLE;
    }
  }

  private void OnEnable()
  {
    if (string.IsNullOrEmpty(this.index))
      this.index = this.row.ToString();
    this.TryFindTextAsset();
    if (Application.isPlaying)
      this.InitDisplayType();
    this.RefreshText();
  }

  public void DebugReload()
  {
    LocalizedText.LoadMainTable();
    this.OnEnable();
    LocalizedText.RefreshAllText();
  }

  public static void ReloadAll()
  {
    LocalizedText.LoadMainTable();
    LocalizedText.RefreshAllText();
  }

  private void InitDisplayType()
  {
  }

  [ContextMenu("Debug Serialization")]
  private void DebugSerialization() => LocalizedText.SerializeMainTable();

  private static string SerializeMainTable()
  {
    string contents = JsonConvert.SerializeObject((object) LocalizedText.mainTable);
    File.WriteAllText("Assets/Resources/Localization/SerializedTermsData.txt", contents);
    return contents;
  }

  private static string SerializeDialogueTable()
  {
    string contents = JsonConvert.SerializeObject((object) LocalizedText.dialogueTable);
    File.WriteAllText("Assets/Resources/Localization/SerializedDialogueData.txt", contents);
    return contents;
  }

  public static void LoadMainTable(bool forceSerialization = true)
  {
    if (Application.isEditor & forceSerialization)
    {
      LocalizedText.MAIN_TABLE = CSVReader.SplitCsvDict((UnityEngine.Resources.Load("Localization/Localized_Text") as TextAsset).text);
      TextAsset textAsset = UnityEngine.Resources.Load("Localization/Unlocalized_Text") as TextAsset;
      LocalizedText.MAIN_TABLE = LocalizedText.MAIN_TABLE.Concat<KeyValuePair<string, List<string>>>((IEnumerable<KeyValuePair<string, List<string>>>) CSVReader.SplitCsvDict(textAsset.text)).ToDictionary<KeyValuePair<string, List<string>>, string, List<string>>((Func<KeyValuePair<string, List<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, List<string>>, List<string>>) (x => x.Value));
      LocalizedText.SerializeMainTable();
    }
    else
      LocalizedText.MAIN_TABLE = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>((UnityEngine.Resources.Load("Localization/SerializedTermsData") as TextAsset).text);
    if (!Application.isPlaying)
    {
      using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = LocalizedText.MAIN_TABLE.Keys.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          string current = enumerator.Current;
          LocalizedText.lineLength = LocalizedText.MAIN_TABLE[current].Count;
        }
      }
    }
    LocalizedText.InitPlatformSpecificTables();
  }

  public static void InitDialogueTable(bool forceSerialization = false)
  {
  }

  private static void InitPlatformSpecificTables()
  {
    LocalizedText.TABLE_PC = new Dictionary<string, List<string>>();
    LocalizedText.TABLE_XB = new Dictionary<string, List<string>>();
    LocalizedText.TABLE_SW = new Dictionary<string, List<string>>();
    LocalizedText.TABLE_PS = new Dictionary<string, List<string>>();
    List<string> stringList = new List<string>();
    foreach (string key in LocalizedText.MAIN_TABLE.Keys)
    {
      if (LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_PC, key, LocalizedText.MAIN_TABLE[key], "_PC") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_XB, key, LocalizedText.MAIN_TABLE[key], "_XB") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_SW, key, LocalizedText.MAIN_TABLE[key], "_SW") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_PS, key, LocalizedText.MAIN_TABLE[key], "_PS"))
        stringList.Add(key);
    }
    for (int index = 0; index < stringList.Count; ++index)
      LocalizedText.MAIN_TABLE.Remove(stringList[index]);
  }

  private static bool TryInsertIntoTable(
    Dictionary<string, List<string>> table,
    string i,
    List<string> contents,
    string refToRemove)
  {
    string str1 = i;
    if (!str1.EndsWith(refToRemove))
      return false;
    string str2 = str1.Substring(0, str1.LastIndexOf(refToRemove));
    table.Add(str2.ToUpperInvariant(), contents);
    return true;
  }

  private void TryFindTextAsset() => this.tmp = this.GetComponent<TMP_Text>();

  public void RefreshText()
  {
    if (this.useDebugLanguage && !Application.isPlaying)
      LocalizedText.CURRENT_LANGUAGE = this.debugLanguage;
    if (!(bool) (UnityEngine.Object) this.tmp)
      this.TryFindTextAsset();
    if (this.autoSet)
    {
      this.currentText = this.GetText();
      this.currentText += this.addendum;
      if ((bool) (UnityEngine.Object) this.tmp)
      {
        this.tmp.text = this.currentText;
        if (!Application.isPlaying && this.tripleIt)
          this.tmp.text = this.tmp.text + this.tmp.text + this.tmp.text;
      }
    }
    this.UpdateSpriteAsset();
  }

  private void UpdateSpriteAsset()
  {
  }

  private static string FailsafeParsing(string s)
  {
    s = s.Replace("\"\"", "\"");
    int currentLanguage = (int) LocalizedText.CURRENT_LANGUAGE;
    return s;
  }

  private static string Frenchify(string s)
  {
    char ch;
    for (int index = 0; index < s.Length; ++index)
    {
      if (LocalizedText.unbreakableSpaceRequiredChars.Contains(s[index]))
      {
        if (index == 0)
        {
          string str1 = s;
          int startIndex = index;
          ch = ' ';
          string str2 = ch.ToString();
          s = str1.Insert(startIndex, str2);
        }
        else
        {
          switch (s[index - 1])
          {
            case ' ':
              string str3 = s.Remove(index - 1, 1);
              int startIndex1 = index - 1;
              ch = ' ';
              string str4 = ch.ToString();
              s = str3.Insert(startIndex1, str4);
              continue;
            case ' ':
              continue;
            default:
              string str5 = s;
              int startIndex2 = index - 1;
              ch = ' ';
              string str6 = ch.ToString();
              s = str5.Insert(startIndex2, str6);
              continue;
          }
        }
      }
    }
    return s;
  }

  private static string ReplaceCustomValues(string s) => s;

  public static bool languageSupportsAllCaps
  {
    get
    {
      return LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Russian && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Ukrainian && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.SimplifiedChinese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.TraditionalChinese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Japanese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Korean;
    }
  }

  public string GetText()
  {
    string text = LocalizedText.GetText(this.index);
    return !string.IsNullOrEmpty(text) ? text : "";
  }

  public void SetText(string text)
  {
    if (!(bool) (UnityEngine.Object) this.tmp)
      this.TryFindTextAsset();
    if ((bool) (UnityEngine.Object) this.tmp)
      this.tmp.text = text;
    this.index = "";
  }

  public void SetTextLocalized(string id) => this.SetText(LocalizedText.GetText(id));

  public void SetIndex(string index)
  {
    if (this.index.Equals(index))
      return;
    this.index = index;
    this.RefreshText();
  }

  private static string GetText(int intid) => LocalizedText.GetText(intid.ToString());

  public static string GetText(string id, bool printDebug = true)
  {
    List<string> stringList1 = (List<string>) null;
    id = id.ToUpperInvariant();
    try
    {
      List<string> stringList2;
      if (LocalizedText.mainTable.TryGetValue(id, out stringList2))
        stringList1 = stringList2;
      if (stringList1 != null)
      {
        string text = LocalizedText.FailsafeParsing(stringList1[(int) LocalizedText.CURRENT_LANGUAGE]);
        if (text.IsNullOrEmpty())
          text = LocalizedText.ReplaceCustomValues(LocalizedText.FailsafeParsing(stringList1[0]));
        return text;
      }
      if (printDebug)
        return "LOC: " + id;
      Debug.LogError((object) ("Failed to load text: " + id));
      return "";
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Failed to load text: {id}\n{ex?.ToString()}");
      return "";
    }
  }

  public static string GetText(string id, LocalizedText.Language language)
  {
    try
    {
      return LocalizedText.mainTable[id.ToUpperInvariant()][(int) language];
    }
    catch (Exception ex)
    {
      return "";
    }
  }

  public static string GetText(string id, TextMeshProUGUI text)
  {
    if ((UnityEngine.Object) text != (UnityEngine.Object) null && (UnityEngine.Object) text.GetComponent<LocalizedText>() == (UnityEngine.Object) null)
      text.gameObject.AddComponent<LocalizedText>().autoSet = false;
    return LocalizedText.GetText(id);
  }

  public static string GetText(string id, TextMeshPro text)
  {
    if ((UnityEngine.Object) text != (UnityEngine.Object) null && (UnityEngine.Object) text.GetComponent<LocalizedText>() == (UnityEngine.Object) null)
      text.gameObject.AddComponent<LocalizedText>().autoSet = false;
    return LocalizedText.GetText(id);
  }

  public static string GetText(string id, Text text)
  {
    if ((UnityEngine.Object) text != (UnityEngine.Object) null && (UnityEngine.Object) text.GetComponent<LocalizedText>() == (UnityEngine.Object) null)
      text.gameObject.AddComponent<LocalizedText>().autoSet = false;
    return LocalizedText.GetText(id);
  }

  public static LocalizedText.Language GetSystemLanguage()
  {
    switch (Application.systemLanguage)
    {
      case SystemLanguage.English:
        return LocalizedText.Language.English;
      case SystemLanguage.French:
        return LocalizedText.Language.French;
      case SystemLanguage.German:
        return LocalizedText.Language.German;
      case SystemLanguage.Italian:
        return LocalizedText.Language.Italian;
      case SystemLanguage.Japanese:
        return LocalizedText.Language.Japanese;
      case SystemLanguage.Korean:
        return LocalizedText.Language.Korean;
      case SystemLanguage.Polish:
        return LocalizedText.Language.Polish;
      case SystemLanguage.Portuguese:
        return LocalizedText.Language.BRPortuguese;
      case SystemLanguage.Russian:
        return LocalizedText.Language.Russian;
      case SystemLanguage.Spanish:
        return LocalizedText.Language.SpanishSpain;
      case SystemLanguage.Turkish:
        return LocalizedText.Language.Turkish;
      case SystemLanguage.Ukrainian:
        return LocalizedText.Language.Ukrainian;
      case SystemLanguage.ChineseSimplified:
        return LocalizedText.Language.SimplifiedChinese;
      case SystemLanguage.ChineseTraditional:
        return LocalizedText.Language.SimplifiedChinese;
      default:
        return LocalizedText.Language.English;
    }
  }

  public static void SetLanguageToSystemLanguage()
  {
    LocalizedText.CURRENT_LANGUAGE = LocalizedText.GetSystemLanguage();
  }

  public static void SetLanguage(int languageInt)
  {
    Debug.Log((object) ("Setting language to" + languageInt.ToString()));
    if (languageInt == -1)
      LocalizedText.SetLanguageToSystemLanguage();
    else
      LocalizedText.CURRENT_LANGUAGE = (LocalizedText.Language) languageInt;
    LocalizedText.RefreshAllText();
  }

  public static void RefreshAllText()
  {
    foreach (LocalizedText localizedText in UnityEngine.Object.FindObjectsByType<LocalizedText>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
      localizedText.RefreshText();
    Action langugageChanged = LocalizedText.OnLangugageChanged;
    if (langugageChanged == null)
      return;
    langugageChanged();
  }

  public static void AppendCSVLine(string line, string basicPath, string fullPath)
  {
  }

  public static string GetNameIndex(string displayName) => "NAME_" + displayName;

  public static string GetDescriptionIndex(string displayName) => "DESC_" + displayName;

  public enum Language
  {
    English,
    French,
    Italian,
    German,
    SpanishSpain,
    SpanishLatam,
    BRPortuguese,
    Russian,
    Ukrainian,
    SimplifiedChinese,
    TraditionalChinese,
    Japanese,
    Korean,
    Polish,
    Turkish,
  }

  public enum FontStyle
  {
    Normal,
    Shadow,
    Fuzzy,
    Outline,
    Custom,
  }
}
