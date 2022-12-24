using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Exo.Events;
using Sirenix.OdinInspector;
using Unity.UIElements;

public class OptionsMenuUI : MenuUI
{
    private int soundVolume = 0;
    public int b_soundVolume
    {
        get { return soundVolume; }
        set
        {
            if (value == soundVolume) return;

            logger.Log("Sound volume chaged to " + value, this);
            soundVolume = value;
            soundVolumeSlider.value = value;
            PlayerPrefs.SetFloat(GameGlobals.key_SoundVolume, ((float)value) / 100);
        }
    }

    private int mouseSensitivity = 0;
    public int b_mouseSensitivity
    {
        get { return mouseSensitivity; }
        set
        {
            if (value == mouseSensitivity) return;

            logger.Log("Mouse sensitivity changed to " + value, this);
            mouseSensitivity = value;
            mouseSensitivitySlider.value = value;
            PlayerPrefs.SetFloat(GameGlobals.key_MouseSensitivity, ((float)value) / 100);
        }
    }

    private int graphicsQuality = 0;
    public int b_graphicsQuality
    {
        get { return graphicsQuality; }
        set
        {
            if (value == graphicsQuality) return;

            logger.Log("Graphics quality changed to " + value, this);
            graphicsQuality = value;
            graphicsQualitySlider.value = value;
            PlayerPrefs.SetInt(GameGlobals.key_GraphicsQuality, value);
        }
    }

    private SliderInt soundVolumeSlider;
    private SliderInt mouseSensitivitySlider;
    private SliderInt graphicsQualitySlider;

    private void Update()
    {
        if (soundVolumeSlider != null) b_soundVolume = soundVolumeSlider.value;
        if (mouseSensitivitySlider != null) b_mouseSensitivity = mouseSensitivitySlider.value;
        if (graphicsQualitySlider != null) b_graphicsQuality = graphicsQualitySlider.value;
    }

    new void OnEnable()
    {
        base.OnEnable();

        // Setup Menu buttons
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Return").clicked += () => OnReturn();

        soundVolumeSlider = root.Q<VisualElement>("SoundVolume")
            .Q<SliderInt>("SliderInt");
        mouseSensitivitySlider = root.Q<VisualElement>("MouseSensitivity")
            .Q<SliderInt>("SliderInt");
        graphicsQualitySlider = root.Q<VisualElement>("GraphicsQuality")
            .Q<SliderInt>("SliderInt");

        b_soundVolume = (int)(PlayerPrefs.GetFloat(GameGlobals.key_SoundVolume, 0.5f) * 100);
        b_mouseSensitivity = (int)(PlayerPrefs.GetFloat(GameGlobals.key_MouseSensitivity, 0.5f) * 100);
        b_graphicsQuality = (PlayerPrefs.GetInt(GameGlobals.key_GraphicsQuality, 2));
    }

    public readonly HookableEvent onReturn = new HookableEvent("onReturn");
    private void OnReturn()
    {
        logger.Log("OnReturn clicked");
        onReturn.Invoke(this);

        HideUI(this);
    }
}
