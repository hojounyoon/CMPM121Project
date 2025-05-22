using UnityEngine;

public class TemporarySpellPowerEffect : RelicEffect
{
    private int amount;
    private float duration;
    private float endTime;

    public TemporarySpellPowerEffect(int amount, float duration)
    {
        this.amount = amount;
        this.duration = duration;
        this.description = $"Gain {amount} spellpower for {duration} seconds";
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null)
        {
            player.spellcaster.spellPower += amount;
            endTime = Time.time + duration;
            Debug.Log($"Applied temporary spellpower: +{amount} for {duration} seconds");
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null && Time.time >= endTime)
        {
            player.spellcaster.spellPower -= amount;
            Debug.Log($"Removed temporary spellpower: -{amount}");
        }
    }
}

public class GainMaxHPEffect : RelicEffect
{
    private int amount;

    public GainMaxHPEffect(int amount)
    {
        this.amount = amount;
        this.description = $"gain {amount} max hp";
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null && player.hp != null)
        {
            player.hp.SetMaxHP(player.hp.max_hp + amount);  // Use SetMaxHP to properly scale current health
            Debug.Log($"Increased max HP by {amount}");
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        // Max HP increase is permanent, so no need to remove
    }
} 