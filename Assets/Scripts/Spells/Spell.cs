using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

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
    //protected string projectileTrajectory;
    //protected float projectileSpeed;
    protected int projectileSprite;
    protected int N;
    protected class Projectile
    {
        public string trajectory = "straight";
        public string speed;
        public float speedEval = 0f;
        public int sprite;
    }
    protected Projectile projectile = new Projectile();

    public class SpellDamage
    {
        public int amountEval = 0;
        public string amount;
        public string type;
    }
    public SpellDamage damage = new SpellDamage();

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
        name = "Default Spell";
        description = "Default description";
        icon = 0;
        cooldown = 2f;
        manaCost = 10;
        baseDamage = 25;
        projectile.trajectory = "straight";
        projectile.speedEval = 8f;
        projectileSprite = 0;

        // override the info(could be done better)
        name = spellInfo.name ?? "No Name";
        description = spellInfo.description ?? "No description";
        icon = spellInfo.icon;
        cooldown = spellInfo.cooldown;
        manaCost = spellInfo.manaCost;
        baseDamage = spellInfo.damage.amountEval;
        projectile.trajectory = spellInfo.projectile.trajectory ?? "straight";
        projectile.speedEval = 8f;
        projectileSprite = spellInfo.projectileSprite;
        N = spellInfo.N;


        Debug.Log(projectile.trajectory);

        
        
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
            projectile.trajectory, 
            where, 
            target - where, 
            projectile.speedEval, 
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
        return projectile.speedEval;
    }

    protected int RPN()
    {
        return 0;
    }
}
