using UnityEngine;

public class RegainHPEffect : RelicEffect
{
    private int hpAmount;

    public RegainHPEffect(int amount)
    {
        this.hpAmount = amount;
        this.description = $"restore {amount} HP";
        this.type = "regain-hp";
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null && player.hp != null)
        {
            // Heal the player, but don't exceed max HP
            player.hp.hp = Mathf.Min(player.hp.hp + hpAmount, player.hp.max_hp);
            Debug.Log($"Restored {hpAmount} HP");
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        // No cleanup needed for HP restoration
    }
} 