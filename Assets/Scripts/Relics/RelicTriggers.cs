using UnityEngine;

public class OnWaveStartTrigger : RelicTrigger
{
    public OnWaveStartTrigger()
    {
        description = "When a new wave starts";
    }

    public override bool CheckTrigger(GameObject target)
    {
        // This will be called by RelicManager when a new wave starts
        return true;
    }
}

public class DealDamageTrigger : RelicTrigger
{
    public DealDamageTrigger()
    {
        description = "Whenever you deal damage to an enemy";
    }

    public override bool CheckTrigger(GameObject target)
    {
        // This will be called when the player deals damage
        return true;
    }
}

public class WaveCompleteTrigger : RelicTrigger
{
    public WaveCompleteTrigger()
    {
        description = "Whenever you finish a wave";
    }

    public override bool CheckTrigger(GameObject target)
    {
        // This will be called when a wave is completed
        return true;
    }
} 