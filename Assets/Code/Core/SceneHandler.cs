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
        if(SwitchToBuildMode != null)
        {
            SwitchToBuildMode();
            CurrentSceneState = SceneState.BuildMode;
        }
    }
    public static void EnablePlayMode()
    {
        if(SwitchToPlayMode != null)
        {
            SwitchToPlayMode();
            CurrentSceneState = SceneState.Playing;
        }
    }
    public static void EnablePauseMode()
    {
        if(SwitchToPauseMode != null)
        {
            SwitchToPauseMode();
            CurrentSceneState = SceneState.Paused;
        }
    }
}
