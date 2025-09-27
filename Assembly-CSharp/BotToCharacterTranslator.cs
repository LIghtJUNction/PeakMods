// Decompiled with JetBrains decompiler
// Type: BotToCharacterTranslator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BotToCharacterTranslator : MonoBehaviour
{
  private Character character;
  private Bot bot;

  private void Awake()
  {
    this.character = this.GetComponentInParent<Character>();
    this.bot = this.GetComponentInParent<Bot>();
  }

  private void Update()
  {
    this.character.input.movementInput = this.bot.MovementInput;
    this.character.input.sprintIsPressed = this.bot.IsSprinting;
    this.character.data.lookValues = (Vector2) HelperFunctions.DirectionToLook(this.bot.LookDirection);
  }
}
