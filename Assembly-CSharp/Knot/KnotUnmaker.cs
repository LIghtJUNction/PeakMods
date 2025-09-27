// Decompiled with JetBrains decompiler
// Type: Knot.KnotUnmaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Knot;

public class KnotUnmaker : MonoBehaviour
{
  public TiedKnotVisualizer visualizer;
  public float grabDistance = 0.05f;
  public float dragAngle = 45f;
  public float eraseSpeed = 0.1f;
  private float elapsed;
  public bool grabbing;
  public Color lineColor;
  public Color good;
  public Color bad;
  public float minDistToDrag = 1f;
  public float maxDistToDrag = 10f;

  public void Update()
  {
    Vector3 position1;
    this.MouseToPlaneRaycast(out position1);
    if (this.visualizer.knot == null || this.visualizer.knot.Count == 0)
    {
      this.grabbing = false;
      this.elapsed = 0.0f;
    }
    else
    {
      if (Input.GetKeyDown(KeyCode.Mouse1))
      {
        List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
        float num = Vector3.Distance(knot[knot.Count - 1].position, position1);
        Debug.Log((object) $"Try Erase Grab {num} < {this.grabDistance}");
        if ((double) num < (double) this.grabDistance)
        {
          Debug.Log((object) "Grabbing");
          this.grabbing = true;
        }
      }
      if (Input.GetKey(KeyCode.Mouse1) && this.grabbing)
      {
        if (this.visualizer.knot.Count < 2)
        {
          this.visualizer.knot.Clear();
          this.grabbing = false;
          return;
        }
        Vector3 vector3_1 = position1.xyo();
        List<TiedKnotVisualizer.KnotPart> knot1 = this.visualizer.knot;
        Vector3 vector3_2 = knot1[knot1.Count - 1].position.xyo();
        Vector3 from = vector3_1 - vector3_2;
        Vector3 vector3_3 = Vector3.zero;
        int num1 = Mathf.Min(10, this.visualizer.knot.Count) - 1;
        for (int index = 0; index < num1; ++index)
        {
          Vector3 vector3_4 = vector3_3;
          List<TiedKnotVisualizer.KnotPart> knot2 = this.visualizer.knot;
          Vector3 vector3_5 = knot2[knot2.Count - (index + 2)].position.xyo();
          List<TiedKnotVisualizer.KnotPart> knot3 = this.visualizer.knot;
          Vector3 vector3_6 = knot3[knot3.Count - 1].position.xyo();
          Vector3 vector3_7 = vector3_5 - vector3_6;
          vector3_3 = vector3_4 + vector3_7;
        }
        Vector3 to = vector3_3 / (float) num1;
        float num2 = Vector3.Angle(from, to);
        // ISSUE: variable of a boxed type
        __Boxed<float> local = (ValueType) num2;
        List<TiedKnotVisualizer.KnotPart> knot4 = this.visualizer.knot;
        // ISSUE: variable of a boxed type
        __Boxed<bool> quality = (ValueType) knot4[knot4.Count - 1].quality;
        Debug.Log((object) $"try Erasing, angle: {local}, Erase Speed {quality}");
        this.lineColor = Color.Lerp(this.good, this.bad, Mathf.InverseLerp(0.0f, this.dragAngle, num2));
        if ((double) num2 < (double) this.dragAngle)
        {
          double num3 = (double) (this.eraseSpeed * (1f - Mathf.InverseLerp(0.0f, this.dragAngle, num2)));
          double minDistToDrag = (double) this.minDistToDrag;
          double maxDistToDrag = (double) this.maxDistToDrag;
          List<TiedKnotVisualizer.KnotPart> knot5 = this.visualizer.knot;
          double num4 = (double) Vector3.Distance(knot5[knot5.Count - 1].position, position1);
          double num5 = (double) Mathf.InverseLerp((float) minDistToDrag, (float) maxDistToDrag, (float) num4);
          float num6 = (float) (num3 * num5);
          double elapsed = (double) this.elapsed;
          double deltaTime = (double) Time.deltaTime;
          List<TiedKnotVisualizer.KnotPart> knot6 = this.visualizer.knot;
          double num7 = knot6[knot6.Count - 1].quality ? (double) num6 : (double) num6 * 0.10000000149011612;
          double num8 = deltaTime * num7;
          this.elapsed = (float) (elapsed + num8);
          List<TiedKnotVisualizer.KnotPart> knot7 = this.visualizer.knot;
          Vector3 position2 = knot7[knot7.Count - 2].position;
          List<TiedKnotVisualizer.KnotPart> knot8 = this.visualizer.knot;
          Vector3 position3 = knot8[knot8.Count - 1].position;
          float num9 = Vector3.Distance(position2, position3);
          if ((double) this.elapsed > (double) num9)
          {
            Debug.Log((object) $"Removing endKnot, knotDist: {num9}");
            this.visualizer.knot.RemoveAt(this.visualizer.knot.Count - 1);
            this.visualizer.Refresh();
            this.elapsed -= num9;
          }
        }
      }
      if (!Input.GetKeyUp(KeyCode.Mouse1))
        return;
      Debug.Log((object) "Stop Grabbing");
      this.grabbing = false;
    }
  }

  public bool MouseToPlaneRaycast(out Vector3 position)
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    float enter;
    if (new Plane(Camera.main.transform.forward, (UnityEngine.Object) KnotTemplateBoss.me != (UnityEngine.Object) null ? KnotTemplateBoss.me.displayRoot.position : Vector3.zero).Raycast(ray, out enter))
    {
      position = ray.direction * enter + ray.origin;
      return true;
    }
    position = Vector3.zero;
    return false;
  }
}
