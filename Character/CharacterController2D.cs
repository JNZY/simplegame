using Game.Combat;
using Game.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character
{
    public class CharacterController2D : Entity
    {
        public Slider healthBar;
        private string charactertest = "test";
        public enum InputState
        {
            CharacterControl,
            DialogControl,
            Pause
        }

        public InputState PlayerInputState { get; set; } = InputState.CharacterControl;

        private void Update()
        {
            switch (PlayerInputState)
            {
                case InputState.CharacterControl:
                    CharacterControl();
                    break;
                case InputState.DialogControl:
                    DialogControl();
                    break;
                case InputState.Pause:
                    nextMoveCommand = Vector2.zero;
                    break;
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

        private void CharacterControl()
        {
            var joystickInput = SystemInfo.operatingSystemFamily != OperatingSystemFamily.MacOSX ? KeyCode.JoystickButton2 : KeyCode.JoystickButton18;
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(joystickInput))
            {
                MeleeAttack();
            }
            
            var move = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
            {
                move = Util.AddVectors(move, Vector2.up);
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                move = Util.AddVectors(move, Vector2.right);
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                move = Util.AddVectors(move, Vector2.down);
            }

            if (Input.GetKey(KeyCode.A))
            {
                move = Util.AddVectors(move, Vector2.left);
            }

            if (move == Vector2.zero)
            {
                var horizontal = Input.GetAxis("Horizontal");
                var vertical = Input.GetAxis("Vertical");

                if (vertical != 0f)
                {
                    move = Util.AddVectors(move, Vector2.up*vertical);
                }

                if (horizontal != 0f)
                {
                    move = Util.AddVectors(move, Vector2.right*horizontal);
                }
            }

            if (move.magnitude > 1f)
            {
                move.Normalize();
            }

            nextMoveCommand = move;
        }

        private void DialogControl()
        {
            //TODO: Dialog Control
        }

        private void Awake()
        {
            this.levelSystem();
            Rigidbody2D = GetComponent<Rigidbody2D>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            type = EntityType.Player;
            team = EntityTeam.Player;
            selfMoving = false;
        }

        protected override void handleDeadMonster(LevelSystem levelSystem)
        {

        }
        protected override void OnStart()
        {
            base.OnStart();
            UpdateHealthBar();
        }

        public override void ReceiveDamage(Damage damageObj)
        {
            base.ReceiveDamage(damageObj);
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            healthBar.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, healthBar.normalizedValue);
        }

    }
}