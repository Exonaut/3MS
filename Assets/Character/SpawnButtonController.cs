using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnButtonController : MonoBehaviour
{
    [Hookable] public event Action OnPush;

    private bool isPushed;

    private void Awake()
    {
        isPushed = false;
    }

    public void ResetButton()
    {
        isPushed = false;
        transform.position = new Vector3(transform.position.x, 1.7f, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.   CompareTag("Player") && !isPushed)
        {
            isPushed = true;
            OnPush.Invoke();
            transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        }
    }
}
