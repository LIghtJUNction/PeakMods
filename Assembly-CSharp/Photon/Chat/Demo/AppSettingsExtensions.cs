// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.AppSettingsExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Realtime;

#nullable disable
namespace Photon.Chat.Demo;

public static class AppSettingsExtensions
{
  public static ChatAppSettings GetChatSettings(this AppSettings appSettings)
  {
    return new ChatAppSettings()
    {
      AppIdChat = appSettings.AppIdChat,
      AppVersion = appSettings.AppVersion,
      FixedRegion = appSettings.IsBestRegion ? (string) null : appSettings.FixedRegion,
      NetworkLogging = appSettings.NetworkLogging,
      Protocol = appSettings.Protocol,
      EnableProtocolFallback = appSettings.EnableProtocolFallback,
      Server = appSettings.IsDefaultNameServer ? (string) null : appSettings.Server,
      Port = (ushort) appSettings.Port,
      ProxyServer = appSettings.ProxyServer
    };
  }
}
