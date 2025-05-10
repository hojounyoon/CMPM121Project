using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class SpellBuilder 
{

    public List<Spell> spellList;

    public void LoadSpells() 
    {
        string FileName = "Assets/Resources/spells_copy.json";
        string JsonString = File.ReadAllText(FileName);
        spellList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Spell>>(JsonString);
        Debug.Log("loading spells");
        Debug.Log(spellList[0]);
    }

    public Spell Build(SpellCaster caster, string spellType)
    {
        LoadSpells();
        Debug.Log("loaded speels");

        Debug.Log($"SpellBuilder.Build called with spellType: {spellType}");

        // Split by space, not underscore
        string[] parts = spellType.Split(' ');
        string baseSpell = parts[parts.Length - 1];
        Spell spell = CreateBaseSpell(caster, baseSpell);
        Debug.Log(spell.GetN());

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
     
        // Find the spell data in the list
        Spell spellData = null;
        foreach (var spell in spellList)
        {
            if (spell.name == baseSpell)
            {
                spellData = spell;
                break;
            }
        }

        if (spellData == null)
        {
            Debug.LogError($"Base spell data not found for {baseSpell}, defaulting to arcane_bolt");
            return new Spell(caster, spellList[0]);
        }

        // Create the appropriate spell type based on the base spell name
        switch (baseSpell)
        {
            case "arcane_bolt":
                return new Spell(caster, spellData);
            case "magic_missile":
                return new MagicMissileSpell(caster, spellData);
            case "arcane_blast":
                return new ArcaneBlastSpell(caster, spellData);
            case "arcane_spray":
                return new ArcaneSpraySpell(caster, spellData);
            default:
                Debug.LogError($"Unknown base spell type: {baseSpell}, defaulting to arcane_bolt");
                return new Spell(caster, spellList[0]);
        }
    }

    private Spell ApplyModifier(Spell baseSpell, string modifier)
    {
        Debug.Log($"Applying modifier {modifier} to {baseSpell.GetName()}");
        switch (modifier)
        {
            case "damage-amplified":
                return new DamageAmplifiedSpell(baseSpell);
            case "speed-amplified":
                return new SpeedAmplifiedSpell(baseSpell);
            case "doubled":
                return new DoublerSpell(baseSpell);
            case "split":
                return new SplitterSpell(baseSpell);
            case "chaotic":
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
        if (spellList == null || spellList.Count == 0)
        {
            Debug.LogError("spellList is empty! Did you call LoadSpells()?");
            return "arcane_bolt";
        }

        // Get all base spells (first 4 spells in the list)
        var baseSpells = spellList.Take(4).ToList();
        if (baseSpells.Count == 0)
        {
            Debug.LogError("No base spells found in spellList!");
            return "arcane_bolt";
        }

        // Pick a random base spell
        string baseSpell = baseSpells[Random.Range(0, baseSpells.Count)].name;
        
        // Get all modifiers (remaining spells in the list)
        var modifiers = spellList.Skip(4).ToList();
        if (modifiers.Count > 0 && Random.value > 0.5f) // 50% chance to add a modifier
        {
            string modifier = modifiers[Random.Range(0, modifiers.Count)].name;
            return $"{modifier} {baseSpell}";
        }

        return baseSpell;
    }

    

    public SpellBuilder()
    {        
    }
}

