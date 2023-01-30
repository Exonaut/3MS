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

    private new void Start()
    {
        base.Start();

        text = root.Q<Label>("Text");
        text.text = content;
    }
}
