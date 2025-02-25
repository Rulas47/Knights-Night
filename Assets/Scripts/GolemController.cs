using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GolemController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public float fuerzaRebote = 2f;
    public float ataqueDistancia = 2.0f; // Distancia máxima a moverse al atacar
    public int vida = 15;
    public float tiempoAtaque = 0.5f; // Tiempo de duración de la animación de ataque
    public float tiempoEsperaAntesDeAtacar = 1.0f; // Tiempo de espera antes de iniciar el ataque

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private bool isRunning;
    private bool isDamaged;
    private bool isPlayerAlive;
    private bool isDead;
    private bool isAttacking;

    void Start()
    {
        isPlayerAlive = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Llamar al ataque cada 4 segundos
        InvokeRepeating(nameof(Atacar), 4f, 4f);
    }

    void Update()
    {
        if(isPlayerAlive && !isDead && !isAttacking)
        {
            Movimiento();
        }

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isAttacking", isAttacking);
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
                transform.localScale = new Vector3(-8, 8, 1);
            else if (direction.x > 0)
                transform.localScale = new Vector3(8, 8, 1);

            movement = new Vector2(direction.x, 0);
            isRunning = true;
        }
        else
        {
            movement = Vector2.zero;
            isRunning = false;
        }

        if (!isDamaged)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    private void Atacar()
    {
        if (!isDead && isPlayerAlive)
        {
            StartCoroutine(EsperarAntesDeAtacar());
        }
    }

    private IEnumerator EsperarAntesDeAtacar()
    {
        // Esperar X segundos antes de realizar el ataque
        yield return new WaitForSeconds(tiempoEsperaAntesDeAtacar);

        // Ahora proceder con el ataque
        StartCoroutine(RealizarAtaque());
    }

    private IEnumerator RealizarAtaque()
    {
        isAttacking = true;

        // Obtener la dirección del ataque (hacia donde está mirando)
        float direccionAtaque = transform.localScale.x > 0 ? 1 : -1;
        Vector2 destino = new Vector2(rb.position.x + direccionAtaque * ataqueDistancia, rb.position.y);

        // Mover al NPC de forma gradual durante el tiempo del ataque
        float elapsedTime = 0f;
        Vector2 startingPosition = rb.position;

        while (elapsedTime < tiempoAtaque)
        {
            rb.position = Vector2.Lerp(startingPosition, destino, elapsedTime / tiempoAtaque);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarnos de que el NPC llegue exactamente al destino final
        rb.position = destino;

        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            KnightController playerScript = collision.gameObject.GetComponent<KnightController>();

            if (playerScript != null)
            {
                playerScript.ReciveDamage(directionDamage, 1);
                isPlayerAlive = !playerScript.isDead;
                if (!isPlayerAlive)
                    isRunning = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Espada"))
        {
            Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
            ReciveDamage(directionDamage, 1);
        }
    }

    public void ReciveDamage(Vector2 direction, int amountDamage)
    {
        if(!isDamaged)
        {
            vida -= amountDamage;
            isDamaged = true;
            if(vida <= 0)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                isDead = true;
                isRunning = false;
                Invoke("LoadMenuScene", 5f);
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direction.x, 0.3f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(NoReciveDamage());
            }
        }
    }

    IEnumerator NoReciveDamage()
    {
        yield return new WaitForSeconds(0.2f);
        isDamaged = false;
        rb.linearVelocity = Vector2.zero;
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
