using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

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

    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook<Object>> play;
    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook<Object>> stopAll;
    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook<Object>> stopTranslate;
    [FoldoutGroup("Event Hooks", true)][SerializeField] List<EventHook<Object>> stopRotate;

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

    private void StartRotate(Object sender = null)
    {
        rotate = true;
        if (rotateAudioSource != null)
        {
            rotateAudioSource.Play();
        }
    }

    private void StartTranslate(Object sender = null)
    {
        translate = true;
        if (moveAudioSource != null)
        {
            moveAudioSource.Play();
        }
    }

    private void StopRotate(Object sender = null)
    {
        rotate = false;
        if (rotateAudioSource != null)
        {
            rotateAudioSource.Stop();
        }
    }

    private void StopTranslate(Object sender = null)
    {
        translate = false;
        if (moveAudioSource != null)
        {
            moveAudioSource.Stop();
        }
    }

    private void StopAll(Object sender = null)
    {
        StopTranslate();
        StopRotate();
    }

    private void StartAll(Object sender = null)
    {
        StartTranslate();
        StartRotate();
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
