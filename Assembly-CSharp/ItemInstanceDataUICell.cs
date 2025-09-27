// Decompiled with JetBrains decompiler
// Type: ItemInstanceDataUICell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.UIElements;

#nullable disable
public class ItemInstanceDataUICell : VisualElement
{
  private ItemInstanceData data;
  private Label label;

  public ItemInstanceDataUICell(ItemInstanceData data)
  {
    this.data = data;
    this.label = new Label();
    this.label.AddToClassList("info");
    this.Add((VisualElement) this.label);
    this.label.text = data.guid.ToString();
  }

  public void Update()
  {
    string str = this.data.guid.ToString() + $" - enteries: {this.data.data.Count}";
    foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in this.data.data)
    {
      str += $"\n{keyValuePair.Key} : {keyValuePair.Value.GetType().Name}";
      str += "\n---";
      str = $"{str}\n{keyValuePair.Value.ToString()}";
      str += "\n---";
    }
    this.label.text = str;
  }
}
