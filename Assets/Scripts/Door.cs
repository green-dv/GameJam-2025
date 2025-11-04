using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Identificador de la llave necesaria")]
    public string requiredKeyID;

    private bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen) return;

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null && inventory.HasKey(requiredKeyID))
            {
                Debug.Log($" Puerta '{requiredKeyID}' abierta.");
                OpenDoor();
            }
            else
            {
                Debug.Log($" Falta la llave '{requiredKeyID}' para abrir esta puerta.");
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        gameObject.SetActive(false);

    }
}
