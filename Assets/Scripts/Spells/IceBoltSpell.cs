using UnityEngine;
using System.Collections;

public class IceBoltSpell : Spell
{
    private float stunDuration = 1f;

    public IceBoltSpell(SpellCaster owner, Spell spellData) : base(owner, spellData)
    {
        // Initialize with the base spell data
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        GameManager.Instance.projectileManager.CreateProjectile(
            projectileSprite,
            projectile.trajectory,
            where,
            target - where,
            projectile.speedEval,
            (Hittable other, Vector3 impact) => {
                if (other.team != team)
                {
                    // Apply damage
                    other.Damage(new Damage(GetDamage(), Damage.Type.ICE));
                    
                    // Apply stun effect
                    EnemyController enemy = other.owner.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.Stun(stunDuration);
                    }
                }
            }
        );
        yield return new WaitForEndOfFrame();
    }

    public override string GetName() => "Ice Bolt";
    public override int GetIcon() => 12;
} 