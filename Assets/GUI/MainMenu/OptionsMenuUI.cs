using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class OptionsMenuUI : MenuUI
{
    private int soundVolume = 0;
    public int b_soundVolume
    {
        get { return soundVolume; }
        set
        {
            if (soundVolume == value) return;

            soundVolume = value;
            float volume = ((float)value) / 100;
            PlayerPrefs.SetFloat(GameGlobals.key_SoundVolume, volume);
            AudioListener.volume = volume;

            if (soundVolumeSlider == null) return;
            soundVolumeSlider.value = value;
            soundVolumeSlider.MarkDirtyRepaint();

            if (logger) logger.Log("Sound volume: " + value, this);
        }
    }

    private int mouseSensitivity = 0;
    public int b_mouseSensitivity
    {
        get { return mouseSensitivity; }
        set
        {
            if (mouseSensitivity == value) return;

            mouseSensitivity = value;
            PlayerPrefs.SetFloat(GameGlobals.key_MouseSensitivity, ((float)value) / 100);

            if (mouseSensitivitySlider == null) return;
            mouseSensitivitySlider.value = value;
            mouseSensitivitySlider.MarkDirtyRepaint();

            if (logger) logger.Log("Mouse sensitivity: " + value, this);
        }
    }

    private int graphicsQuality = 0;
    public int b_graphicsQuality
    {
        get { return graphicsQuality; }
        set
        {
            if (graphicsQuality == value) return;

            graphicsQuality = value;
            PlayerPrefs.SetInt(GameGlobals.key_GraphicsQuality, value);
            QualitySettings.SetQualityLevel(value);

            if (graphicsQualityDropdown == null) return;
            graphicsQualityDropdown.index = value;
            graphicsQualityDropdown.MarkDirtyRepaint();

            if (logger) logger.Log("Graphics quality: " + value, this);
        }
    }

    private Resolution resolution = new Resolution();
    public Resolution b_resolution
    {
        get { return resolution; }
        set
        {
            if (resolution.width == value.width && resolution.height == value.height) return;

            resolution = value;
            PlayerPrefs.SetInt(GameGlobals.key_Resolution_Width, resolution.width);
            PlayerPrefs.SetInt(GameGlobals.key_Resolution_Height, resolution.height);

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            if (resolutionDropdown == null) return;
            resolutionDropdown.index = resolutionChoices.IndexOf(string.Format("{0}x{1}", value.width, value.height));
            resolutionDropdown.MarkDirtyRepaint();

            if (logger) logger.Log(string.Format("Resolution: {0}x{1}", value.width, value.height), this);
        }
    }

    private int fullscreen = -1;
    public int b_fullscreen
    {
        get { return fullscreen; }
        set
        {
            if (fullscreen == value) return;

            fullscreen = value;
            PlayerPrefs.SetInt(GameGlobals.key_Fullscreen, value);
            bool fs = fullscreen == 0 ? false : true;
            Screen.fullScreen = fs;

            if (fullscreenToggle == null) return;
            fullscreenToggle.value = fs;
            fullscreenToggle.MarkDirtyRepaint();

            if (logger) logger.Log(string.Format("Fullscreen: {0}", fs), this);
        }
    }

    private SliderInt soundVolumeSlider;
    private SliderInt mouseSensitivitySlider;
    private DropdownField graphicsQualityDropdown;
    private DropdownField resolutionDropdown;
    private Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<string> resolutionChoices = new List<string>();

    private List<string> qualityChoices = new List<string>();

    protected new void Update()
    {
        base.Update();

        if (soundVolumeSlider != null) b_soundVolume = soundVolumeSlider.value;
        if (mouseSensitivitySlider != null) b_mouseSensitivity = mouseSensitivitySlider.value;
        if (graphicsQualityDropdown != null) b_graphicsQuality = graphicsQualityDropdown.index;
        if (resolutionDropdown != null
            && resolutionDropdown.index < resolutions.Length
            && resolutionDropdown.index >= 0) b_resolution = resolutions[resolutionDropdown.index];
        if (fullscreenToggle != null) b_fullscreen = fullscreenToggle.value ? 1 : 0;
    }

    private new void OnEnable()
    {
        fullscreen = -1;
        resolution = new Resolution();
        soundVolume = -1;
        mouseSensitivity = -1;
        graphicsQuality = -1;

        // Apply player prefs
        b_soundVolume = (int)(PlayerPrefs.GetFloat(GameGlobals.key_SoundVolume, 0.5f) * 100);
        b_mouseSensitivity = (int)(PlayerPrefs.GetFloat(GameGlobals.key_MouseSensitivity, 0.5f) * 100);
        b_graphicsQuality = PlayerPrefs.GetInt(GameGlobals.key_GraphicsQuality, QualitySettings.names.Length - 1);

        var res = new Resolution();
        res.width = PlayerPrefs.GetInt(GameGlobals.key_Resolution_Width, Screen.currentResolution.width);
        res.height = PlayerPrefs.GetInt(GameGlobals.key_Resolution_Height, Screen.currentResolution.height);
        b_resolution = res;

        b_fullscreen = PlayerPrefs.GetInt(GameGlobals.key_Fullscreen, 1);

        StartCoroutine(InitializeOnNextFrame());
    }

    protected new IEnumerator InitializeOnNextFrame()
    {
        yield return new WaitForEndOfFrame();
        this.Initialize();
    }

    protected new void Initialize()
    {
        base.Initialize();

        fullscreen = -1;
        resolution = new Resolution();
        soundVolume = -1;
        mouseSensitivity = -1;
        graphicsQuality = -1;

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

        // Resolution setup
        resolutionDropdown = root.Q<VisualElement>("Resolution")
            .Q<DropdownField>("Dropdown");
        resolutions = Screen.resolutions;
        resolutionChoices.Clear();
        foreach (var resolution in resolutions)
        {
            resolutionChoices.Add(string.Format("{0}x{1}", resolution.width, resolution.height));
        }
        resolutionDropdown.choices = resolutionChoices;

        //Fullscreen toggle
        fullscreenToggle = root.Q<VisualElement>("Fullscreen")
            .Q<Toggle>("Toggle");

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
}
