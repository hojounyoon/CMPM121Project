using UnityEngine;

public class ArcaneBlastSpell : Spell
{
    public ArcaneBlastSpell(SpellCaster caster, Spell data) : base(caster, data)
    {
        // Initialize arcane blast specific properties
    }

    public override void Cast(Vector3 position, Vector3 direction)
    {
        // Implement arcane blast specific casting behavior
        // For example, create an exploding projectile
        base.Cast(position, direction);
    }
} 