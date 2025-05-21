using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

public class RelicManager
{
    private static RelicManager theInstance;
    public static RelicManager Instance 
    { 
        get
        {
            if (theInstance == null)
                theInstance = new RelicManager();
            return theInstance;
        }
    }

    private List<Relic> availableRelics = new List<Relic>();
    private PlayerController player;

    private RelicManager()
    {
        LoadAvailableRelics();
        if (GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player.GetComponent<PlayerController>();
        }
    }

    private void LoadAvailableRelics()
    {
        TextAsset relicsJson = Resources.Load<TextAsset>("relics");
        if (relicsJson == null)
        {
            Debug.LogError("Failed to load relics.json!");
            return;
        }

        JArray relicsArray = JArray.Parse(relicsJson.text);
        foreach (JObject relicObj in relicsArray)
        {
            string name = relicObj["name"].ToString();
            int spriteIndex = relicObj["sprite"].ToObject<int>();
            
            JObject triggerObj = relicObj["trigger"] as JObject;
            JObject effectObj = relicObj["effect"] as JObject;

            RelicTrigger trigger = CreateTrigger(triggerObj);
            RelicEffect effect = CreateEffect(effectObj);

            if (trigger != null && effect != null)
            {
                Relic relic = new Relic(name, spriteIndex, trigger, effect);
                availableRelics.Add(relic);
            }
        }
    }

    public RelicTrigger CreateTrigger(JObject triggerObj)
    {
        string type = triggerObj["type"].ToString();
        
        switch (type)
        {
            case "take-damage":
                return new TakeDamageTrigger();
            case "stand-still":
                float time = float.Parse(triggerObj["amount"].ToString());
                return new StandStillTrigger(time);
            case "on-kill":
                return new OnKillTrigger();
            default:
                Debug.LogError($"Unknown trigger type: {type}");
                return null;
        }
    }

    public RelicEffect CreateEffect(JObject effectObj)
    {
        string type = effectObj["type"].ToString();
        string amount = effectObj["amount"].ToString();
        string until = effectObj["until"]?.ToString();

        switch (type)
        {
            case "gain-mana":
                int manaAmount = int.Parse(amount);
                return new GainManaEffect(manaAmount);
            case "gain-spellpower":
                if (amount.Contains("wave"))
                {
                    // Parse "10 wave 5 * +" format
                    string[] parts = amount.Split(' ');
                    int baseAmount = int.Parse(parts[0]);
                    int waveMultiplier = int.Parse(parts[2]);
                    return new GainSpellPowerEffect(baseAmount, until, waveMultiplier);
                }
                else
                {
                    int spellPowerAmount = int.Parse(amount);
                    return new GainSpellPowerEffect(spellPowerAmount, until);
                }
            default:
                Debug.LogError($"Unknown effect type: {type}");
                return null;
        }
    }

    public void AddRelic(Relic relic)
    {
        // Remove the relic from available relics when it's selected
        availableRelics.RemoveAll(r => r.name == relic.name);
        Debug.Log($"Added relic: {relic.name}");
    }

    public bool HasRelic(string relicName)
    {
        // Check if the relic is still in available relics
        return !availableRelics.Exists(r => r.name == relicName);
    }

    public List<Relic> GetAvailableRelics()
    {
        return new List<Relic>(availableRelics);
    }

    public void OnPlayerTakeDamage()
    {
        if (player == null) return;
        
        foreach (var relic in availableRelics)
        {
            if (relic.trigger is TakeDamageTrigger)
            {
                relic.effect.ApplyEffect(player.gameObject);
            }
        }
    }

    public void OnEnemyKilled()
    {
        if (player == null) return;
        
        foreach (var relic in availableRelics)
        {
            if (relic.trigger is OnKillTrigger)
            {
                relic.effect.ApplyEffect(player.gameObject);
            }
        }
    }

    public void OnPlayerCastSpell()
    {
        if (player == null) return;
        
        foreach (var relic in availableRelics)
        {
            if (relic.effect is GainSpellPowerEffect spellPowerEffect)
            {
                if (relic.effect.until == "cast-spell")
                {
                    spellPowerEffect.RemoveEffect(player.gameObject);
                }
            }
        }
    }

    public void OnPlayerMove()
    {
        if (player == null) return;
        
        foreach (var relic in availableRelics)
        {
            if (relic.effect is GainSpellPowerEffect spellPowerEffect)
            {
                if (relic.effect.until == "move")
                {
                    spellPowerEffect.RemoveEffect(player.gameObject);
                }
            }
        }
    }

    public void OnWaveStart(int waveNumber)
    {
        if (player == null) return;
        
        foreach (var relic in availableRelics)
        {
            if (relic.effect is GainSpellPowerEffect spellPowerEffect)
            {
                spellPowerEffect.UpdateForWave(waveNumber);
            }
        }
    }

    public void UpdatePlayerReference()
    {
        if (GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player.GetComponent<PlayerController>();
        }
    }
} 