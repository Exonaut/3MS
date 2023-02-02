using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAi : MonoBehaviour
{
    private Transform target;
    private int damage;
    private float homingParameter;
    private bool initialized = false;
    private float fireballSpeed;
    private float lifetime;
    private float initTime;

    public void Initialize(Transform target, int damage, float homingParameter, float fireballSpeed, float lifetime)
    {
        this.target = target;
        this.damage = damage;
        this.homingParameter = homingParameter;
        this.fireballSpeed = fireballSpeed;
        this.lifetime = lifetime;
        transform.forward = target.position - transform.position;
        initTime = Time.time;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            transform.forward = Vector3.Lerp(transform.forward, target.position - transform.position, homingParameter * Time.deltaTime);
            transform.position += fireballSpeed * Time.deltaTime * transform.forward;
            if (Time.time - initTime >= lifetime)
                Destroy(this);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Here");
        if (other.gameObject.CompareTag("Player"))
        {
            var hitable = other.gameObject.GetComponent<Hitable>();
            hitable.Hit(damage);
        }

        if (!other.gameObject.CompareTag("Boss"))
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Here2");
    }
}
