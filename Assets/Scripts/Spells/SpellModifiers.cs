using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;

public class DamageAmplifiedSpell : Spell
{
    private Spell baseSpell;
    private float damageMultiplier = 1.5f;
    private float manaMultiplier = 1.5f;

    public DamageAmplifiedSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    public override int GetDamage()
    {
        return (int)(baseSpell.GetDamage() * damageMultiplier);
    }

    public override int GetManaCost()
    {
        return (int)(GetManaCost() * manaMultiplier);
    }

    // Delegate other methods to base spell
    public override string GetName() => "Damage-Amplified " + baseSpell.GetName();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
    protected override IEnumerator DoCast(Vector3 where, Vector3 target) => baseSpell.Cast(where, target, team);
}

public class SpeedAmplifiedSpell : Spell
{
    private Spell baseSpell;
    private float speedMultiplier = 1.75f;

    public SpeedAmplifiedSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    public override float GetProjectileSpeed()
    {
        return baseSpell.GetProjectileSpeed() * speedMultiplier;
    }

    public override string GetName() => "Speed-Amplified " + baseSpell.GetName();
    public override int GetDamage() => baseSpell.GetDamage();
    public override int GetManaCost() => baseSpell.GetManaCost();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
    protected override IEnumerator DoCast(Vector3 where, Vector3 target) => baseSpell.Cast(where, target, team);
}

public class HeavySpell : Spell
{
    private Spell baseSpell;
    private float speedMultiplier = 0.5f;
    private float damageMultiplier = 1.5f;

    public HeavySpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    public override float GetProjectileSpeed()
    {
        return baseSpell.GetProjectileSpeed() * speedMultiplier;
    }

    public override string GetName() => "Heavy " + baseSpell.GetName();
    public override int GetDamage() => (int)(baseSpell.GetDamage() * damageMultiplier);
    public override int GetManaCost() => baseSpell.GetManaCost();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
    protected override IEnumerator DoCast(Vector3 where, Vector3 target) => baseSpell.Cast(where, target, team);
}

public class DoublerSpell : Spell
{
    private Spell baseSpell;
    private float delay = 0.5f;
    private float manaMultiplier = 1.5f;
    private float cooldownMultiplier = 1.5f;

    public DoublerSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    public override int GetManaCost()
    {
        return (int)(baseSpell.GetManaCost() * manaMultiplier);
    }

    public override float GetCooldown()
    {
        return baseSpell.GetCooldown() * cooldownMultiplier;
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        yield return baseSpell.Cast(where, target, team);
        yield return new WaitForSeconds(delay);
        yield return baseSpell.Cast(where, target, team);
    }

    public override string GetName() => "Doubled " + baseSpell.GetName();
    public override int GetDamage() => baseSpell.GetDamage();
    public override int GetIcon() => baseSpell.GetIcon();
}

public class SplitterSpell : Spell
{
    private Spell baseSpell;
    private float angle = 10f;
    private float manaMultiplier = 1.5f;

    public SplitterSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        // First normal cast
        yield return baseSpell.Cast(where, target, team);

        // Second cast at an angle
        Vector3 direction = (target - where).normalized;
        Vector3 rotatedDirection = Quaternion.Euler(0, 0, angle) * direction;
        Vector3 rotatedTarget = where + rotatedDirection;
        
        yield return baseSpell.Cast(where, rotatedTarget, team);
    }

    public override string GetName() => "Split " + baseSpell.GetName();
    public override int GetDamage() => baseSpell.GetDamage();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
    public override int GetManaCost() => (int)(baseSpell.GetManaCost() * manaMultiplier);
}

public class BounceSpell : Spell
{
    private Spell baseSpell;
    private float damageMultiplier = 1.5f;

    public BounceSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        //yield return baseSpell.Cast(where, target, team);
        GameManager.Instance.projectileManager.CreateProjectile(
            baseSpell.projectileSprite, 
            baseSpell.projectile.trajectory, 
            where, 
            target - where, 
            baseSpell.projectile.speedEval, 
            (Hittable other, Vector3 impact) => {
                // Do the normal hit damage
                if (other.team != team)
                {
                    other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
                    DoCast(Vector3.Lerp(other.owner.transform.position, GameManager.Instance.player.transform.position, 0.2f), GameManager.Instance.player.transform.position);
                    GameManager.Instance.projectileManager.CreateProjectile(
                        baseSpell.projectileSprite, 
                        baseSpell.projectile.trajectory, 
                        Vector3.Lerp(other.owner.transform.position, GameManager.Instance.player.transform.position, 0.2f), 
                        Vector3.Lerp(other.owner.transform.position, GameManager.Instance.player.transform.position, 0.2f) - target, 
                        baseSpell.projectile.speedEval, 
                        (Hittable other, Vector3 impact) => {
                            // Do the normal hit damage
                            if (other.team != team)
                            {
                                other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
                            }
                        }
                    );
                }
            }
        );
        yield return new WaitForEndOfFrame();
        Debug.Log($"speed: {baseSpell.projectile.speedEval}");
        
    }

    public override string GetName() => "Bounce " + baseSpell.GetName();
    public override int GetManaCost() => baseSpell.GetManaCost();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
    public override int GetDamage() => baseSpell.GetDamage();
}

public class ChaosSpell : Spell
{
    private Spell baseSpell;
    private float damageMultiplier = 1.5f;

    public ChaosSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        // First normal cast
        yield return baseSpell.Cast(where, target, team);

        // Second cast with random angle
        float randomAngle = UnityEngine.Random.Range(-45f, 45f);
        Vector3 randomDirection = Quaternion.Euler(0, 0, randomAngle) * (target - where);
        Vector3 randomizedTarget = where + randomDirection;
        
        yield return baseSpell.Cast(where, randomizedTarget, team);
    }

    public override string GetName() => "Chaotic " + baseSpell.GetName();
    public override int GetManaCost() => baseSpell.GetManaCost();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
    public override int GetDamage() => (int)(baseSpell.GetDamage() * damageMultiplier);
}

public class HomingSpell : Spell
{
    private Spell baseSpell;
    private float damageMultiplier = 0.75f;
    private int manaAdder = 10;

    public HomingSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    public override int GetDamage()
    {
        return (int)(baseSpell.GetDamage() * damageMultiplier);
    }

    public override int GetManaCost()
    {
        return baseSpell.GetManaCost() + manaAdder;
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        GameManager.Instance.projectileManager.CreateProjectile(
            baseSpell.projectileSprite,
            "homing",
            where,
            target - where,
            baseSpell.GetProjectileSpeed(),
            OnHit
        );
        yield return new WaitForEndOfFrame();
    }

    public override string GetName() => "Homing " + baseSpell.GetName();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
}

// Similar implementations for other modifiers... 