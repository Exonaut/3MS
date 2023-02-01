using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections;

public class ReturnToSceneUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object returnScene;

    protected new void Initialize()
    {
        base.Initialize();

        onReturn += ReturnToMenu;
    }

    private void ReturnToMenu(MenuUI caller)
    {
        SceneManager.LoadScene("MainMenu");
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
}
