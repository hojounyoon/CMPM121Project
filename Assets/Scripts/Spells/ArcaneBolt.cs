using UnityEngine;
using System.Collections;

public class ArcaneBolt : Spell
{
    public ArcaneBolt(SpellCaster owner) : base(owner)
    {
    }

    protected override void InitializeSpell()
    {
        name = "Arcane Bolt";
        description = "A straight-flying bolt.";
        icon = 0;
        cooldown = 2f;
        mana_cost = "10";
        baseDamage = 25;
        projectile.trajectory = "straight";
        projectile.speedEval = 8f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 5f));
    }

    public override float GetProjectileSpeed()
    {
        return projectile.speedEval * (1 + owner.spellPower / 5f);
    }
} 