using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoRestored;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var weaponController = other.gameObject.GetComponentInChildren<WeaponController>();
            if (!weaponController.HasFullAmmo)
            {
                weaponController.RestoreAmmo(ammoRestored);
                Destroy(gameObject);
            }
        }
    }
}
