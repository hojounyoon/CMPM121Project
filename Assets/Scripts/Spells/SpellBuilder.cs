using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SpellBuilder 
{
    public Spell Build(SpellCaster owner, string spellType = "arcane_bolt")
    {
        if (owner == null)
        {
            Debug.LogError("SpellBuilder: Owner is null!");
            return null;
        }

        if (string.IsNullOrEmpty(spellType))
        {
            Debug.LogError("SpellBuilder: Spell type is null or empty!");
            return new ArcaneBolt(owner); // Default to ArcaneBolt
        }

        // Split the spell type to check for modifiers
        string[] spellParts = spellType.Split('_');
        
        // The last part is always the base spell
        string baseSpell = spellParts[spellParts.Length - 1];
        
        // Create the base spell
        Spell spell = CreateBaseSpell(owner, baseSpell);
        
        // If there are modifiers, apply them
        if (spellParts.Length > 1)
        {
            string modifier = spellParts[0];
            spell = ApplyModifier(spell, modifier);
        }
        
        return spell;
    }

    private Spell CreateBaseSpell(SpellCaster owner, string baseType)
    {
        switch (baseType)
        {
            case "arcane_bolt":
                return new ArcaneBolt(owner);
            case "magic_missile":
                return new MagicMissile(owner);
            case "arcane_blast":
                return new ArcaneBlast(owner);
            case "arcane_spray":
                return new ArcaneSpray(owner);
            default:
                return new ArcaneBolt(owner); // Default to ArcaneBolt
        }
    }

    private Spell ApplyModifier(Spell baseSpell, string modifier)
    {
        switch (modifier)
        {
            case "damage_amp":
                return new DamageAmplifiedSpell(baseSpell);
            case "speed_amp":
                return new SpeedAmplifiedSpell(baseSpell);
            case "doubler":
                return new DoublerSpell(baseSpell);
            case "splitter":
                return new SplitterSpell(baseSpell);
            case "chaos":
                return new ChaosSpell(baseSpell);
            case "homing":
                return new HomingSpell(baseSpell);
            default:
                return baseSpell;
        }
    }

    public SpellBuilder()
    {        
    }
}
