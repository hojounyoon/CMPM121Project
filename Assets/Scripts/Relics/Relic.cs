using UnityEngine;
using System;
using System.Collections.Generic;

public class Relic
{
    public string name;
    public int spriteIndex;
    public RelicTrigger trigger;
    public RelicEffect effect;

    public Relic(string name, int spriteIndex, RelicTrigger trigger, RelicEffect effect)
    {
        this.name = name;
        this.spriteIndex = spriteIndex;
        this.trigger = trigger;
        this.effect = effect;
    }
}

public abstract class RelicTrigger
{
    public string description;
    public string type;

    public abstract bool CheckTrigger(GameObject player);
}

public abstract class RelicEffect
{
    public string description;
    public string type;
    public string amount;
    public string until; // Optional condition for when effect ends

    public abstract void ApplyEffect(GameObject player);
    public abstract void RemoveEffect(GameObject player);
}

// Concrete trigger implementations
public class TakeDamageTrigger : RelicTrigger
{
    public TakeDamageTrigger()
    {
        description = "Whenever you take damage";
        type = "take-damage";
    }

    public override bool CheckTrigger(GameObject player)
    {
        // This will be called when the player takes damage
        return true;
    }
}

public class StandStillTrigger : RelicTrigger
{
    private float requiredTime;
    private float currentTime;
    private Vector3 lastPosition;

    public StandStillTrigger(float time)
    {
        description = $"When you don't move for {time} seconds";
        type = "stand-still";
        requiredTime = time;
        currentTime = 0f;
    }

    public override bool CheckTrigger(GameObject player)
    {
        if (player.transform.position != lastPosition)
        {
            currentTime = 0f;
            lastPosition = player.transform.position;
            return false;
        }

        currentTime += Time.deltaTime;
        return currentTime >= requiredTime;
    }
}

public class OnKillTrigger : RelicTrigger
{
    public OnKillTrigger()
    {
        description = "When you kill an enemy";
        type = "on-kill";
    }

    public override bool CheckTrigger(GameObject player)
    {
        // This will be called when an enemy is killed
        return true;
    }
}

// Concrete effect implementations
public class GainManaEffect : RelicEffect
{
    private int manaAmount;

    public GainManaEffect(int amount)
    {
        description = $"you gain {amount} mana";
        type = "gain-mana";
        this.manaAmount = amount;
    }

    public override void ApplyEffect(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.spellcaster != null)
        {
            playerController.spellcaster.mana += manaAmount;
            playerController.spellcaster.mana = Mathf.Min(
                playerController.spellcaster.mana,
                playerController.spellcaster.max_mana
            );
        }
    }

    public override void RemoveEffect(GameObject player)
    {
        // No cleanup needed for mana gain
    }
}

public class GainSpellPowerEffect : RelicEffect
{
    private int spellPowerAmount;
    private int baseAmount;
    private int waveMultiplier;
    private bool isTemporary;
    private bool isActive;

    public GainSpellPowerEffect(int amount, string until = null, int waveMultiplier = 0)
    {
        description = $"you gain {amount} spellpower";
        type = "gain-spellpower";
        this.baseAmount = amount;
        this.spellPowerAmount = amount;
        this.waveMultiplier = waveMultiplier;
        this.isTemporary = until != null;
        this.isActive = false;
    }

    public void UpdateForWave(int wave)
    {
        if (waveMultiplier > 0)
        {
            spellPowerAmount = baseAmount + (wave * waveMultiplier);
        }
    }

    public override void ApplyEffect(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.spellcaster != null)
        {
            playerController.spellcaster.spellPower += spellPowerAmount;
            isActive = true;
        }
    }

    public override void RemoveEffect(GameObject player)
    {
        if (isActive)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.spellcaster != null)
            {
                playerController.spellcaster.spellPower -= spellPowerAmount;
                isActive = false;
            }
        }
    }
} 