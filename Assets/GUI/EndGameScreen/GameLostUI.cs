using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameLostUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object retryGameScene;
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object returnScene;

    private new void OnEnable()
    {
        base.OnEnable();

        root.Q<Button>("Retry").clicked += OnRetryClicked;
        root.Q<Button>("Return").clicked += ReturnToMenu;
    }

    private void OnRetryClicked()
    {
        if (retryGameScene) SceneManager.LoadScene(retryGameScene.name);
    }

    private void ReturnToMenu()
    {
        if (returnScene) SceneManager.LoadScene(returnScene.name);
    }
}
