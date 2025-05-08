using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        switch (baseSpell)
        {
            // case "arcane_bolt":
            //     return new ArcaneBolt(caster);
            // case "magic_missile":
            //     return new MagicMissile(caster);
            // case "arcane_blast":
            //     return new ArcaneBlast(caster);
            // case "arcane_spray":
            //     return new ArcaneSpray(caster);
            // default:
            //     Debug.LogError($"Unknown base spell type: {baseSpell}");
            //     return new ArcaneBolt(caster); // Fallback
        }
        return new Spell(caster, spellList[1]);
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
        if (spellList == null || spellList.Count == 0)
        {
            Debug.LogError("spellList is empty! Did you call LoadSpells()?");
            return "arcane_bolt";
        }

        // Get all spell names from spellList
        List<string> allSpellNames = new List<string>();
        foreach (var spell in spellList)
        {
            // Use the correct property for the spell name (e.g., spell.name or spell.id)
            allSpellNames.Add(spell.name); // or spell.id if that's what you use
        }

        // Pick a random spell name
        string randomName = allSpellNames[Random.Range(0, allSpellNames.Count)];
        Debug.Log($"Generated random spell name: {randomName}");    
        return randomName;
    }

    public SpellBuilder()
    {        
    }
}
