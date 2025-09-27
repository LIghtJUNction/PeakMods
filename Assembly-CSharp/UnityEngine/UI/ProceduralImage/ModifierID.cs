// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.ProceduralImage.ModifierID
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace UnityEngine.UI.ProceduralImage;

[AttributeUsage(AttributeTargets.Class)]
public class ModifierID : Attribute
{
  private string name;

  public ModifierID(string name) => this.name = name;

  public string Name => this.name;
}
