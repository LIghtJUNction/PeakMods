// Decompiled with JetBrains decompiler
// Type: MirrorScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MirrorScript : MonoBehaviour
{
  [Tooltip("Maximum number of per pixel lights that will show in the mirrored image")]
  public int MaximumPerPixelLights = 2;
  [Tooltip("Texture size for the mirror, depending on how close the player can get to the mirror, this will need to be larger")]
  public int TextureSize = 768 /*0x0300*/;
  [Tooltip("Subtracted from the near plane of the mirror")]
  public float ClipPlaneOffset = 0.07f;
  [Tooltip("Far clip plane for mirro camera")]
  public float FarClipPlane = 1000f;
  [Tooltip("What layers will be reflected?")]
  public LayerMask ReflectLayers = (LayerMask) -1;
  [Tooltip("Add a flare layer to the reflection camera?")]
  public bool AddFlareLayer;
  [Tooltip("For quads, the normal points forward (true). For planes, the normal points up (false)")]
  public bool NormalIsForward = true;
  [Tooltip("Aspect ratio (width / height). Set to 0 to use default.")]
  public float AspectRatio;
  [Tooltip("Set to true if you have multiple mirrors facing each other to get an infinite effect, otherwise leave as false for a more realistic mirror effect.")]
  public bool MirrorRecursion;
}
