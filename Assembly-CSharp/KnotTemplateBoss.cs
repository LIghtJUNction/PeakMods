// Decompiled with JetBrains decompiler
// Type: KnotTemplateBoss
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Knot;
using pworld.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class KnotTemplateBoss : MonoBehaviour
{
  public Transform displayRoot;
  public List<KnotTemplate> startTemplates = new List<KnotTemplate>();
  public LinkedList<KnotTemplate> templates = new LinkedList<KnotTemplate>();
  private LinkedListNode<KnotTemplate> current;
  public static KnotTemplateBoss me;

  private void Awake() => KnotTemplateBoss.me = this;

  public LinkedListNode<KnotTemplate> Current
  {
    get => this.current;
    set
    {
      this.displayRoot.KillAllChildren(true);
      this.current = value;
      Object.Instantiate<KnotTemplate>(this.current.Value, this.displayRoot);
    }
  }

  private void Start()
  {
    this.templates = new LinkedList<KnotTemplate>((IEnumerable<KnotTemplate>) this.startTemplates);
    this.Current = this.templates.First;
  }

  public void Next()
  {
    this.Current = this.current.Next != null ? this.Current.Next : this.templates.First;
    Object.FindFirstObjectByType<KnotMaker>().Clear();
    Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
    Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
  }

  public void Previous()
  {
    this.Current = this.Current.Previous != null ? this.current.Previous : this.templates.Last;
    Object.FindFirstObjectByType<KnotMaker>().Clear();
    Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
    Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
  }

  private void Update()
  {
  }
}
