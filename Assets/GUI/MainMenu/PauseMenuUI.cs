using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System;

public class PauseMenuUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private UnityEngine.Object mainMenuScene;

    protected new void Initialize()
    {
        base.Initialize();

        // Setup Menu buttons
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Continue").clicked += () => OnContinue();
        root.Q<Button>("Options").clicked += () => OnOptions();
        root.Q<Button>("End").clicked += () => OnEnd();
        root.Q<Button>("Controlls").clicked += () => OnControlls();
    }

    private new void OnEnable()
    {
        StartCoroutine(InitializeOnNextFrame());
    }

    protected new IEnumerator InitializeOnNextFrame()
    {
        yield return new WaitForEndOfFrame();
        this.Initialize();
    }

    [Hookable] public event Action<PauseMenuUI> onStartGame;
    private void OnContinue()
    {
        logger.Log("OnContinue clicked");
        onStartGame?.Invoke(this);

        HideUI(this);
        FindObjectOfType<InputHandler>().SelectPlayerLayout(this);
    }

    [Hookable] public event Action<PauseMenuUI> onOptions;
    private void OnOptions()
    {
        logger.Log("OnOptions clicked");
        onOptions?.Invoke(this);

        HideUI(this);
    }

    [Hookable] public event Action<PauseMenuUI> onControlls;
    private void OnControlls()
    {
        logger.Log("OnControlls clicked");
        onControlls?.Invoke(this);

        HideUI(this);
    }

    [Hookable] public event Action<PauseMenuUI> onEndGame;
    private void OnEnd()
    {
        logger.Log("OnEnd clicked");
        onEndGame?.Invoke(this);

        SceneManager.LoadScene("MainMenu");
    }
}
