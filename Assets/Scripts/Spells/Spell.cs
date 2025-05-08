using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;
    public string name;
    protected string description;
    protected int icon;
    protected float cooldown;
    protected int manaCost;
    protected int baseDamage;
    protected string projectileTrajectory;
    protected float projectileSpeed;
    protected int projectileSprite;
    protected int N;


    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public Spell(SpellCaster owner, Spell spellInfo)
    {
        this.owner = owner;
        InitializeSpell(spellInfo);
    }
    
    public Spell()
    {

    }

    protected virtual void InitializeSpell(Spell spellInfo)
    {
        //Default values, to be overridden by specific spells
        name = spellInfo.name;
        description = spellInfo.description;
        icon = spellInfo.icon;
        cooldown = spellInfo.cooldown;
        manaCost = spellInfo.manaCost;
        baseDamage = spellInfo.baseDamage;
        projectileTrajectory = spellInfo.projectileTrajectory;
        projectileSpeed = spellInfo.projectileSpeed;
        projectileSprite = spellInfo.projectileSprite;
        N = spellInfo.N;

        
        Debug.Log("spell initalized and info filled");
    }

    protected virtual void InitializeSpell()
    {
        //Default values, to be overridden by specific spells
        
        
        Debug.Log("spell initalized with nothing in it");
    }

    public virtual string GetName()
    {
        return name;
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

    public virtual int GetN()
    {
        return N;
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
