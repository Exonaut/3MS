using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Exo.Events;
using Sirenix.OdinInspector;

public class OptionsMenuUI : MenuUI
{
    new void OnEnable()
    {
        base.OnEnable();

        // Setup Menu buttons
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Return").clicked += () => OnReturn();
    }

    public readonly HookableEvent onReturn = new HookableEvent("onReturn");
    private void OnReturn()
    {
        logger.Log("OnReturn clicked");
        onReturn.Invoke(this);

        HideUI(this);
    }
}
