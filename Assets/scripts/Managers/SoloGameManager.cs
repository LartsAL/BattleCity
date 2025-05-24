using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class SoloGameManager : MonoBehaviour
    {
        public GameObject pauseUI;
        public float gameOverScreenDelay = 3.0f;
        
        private bool _isPaused = false;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            _isPaused = true;

            TimeUtils.StopTime();
            Cursor.lockState = CursorLockMode.None;
            
            pauseUI.SetActive(true);
        }
        
        public void ResumeGame()
        {
            _isPaused = false;
            
            TimeUtils.ResumeTime();
            Cursor.lockState = CursorLockMode.Locked;
            
            pauseUI.SetActive(false);
        }
        
        public void ToMainMenuScene()
        {
            TimeUtils.ResumeTime();
            SceneManager.LoadScene("MainMenu");
        }

        public void ToGameOverScene()
        {
            TimeUtils.ResumeTime();
            StartCoroutine(TimeUtils.WaitAndDo(gameOverScreenDelay, () => SceneManager.LoadScene("GameOver")));
        }
    }
}