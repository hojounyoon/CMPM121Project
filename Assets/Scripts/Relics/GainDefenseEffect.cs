using UnityEngine;

public class GainDefenseEffect : RelicEffect
{
    private float defenseAmount;

    public GainDefenseEffect(float amount)
    {
        this.defenseAmount = amount;
        this.description = $"gain {amount * 100}% damage reduction";
        this.type = "gain-defense";
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null && player.hp != null)
        {
            // Add defense to the player's Hittable component
            player.hp.defense += defenseAmount;
            Debug.Log($"Increased defense by {defenseAmount * 100}%");
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null && player.hp != null)
        {
            // Remove defense from the player's Hittable component
            player.hp.defense -= defenseAmount;
            Debug.Log($"Decreased defense by {defenseAmount * 100}%");
        }
    }
} 