using Game.Entities;
using MyBox;
using UnityEngine;

namespace Game.Combat
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 1;
        public float maxRange = 10;
        public DamageType damageType = DamageType.Physical;
        public float knockBackForceMultiplier;
        public bool ignoreWalls;
        public bool multipleTargets;
        [ConditionalField("multipleTargets")] public bool limitedHits;
        [ConditionalField("multipleTargets")] public int hitAmount = 1;
        [ConditionalField("multipleTargets")] public float damageReductionPerHit;

        private int _amountHits;
        private float _damage;
        private Vector2 _direction;
        private Vector2 _startPos;
        private Collider2D _collider;
        private Entity _shooter;

        private void FixedUpdate()
        {
            transform.Translate(_direction * speed * 0.1f);
            if (Vector2.Distance(_startPos, transform.position) >= maxRange)
            {
                Destroy(gameObject);
            }
        }

        public void Fire(Entity shooter, Vector2 pos, Vector2 direction, float damage, DamageType type, float knockBackMultiplier = 0)
        {
            Fire(shooter, pos, direction, damage);
            damageType = type;
            knockBackForceMultiplier = knockBackMultiplier;
        }

        public void Fire(Entity shooter, Vector2 pos, Vector2 direction, float damage)
        {
            _direction = direction;
            _damage = damage;
            _startPos = pos;
            _shooter = shooter;
            _collider = GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(_shooter.gameObject.GetComponent<Collider2D>(), _collider);
            transform.position = _startPos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var entity = other.gameObject.GetComponent<Entity>();
            if (!entity)
            {
                if(ignoreWalls) return;
                _amountHits++;
                CountHit();
                return;
            }
            if (!multipleTargets)
            {
                entity.ReceiveDamage(Damage.Create(_damage, damageType));
                entity.AddForce(_direction * knockBackForceMultiplier * 10);
                Destroy(gameObject);
                return;
            }
            
            entity.ReceiveDamage(Damage.Create(_damage - _amountHits*damageReductionPerHit, damageType));
            entity.AddForce(_direction * knockBackForceMultiplier * 10);
            _amountHits++;
            CountHit();
        }

        private void CountHit()
        {
            if (limitedHits && _amountHits >= hitAmount)
            {
                Destroy(gameObject);
            }
        }
    }
}