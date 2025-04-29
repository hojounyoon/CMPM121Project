using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;
    protected string spellName;
    protected string description;
    protected int icon;
    protected float cooldown;
    protected int manaCost;
    protected int baseDamage;
    protected string projectileTrajectory;
    protected float projectileSpeed;
    protected int projectileSprite;

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
        InitializeSpell();
    }

    protected virtual void InitializeSpell()
    {
        // Default values, to be overridden by specific spells
        spellName = "Default Spell";
        description = "Default description";
        icon = 0;
        cooldown = 2f;
        manaCost = 10;
        baseDamage = 25;
        projectileTrajectory = "straight";
        projectileSpeed = 8f;
        projectileSprite = 0;
    }

    public virtual string GetName()
    {
        return spellName;
    }

    public virtual int GetManaCost()
    {
        return manaCost;
    }

    public virtual int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 100f));
    }

    public virtual float GetCooldown()
    {
        return cooldown;
    }

    public virtual int GetIcon()
    {
        return icon;
    }

    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;
        yield return DoCast(where, target);
    }

    protected virtual IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        GameManager.Instance.projectileManager.CreateProjectile(
            projectileSprite, 
            projectileTrajectory, 
            where, 
            target - where, 
            projectileSpeed, 
            OnHit
        );
        yield return new WaitForEndOfFrame();
    }

    protected virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }
    }

    public virtual float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
