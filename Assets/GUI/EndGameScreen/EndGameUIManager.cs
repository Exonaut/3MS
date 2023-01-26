using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EndGameUIManager : MonoBehaviour
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private MenuUI winUI;
    [FoldoutGroup("Dependencies")][SerializeField, Required] private MenuUI looseUI;

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;

        if (GameGlobals.gameWon)
        {
            winUI.ShowUI(this);
            return;
        }

        looseUI.ShowUI(this);

        enabled = false;
    }
}
