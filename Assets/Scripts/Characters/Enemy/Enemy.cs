using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
public class Enemy : MonoBehaviour
{
    [Header("Identity")]
    public string weight;

    [DisplayOnly] public float lightningCooldown;
    [DisplayOnly] public bool isSpeared;
    [DisplayOnly] public bool isFreezed;
    [DisplayOnly] public bool isMarked;
    [DisplayOnly] public bool stunned = false;
    [DisplayOnly] [SerializeField] private string currentState;

    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public HealthSystem health { get; private set; }
    public EnemyBrain brain { get; private set; }
    public ILocomotion locomotion { get; private set; }
    public IPerception perception { get; private set; }
    public IEnemyAttack attack { get; private set; }
    public EnemyAttackTelegraph telegraph { get; private set; }

    [DisplayOnly] public float attackCooldown;
    [DisplayOnly] public float contactGrace;

    private Player player;
    public Transform Target => player != null ? player.transform : null;
    public bool HasTarget => player != null;
    public float DistanceToTarget => player != null ? Vector2.Distance(player.transform.position, transform.position) : Mathf.Infinity;
    public float DirectionToTarget => player != null ? Mathf.Sign(player.transform.position.x - transform.position.x) : FacingDirection;

    public int FacingDirection { get; private set; } = 1;
    public bool CanAct => !isFreezed && !stunned && !isSpeared;
    public bool InAttackRange => attack != null && DistanceToTarget <= attack.Range;
    public bool CanAttack => InAttackRange && attackCooldown <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<HealthSystem>();
        locomotion = GetComponent<ILocomotion>();
        perception = GetComponent<IPerception>();
        attack = GetComponent<IEnemyAttack>();
        telegraph = GetComponent<EnemyAttackTelegraph>();
        brain = GetComponent<EnemyBrain>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        player = Player.instance;
        brain.Begin();
    }

    private void Update()
    {
        if (lightningCooldown > 0f)
        {
            lightningCooldown -= Time.deltaTime;
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (contactGrace > 0f)
        {
            contactGrace -= Time.deltaTime;
        }

        brain.Tick();
        currentState = brain.Current.GetType().Name;
    }

    private void FixedUpdate()
    {
        brain.FixedTick();
    }

    private void HandleDeath()
    {
        Destroy(gameObject, 2f);
    }

    public void SuppressContactDamage(float seconds)
    {
        if (seconds > contactGrace)
        {
            contactGrace = seconds;
        }
    }

    public void SetVelocity(float x, float y)
    {
        rb.linearVelocity = new Vector2(x, y);
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void FaceDirection(float dir)
    {
        if (dir > 0.01f && FacingDirection < 0)
        {
            Flip();
        }
        else if (dir < -0.01f && FacingDirection > 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        FacingDirection *= -1;
    }

    public IEnumerator Stun(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
    }

    public IEnumerator Freeze(float time)
    {
        isFreezed = true;
        yield return new WaitForSeconds(time);
        isFreezed = false;
    }
}
