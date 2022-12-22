using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Exo.Events;
using Sirenix.OdinInspector;

public class MainMenuUI : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Logger logger;

    private void Start()
    {
        logger = logger == null ? Logger.GetDefaultLogger(this) : logger;
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button btnStartGame = root.Q<Button>("StartGame");
        Button btnOptions = root.Q<Button>("Options");
        Button btnEndGame = root.Q<Button>("EndGame");

        btnStartGame.clicked += () => OnStartGame();
        btnOptions.clicked += () => OnOptions();
        btnEndGame.clicked += () => OnEndGame();
    }

    public readonly HookableEvent onStartGame = new HookableEvent("onStartGame");
    private void OnStartGame()
    {
        logger.Log("OnStartGame clicked");
        onStartGame.Invoke(this);

        // TODO
    }

    public readonly HookableEvent onOptions = new HookableEvent("onOptions");
    private void OnOptions()
    {
        logger.Log("OnOptions clicked");
        onOptions.Invoke(this);

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
