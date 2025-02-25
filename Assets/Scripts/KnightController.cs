using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KnightController : MonoBehaviour
{
    public float vida = 5;
    public float velocidad = 5f;
    public Animator animator;
    public float fuerzaSalto = 4f;
    public float fuerzaRebote = 2f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;
    public LayerMask capaEnemigo;
    public float scale;
    public float maxHealth = 5f;

    private bool isGrounded;
    private bool isDamaged;
    private bool isHitting;
    public bool isDead;
    private bool isRolling;

    [SerializeField] private float attackCooldown = 0.5f;
    private bool canAttack = true;

    public float fuerzaRodar = 10f;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(!isDead)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isRolling && !isDamaged)
            {
                StartRolling();
            }
            
            if(!isHitting || !isGrounded || isRolling)
            {
                Movimiento();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);

                isGrounded = hit.collider != null;

                if (isGrounded && Input.GetKeyDown(KeyCode.W) && !isDamaged)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                    audioSource.Play();
                }
            }

            if (Input.GetMouseButtonDown(0) && !isHitting)
            {
                StartHitting();
            }
        }
        else 
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDamaged", isDamaged);
        animator.SetBool("isHitting", isHitting);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isRolling", isRolling);
    }

    public void Movimiento()
{
    float inputX = Input.GetAxis("Horizontal");
    float velocidadX = inputX * velocidad; // Se elimina Time.deltaTime aquí

    animator.SetFloat("movement", Mathf.Abs(velocidadX));

    if (inputX < 0)
    {
        transform.localScale = new Vector3(-1 * scale, 1 * scale, 1);
    }
    if (inputX > 0)
    {
        transform.localScale = new Vector3(1 * scale, 1 * scale, 1);
    }

    if (!isDamaged)
    {
        rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y); // Se usa velocidad en lugar de modificar posición
    }
}

    public void StartRolling()
    {
        isRolling = true;

        float direccion = transform.localScale.x;
        rb.AddForce(new Vector2(direccion * fuerzaRodar, 0), ForceMode2D.Impulse);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

    }

    public void StopRolling()
    {
        isRolling = false;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }

    public void ReciveDamage(Vector2 direction, int amountDamage)
    {
        if(!isDamaged)
        {
            isDamaged = true;
            vida -= amountDamage;
            if(vida <= 0)
            {
                isDead = true;
                Invoke("LoadMenuScene", 5f);
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direction.x, 0.3f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void StartHealing(float healAmount, float duration)
    {
        StartCoroutine(HealOverTime(healAmount, duration));
    }

    private IEnumerator HealOverTime(float healAmount, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            ReceiveHeal(healAmount);
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public void ReceiveHeal(float healAmount)
    {
        if (vida < maxHealth) // Asegura que no supere la vida máxima
        {
            vida = Mathf.Min(vida + healAmount, maxHealth);
        }
    }

    public void StartJumpBoost(float amount, float duration)
    {
        StartCoroutine(JumpBoostCoroutine(amount, duration));
    }

    private IEnumerator JumpBoostCoroutine(float amount, float duration)
    {
        fuerzaSalto += amount; // Aumenta la fuerza de salto
        yield return new WaitForSeconds(duration); // Espera el tiempo del boost
        fuerzaSalto -= amount; // Restaura la fuerza de salto
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void StopReciveDamage()
    {
        isDamaged = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void StartHitting()
    {
        if (!isHitting)
        {
            isHitting = true;
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    void ResetAttack()
    {
        isHitting = false;
        canAttack = true;
    }

    public void StopHitting()
    {
        isHitting = false;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
