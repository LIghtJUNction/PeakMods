// Decompiled with JetBrains decompiler
// Type: SteamAuthTicketService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class SteamAuthTicketService : GameService
{
  private Optionable<SteamAuthTicketService.GeneratedTicket> m_currentTicket;
  private const float TICKET_MAX_LIFETIME = 60f;

  public Optionable<SteamAuthTicketService.GeneratedTicket> CurrentTicket => this.m_currentTicket;

  public override void Update()
  {
    base.Update();
    this.VerifyHasValidTicket();
  }

  public void VerifyHasValidTicket()
  {
    if (!this.m_currentTicket.IsNone && (!this.m_currentTicket.IsSome || (double) this.m_currentTicket.Value.TimePassed <= 60.0))
      return;
    this.GenerateNewTicket();
  }

  private void GenerateNewTicket()
  {
    if (this.m_currentTicket.IsSome)
      this.CancelSteamTicket(false);
    (string, HAuthTicket) steamAuthTicket = SteamAuthTicketService.GetSteamAuthTicket();
    this.m_currentTicket = Optionable<SteamAuthTicketService.GeneratedTicket>.Some(new SteamAuthTicketService.GeneratedTicket(steamAuthTicket.Item2, steamAuthTicket.Item1));
    Debug.Log((object) "Generated new ticket");
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    this.CancelSteamTicket(true);
  }

  public void CancelSteamTicket(bool immediate)
  {
    if (!this.m_currentTicket.IsSome)
      return;
    HAuthTicket ticket = this.m_currentTicket.Value.Ticket;
    Debug.Log((object) ("Cancel Steam Auth Ticket: " + this.m_currentTicket.Value.Ticket.ToString()));
    this.m_currentTicket = Optionable<SteamAuthTicketService.GeneratedTicket>.None;
    if (immediate)
      SteamUser.CancelAuthTicket(ticket);
    else
      GameHandler.Instance.StartCoroutine(CancelOverTime());

    IEnumerator CancelOverTime()
    {
      yield return (object) new WaitForSecondsRealtime(5f);
      SteamUser.CancelAuthTicket(ticket);
    }
  }

  public static (string, HAuthTicket) GetSteamAuthTicket()
  {
    byte[] array = new byte[1024 /*0x0400*/];
    CSteamID steamId = SteamUser.GetSteamID();
    SteamNetworkingIdentity pSteamNetworkingIdentity = new SteamNetworkingIdentity();
    pSteamNetworkingIdentity.SetSteamID(steamId);
    uint pcbTicket;
    HAuthTicket authSessionTicket = SteamUser.GetAuthSessionTicket(array, array.Length, out pcbTicket, ref pSteamNetworkingIdentity);
    Array.Resize<byte>(ref array, (int) pcbTicket);
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; (long) index < (long) pcbTicket; ++index)
      stringBuilder.AppendFormat("{0:x2}", (object) array[index]);
    return (stringBuilder.ToString(), authSessionTicket);
  }

  public struct GeneratedTicket(HAuthTicket ticket, string data)
  {
    public string TicketData { get; private set; } = data;

    public HAuthTicket Ticket { get; private set; } = ticket;

    public float TimeCreated { get; private set; } = Time.realtimeSinceStartup;

    public float TimePassed => Time.realtimeSinceStartup - this.TimeCreated;
  }
}
