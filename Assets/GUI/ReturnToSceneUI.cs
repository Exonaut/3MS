using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class ReturnToSceneUI : MenuUI
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Object returnScene;

    private new void OnEnable()
    {
        base.OnEnable();

        onReturn += ReturnToMenu;
    }

    private void ReturnToMenu(Object caller)
    {
        if (returnScene != null) SceneManager.LoadScene(returnScene.name);
    }
}
