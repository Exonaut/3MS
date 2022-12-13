using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Exo.Events;

public class Timer : MonoBehaviour
{
    [FoldoutGroup("Dependencies", true)][SerializeField, Required] Logger logger;

    [FoldoutGroup("EventHooks", true)][SerializeField] List<EventHook> startHooks;
    [FoldoutGroup("EventHooks", true)][SerializeField] List<EventHook> pauseHooks;
    [FoldoutGroup("EventHooks", true)][SerializeField] List<EventHook> resetHooks;

    [HideInInspector] public HookableEvent OnStart = new HookableEvent("Start");
    [HideInInspector] public HookableEvent OnEnd = new HookableEvent("End");
    [HideInInspector] public HookableEvent OnPause = new HookableEvent("Pause");
    [HideInInspector] public HookableEvent OnReset = new HookableEvent("Reset");

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

        foreach (EventHook hook in startHooks)
        {
            hook.AddListener(StartTimer);
        }

        foreach (EventHook hook in pauseHooks)
        {
            hook.AddListener(PauseTimer);
        }

        foreach (EventHook hook in resetHooks)
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
    public void StartTimer()
    {
        paused = false;
        OnStart.Invoke(this);
    }

    public void StartTimer(MonoBehaviour sender)
    {
        StartTimer();
    }

    [HorizontalGroup("Functions")]
    [Button("Pause")]
    public void PauseTimer()
    {
        paused = true;
        OnPause.Invoke(this);
    }

    public void PauseTimer(MonoBehaviour sender)
    {
        PauseTimer();
    }

    [HorizontalGroup("Functions")]
    [Button("Reset")]
    public void ResetTimer()
    {
        if (inverse) timer = 0;
        else timer = time;
        OnReset.Invoke(this);
    }

    public void ResetTimer(MonoBehaviour sender)
    {
        ResetTimer();
    }

}