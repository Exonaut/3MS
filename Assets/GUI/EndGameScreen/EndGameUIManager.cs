using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EndGameUIManager : MonoBehaviour
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] private MenuUI winUI;
    [FoldoutGroup("Dependencies")][SerializeField, Required] private MenuUI looseUI;

    [FoldoutGroup("Sounds")][SerializeField, Required] private AudioClip looseSound;
    [FoldoutGroup("Sounds")][SerializeField, Required] private AudioClip winSound;

    private void Update()
    {
        if (!enabled) return;

        Cursor.lockState = CursorLockMode.None;

        var audioSource = GetComponent<AudioSource>();

        if (GameGlobals.gameWon)
        {
            winUI.ShowUI(this);
            audioSource.PlayOneShot(winSound);
            Destroy(this);
            return;
        }

        looseUI.ShowUI(this);
        audioSource.PlayOneShot(looseSound);

        Destroy(this);
    }
}
