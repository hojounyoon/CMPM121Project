using UnityEngine;
using System.Collections;

public class ArcaneSpray : Spell
{
    public ArcaneSpray(SpellCaster caster) : base(caster)
    {
    }

    public override string GetName()
    {
        return "Arcane Spray";
    }

    protected override void InitializeSpell()
    {
        name = "Arcane Spray";
        description = "A spray of arcane bolts in a cone.";
        icon = 4;
        cooldown = 5f;
        manaCost = 30;
        baseDamage = 8;
        N = 5;
        projectile.trajectory = "spread";
        projectile.speedEval = 12f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 3f));
    }
} 