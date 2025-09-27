// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.DemoVoiceUI.PhotonDemoExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Realtime;

#nullable disable
namespace Photon.Voice.Unity.Demos.DemoVoiceUI;

public static class PhotonDemoExtensions
{
  internal const string MUTED_KEY = "mu";
  internal const string PHOTON_VAD_KEY = "pv";
  internal const string WEBRTC_AEC_KEY = "ec";
  internal const string WEBRTC_VAD_KEY = "wv";
  internal const string WEBRTC_AGC_KEY = "gc";
  internal const string MIC_KEY = "m";

  public static bool Mute(this Player player)
  {
    Player player1 = player;
    Hashtable propertiesToSet = new Hashtable(1);
    propertiesToSet.Add((object) "mu", (object) true);
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool Unmute(this Player player)
  {
    Player player1 = player;
    Hashtable propertiesToSet = new Hashtable(1);
    propertiesToSet.Add((object) "mu", (object) false);
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool IsMuted(this Player player) => player.HasBoolProperty("mu");

  public static bool SetPhotonVAD(this Player player, bool value)
  {
    Player player1 = player;
    Hashtable propertiesToSet = new Hashtable(1);
    propertiesToSet.Add((object) "pv", (object) value);
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool SetWebRTCVAD(this Player player, bool value)
  {
    Player player1 = player;
    Hashtable propertiesToSet = new Hashtable(1);
    propertiesToSet.Add((object) "wv", (object) value);
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool SetAEC(this Player player, bool value)
  {
    Player player1 = player;
    Hashtable propertiesToSet = new Hashtable(1);
    propertiesToSet.Add((object) "ec", (object) value);
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool SetAGC(this Player player, bool agcEnabled, int gain, int level)
  {
    Player player1 = player;
    Hashtable hashtable = new Hashtable(1);
    hashtable.Add((object) "gc", (object) new object[3]
    {
      (object) agcEnabled,
      (object) gain,
      (object) level
    });
    Hashtable propertiesToSet = hashtable;
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool SetMic(this Player player, Recorder.MicType type)
  {
    Player player1 = player;
    Hashtable propertiesToSet = new Hashtable(1);
    propertiesToSet.Add((object) "m", (object) type);
    return player1.SetCustomProperties(propertiesToSet);
  }

  public static bool HasPhotonVAD(this Player player) => player.HasBoolProperty("pv");

  public static bool HasWebRTCVAD(this Player player) => player.HasBoolProperty("wv");

  public static bool HasAEC(this Player player) => player.HasBoolProperty("ec");

  public static bool HasAGC(this Player player)
  {
    return player.GetObjectProperty("gc") is object[] objectProperty && objectProperty.Length != 0 && (bool) objectProperty[0];
  }

  public static int GetAGCGain(this Player player)
  {
    return !(player.GetObjectProperty("gc") is object[] objectProperty) || objectProperty.Length <= 1 ? 0 : (int) objectProperty[1];
  }

  public static int GetAGCLevel(this Player player)
  {
    return !(player.GetObjectProperty("gc") is object[] objectProperty) || objectProperty.Length <= 2 ? 0 : (int) objectProperty[2];
  }

  public static Recorder.MicType? GetMic(this Player player)
  {
    Recorder.MicType? nullable = new Recorder.MicType?();
    Recorder.MicType? mic;
    try
    {
      mic = new Recorder.MicType?((Recorder.MicType) player.GetObjectProperty("m"));
    }
    catch
    {
      mic = new Recorder.MicType?();
    }
    return mic;
  }

  private static bool HasBoolProperty(this Player player, string prop)
  {
    object obj;
    return player.CustomProperties.TryGetValue((object) prop, out obj) && (bool) obj;
  }

  private static int? GetIntProperty(this Player player, string prop)
  {
    object obj;
    return player.CustomProperties.TryGetValue((object) prop, out obj) ? new int?((int) obj) : new int?();
  }

  private static object GetObjectProperty(this Player player, string prop)
  {
    object obj;
    return player.CustomProperties.TryGetValue((object) prop, out obj) ? obj : (object) null;
  }
}
