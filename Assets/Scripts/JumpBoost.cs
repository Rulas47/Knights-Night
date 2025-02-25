using UnityEngine;

public class JumpBoost : MonoBehaviour
{    
    public float jumpIncrease = 1f; // Cuánto aumenta la fuerza de salto
    public float boostDuration = 5f; // Duración del boost en segundos

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnightController player = collision.GetComponent<KnightController>();

        if (player != null)
        {
            player.StartJumpBoost(jumpIncrease, boostDuration); // Llama al método en KnightController
            Destroy(gameObject); // Destruye el objeto sin afectar el boost
        }
    }
}