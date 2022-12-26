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

    new void OnEnable()
    {
        base.OnEnable();

        // Setup Menu buttons
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("StartGame").clicked += () => OnStartGame();
        root.Q<Button>("Options").clicked += () => OnOptions();
        root.Q<Button>("Credits").clicked += () => OnCredits();
        root.Q<Button>("EndGame").clicked += () => OnEndGame();
    }

    [Hookable] public event Action<MonoBehaviour> onStartGame;
    private void OnStartGame()
    {
        logger.Log("OnStartGame clicked");
        onStartGame?.Invoke(this);

        if (startGameScene != null) SceneManager.LoadScene(startGameScene.name);
    }

    [Hookable] public event Action<MonoBehaviour> onOptions;
    private void OnOptions()
    {
        logger.Log("OnOptions clicked");
        onOptions?.Invoke(this);

        HideUI(this);
    }

    [Hookable] public event Action<MonoBehaviour> onCredits;
    private void OnCredits()
    {
        logger.Log("OnCredits clicked");
        onCredits?.Invoke(this);

        // TODO
    }

    [Hookable] public event Action<MonoBehaviour> onEndGame;
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
