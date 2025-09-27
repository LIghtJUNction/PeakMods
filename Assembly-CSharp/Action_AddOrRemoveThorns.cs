// Decompiled with JetBrains decompiler
// Type: Action_AddOrRemoveThorns
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Action_AddOrRemoveThorns : ItemAction
{
  public int thornCount;
  public bool specificBodyPart;
  public BodypartType location;
  public Vector3 minOffset;
  public Vector3 maxOffset;

  public override void RunAction()
  {
    int thornCount = this.thornCount;
    if (thornCount > 0)
    {
      for (; thornCount > 0; --thornCount)
      {
        if (this.specificBodyPart)
        {
          Vector3 vector = Vector3.Lerp(this.minOffset, this.maxOffset, Random.Range(0.0f, 1f));
          Transform transform = this.character.GetBodypart(this.location).transform;
          this.character.refs.afflictions.AddThorn(transform.position + transform.TransformVector(vector));
        }
        else
          this.character.refs.afflictions.AddThorn();
      }
    }
    else
    {
      for (; thornCount < 0; ++thornCount)
        this.character.refs.afflictions.RemoveRandomThornLinq();
    }
  }
}
