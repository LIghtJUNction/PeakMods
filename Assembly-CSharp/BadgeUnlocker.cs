// Decompiled with JetBrains decompiler
// Type: BadgeUnlocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BadgeUnlocker : MonoBehaviour
{
  public int testBadge;
  public bool useTestBadge;
  private Character character;
  public Renderer badgeSashRenderer;

  private void Start() => this.character = this.GetComponent<Character>();

  public void Update()
  {
    if (!this.useTestBadge)
      return;
    int length = GUIManager.instance.mainBadgeManager.badgeData.Length;
    Texture2D texture2D = new Texture2D(length, 1);
    texture2D.filterMode = FilterMode.Point;
    for (int x = 0; x < length; ++x)
      texture2D.SetPixel(x, 1, Color.black);
    texture2D.SetPixel(this.testBadge, 1, Color.white);
    texture2D.Apply();
    this.badgeSashRenderer.materials[0].SetTexture("BadgeUnlockTexture", (Texture) texture2D);
  }

  public static void SetBadges(Character refCharacter, Renderer sashRenderer)
  {
    int length = refCharacter.data.badgeStatus.Length;
    Texture2D texture2D = new Texture2D(length, 1);
    texture2D.filterMode = FilterMode.Point;
    for (int x = 0; x < length; ++x)
    {
      if (refCharacter.data.badgeStatus[x])
      {
        if ((Object) GUIManager.instance.mainBadgeManager.badgeData[x] != (Object) null)
          texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[x].visualID, 1, Color.white);
        else
          texture2D.SetPixel(x, 1, Color.white);
      }
      else if ((Object) GUIManager.instance.mainBadgeManager.badgeData[x] != (Object) null)
        texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[x].visualID, 1, Color.black);
      else
        texture2D.SetPixel(x, 1, Color.black);
    }
    texture2D.Apply();
    if ((Object) sashRenderer == (Object) null)
      return;
    sashRenderer.materials[0].SetTexture("BadgeUnlockTexture", (Texture) texture2D);
  }

  public void BadgeUnlockVisual()
  {
    if (!(bool) (Object) this.character)
      this.character = this.GetComponent<Character>();
    BadgeUnlocker.SetBadges(this.character, this.badgeSashRenderer);
  }
}
