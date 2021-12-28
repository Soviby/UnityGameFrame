using System;
using System.Collections;
using CC.Console;
using UnityEngine;


public class GMManager : Singleton<GMManager>
{
    protected override void OnInitialized()
    {
        AddGMs();
    }

    private void AddGMs()
    {
        ConsoleCommands.AddCommand("Home", GameSceneManager.Instance.GMJumpMainScene, "Jump to the home page");
        ConsoleCommands.AddCommand("JumpScene", GameSceneManager.Instance.GMJumpScene, "Jump scene");
        ConsoleCommands.AddCommand("StopBGM", SoundManager.GMBMGStop, "Stop the BGM");
        ConsoleCommands.AddCommand("PlayBGM", SoundManager.GMBMGPlay, "Play the BGM");
        ConsoleCommands.AddCommand("PlayEffect", SoundManager.GMEffectPlay, "Play sound effects");
    }


}