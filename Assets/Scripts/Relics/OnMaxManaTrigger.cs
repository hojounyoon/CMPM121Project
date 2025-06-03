using UnityEngine;

public class OnMaxManaTrigger : RelicTrigger
{
    public OnMaxManaTrigger()
    {
        description = "When your mana reaches maximum";
        type = "on-max-mana";
    }

    public override bool CheckTrigger(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null && player.spellcaster != null)
        {
            return player.spellcaster.mana >= player.spellcaster.max_mana;
        }
        return false;
    }
} 