using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class OptionsMenuUI : MenuUI
{
    private int soundVolume = 0;
    public int b_soundVolume
    {
        get { return soundVolume; }
        set
        {
            if (value == soundVolume) return;

            logger.Log("Sound volume: " + value, this);
            soundVolume = value;
            soundVolumeSlider.value = value;
            float volume = ((float)value) / 100;
            PlayerPrefs.SetFloat(GameGlobals.key_SoundVolume, volume);
            AudioListener.volume = volume;
        }
    }

    private int mouseSensitivity = 0;
    public int b_mouseSensitivity
    {
        get { return mouseSensitivity; }
        set
        {
            if (value == mouseSensitivity) return;

            logger.Log("Mouse sensitivity: " + value, this);
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
            graphicsQualityDropdown.index = value;

            if (value == graphicsQuality) return;

            logger.Log("Graphics quality: " + value, this);
            graphicsQuality = value;
            graphicsQualityDropdown.index = value;
            PlayerPrefs.SetInt(GameGlobals.key_GraphicsQuality, value);
            QualitySettings.SetQualityLevel(value);
        }
    }

    private Resolution resolution = new Resolution();
    public Resolution b_resolution
    {
        get { return resolution; }
        set
        {
            resolutionDropdown.index = resolutionChoices.IndexOf(string.Format("{0}x{1}", value.width, value.height));

            if (resolution.width == value.width && resolution.height == value.height) return;

            resolution = value;
            PlayerPrefs.SetInt(GameGlobals.key_Resolution_Width, resolution.width);
            PlayerPrefs.SetInt(GameGlobals.key_Resolution_Height, resolution.height);

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            logger.Log(string.Format("Resolution: {0}x{1}", value.width, value.height), this);
        }
    }

    private int fullscreen = -1;
    public int b_fullscreen
    {
        get { return fullscreen; }
        set
        {
            if (value == fullscreen) return;
            fullscreen = value;

            bool fs = value == 0 ? false : true;

            PlayerPrefs.SetInt(GameGlobals.key_Fullscreen, value);
            Screen.fullScreen = fs;

            logger.Log(string.Format("Fullscreen: {0}", fs), this);
        }
    }

    private SliderInt soundVolumeSlider;
    private SliderInt mouseSensitivitySlider;

    private DropdownField graphicsQualityDropdown;
    private DropdownField resolutionDropdown;

    private Resolution[] resolutions;
    private List<string> resolutionChoices = new List<string>();

    private List<string> qualityChoices = new List<string>();

    private void Update()
    {
        if (soundVolumeSlider != null) b_soundVolume = soundVolumeSlider.value;
        if (mouseSensitivitySlider != null) b_mouseSensitivity = mouseSensitivitySlider.value;
        if (graphicsQualityDropdown != null) b_graphicsQuality = graphicsQualityDropdown.index;
        if (resolutionDropdown != null)
        {
            b_resolution = resolutions[resolutionDropdown.index];
        }
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

        // Graphics quality setup
        graphicsQualityDropdown = root.Q<VisualElement>("GraphicsQuality")
            .Q<DropdownField>("Dropdown");
        qualityChoices = new List<string>(QualitySettings.names);
        graphicsQualityDropdown.choices = qualityChoices;

        // Resolution setuo
        resolutionDropdown = root.Q<VisualElement>("Resolution")
            .Q<DropdownField>("Dropdown");
        resolutions = Screen.resolutions;
        resolutionChoices.Clear();
        foreach (var resolution in resolutions)
        {
            resolutionChoices.Add(string.Format("{0}x{1}", resolution.width, resolution.height));
        }
        resolutionDropdown.choices = resolutionChoices;

        // Apply player prefs
        b_soundVolume = (int)(PlayerPrefs.GetFloat(GameGlobals.key_SoundVolume, 0.5f) * 100);
        b_mouseSensitivity = (int)(PlayerPrefs.GetFloat(GameGlobals.key_MouseSensitivity, 0.5f) * 100);
        b_graphicsQuality = PlayerPrefs.GetInt(GameGlobals.key_GraphicsQuality, QualitySettings.names.Length - 1);

        var res = new Resolution();
        res.width = PlayerPrefs.GetInt(GameGlobals.key_Resolution_Width, Screen.currentResolution.width);
        res.height = PlayerPrefs.GetInt(GameGlobals.key_Resolution_Height, Screen.currentResolution.height);
        b_resolution = res;

        b_fullscreen = PlayerPrefs.GetInt(GameGlobals.key_Fullscreen, 1);
    }

    [Hookable] public event Action<OptionsMenuUI> onReturn;
    private void OnReturn()
    {
        logger.Log("OnReturn clicked");
        onReturn?.Invoke(this);

        HideUI(this);
    }
}
