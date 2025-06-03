using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int dmg;
    public Transform target;
    public int speed;
    public Hittable hp;
    public HealthBar healthui;
    public bool dead;
    private bool isStunned = false;
    private float stunEndTime = 0f;

    public float last_attack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        healthui.SetHealth(hp);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned && Time.time >= stunEndTime)
        {
            isStunned = false;
        }

        if (!isStunned)
        {
            Vector3 direction = target.position - transform.position;
            if (direction.magnitude < 2f)
            {
                DoAttack();
            }
            else
            {
                GetComponent<Unit>().movement = direction.normalized * speed;
            }
        }
        else
        {
            GetComponent<Unit>().movement = Vector3.zero;
        }
    }
    
    void DoAttack()
    {
        if (last_attack + 2 < Time.time)
        {
            last_attack = Time.time;
            target.gameObject.GetComponent<PlayerController>().hp.Damage(new Damage(dmg, Damage.Type.PHYSICAL));
        }
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            // Notify StatManager before removing the enemy
            if (StatManager.Instance != null)
            {
                StatManager.Instance.OnEnemyDefeated();
            }
            
            // Notify RelicManager of the kill
            if (RelicManager.Instance != null)
            {
                RelicManager.Instance.OnEnemyKilled();
            }
            
            // Remove from GameManager first
            GameManager.Instance.RemoveEnemy(gameObject);
            
            // Clean up components
            if (hp != null)
            {
                hp.OnDeath -= Die; // Unsubscribe from the event
            }
            
            // Destroy the game object
            Destroy(gameObject);
            //Debug.Log("Enemy destroyed and cleaned up");
        }
    }
}
