using UnityEngine;

namespace Game.UI
{
    public class EscMenuHandler : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit();
        }

        public void ReportBug()
        {
            Application.OpenURL("https://discord.gg/WhcUFngNs7");
        }

        public void ReturnToMainMenu()
        {
            GameHandler.Instance.ReturnToMainMenu();
        }
    }
}