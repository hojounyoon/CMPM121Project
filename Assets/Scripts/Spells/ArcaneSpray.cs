using UnityEngine;
using System.Collections;

public class ArcaneSpray : Spell
{
    public ArcaneSpray(SpellCaster caster) : base(caster)
    {
        // Initialize arcane spray specific properties
    }

    public override string GetName()
    {
        return "Arcane Spray";
    }

    protected override void InitializeSpell()
    {
        spellName = "Arcane Spray";
        description = "A spray of arcane bolts in a cone.";
        icon = 4;
        cooldown = 5f;
        manaCost = 30;
        baseDamage = 8;
        projectileTrajectory = "spread";
        projectileSpeed = 12f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 3f));
    }
} 