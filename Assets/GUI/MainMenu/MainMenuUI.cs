using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Exo.Events;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class MainMenuUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object startGameScene;

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

    public readonly HookableEvent onStartGame = new HookableEvent("onStartGame");
    private void OnStartGame()
    {
        logger.Log("OnStartGame clicked");
        onStartGame.Invoke(this);

        if (startGameScene != null) SceneManager.LoadScene(startGameScene.name);
    }

    public readonly HookableEvent onOptions = new HookableEvent("onOptions");
    private void OnOptions()
    {
        logger.Log("OnOptions clicked");
        onOptions.Invoke(this);

        HideUI(this);
    }

    public readonly HookableEvent onCredits = new HookableEvent("onOptions");
    private void OnCredits()
    {
        logger.Log("OnCredits clicked");
        onCredits.Invoke(this);

        // TODO
    }

    public readonly HookableEvent onEndGame = new HookableEvent("onEndGame");
    private void OnEndGame()
    {
        logger.Log("OnEndGame clicked");
        onEndGame.Invoke(this);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
