using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float enemyHealth = 100f;
    private GameObject player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;  // Adicionando referência ao Animator

    public float followDistance = 20f;
    public float attackDistance = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isWalking = false;  // Para controlar a animação de andar
    // private bool isDead = false;  // Para controlar a animação de morte

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();  // Obtendo o componente Animator

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no inimigo!");
        }
    }

    void Update()
    {
        // Verificar a distância entre o inimigo e o jogador
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // Se o agente não está no NavMesh, não podemos fazer ele parar, pois isso causa o erro.
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("O agente não está em um NavMesh válido!");
            return;
        }

        // Se o inimigo estiver dentro da distância para seguir, ele vai seguir o jogador
        if (distanceToPlayer <= followDistance ) //&& !isDead
        {
            FollowPlayer();

            if (distanceToPlayer <= attackDistance)
            {
                AttackPlayer();
            }
        }
        else
        {
            // Se o inimigo não estiver perto o suficiente, ele vai parar
            StopMovement();
        }

        // Lógica para ataque, controle do cooldown
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
        }

        if (attackTimer >= attackCooldown)
        {
            isAttacking = false;
            attackTimer = 0f;
        }

        // Atualizar as animações com base nos estados
        UpdateAnimations();
    }

    void FollowPlayer()
    {
        // Ativa a animação de caminhada
        isWalking = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.transform.position);
    }

    void StopMovement()
    {
        // Para o movimento do agente
        isWalking = false;
        navMeshAgent.isStopped = true;
    }

    void AttackPlayer()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log("Inimigo atacou o jogador!");
        }
    }

    // Atualizar as animações de acordo com os estados
    void UpdateAnimations()
    {
        // Enviar os parâmetros para o Animator
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isAttacking", isAttacking);
        // animator.SetBool("isDead", isDead);
    }

    // Função de dano
    public void Damage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    // Função de morte do inimigo
    void Die()
    {
        // if (isDead) return;

        // isDead = true;
        // animator.SetBool("isDead", true);  // Aciona a animação de morte
        navMeshAgent.isStopped = true;  // Para o agente de navegação
        Debug.Log($"{enemyName} morreu!");
        Destroy(gameObject);  // Destroi o objeto depois de 3 segundos
    }

    void OnTriggerEnter(Collider obj)
    {
        Bullet bullet = obj.GetComponent<Bullet>();
        if (bullet != null)
        {
            Damage(bullet.GetDamage());  // Agora chamando Damage()
            Destroy(obj.gameObject);
        }
    }
}
