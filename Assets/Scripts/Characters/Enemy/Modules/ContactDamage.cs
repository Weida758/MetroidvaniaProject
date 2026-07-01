using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ContactDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private Collider2D bodyCollider;

    private Enemy enemy;
    private ContactFilter2D filter;
    private readonly List<Collider2D> overlaps = new List<Collider2D>();
    private float cooldownTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        filter = new ContactFilter2D();
        filter.SetLayerMask(1 << LayerMask.NameToLayer("Player"));
        filter.useTriggers = true;
    }

    private void Update()
    {
        if (enemy.isSpeared || enemy.contactGrace > 0f)
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (bodyCollider.Overlap(filter, overlaps) == 0)
        {
            return;
        }

        HealthSystem health = overlaps[0].GetComponentInParent<HealthSystem>();
        if (health == null)
        {
            return;
        }

        health.TakeDamage(damage);
        cooldownTimer = damageInterval;
    }
}
