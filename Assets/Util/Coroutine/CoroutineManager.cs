using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance = null;
    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject inst = new GameObject("CoroutineManager");
                DontDestroyOnLoad(inst);
                instance = inst.AddComponent<CoroutineManager>();
            }
            return instance;
        }
    }
}
