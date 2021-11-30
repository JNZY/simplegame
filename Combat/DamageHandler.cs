using Game.Entities;
using UnityEngine;

namespace Game.Combat
{
    public abstract class DamageHandler : MonoBehaviour
    {
        public float maxHealth = 10;
        public float health;
        public bool isAlive = true;

        private void Start()
        {
            health = maxHealth;
            OnStart();
        }

        protected virtual void OnStart() {}

        public virtual void ReceiveDamage(Damage damageObj)
        {
            if(!isAlive) return;
            health -= damageObj.Amount;
            if (health <= 0)
            {
                isAlive = false;
            }
        }

        public void Heal(float amount)
        {
            if (!isAlive) return;
            if (health + amount > maxHealth)
            {
                health = maxHealth;
            }
            else
            {
                health += amount;
            }
        }

        public void HealFull()
        {
            if(!isAlive) return;
            health = maxHealth;
        }

        protected void MeleeAttack(Vector2 direction, float range, float damage, DamageType type, float typeModifier = 0f, float knockBackMultiplier = 0f)
        {
            var results = Util.Raycast(transform.position, direction, range);
            foreach (var raycastHit2D in results)
            {
                var hitTransform = raycastHit2D.transform;
                var damageEntity = hitTransform.gameObject.GetComponent<DamageHandler>();
                if (damageEntity == null || damageEntity == this) continue;
                damageEntity.ReceiveDamage(Damage.Create(damage, type, typeModifier));
                if (damageEntity is Entity entity)
                {
                    entity.AddForce(direction * knockBackMultiplier * 10);
                }
            }
        }
    }
}