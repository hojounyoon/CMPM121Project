using UnityEngine;
using System.Collections;

public class MagicMissile : Spell
{
    public MagicMissile(SpellCaster caster) : base(caster)
    {
    }

    public override string GetName()
    {
        return "Magic Missile";
    }

    protected override void InitializeSpell()
    {
        name = "Magic Missile";
        description = "A homing bolt.";
        icon = 2;
        cooldown = 3f;
        mana_cost = "20";
        baseDamage = 10;
        projectile.trajectory = "homing";
        projectile.speedEval = 10f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 3f));
    }
} 