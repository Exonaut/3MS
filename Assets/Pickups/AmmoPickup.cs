using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, IPickup
{
    public int Amount { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var weaponController = other.gameObject.GetComponentInChildren<WeaponController>();
            if (!weaponController.HasFullAmmo)
            {
                weaponController.RestoreAmmo(Amount);
                Destroy(gameObject);
            }
        }
    }
}
