using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Object = UnityEngine.Object;

public class Timer : MonoBehaviour
{
    [FoldoutGroup("Dependencies", true)][SerializeField, Required] Logger logger;

    [FoldoutGroup("EventHooks", true)][SerializeField] List<EventHook<Object>> startHooks;
    [FoldoutGroup("EventHooks", true)][SerializeField] List<EventHook<Object>> pauseHooks;
    [FoldoutGroup("EventHooks", true)][SerializeField] List<EventHook<Object>> resetHooks;

    [Hookable] public event Action<Timer> OnStart;
    [Hookable] public event Action<Timer> OnEnd;
    [Hookable] public event Action<Timer> OnPause;
    [Hookable] public event Action<Timer> OnReset;

    [FoldoutGroup("Settings", true)][SerializeField, Min(0)] float time = 10;
    [FoldoutGroup("Settings", true)][SerializeField] bool loop;
    [FoldoutGroup("Settings", true)][SerializeField] bool startOnAwake;
    [FoldoutGroup("Settings", true)][SerializeField] bool paused;
    [FoldoutGroup("Settings", true)][SerializeField] bool inverse;

    [FoldoutGroup("Info", true)][SerializeField, ReadOnly, PropertyRange(0, "time")] public float timer;

    private void Start()
    {
        ResetTimer();
    }

    private void Awake()
    {

        if (startOnAwake)
        {
            ResetTimer();
            StartTimer();
        }

        foreach (var hook in startHooks)
        {
            hook.AddListener(StartTimer);
        }

        foreach (var hook in pauseHooks)
        {
            hook.AddListener(PauseTimer);
        }

        foreach (var hook in resetHooks)
        {
            hook.AddListener(ResetTimer);
        }
    }

    private void Update()
    {
        if (paused)
        {
            return;
        }

        if (inverse) timer += Time.deltaTime;
        else timer -= Time.deltaTime;

        if (inverse && timer >= time)
        {
            if (loop)
            {
                timer = 0;
            }
            else
            {
                timer = time;
                paused = true;
            }
            OnEnd.Invoke(this);
        }

        if (!inverse && timer <= 0)
        {
            if (loop)
            {
                timer = time;
            }
            else
            {
                timer = 0;
                paused = true;
            }
            OnEnd.Invoke(this);
        }
    }

    [HorizontalGroup("Functions")]
    [Button("Start")]
    public void StartTimer(Object sender = null)
    {
        paused = false;
        OnStart.Invoke(this);
    }

    [HorizontalGroup("Functions")]
    [Button("Pause")]
    public void PauseTimer(Object sender = null)
    {
        paused = true;
        OnPause.Invoke(this);
    }

    [HorizontalGroup("Functions")]
    [Button("Reset")]
    public void ResetTimer(Object sender = null)
    {
        if (inverse) timer = 0;
        else timer = time;
        OnReset.Invoke(this);
    }

}