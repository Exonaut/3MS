using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Exo.Events;

public class LinearMover : MonoBehaviour
{
    [FoldoutGroup("Settings", true)][SerializeField] bool startActiveTransition = true;
    [FoldoutGroup("Settings", true)][SerializeField] bool startActiveRotation = true;

    [FoldoutGroup("Translation", true)][SerializeField][Range(0, 100)] private float moveSpeed = 1f;
    [FoldoutGroup("Translation", true)][SerializeField] private Vector3 direction = Vector3.forward;

    [FoldoutGroup("Rotation", true)][SerializeField][Range(0, 10)] private float rotateSpeed = 1f;
    [FoldoutGroup("Rotation", true)][SerializeField] private Vector3 rotateDirection = Vector3.up;

    [FoldoutGroup("Gizmo Settings", true)][SerializeField][Range(1, 10)] private int thickness = 3;

    [FoldoutGroup("Sounds", true)][SerializeField] private AudioSource moveAudioSource;
    [FoldoutGroup("Sounds", true)][SerializeField] private AudioSource rotateAudioSource;

    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook> play;
    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook> stopAll;
    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook> stopTranslate;
    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook> stopRotate;

    bool translate;
    bool rotate;

    private void OnEnable()
    {
        play.ForEach(e => e.AddListener(StartAll));
        stopAll.ForEach(e => e.AddListener(StopAll));
        stopTranslate.ForEach(e => e.AddListener(StopTranslate));
        stopRotate.ForEach(e => e.AddListener(StopRotate));
    }

    private void OnDisable()
    {
        play.ForEach(e => e.RemoveListener(StartAll));
        stopAll.ForEach(e => e.RemoveListener(StopAll));
        stopTranslate.ForEach(e => e.RemoveListener(StopTranslate));
        stopRotate.ForEach(e => e.RemoveListener(StopRotate));
    }

    private void Start()
    {
        if (startActiveTransition)
        {
            StartTranslate();
        }

        if (startActiveRotation)
        {
            StartRotate();
        }
    }

    private void Update()
    {
        if (translate) transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
        if (rotate) transform.Rotate(rotateDirection.normalized * rotateSpeed * Time.deltaTime * 360);
    }

    private void StartRotate()
    {
        rotate = true;
        if (rotateAudioSource != null)
        {
            rotateAudioSource.Play();
        }
    }

    private void StartRotate(MonoBehaviour sender)
    {
        StartRotate();
    }

    private void StartTranslate()
    {
        translate = true;
        if (moveAudioSource != null)
        {
            moveAudioSource.Play();
        }
    }

    private void StartTranslate(MonoBehaviour sender)
    {
        StartTranslate();
    }

    private void StopRotate()
    {
        rotate = false;
        if (rotateAudioSource != null)
        {
            rotateAudioSource.Stop();
        }
    }

    private void StopRotate(MonoBehaviour sender)
    {
        StopRotate();
    }

    private void StopTranslate()
    {
        translate = false;
        if (moveAudioSource != null)
        {
            moveAudioSource.Stop();
        }
    }

    private void StopTranslate(MonoBehaviour sender)
    {
        StopTranslate();
    }

    private void StopAll()
    {
        StopTranslate();
        StopRotate();
    }

    private void StopAll(MonoBehaviour sender)
    {
        StopAll();
    }

    private void StartAll()
    {
        StartTranslate();
        StartRotate();
    }

    private void StartAll(MonoBehaviour sender)
    {
        StartAll();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color color = Color.red;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + direction.normalized * moveSpeed * 10;
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, color, null, thickness);

        color = Color.blue;
        endPosition = transform.position + rotateDirection.normalized * rotateSpeed * 10;
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, color, null, thickness);
    }
#endif
}
