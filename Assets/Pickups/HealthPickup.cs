using UnityEngine;


public class HealthPickup : MonoBehaviour, IPickup
{
    public int Amount { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var hitable = other.gameObject.GetComponent<Hitable>();
            if (!hitable.HasFullHealth)
            {
                hitable.Heal(Amount);
                Destroy(gameObject);
            }
        }
    }
}
