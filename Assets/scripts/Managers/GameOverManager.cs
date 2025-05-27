using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameOverManager : MonoBehaviour
    {
        public void ToMainMenuScene()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void ToRecordsTableScene()
        {
            throw new NotImplementedException();
        }
    }
}