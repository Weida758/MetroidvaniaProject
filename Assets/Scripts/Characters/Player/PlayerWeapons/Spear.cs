using Characters.Player.PlayerWeapons.Data;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Spear", fileName = "Scriptable Objects/SpearWeapon")]
public class SpearWeapon : Weapon
{
    public AttackStep[] attacks;
    public float comboWindow = 0.5f;

    public GameObject spearProjectilePrefab;
    public float throwCooldown = 2f;
    public float aimSlowmo = 0.25f;

    public float baseLungeSpeed = 10f;
    public float lungeHeldTimeMax = 1.5f;
    public float lungeHeldTimeMultiplier = 5f;

    public float lightningChainRadius = 2.5f;

    private float throwCDRemaining;
    private float initialLungeTime;

    public override void OnEquip(Player p)
    {
        base.OnEquip(p);
        throwCDRemaining = 0f;
    }

    public override void WeaponUpdate(ref Player p)
    {
        base.WeaponUpdate(ref p);
        if (throwCDRemaining > 0f) { throwCDRemaining -= Time.deltaTime; }
        
        if (p.lungeTime >= 0)
        {
            p.lungeTime -= Time.deltaTime;
            bool wallHit = Physics2D.Raycast(
                p.rb.transform.position,
                new Vector2(p.getFacingDirection(), 0),
                0.75f,
                1 << LayerMask.NameToLayer("Wall"));
            
            if (p.getGrounded() && (initialLungeTime - p.lungeTime) >= 0.5f || wallHit)
            {
                p.lungeTime = 0;
                p.rb.linearVelocity = new Vector2(0, -2);
            }
        }
    }

    public override bool OnBasicAttack(Player p)
    {
        if (p.actions == null) return false;
        if (p.actions.Current is AttackAction) return false;
        if (attacks == null || attacks.Length == 0) return false;
        p.actions.Enter(new AttackAction(p.actions.machine, p, attacks, comboWindow));
        return true;
    }

    public override bool OnSpecialAttackPressed(Player p)
    {
        if (!ThrowReady) return false;
        if (p.actions == null) return false;
        if (p.actions.Current is AimAction) return false;
        p.actions.Enter(new AimAction(p.actions.machine, p, aimSlowmo));
        return true;
    }

    public override bool OnSpecialAttackReleased(Player p)
    {
        if (!(p.actions.Current is AimAction aim)) return false;

        Vector2 direction = aim.GetAimDirection();
        RaycastHit2D hit = Physics2D.Raycast(
            p.transform.position, direction, 10f);

        StartThrowCooldown();

        if (hit && hit.collider.gameObject.layer ==8)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            var aimEuler = p.aim.transform.eulerAngles;
            var rotation = Quaternion.Euler(aimEuler.x, aimEuler.y, angle);

            var spawned = Instantiate(spearProjectilePrefab, hit.point, rotation);
            p.actions.Enter(new TetherPullAction(
                p.actions.machine, p, hit.point, hit.collider.gameObject, spawned));
        }
        else
        {
            p.actions.ExitToNone();
        }
        return true;
    }

    public override bool OnMovementAbilityPressed(Player p)
    {
        if (!p.getGrounded()) return false;
        if (p.actions == null) return false;
        if (p.actions.Current is SpearLungeAction) return false;
        p.actions.Enter(new SpearLungeAction(p.actions.machine, p));
        return true;
    }

    public override bool OnMovementAbilityReleased(Player p)
    {
        if (!(p.actions.Current is SpearLungeAction)) return false;
        
        if (!p.getGrounded())
        {
            p.lungeHeldTime = 0f;
            p.actions.ExitToNone();
            return true;
        }

        if (p.lungeHeldTime >= lungeHeldTimeMax)
            p.lungeHeldTime = lungeHeldTimeMax;
        
        int totalLungeSpeed = (int)baseLungeSpeed
            + ((int)(p.lungeHeldTime / 0.5f) * (int)lungeHeldTimeMultiplier);

        p.rb.AddForce(
            new Vector2(totalLungeSpeed * p.getFacingDirection(), totalLungeSpeed),
            ForceMode2D.Impulse);

        p.lungeTime = 0.75f + (int)(0.2 * (p.lungeHeldTime / 0.5f));
        initialLungeTime = p.lungeTime;
        p.lungeHeldTime = 0f;
        
        if (p.locomotion != null)
            p.locomotion.machine.ChangeState(p.locomotion.jump);

        p.actions.ExitToNone();
        return true;
    }

    public override bool OnAbility(Player p)
    {
        if (p.SpearEnemy == null) return false;

        
        Vector2 distance = Vector2.zero;
        RaycastHit2D hit = Physics2D.Raycast(
            p.SpearEnemy.transform.position, Vector2.down,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"));
        if (hit)
            distance = (Vector2)hit.point - (Vector2)p.SpearEnemy.transform.position;

        int damage = (int)(((Mathf.Abs(distance.y) / 10) + 1f) * 2f);

        Debug.DrawLine(p.SpearEnemy.transform.position, hit.point, Color.green);
        Debug.Log(damage);
        
        Collider2D[] chain = Physics2D.OverlapCircleAll(
            p.SpearEnemy.transform.position, lightningChainRadius,
            1 << LayerMask.NameToLayer("Enemy"));
        GameObject previous = p.SpearEnemy;
        foreach (Collider2D c in chain)
        {
            if (c.gameObject != previous)
            {
                p.SpearEnemy = c.gameObject;
                Debug.Log("hit");
                // DO NOT UNCOMMENT INFINITE RECURSION ONLY UNCOMMENT AFTER ADDING COOLDOWN
                // OnAbility(p);
            }
        }
        // UNCOMMENT AFTER INFINITE RECURSION FIX
        // p.SpearEnemy = null;

        return true;
    }

    public bool ThrowReady => throwCDRemaining <= 0f;
    public void StartThrowCooldown() => throwCDRemaining = throwCooldown;
}