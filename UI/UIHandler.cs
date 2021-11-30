using System;
using Game.Character;
using UnityEngine;

namespace Game.UI
{
    public class UIHandler : MonoBehaviour
    {
        public CharacterController2D controller;
        public GameObject escMenu;

        public static UIHandler Instance;

        private CharacterController2D.InputState _prev;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            escMenu.SetActive(false);
        }

        public void EscMenu()
        {
            escMenu.SetActive(!escMenu.activeSelf);
            if (escMenu.activeSelf)
            {
                _prev = controller.PlayerInputState;
                controller.PlayerInputState = CharacterController2D.InputState.Pause;
                Util.SetCursorVisibility(true);
            }
            else
            {
                controller.PlayerInputState = _prev;
                Util.SetCursorVisibility(false);
            }
        }
        
        private void Update()
        {
            var joystickInput = SystemInfo.operatingSystemFamily != OperatingSystemFamily.MacOSX ? KeyCode.JoystickButton7 : KeyCode.JoystickButton9;
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(joystickInput))
            {
                EscMenu();
            }
        }
    }
   
}