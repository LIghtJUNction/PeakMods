// Decompiled with JetBrains decompiler
// Type: Zorro.Core.CLI.ConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zorro.Core.CLI.ParsableTypes;
using Zorro.Core.Serizalization;
using Zorro.Core.SmallShadows;

#nullable disable
namespace Zorro.Core.CLI;

public class ConsoleCommands
{
  public static List<ConsoleCommand> ConsoleCommandMethods = new List<ConsoleCommand>()
  {
    new ConsoleCommand(new Action(AchievementManager.ClearAchievements).Method),
    new ConsoleCommand(new Action<ACHIEVEMENTTYPE>(AchievementManager.Grant).Method),
    new ConsoleCommand(new Action<int>(AchievementManager.GrantAscentLevel).Method),
    new ConsoleCommand(new Action(Ascents.LockAll).Method),
    new ConsoleCommand(new Action(Ascents.UnlockAll).Method),
    new ConsoleCommand(new Action(Ascents.UnlockOne).Method),
    new ConsoleCommand(new Action(Backpack.PrintBackpacks).Method),
    new ConsoleCommand(new Action(Character.Die).Method),
    new ConsoleCommand(new Action(Character.GainFullStamina).Method),
    new ConsoleCommand(new Action(Character.InfiniteStamina).Method),
    new ConsoleCommand(new Action(Character.LockStatuses).Method),
    new ConsoleCommand(new Action(Character.PassOut).Method),
    new ConsoleCommand(new Action(Character.Revive).Method),
    new ConsoleCommand(new Action(Character.TestWin).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddCold).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddCurse).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddDrowsy).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddHot).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddHunger).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddInjury).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.AddPoison).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearAll).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearAllAilments).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearCold).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearCurse).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearDrowsy).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearHot).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearHunger).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearInjury).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.ClearPoison).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.Die).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.GetThorned).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.GetUnThorned).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.Hungry).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.Starve).Method),
    new ConsoleCommand(new Action(CharacterAfflictions.TestExactStatus).Method),
    new ConsoleCommand(new Action(CharacterCustomization.Randomize).Method),
    new ConsoleCommand(new Action<Item>(ItemDatabase.Add).Method),
    new ConsoleCommand(new Action<Segment>(MapHandler.JumpToSegment).Method),
    new ConsoleCommand(new Action(PassportManager.TestAllCosmetics).Method),
    new ConsoleCommand(new Action<Player>(Player.PrintInventory).Method),
    new ConsoleCommand(new Action<int>(ApplicationCLI.SetTargetFramerate).Method),
    new ConsoleCommand(new Action(ConsoleSettings.Clear).Method),
    new ConsoleCommand(new Action(ConsoleSettings.ClearMuted).Method),
    new ConsoleCommand(new Action(ConsoleSettings.Pause).Method),
    new ConsoleCommand(new Action<float>(ConsoleSettings.SetDPI).Method),
    new ConsoleCommand(new Action(ConsoleSettings.Unpause).Method),
    new ConsoleCommand(new Func<ScriptPath, Task>(Script.Execute).Method),
    new ConsoleCommand(new Action(IBinarySerializable.EnableLog).Method),
    new ConsoleCommand(new Action(SmallShadowHandler.DebugDisable).Method),
    new ConsoleCommand(new Action(SmallShadowHandler.DebugEnable).Method),
    new ConsoleCommand(new Action(Zorro.UI.Modal.Modal.TestModal).Method)
  };
  public static Dictionary<System.Type, CLITypeParser> TypeParsers = new Dictionary<System.Type, CLITypeParser>()
  {
    {
      typeof (ACHIEVEMENTTYPE),
      (CLITypeParser) new AchievementCLIParser()
    },
    {
      typeof (Item),
      (CLITypeParser) new ItemCLIParser()
    },
    {
      typeof (bool),
      (CLITypeParser) new BoolCLIParser()
    },
    {
      typeof (byte),
      (CLITypeParser) new ByteCLIParser()
    },
    {
      typeof (float),
      (CLITypeParser) new FloatCLIParser()
    },
    {
      typeof (int),
      (CLITypeParser) new IntCLIParser()
    },
    {
      typeof (ScriptPath),
      (CLITypeParser) new ScriptPathCLIParser()
    },
    {
      typeof (ushort),
      (CLITypeParser) new UShortCLIParser()
    }
  };
}
