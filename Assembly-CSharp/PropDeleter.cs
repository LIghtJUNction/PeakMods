// Decompiled with JetBrains decompiler
// Type: PropDeleter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PropDeleter : LevelGenStep
{
  public HelperFunctions.LayerType layerType;
  public float radius = 10f;
  public Transform[] requiredParents;

  private void OnDrawGizmosSelected()
  {
    Gizmos.DrawWireSphere(this.transform.position, this.radius);
  }

  public override void Go()
  {
    foreach (Collider collider in Physics.OverlapSphere(this.transform.position, this.radius, (int) HelperFunctions.GetMask(this.layerType)))
    {
      if (!((Object) collider == (Object) null) && !((Object) collider.gameObject == (Object) null))
      {
        int num = 0;
        Transform transform1 = collider.transform;
        while (num < 5)
        {
          ++num;
          Transform parent = transform1.parent;
          if (!((Object) parent == (Object) null))
          {
            PropGrouper componentInParent = transform1.GetComponentInParent<PropGrouper>();
            if (!((Object) componentInParent == (Object) null))
            {
              Transform transform2 = componentInParent.transform;
              bool flag = false;
              for (int index = 0; index < this.requiredParents.Length; ++index)
              {
                if ((Object) transform2 == (Object) this.requiredParents[index])
                  flag = true;
              }
              if (flag || this.requiredParents.Length == 0)
              {
                if ((bool) (Object) parent.GetComponent<PropSpawner>() || (bool) (Object) parent.GetComponent<PropSpawner_Line>())
                {
                  Object.DestroyImmediate((Object) transform1.gameObject);
                  break;
                }
                transform1 = parent;
              }
              else
                break;
            }
          }
          else
            break;
        }
      }
    }
  }

  public override void Clear()
  {
  }
}
