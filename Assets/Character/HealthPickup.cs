using UnityEngine;


public class HealthPickup : MonoBehaviour
{
    public int restoredHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var hitable = other.gameObject.GetComponent<Hitable>();
            if (!hitable.HasFullHealth)
            {
                hitable.Heal(restoredHealth);
                Destroy(gameObject);
            }
        }
    }
}
