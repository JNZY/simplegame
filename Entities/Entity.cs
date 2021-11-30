using System;
using Game.Combat;
using MyBox;
using Spine.Unity;
using UnityEngine;

namespace Game.Entities
{
    public class Entity : DamageHandler
    {
        public EntityType type;
        public EntityTeam team;
        public Vector2 nextMoveCommand;
        public Vector2 lastDirection;
        public bool damageable = true;
        public bool selfMoving = true;
        public float moveSpeed = 2f;
        public long moveDuration = 3000;
        public long movePause = 5000;
        public float aggroRange = 6;
        public float aggroFollowRange = 8;
        public float aggroSpeedBonus = 1;
        public bool canMeleeAttack = true;
        [ConditionalField("canMeleeAttack")] public DamageType meleeDamageType = DamageType.Physical;
        [ConditionalField("canMeleeAttack")] public float meleeDamage = 1;
        [ConditionalField("canMeleeAttack")] public float meleeAttackRange = 1;
        [ConditionalField("canMeleeAttack")] public float meleeAttackKnockBack;
        public bool canRangeAttack = true;
        [ConditionalField("canRangeAttack")] public DamageType rangeDamageType = DamageType.Physical;
        [ConditionalField("canRangeAttack")] public Projectile projectile;
        [ConditionalField("canRangeAttack")] public float rangeDamage = 1;
        [ConditionalField("canRangeAttack")] public float rangeAttackRange = 10;
        [ConditionalField("canRangeAttack")] public float rangeAttackKnockBack = 10;
        public long attackDuration = 1000;
        public long attackCooldown = 1000;
        private long _lastSelfMovement;
        private long _lastAttack;
        private Entity _attackTarget;
        private readonly Color _low = Color.red;
        private readonly Color _high = Color.green;
        private Vector3 _healthBarLocalScale;
        private SpriteRenderer _healthBar;
        private GameObject _healthBarBackground;
        protected SpriteRenderer SpriteRenderer;
        protected Rigidbody2D Rigidbody2D;
        private Animator _animator;
        private bool _isAttacking;
        private LevelSystem levelSystem;


        protected enum State
        {
            Idle,
            Moving
        }

        protected State CurrentState;

        public override void ReceiveDamage(Damage damageObj)
        {
            if(!damageable) return;
            base.ReceiveDamage(damageObj);
            if (type != EntityType.Player)
            {
                var newScale = _healthBarLocalScale;
                newScale.x = (health / maxHealth) * _healthBarLocalScale.x;
                _healthBar.transform.localScale = newScale;
                _healthBar.color = Color.Lerp(_low, _high, health / maxHealth);
                var barEnabled = health < maxHealth && health > 0;
                _healthBar.gameObject.SetActive(barEnabled);
                _healthBarBackground.SetActive(barEnabled);   
            }
            OnReceiveDamage(damageObj);
            if (!isAlive)
            {
                Death();
            }
        }
        
        protected virtual void OnReceiveDamage(Damage damageObj){}

        private void Death()
        {
           

            if (type != EntityType.Player)
            {
                Destroy(_healthBar.gameObject);
                Destroy(_healthBarBackground);

                handleDeadMonster();
            }

            nextMoveCommand = Vector2.zero;
            Rigidbody2D.velocity = Vector2.zero;
            var entityCollider = GetComponent<Collider2D>();
            if(entityCollider != null) entityCollider.enabled = false;
            OnDeath();
            var skeletonAnimation = GetComponent<SkeletonAnimation>();
            if(skeletonAnimation == null) return;
            skeletonAnimation.loop = false;
            skeletonAnimation.AnimationName = "Dead";
        }

        private void handleDeadMonster()
        {
            if(this.levelSystem == null)
            {
                 this.levelSystem = new LevelSystem();
                Debug.Log("created new level system");
            }
            Debug.Log("monster ded");
            // this give exp when monster is dead
            var monster = GetComponent<MonsterManager>();
            
            int expGain = monster.getExpDrop();

            this.levelSystem.addExperience(expGain);
            Debug.Log(this.levelSystem.getLevel());
            Debug.Log("Monster gives"+expGain+"current level: "+ this.levelSystem.getLevel());

            //levelWindow.setLevelSystem(ls);

        }

        protected virtual void OnDeath(){}

        protected override void OnStart()
        {
            _animator = GetComponent<Animator>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            switch (type)
            {
                case EntityType.Player:
                    selfMoving = false;
                    team = EntityTeam.Player;
                    break;
                case EntityType.Npc:
                    damageable = false;
                    selfMoving = false;
                    moveSpeed = 0;
                    aggroRange = 0;
                    Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                    break;
            }
            if(type == EntityType.Player) return;
            var textureHolder = GetComponentInChildren<HealthBarTextureHolder>();
            var healthBarObj = textureHolder.gameObject;
            _healthBar = healthBarObj.GetComponent<SpriteRenderer>();
            _healthBarLocalScale = healthBarObj.transform.localScale;
            healthBarObj.SetActive(health < maxHealth && health > 0);
            _healthBar.sortingOrder = 2;
            _healthBar.sprite = textureHolder.foreground;
            _healthBarBackground = Instantiate(healthBarObj, transform);
            var spriteRenderer = _healthBarBackground.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder--;
            spriteRenderer.sprite = textureHolder.background;
        }
        
        protected void IdleState()
        {
            Rigidbody2D.velocity = nextMoveCommand;
            UpdateAnimator(Vector2.zero);
            if(nextMoveCommand == Vector2.zero) return;
            CurrentState = State.Moving;
        }

        protected void MoveState()
        {
            UpdateAnimator(nextMoveCommand);
            Rigidbody2D.velocity = new Vector2(nextMoveCommand.x * moveSpeed, nextMoveCommand.y * moveSpeed);
            if(SpriteRenderer) SpriteRenderer.flipX = GetDirection().x >= 0;
        }
        
        public Vector2 GetDirection()
        {
            return nextMoveCommand == Vector2.zero ? lastDirection : nextMoveCommand;
        }

        private Vector2 GetTargetDirection() =>
            _attackTarget.gameObject.GetComponent<Collider2D>().attachedRigidbody.worldCenterOfMass - (Vector2) transform.position;

        private void UpdateAnimator(Vector2 direction)
        {
            if(!_animator) return;
            var finalDirection = Util.GetCombinedDirection(direction);
            _animator.SetInteger("WalkX", finalDirection[0]);
            _animator.SetInteger("WalkY", finalDirection[1]);
        }

        private void Update()
        {
            if (type == EntityType.Aggressive)
            {
                var pos = (Vector2) transform.position;
                var meleeResults = Util.GetOverlapCircleCollider(pos, aggroRange);
                Entity closest = null;
                foreach (var hit in meleeResults)
                {
                    var obj = hit.gameObject;
                    var entity = obj.GetComponent<Entity>();
                    if(!entity || entity == this || !entity.isAlive) continue;
                    if (!closest || Vector2.Distance(pos, hit.transform.position) < Vector2.Distance(pos, closest.transform.position))
                    {
                        closest = entity;
                    }
                }

                if (closest && !_attackTarget) _attackTarget = closest;
                if (_attackTarget)
                {
                    var distance = Vector2.Distance(pos, _attackTarget.transform.position);
                    if (distance > aggroFollowRange || !_attackTarget.isAlive)
                    {
                        _attackTarget = null;
                    }
                    else
                    {
                        if (canMeleeAttack && distance <= meleeAttackRange)
                        {
                            MeleeAttack();
                        }
                        else if (canRangeAttack && distance <= rangeAttackRange)
                        {
                            RangeAttack();
                        }
                    }
                }
            }
            
            switch (CurrentState)
            {
                case State.Idle:
                    IdleState();
                    break;
                case State.Moving:
                    MoveState();
                    break;
            }
        }

        private void FixedUpdate()
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            if (selfMoving && isAlive && !_attackTarget)
            {
                if (_lastSelfMovement + movePause + moveDuration <= now)
                {
                    var randomVector = Util.GetRandomDirectionVector();
                    nextMoveCommand = randomVector.normalized;
                    _lastSelfMovement = now;
                }
                else if (_lastSelfMovement + moveDuration <= now)
                {
                    nextMoveCommand = Vector2.zero;
                }   
            }
            else if (isAlive && _attackTarget)
            {
                var targetDirection = GetTargetDirection();
                if ((canMeleeAttack && targetDirection.magnitude <= .5f) || (canRangeAttack && targetDirection.magnitude <= rangeAttackRange / 2))
                {
                    nextMoveCommand = Vector2.zero;
                }
                else
                {
                    nextMoveCommand = targetDirection.normalized * aggroSpeedBonus;   
                }
            }

            if (_isAttacking && _lastAttack + attackDuration <= now)
            {
                _isAttacking = false;
            }

            if (_isAttacking)
            {
                nextMoveCommand = Vector2.zero;
            }

            var movement = nextMoveCommand * moveSpeed;
            if (movement != Vector2.zero)
            {
                lastDirection = movement;
                Rigidbody2D.velocity = movement;
            }
            else
            {
                CurrentState = State.Idle;
                Rigidbody2D.velocity = Vector2.zero;
                if(SpriteRenderer) SpriteRenderer.flipX = GetDirection().x >= 0;
            }
        }

        protected void MeleeAttack()
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (_lastAttack + attackDuration + attackCooldown > now) return;
            _lastAttack = now;
            _isAttacking = true;
            MeleeAttack(GetDirection(), meleeAttackRange, meleeDamage, meleeDamageType, knockBackMultiplier: meleeAttackKnockBack);
        }

        protected void RangeAttack()
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (_lastAttack + attackDuration + attackCooldown > now) return;
            _lastAttack = now;
            _isAttacking = true;
            var newProjectile = Instantiate(projectile);
            newProjectile.Fire(this, transform.position, GetTargetDirection(), rangeDamage, rangeDamageType, rangeAttackKnockBack);
        }

        public void AddForce(Vector2 force)
        {
            Rigidbody2D.AddForce(force);
        }
    }
}