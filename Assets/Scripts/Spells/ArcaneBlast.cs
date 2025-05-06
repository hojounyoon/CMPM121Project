using UnityEngine;
using System.Collections;

public class ArcaneBlast : Spell
{
    public ArcaneBlast(SpellCaster caster) : base(caster)
    {
        // Initialize arcane blast specific properties
    }

    public override string GetName()
    {
        return "Arcane Blast";
    }

    protected override void InitializeSpell()
    {
        spellName = "Arcane Blast";
        description = "A short-range blast of arcane energy.";
        icon = 3;
        cooldown = 4f;
        manaCost = 25;
        baseDamage = 15;
        projectileTrajectory = "straight";
        projectileSpeed = 8f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 3f));
    }
} 