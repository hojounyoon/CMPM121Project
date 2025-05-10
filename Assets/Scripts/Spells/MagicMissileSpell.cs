using UnityEngine;

public class MagicMissileSpell : Spell
{
    public MagicMissileSpell(SpellCaster caster, Spell data) : base(caster, data)
    {
        // Initialize magic missile specific properties
    }

    public override void Cast(Vector3 position, Vector3 direction)
    {
        // Implement magic missile specific casting behavior
        // For example, create a homing projectile
        base.Cast(position, direction);
    }
} 