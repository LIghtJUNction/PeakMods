// Decompiled with JetBrains decompiler
// Type: Knot.KnotMaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#nullable disable
namespace Knot;

public class KnotMaker : MonoBehaviour
{
  public TiedKnotVisualizer visualizer;
  public TextMeshProUGUI scoreText;
  public float score;
  public float minKnotSpacing = 0.01f;
  public int maxPartJumpAllowed = 10;
  public float width = 0.07f;
  [SerializeField]
  private float grabDistance;
  public bool grabbedRope;

  private void Update()
  {
    this.scoreText.text = this.score.ToString();
    if (Input.GetKey(KeyCode.Escape))
      this.Clear();
    if (Input.GetKeyDown(KeyCode.Mouse0) && this.TryGrab())
    {
      this.grabbedRope = true;
      this.TieKnotFillToPoint(Input.mousePosition);
    }
    if (this.grabbedRope)
    {
      this.TieKnotFillToPoint(Input.mousePosition);
      Vector3 position;
      if (this.MouseToPlaneRaycast(out position, Input.mousePosition))
      {
        this.visualizer.knot.Add(new TiedKnotVisualizer.KnotPart(false, position.xyo(), -1));
        this.visualizer.Refresh();
        this.visualizer.knot.RemoveLast<TiedKnotVisualizer.KnotPart>();
      }
    }
    else
      this.visualizer.Refresh();
    if (!Input.GetKeyUp(KeyCode.Mouse0))
      return;
    this.grabbedRope = false;
  }

  private bool TryGrab()
  {
    RaycastHit[] source = Physics.SphereCastAll(Camera.main.ScreenPointToRay(Input.mousePosition), this.width);
    if (this.visualizer.knot.Count == 0)
      return ((IEnumerable<RaycastHit>) source).Any<RaycastHit>((Func<RaycastHit, bool>) (hit => hit.transform.GetSiblingIndex() < this.maxPartJumpAllowed));
    Vector3 position;
    if (this.visualizer.knot.Count > 0 && this.MouseToPlaneRaycast(out position, Input.mousePosition))
    {
      Vector3 a = position.xyo();
      List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
      Vector3 b = knot[knot.Count - 1].position.xyo();
      float num = Vector3.Distance(a, b);
      Debug.Log((object) $"distance: {num}");
      if ((double) num < (double) this.grabDistance)
        return true;
    }
    return false;
  }

  public bool MouseToPlaneRaycast(out Vector3 position, Vector3 mousePosition)
  {
    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    float enter;
    if (new Plane(Camera.main.transform.forward, (UnityEngine.Object) KnotTemplateBoss.me != (UnityEngine.Object) null ? KnotTemplateBoss.me.displayRoot.position : Vector3.zero).Raycast(ray, out enter))
    {
      position = ray.direction * enter + ray.origin;
      return true;
    }
    position = Vector3.zero;
    return false;
  }

  public void Clear()
  {
    this.score = 0.0f;
    this.grabbedRope = false;
    this.visualizer.knot.Clear();
  }

  private void TieKnotFillToPoint(Vector3 mousePosition)
  {
    if (this.visualizer.knot.Count == 0)
    {
      this.TieKnot(mousePosition);
    }
    else
    {
      Camera main = Camera.main;
      List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
      Vector3 position = knot[knot.Count - 1].position;
      Vector3 screenPoint = main.WorldToScreenPoint(position);
      int num = Mathf.Min(Mathf.FloorToInt(Vector3.Distance(screenPoint, mousePosition) / this.minKnotSpacing), 100);
      Vector3 normalized = (screenPoint - mousePosition).normalized;
      for (int index = 0; index < num; ++index)
        this.TieKnot(screenPoint + -normalized * (this.minKnotSpacing * (float) (index + 1)));
    }
  }

  private void TieKnot(Vector3 mousePosition)
  {
    RaycastHit[] source = Physics.SphereCastAll(Camera.main.ScreenPointToRay(mousePosition), this.width);
    if (source.Length != 0)
    {
      int templateProgress = 0;
      if (this.visualizer.count > 0 && this.visualizer.knot.Any<TiedKnotVisualizer.KnotPart>((Func<TiedKnotVisualizer.KnotPart, bool>) (knot => knot.part != -1)))
        templateProgress = this.visualizer.knot.Last<TiedKnotVisualizer.KnotPart>((Func<TiedKnotVisualizer.KnotPart, bool>) (knot => knot.part != -1)).part;
      int hitPart = ((IEnumerable<RaycastHit>) source).ToList<RaycastHit>().OrderBy<RaycastHit, int>((Func<RaycastHit, int>) (hit => Mathf.Abs(hit.transform.GetSiblingIndex() - (templateProgress + 1)))).First<RaycastHit>().collider.transform.GetSiblingIndex();
      int num1 = templateProgress - 1;
      int num2 = templateProgress + this.maxPartJumpAllowed;
      bool quality = true;
      if (hitPart > templateProgress && hitPart < num2)
        templateProgress = hitPart;
      if (hitPart <= num1)
      {
        quality = false;
        hitPart = -1;
        --this.score;
      }
      if (hitPart >= num2)
      {
        quality = false;
        hitPart = -1;
        --this.score;
      }
      AddKnotPositionAtMousePosition(quality, hitPart);
    }
    else
    {
      --this.score;
      AddKnotPositionAtMousePosition(false, -1);
    }

    void AddKnotPositionAtMousePosition(bool quality, int hitPart)
    {
      Vector3 position;
      if (!this.MouseToPlaneRaycast(out position, mousePosition))
        return;
      this.visualizer.knot.Add(new TiedKnotVisualizer.KnotPart(quality, position.xyo(), hitPart));
      Debug.Log((object) $"Quality: {quality}, Position: {position.xyo()}");
    }
  }
}
