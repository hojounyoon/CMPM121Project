using UnityEngine;
using System;

public class Hittable
{

    public enum Team { PLAYER, MONSTERS }
    public Team team;

    public int hp;
    public int max_hp;
    public float defense;

    public GameObject owner;

    public void Damage(Damage damage)
    {
        EventBus.Instance.DoDamage(owner.transform.position, damage, this);
        int actualDamage = Mathf.Max(1, Mathf.RoundToInt(damage.amount * (1 - defense)));
        hp -= actualDamage;
        if (hp <= 0)
        {
            hp = 0;
            // Check if this is an enemy
            var enemy = owner.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Die();
            }
            else
            {
                OnDeath?.Invoke();
            }
        }
        else
        {
            // Check if this is the player and notify RelicManager
            if (team == Team.PLAYER && RelicManager.Instance != null)
            {
                RelicManager.Instance.OnPlayerTakeDamage();
            }
        }
    }

    public event Action OnDeath;

    public Hittable(int hp, Team team, GameObject owner)
    {
        this.hp = hp;
        this.max_hp = hp;
        this.team = team;
        this.owner = owner;
        this.defense = 0f;
    }

    public void SetMaxHP(int max_hp)
    {
        float perc = this.hp * 1.0f / this.max_hp;
        this.max_hp = max_hp;
        this.hp = Mathf.RoundToInt(perc * max_hp);
    }
    public void ReduceSpeed(int amount)
    {
        EnemyController enemy = owner.GetComponent<EnemyController>();
        enemy.speed -= amount;
    }
}
