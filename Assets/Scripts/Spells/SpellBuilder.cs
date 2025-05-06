using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SpellBuilder 
{
    public Spell Build(SpellCaster caster, string spellType)
    {
        Debug.Log($"SpellBuilder.Build called with spellType: {spellType}");

        // Split by space, not underscore
        string[] parts = spellType.Split(' ');
        string baseSpell = parts[parts.Length - 1];
        Spell spell = CreateBaseSpell(caster, baseSpell);

        // Apply modifiers in order (if any)
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string modifier = parts[i];
            Debug.Log($"Applying modifier: {modifier}");
            spell = ApplyModifier(spell, modifier);
        }

        Debug.Log($"Final spell created: {spell.GetName()}");
        return spell;
    }

    private Spell CreateBaseSpell(SpellCaster caster, string baseSpell)
    {
        Debug.Log($"Creating base spell: {baseSpell}");
        switch (baseSpell)
        {
            case "arcane_bolt":
                return new ArcaneBolt(caster);
            case "magic_missile":
                return new MagicMissile(caster);
            case "arcane_blast":
                return new ArcaneBlast(caster);
            case "arcane_spray":
                return new ArcaneSpray(caster);
            default:
                Debug.LogError($"Unknown base spell type: {baseSpell}");
                return new ArcaneBolt(caster); // Fallback
        }
    }

    private Spell ApplyModifier(Spell baseSpell, string modifier)
    {
        Debug.Log($"Applying modifier {modifier} to {baseSpell.GetName()}");
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
                Debug.LogError($"Unknown modifier: {modifier}");
                return baseSpell;
        }
    }

    public string GenerateRandomSpell()
    {
        // You may want to pass in the arrays or make them static/shared
        string[] baseSpells = { "arcane_bolt", "magic_missile", "arcane_blast", "arcane_spray" };
        string[] modifierSpells = { "damage_amp", "speed_amp", "doubler", "splitter", "chaos", "homing" };

        string baseSpell = baseSpells[Random.Range(0, baseSpells.Length)];
        string spellType = baseSpell;

        // 50% chance to add a random modifier (or more, if you want)
        if (Random.value < 0.5f)
        {
            string modifier = modifierSpells[Random.Range(0, modifierSpells.Length)];
            spellType = modifier + " " + baseSpell;
        }

        return spellType;
    }

    public SpellBuilder()
    {        
    }
}
