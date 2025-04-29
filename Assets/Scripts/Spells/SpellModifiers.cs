using UnityEngine;
using System.Collections;

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
        return (int)(baseSpell.GetManaCost() * manaMultiplier);
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

    public override int GetManaCost()
    {
        return (int)(baseSpell.GetManaCost() * manaMultiplier);
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        Vector3 direction = (target - where).normalized;
        Vector3 rotatedDirection = Quaternion.Euler(0, 0, angle) * direction;
        Vector3 rotatedTarget = where + rotatedDirection;

        yield return baseSpell.Cast(where, target, team);
        yield return baseSpell.Cast(where, rotatedTarget, team);
    }

    public override string GetName() => "Split " + baseSpell.GetName();
    public override int GetDamage() => baseSpell.GetDamage();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
}

public class ChaosSpell : Spell
{
    private Spell baseSpell;
    private float damageMultiplier = 1.5f;

    public ChaosSpell(Spell spell) : base(spell.owner)
    {
        this.baseSpell = spell;
    }

    public override int GetDamage()
    {
        return (int)(baseSpell.GetDamage() * (1 + Mathf.Sin(Time.time * 5) * 0.5f));
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        GameManager.Instance.projectileManager.CreateProjectile(
            baseSpell.GetIcon(),
            "spiraling",
            where,
            target - where,
            baseSpell.GetProjectileSpeed(),
            OnHit
        );
        yield return new WaitForEndOfFrame();
    }

    public override string GetName() => "Chaotic " + baseSpell.GetName();
    public override int GetManaCost() => baseSpell.GetManaCost();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override int GetIcon() => baseSpell.GetIcon();
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
            baseSpell.GetIcon(),
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