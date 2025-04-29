using UnityEngine;
using System.Collections;

public class MagicMissile : Spell
{
    public MagicMissile(SpellCaster owner) : base(owner)
    {
    }

    protected override void InitializeSpell()
    {
        spellName = "Magic Missile";
        description = "A homing bolt.";
        icon = 2;
        cooldown = 3f;
        manaCost = 20;
        baseDamage = 10;
        projectileTrajectory = "homing";
        projectileSpeed = 10f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 3f));
    }
} 