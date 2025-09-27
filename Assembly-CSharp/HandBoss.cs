// Decompiled with JetBrains decompiler
// Type: HandBoss
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Knot;
using pworld.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class HandBoss : MonoBehaviour
{
  public GameObject grabMaking;
  public GameObject grabUnmaking;
  public GameObject idle;
  public Image handImage;
  public KnotMaker knotMaker;
  public KnotUnmaker knotUnmaker;
  public LineRenderer lr;

  private void Start() => Cursor.visible = false;

  private void DisableAll()
  {
    this.grabMaking.SetActive(false);
    this.grabUnmaking.SetActive(false);
    this.idle.SetActive(false);
    this.lr.gameObject.SetActive(false);
  }

  private void Update()
  {
    this.DisableAll();
    this.transform.position = Input.mousePosition;
    if (this.knotMaker.grabbedRope)
      this.grabMaking.SetActive(true);
    else if (this.knotUnmaker.grabbing)
    {
      this.lr.gameObject.SetActive(true);
      LineRenderer lr1 = this.lr;
      List<TiedKnotVisualizer.KnotPart> knot1 = this.knotUnmaker.visualizer.knot;
      Vector3 position1 = knot1[knot1.Count - 1].position;
      List<TiedKnotVisualizer.KnotPart> knot2 = this.knotUnmaker.visualizer.knot;
      double n1 = (double) knot2[knot2.Count - 1].position.z - 1.0;
      Vector3 position2 = position1.xyn((float) n1);
      lr1.SetPosition(0, position2);
      this.lr.startColor = this.knotUnmaker.lineColor;
      this.lr.endColor = this.knotUnmaker.lineColor;
      this.grabUnmaking.SetActive(true);
      Camera main = Camera.main;
      List<TiedKnotVisualizer.KnotPart> knot3 = this.knotUnmaker.visualizer.knot;
      Vector3 position3 = knot3[knot3.Count - 1].position;
      main.WorldToScreenPoint(position3);
      LineRenderer lr2 = this.lr;
      Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      List<TiedKnotVisualizer.KnotPart> knot4 = this.knotUnmaker.visualizer.knot;
      double n2 = (double) knot4[knot4.Count - 1].position.z - 1.0;
      Vector3 position4 = worldPoint.xyn((float) n2);
      lr2.SetPosition(1, position4);
    }
    else
      this.idle.SetActive(true);
  }
}
