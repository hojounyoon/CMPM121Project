using UnityEngine;
using System.Collections;

public class ArcaneSpraySpell : Spell
{
    public ArcaneSpraySpell(SpellCaster caster, Spell data) : base(caster, data)
    {
        // Initialize arcane spray specific properties
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        int numProjectiles = GetN();
        Debug.Log($"ArcaneSpray creating {numProjectiles} projectiles");
        
        if (numProjectiles <= 0)
        {
            Debug.LogError("ArcaneSpray: N value is 0 or negative! Check spells.json configuration.");
            yield break;
        }

        // Calculate the spread angle based on number of projectiles
        float totalSpread = 45f; // Total spread angle in degrees
        float angleStep = totalSpread / (numProjectiles - 1);
        float startAngle = -totalSpread / 2f;

        // Create projectiles in a spread pattern
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * (target - where).normalized;

            GameManager.Instance.projectileManager.CreateProjectile(
                projectileSprite,
                projectile.trajectory,
                where,
                direction,
                projectile.speedEval,
                OnHit
            );
        }

        yield return new WaitForEndOfFrame();
    }
} 