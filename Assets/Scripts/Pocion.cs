using UnityEngine;

public class Pocion : MonoBehaviour
{
    public float healAmount = 0.5f; // Cantidad de vida que restaura por segundo
    public float healDuration = 6f; // Duración total de la curación

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnightController player = collision.GetComponent<KnightController>();

        if (player != null)
        {
            player.StartHealing(healAmount, healDuration); // Activa la curación en el jugador
            Destroy(gameObject); // La poción desaparece, pero la curación sigue
        }
    }
}
