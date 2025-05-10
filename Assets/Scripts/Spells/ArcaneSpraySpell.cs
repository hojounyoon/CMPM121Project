using UnityEngine;

public class ArcaneSpraySpell : Spell
{
    public ArcaneSpraySpell(SpellCaster caster, Spell data) : base(caster, data)
    {
        // Initialize arcane spray specific properties
    }

    public override void Cast(Vector3 position, Vector3 direction)
    {
        // Implement arcane spray specific casting behavior
        // For example, create multiple projectiles in a cone
        base.Cast(position, direction);
    }
} 