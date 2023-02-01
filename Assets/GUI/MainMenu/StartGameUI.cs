using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class StartGameUI : MenuUI
{
    [FoldoutGroup("Text", expanded: true)]
    [FoldoutGroup("Text")][SerializeField, TextArea(8, 16)] protected string content;

    Label text;

    protected new void Initialize()
    {
        base.Initialize();

        text = root.Q<Label>("Text");
        text.text = content;
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
