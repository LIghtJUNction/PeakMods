// Decompiled with JetBrains decompiler
// Type: IInteractibleConstant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
public interface IInteractibleConstant : IInteractible
{
  bool IsConstantlyInteractable(Character interactor);

  float GetInteractTime(Character interactor);

  void Interact_CastFinished(Character interactor);

  void CancelCast(Character interactor);

  void ReleaseInteract(Character interactor);

  bool holdOnFinish { get; }
}
