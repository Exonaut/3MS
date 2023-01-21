using UnityEngine;


public class HealthPickup : MonoBehaviour
{
    public int restoredHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Hitable>().Heal(restoredHealth);
            Destroy(gameObject);
        }
    }
}
