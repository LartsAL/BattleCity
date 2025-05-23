using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartSoloGame()
        {
            SceneManager.LoadScene("SoloGame");
        }

        public void StartMultiplayerGame()
        {
            throw new NotImplementedException();
        }
        
        public void Exit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
