// Decompiled with JetBrains decompiler
// Type: ReconnectDataDebugPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
public class ReconnectDataDebugPage : DebugPage
{
  private Label label;

  public ReconnectDataDebugPage()
  {
    this.label = new Label();
    this.label.AddToClassList("info");
    this.Add((VisualElement) this.label);
  }

  public override void Update()
  {
    base.Update();
    string str1;
    if ((UnityEngine.Object) Singleton<ReconnectHandler>.Instance != (UnityEngine.Object) null)
    {
      str1 = $"Reconnect Data found: {Singleton<ReconnectHandler>.Instance.Data.Count}";
      foreach (KeyValuePair<string, Optionable<ReconnectData>> keyValuePair in Singleton<ReconnectHandler>.Instance.Data)
      {
        string str2;
        Optionable<ReconnectData> optionable1;
        keyValuePair.Deconstruct(ref str2, ref optionable1);
        string str3 = str2;
        Optionable<ReconnectData> optionable2 = optionable1;
        str1 += $"{Environment.NewLine}{str3} : {optionable2}";
      }
    }
    else
      str1 = "No reconnect handler found";
    this.label.text = str1;
  }
}
