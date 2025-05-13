using UnityEngine;
using System.Collections;

public class ArcaneBlastSpell : Spell
{
    public ArcaneBlastSpell(SpellCaster caster, Spell data) : base(caster, data)
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
                }
                // Create the explosion
                CreateExplosion(impact);
            }
        );
        yield return new WaitForEndOfFrame();
    }

    private void CreateExplosion(Vector3 impact)
    {
        int numProjectiles = GetN();
        Debug.Log($"ArcaneBlast N value from spell data: {numProjectiles}");
        
        if (numProjectiles <= 0)
        {
            Debug.LogError("ArcaneBlast: N value is 0 or negative! Check spells.json configuration.");
            return;
        }
        
        Debug.Log($"Creating explosion at {impact} with {numProjectiles} projectiles");
        
        // Create projectiles in a circle around the impact point
        for (int i = 0; i < numProjectiles; i++)
        {
            // Calculate direction for this projectile
            float angle = (360f / numProjectiles) * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            // Create the secondary projectile
            GameManager.Instance.projectileManager.CreateProjectile(
                projectileSprite,
                "straight", // Secondary projectiles go straight
                impact,
                direction,
                projectile.speedEval * 2f, // Make secondary projectiles faster
                OnHit
            );
        }
    }
} 