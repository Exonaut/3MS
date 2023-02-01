using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System;

public class MainMenuUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private UnityEngine.Object startGameScene;

    protected new void Initialize()
    {
        base.Initialize();

        root.Q<Button>("StartGame").clicked += () => OnStartGame();
        root.Q<Button>("Options").clicked += () => OnOptions();
        root.Q<Button>("Controlls").clicked += () => OnControlls();
        root.Q<Button>("Credits").clicked += () => OnCredits();
        root.Q<Button>("EndGame").clicked += () => OnEndGame();
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

    [Hookable] public event Action<MainMenuUI> onStartGame;
    private void OnStartGame()
    {
        logger.Log("OnStartGame clicked");
        onStartGame?.Invoke(this);

        SceneManager.LoadScene("MainLevel");
    }

    [Hookable] public event Action<MainMenuUI> onOptions;
    private void OnOptions()
    {
        logger.Log("OnOptions clicked");
        onOptions?.Invoke(this);

        HideUI(this);
    }

    [Hookable] public event Action<MainMenuUI> onControlls;
    private void OnControlls()
    {
        logger.Log("OnControlls clicked");
        onControlls?.Invoke(this);

        HideUI(this);
    }


    [Hookable] public event Action<MainMenuUI> onCredits;
    private void OnCredits()
    {
        logger.Log("OnCredits clicked");
        onCredits?.Invoke(this);

        HideUI(this);
    }

    [Hookable] public event Action<MainMenuUI> onEndGame;
    private void OnEndGame()
    {
        logger.Log("OnEndGame clicked");
        onEndGame?.Invoke(this);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
