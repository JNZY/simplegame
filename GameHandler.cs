using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameHandler : MonoBehaviour
    {
        public GameObject loadingScreen;
        public static GameHandler Instance;
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            loadingScreen.SetActive(true);
        }
        private void Start()
        {
            Util.SetDiscordState("In Game");
            Util.SetCursorVisibility(false);
            loadingScreen.SetActive(false);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && !UIHandler.Instance.escMenu.activeSelf)
            {
                Util.SetCursorVisibility(false);
            }
        }

        public void ReturnToMainMenu()
        {
            loadingScreen.SetActive(true);
            Util.SetDiscordState("Returning to Main Menu");
            SaveGame();
            SceneManager.LoadSceneAsync((int) Scene.MainMenu);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void SaveGame()
        {
            //TODO: Save Game
        }
    }
   
}