using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGlobals
{
    public static float gravity = 10f;
    public static bool gameWon = false;

    #region Keys

    public static readonly string key_SoundVolume = "SoundVolume";
    public static readonly string key_MouseSensitivity = "MouseSensitivity";
    public static readonly string key_GraphicsQuality = "GraphicsQuality";
    public static readonly string key_Resolution_Width = "Resolution_Width";
    public static readonly string key_Resolution_Height = "Resolution_Height";
    public static readonly string key_Fullscreen = "Fullscreen";

    #endregion
}
