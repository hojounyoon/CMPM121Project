using UnityEngine;

public class OnSpellDropTrigger : RelicTrigger
{
    public OnSpellDropTrigger()
    {
        description = "When you drop a spell";
        type = "on-spell-drop";
    }

    public override bool CheckTrigger(GameObject target)
    {
        // This will be called when a spell is dropped
        return true;
    }
} 