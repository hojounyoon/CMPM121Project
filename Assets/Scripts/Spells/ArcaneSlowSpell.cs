using UnityEngine;
using System.Collections;

public class ArcaneSlowSpell : Spell
{
    public ArcaneSlowSpell(SpellCaster caster, Spell data) : base(caster, data)
    {
        // Initialize arcane blast specific properties
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        // Create the main projectile with a custom OnHit handler
        GameManager.Instance.projectileManager.CreateProjectile(
            projectileSprite, 
            projectile.trajectory, 
            where, 
            target - where, 
            projectile.speedEval, 
            (Hittable other, Vector3 impact) => {
                // Do the normal hit damage
                if (other.team != team)
                {
                    other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
                    other.ReduceSpeed(3);
                }
            }
        );
        yield return new WaitForEndOfFrame();
    }

    
} 