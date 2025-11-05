using System.Collections;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    [Header("Componentes")]
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private Transform jugador;

    [Header("Stats")]
    public float velocidad = 2f;
    public float fuerzaSalto = 7f;
    public float distanciaAtaque = 4f;
    public float stoppingDistance = 0.5f;
    private bool mirandoDerecha = true;
    private bool atacando = false;

    [Header("Supervivencia / Cronómetro")]
    public float tiempoTotal = 180f;
    public float incrementoVelocidadCada = 30f;
    public float factorAumentoVelocidad = 1.1f;
    private float tiempoTranscurrido = 0f;
    private bool desapareciendo = false;

    [Header("UI")]
    public TextMeshProUGUI textoCronometro;

    [Header("Ataque Especial")]
    public GameObject proyectilPrefab;
    public Transform puntoSpawnArriba;
    public float tiempoEntreLluvias = 0.5f;
    public int cantidadLluvias = 5;
    public float tiempoCooldownHabilidad = 6f;
    private bool puedeUsarHabilidad = true;

    [Header("Daño por contacto")]
    public int contactDamage = 1;
    public float contactDamageCooldown = 1f;
    private bool puedeHacerDanio = true;

    [Header("Detección de suelo")]
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundMask;
    private bool estaEnSuelo = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(TiempoSupervivencia());
    }

    void Update()
    {
        if (jugador == null || desapareciendo) return;

        CheckGround();
        MirarJugador();

        float distancia = Vector2.Distance(jugador.position, transform.position);

        if (!atacando)
        {
            if (distancia <= distanciaAtaque && puedeUsarHabilidad)
            {
                float chance = Random.value;
                if (chance > 0.5f)
                {
                    StartCoroutine(UsarHabilidadEspecial());
                    return;
                }
            }

            MoverYSeguirJugador(distancia);
        }

        animator.SetFloat("VelocidadY", rb.velocity.y);
    }

    private void CheckGround()
    {
        Vector2 origin = new Vector2(transform.position.x, col.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundDistance, groundMask);
        estaEnSuelo = hit.collider != null;
        animator.SetBool("Grounded", estaEnSuelo);
    }

    private void MoverYSeguirJugador(float distancia)
    {
        float dir = Mathf.Sign(jugador.position.x - transform.position.x);
        float alturaRelativa = jugador.position.y - transform.position.y;

        if (estaEnSuelo || rb.velocity.y < 0)
        {
            if (distancia > stoppingDistance)
            {
                animator.SetBool("Walking", true);
                rb.velocity = new Vector2(dir * velocidad, rb.velocity.y);
            }
            else
            {
                animator.SetBool("Walking", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if (alturaRelativa > 1.5f && estaEnSuelo)
            {
                Saltar();
            }
        }
    }

    private void Saltar()
    {
        animator.SetTrigger("Jumping");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        estaEnSuelo = false;
    }

    private void MirarJugador()
    {
        if ((jugador.position.x > transform.position.x && !mirandoDerecha) ||
            (jugador.position.x < transform.position.x && mirandoDerecha))
        {
            mirandoDerecha = !mirandoDerecha;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private IEnumerator UsarHabilidadEspecial()
    {
        atacando = true;
        puedeUsarHabilidad = false;
        animator.SetTrigger("Special");

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < cantidadLluvias; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(jugador.position.x - 5f, jugador.position.x + 5f),
                puntoSpawnArriba.position.y,
                0
            );
            Instantiate(proyectilPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(tiempoEntreLluvias);
        }

        atacando = false;
        yield return new WaitForSeconds(tiempoCooldownHabilidad);
        puedeUsarHabilidad = true;
    }

    private IEnumerator TiempoSupervivencia()
    {
        float tiempoRestante = tiempoTotal;
        float tiempoSiguienteAumento = incrementoVelocidadCada;

        while (tiempoRestante > 0)
        {
            if (textoCronometro != null)
            {
                int minutos = Mathf.FloorToInt(tiempoRestante / 60);
                int segundos = Mathf.FloorToInt(tiempoRestante % 60);
                textoCronometro.text = $"⏱ {minutos:00}:{segundos:00}";
                textoCronometro.color = (tiempoRestante <= 10) ? Color.red : Color.white;
            }

            tiempoTranscurrido += 1f;
            if (tiempoTranscurrido >= tiempoSiguienteAumento)
            {
                velocidad *= factorAumentoVelocidad;
                tiempoSiguienteAumento += incrementoVelocidadCada;
            }

            yield return new WaitForSeconds(1f);
            tiempoRestante -= 1f;
        }

        StartCoroutine(DesaparecerBoss());
    }

    private IEnumerator DesaparecerBoss()
    {
        desapareciendo = true;
        animator.SetTrigger("Muerte");
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puedeHacerDanio)
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(contactDamage);
                StartCoroutine(ContactDamageCooldown());
            }
        }
    }

    private IEnumerator ContactDamageCooldown()
    {
        puedeHacerDanio = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        puedeHacerDanio = true;
    }
}
