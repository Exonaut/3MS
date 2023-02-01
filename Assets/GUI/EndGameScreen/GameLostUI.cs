using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections;

public class GameLostUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object retryGameScene;
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object returnScene;

    protected new void Initialize()
    {
        base.Initialize();

        root.Q<Button>("Retry").clicked += OnRetryClicked;
        root.Q<Button>("Return").clicked += ReturnToMenu;
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

    private void OnRetryClicked()
    {
        SceneManager.LoadScene("MainLevel");
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
