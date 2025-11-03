using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Identificador único de llave y puerta que abre")]
    public string keyID;

    [Header("Efectos opcionales")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.AddKey(keyID);
                Debug.Log($" Llave '{keyID}' recogida.");
            }

            if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);
            if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject); //  La llave desaparece
        }
    }
}
