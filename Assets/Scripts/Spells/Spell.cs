using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

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
    protected JObject data;
    public class Projectile
    {
        public string trajectory = "straight";
        public string speed;
        public float speedEval = 0f;
        public int sprite;
    }
    public Projectile projectile = new();

    public class SpellDamage
    {
        public int amountEval = 0;
        public string amount;
        public string type;
    }
    public SpellDamage damage = new();

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
        N = 0;

        // Add debug logging for N value
        Debug.Log($"Initializing spell {spellInfo.name} with N value: {spellInfo.N}");
        
        // override the info(could be done better)
        name = spellInfo.name ?? "No Name";
        description = spellInfo.description ?? "No description";
        icon = spellInfo.icon;
        cooldown = spellInfo.cooldown;
        manaCost = spellInfo.manaCost;
        baseDamage = RPN(spellInfo.damage.amount);
        projectile.trajectory = spellInfo.projectile.trajectory ?? "straight";
        projectile.speedEval = RPN(spellInfo.projectile.speed);
        projectileSprite = spellInfo.projectileSprite;
        
        // Get N value from data if it exists
        if (spellInfo.data != null && spellInfo.data.ContainsKey("N"))
        {
            var nValue = spellInfo.data["N"];
            if (nValue.Type == JTokenType.String)
            {
                N = RPN(nValue.ToString());
                Debug.Log($"Evaluated N value from RPN: {nValue} = {N}");
            }
            else
            {
                N = nValue.Value<int>();
                Debug.Log($"Using direct N value: {N}");
            }
        }
        else
        {
            N = spellInfo.N;
        }

        // Initialize the data field
        data = new JObject();
        data["N"] = N;

        Debug.Log($"Final N value after initialization: {N}");
        Debug.Log($"Data object N value: {data["N"]}");
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
        if (data != null && data.ContainsKey("N"))
        {
            int n = data["N"].Value<int>();
            Debug.Log($"GetN() called for {GetName()}, returning {n}");
            return n;
        }
        Debug.LogWarning($"GetN() called for {GetName()}, but N value not found in spell data");
        return 0;
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

    protected int RPN(string text)
    {
        int val = 0;
        Stack<string> numStack = new Stack<string>();
        string[] tokens = text.Split(' ');
        Debug.Log(owner.spellPower);
        if (tokens.Count() == 1)
        {
            return System.Int32.Parse(tokens[0]); 
        }

        for (int i = 0; i < tokens.Count(); i++) 
        {
            if (tokens[i] == "+" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "power")
                {
                    val2 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {
                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "power")
                {
                    val1 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                Debug.Log($"val1 is: {val1} val 2 is: {val2}");
                val = val1 + val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "-" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "power")
                {
                    val2 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "power")
                {
                    val1 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 - val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "*" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "power")
                {
                    val2 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "power")
                {
                    val1 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 * val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "/" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "power")
                {
                    val2 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "power")
                {
                    val1 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 / val2;
                numStack.Push(val.ToString());
                continue;
            }
            if (tokens[i] == "%" ) {
                int val1 = 0;
                int val2 = 0;
                if(numStack.Peek() == "power")
                {
                    val2 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val2 = System.Int32.Parse(numStack.Pop());
                }
                if(numStack.Peek() == "power")
                {
                    val1 = owner.spellPower + 1;
                    numStack.Pop();
                }
                else
                {

                    val1 = System.Int32.Parse(numStack.Pop());
                }
                val = val1 % val2;
                numStack.Push(val.ToString());
                continue;
            }
            numStack.Push(tokens[i]);
        }
        return val;
     
    }

    public virtual void Cast(Vector3 position, Vector3 direction)
    {
        // Base casting behavior
    }
}
