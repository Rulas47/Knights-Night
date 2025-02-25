using UnityEngine;
using System.Collections;

public class WolfController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public float fuerzaRebote = 2f;
    public int vida = 1;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private AudioSource audioSource;

    private bool isRunning;
    private bool isDamaged;
    private bool isPlayerAlive;
    private bool isDead;

    void Start()
    {
        isPlayerAlive = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isPlayerAlive && !isDead)
        {
            Movimiento();
        }

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isDead", isDead);
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if(direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if(direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            movement = new Vector2(direction.x, 0);

            isRunning = true;
        }
        else
        {
            movement = Vector2.zero;

            isRunning = false;
        }

        if(!isDamaged)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
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
                isDead = true;
                isRunning = false;
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                audioSource.Play();
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
        yield return new WaitForSeconds(0.4f);
        isDamaged = false;
        rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
