using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painable : MonoBehaviour
{
    [FoldoutGroup("Pain")][PropertyRange(0, 1)][Tooltip("Liklihood of getting stunned after a hit.")] public float painThreshold;
    [FoldoutGroup("Pain")][MinValue(0)] public float painLength;

    public bool IsInPain { get; private set; }

    private bool gotHitThisFrame;
    private float lastPainTime;

    void Start()
    {
        gotHitThisFrame = false;
        GetComponent<Hitable>().onDamage += _ => gotHitThisFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gotHitThisFrame)
        {
            gotHitThisFrame = false;
            if (Random.Range(0f, 1f) < painThreshold)
            {
                lastPainTime = Time.time;
                IsInPain = true;
            }
        }
        if (Time.time - lastPainTime >= painLength)
            IsInPain = false;
    }
}
