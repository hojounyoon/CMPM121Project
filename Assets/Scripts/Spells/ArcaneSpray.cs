using UnityEngine;
using System.Collections;

public class ArcaneSpray : Spell
{
    public ArcaneSpray(SpellCaster owner) : base(owner)
    {
    }

    private int numProjectiles;
    private float sprayAngle;
    private float projectileLifetime;

    protected override void InitializeSpell()
    {
        spellName = "Arcane Spray";
        description = "A large number of short-range bolts in a cone.";
        icon = 1;
        cooldown = 0.5f;
        manaCost = 15;
        baseDamage = 3;
        numProjectiles = 7;
        sprayAngle = 0.3f;
        projectileTrajectory = "straight";
        projectileSpeed = 8f;
        projectileSprite = 0;
        projectileLifetime = 0.1f;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 5f));
    }

    public int GetNumProjectiles()
    {
        return (int)(numProjectiles * (1 + owner.spellPower / 25f));
    }

    public float GetProjectileLifetime()
    {
        return projectileLifetime * (1 + owner.spellPower / 100f);
    }

    protected override IEnumerator DoCast(Vector3 where, Vector3 target)
    {
        Vector3 direction = (target - where).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x);
        
        for (int i = 0; i < GetNumProjectiles(); i++)
        {
            float angleOffset = Random.Range(-sprayAngle, sprayAngle);
            float finalAngle = baseAngle + angleOffset;
            Vector3 projectileDirection = new Vector3(
                Mathf.Cos(finalAngle),
                Mathf.Sin(finalAngle),
                0
            );

            GameManager.Instance.projectileManager.CreateProjectile(
                projectileSprite,
                projectileTrajectory,
                where,
                projectileDirection,
                projectileSpeed,
                OnHit,
                GetProjectileLifetime()
            );
        }
        
        yield return new WaitForEndOfFrame();
    }
} 