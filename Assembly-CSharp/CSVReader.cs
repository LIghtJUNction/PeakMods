// Decompiled with JetBrains decompiler
// Type: CSVReader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#nullable disable
public class CSVReader
{
  public static string[] cachedCsvLineArray = (string[]) null;
  private static char currentParsedCharacter;
  private const char quoteChar = '"';
  private static StringBuilder staticStringBuilder = new StringBuilder();

  public static void DebugOutputGrid(string[,] grid)
  {
    string str = "";
    for (int index1 = 0; index1 < grid.GetUpperBound(1); ++index1)
    {
      for (int index2 = 0; index2 < grid.GetUpperBound(0); ++index2)
        str = str + grid[index2, index1] + "|";
      str += "\n";
    }
  }

  public static Dictionary<string, List<string>> SplitCsvDict(
    string csvText,
    int overrideColumnCount = 0,
    bool debug = false)
  {
    string[,] grid = CSVReader.SplitCsvGrid(csvText, overrideColumnCount: overrideColumnCount);
    Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
    if (debug && Application.isEditor)
      CSVReader.DebugOutputGrid(grid);
    for (int index1 = 0; index1 < grid.GetLength(1); ++index1)
    {
      List<string> stringList = new List<string>();
      if (grid[0, index1] != null)
      {
        string upperInvariant = grid[0, index1].ToUpperInvariant();
        for (int index2 = 1; index2 < grid.GetLength(0); ++index2)
        {
          string.IsNullOrEmpty(grid[index2, index1]);
          stringList.Add(grid[index2, index1]);
        }
        if (!string.IsNullOrEmpty(upperInvariant) && !string.IsNullOrEmpty(upperInvariant) && !dictionary.ContainsKey(upperInvariant))
          dictionary.Add(upperInvariant, stringList);
      }
    }
    return dictionary;
  }

  public static string[,] SplitCsvGrid(string csvText, bool cutQuotes = true, int overrideColumnCount = 0)
  {
    csvText = csvText.Replace("\n", "");
    csvText = csvText.Replace("\\n", "\n");
    string[] strArray1 = csvText.Replace("\r", "").Split(new string[4]
    {
      ",ENDLINE,",
      ",ENDLINE",
      "ENDLINE,",
      "ENDLINE"
    }, StringSplitOptions.None);
    int a = overrideColumnCount;
    CSVReader.cachedCsvLineArray = new string[CSVReader.GetCsvLineLength(strArray1[0])];
    int num = Mathf.Max(a, CSVReader.cachedCsvLineArray.Length);
    string[,] strArray2 = new string[num + 1, strArray1.Length + 1];
    for (int index1 = 0; index1 < strArray1.Length; ++index1)
    {
      CSVReader.SplitCsvLine(strArray1[index1], CSVReader.cachedCsvLineArray, cutQuotes);
      for (int index2 = 0; index2 < num; ++index2)
        strArray2[index2, index1] = CSVReader.cachedCsvLineArray[index2];
    }
    return strArray2;
  }

  public static void SplitCsvLine(string line, string[] output, bool cutQuotes = true)
  {
    int index1 = 0;
    int num1 = -1;
    line = CSVReader.ReplaceUnQuotedChars(line, ',', '|');
    for (int index2 = 0; index2 < line.Length && index1 != output.Length; ++index2)
    {
      if (line[index2].Equals('|'))
      {
        output[index1] = index2 - num1 - 1 != 0 ? line.Substring(num1 + 1, index2 - num1 - 1) : "";
        ++index1;
        num1 = index2;
      }
    }
    int index3 = output.Length - 1;
    if (index1 <= index3)
    {
      output[index3] = line.Substring(num1 + 1, line.Length - num1 - 1);
      int num2 = index1 + 1;
    }
    if (!cutQuotes)
      return;
    for (int index4 = 0; index4 < output.Length; ++index4)
      output[index4] = output[index4].Trim('"');
  }

  public static int GetCsvLineLength(string line)
  {
    line = CSVReader.ReplaceUnQuotedChars(line, ',', '|');
    return line.Split('|', StringSplitOptions.None).Length;
  }

  public static string[] SplitCsvLine(string line, bool cutQuotes = true)
  {
    line = CSVReader.ReplaceUnQuotedChars(line, ',', '|');
    string[] strArray = line.Split('|', StringSplitOptions.None);
    if (cutQuotes)
    {
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = strArray[index].Trim('"');
    }
    return strArray;
  }

  private static string ReplaceUnQuotedChars(string line, char original, char replace)
  {
    CSVReader.staticStringBuilder.Clear();
    bool flag = false;
    for (int index = 0; index < line.Length; ++index)
    {
      CSVReader.currentParsedCharacter = line[index];
      if (CSVReader.currentParsedCharacter.Equals('"'))
        flag = !flag;
      else if (!flag && CSVReader.currentParsedCharacter.Equals(original))
        CSVReader.staticStringBuilder.Append(replace);
      else
        CSVReader.staticStringBuilder.Append(CSVReader.currentParsedCharacter);
    }
    return CSVReader.staticStringBuilder.ToString();
  }
}
