// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.DemoVoiceUI.MicRef
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Photon.Voice.Unity.Demos.DemoVoiceUI;

public struct MicRef(MicType micType, DeviceInfo device)
{
  public readonly MicType MicType = micType;
  public readonly DeviceInfo Device = device;

  public override string ToString() => $"Mic reference: {this.Device.Name}";
}
