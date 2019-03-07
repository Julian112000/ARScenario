using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneState
{
    BuildMode,
    Playing,
    Paused
}
public class SceneHandler : MonoBehaviour
{
    public static SceneState CurrentSceneState = SceneState.BuildMode;
    //
    public delegate void BuildMode();
    public static event BuildMode SwitchToBuildMode;
    //
    public delegate void PlayMode();
    public static event PlayMode SwitchToPlayMode;
    //
    public delegate void PauseMode();
    public static event PauseMode SwitchToPauseMode;

    public static void EnableBuildMode()
    {
        SwitchToBuildMode();
        CurrentSceneState = SceneState.BuildMode;
    }
    public static void EnablePlayMode()
    {
        SwitchToPlayMode();
        CurrentSceneState = SceneState.Playing;
    }
    public static void EnablePauseMode()
    {
        SwitchToPauseMode();
        CurrentSceneState = SceneState.Paused;
    }
}
