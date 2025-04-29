using UnityEngine;
using System.Collections;

public class ArcaneBolt : Spell
{
    public ArcaneBolt(SpellCaster owner) : base(owner)
    {
    }

    protected override void InitializeSpell()
    {
        spellName = "Arcane Bolt";
        description = "A straight-flying bolt.";
        icon = 0;
        cooldown = 2f;
        manaCost = 10;
        baseDamage = 25;
        projectileTrajectory = "straight";
        projectileSpeed = 8f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 5f));
    }

    public override float GetProjectileSpeed()
    {
        return projectileSpeed * (1 + owner.spellPower / 5f);
    }
} 