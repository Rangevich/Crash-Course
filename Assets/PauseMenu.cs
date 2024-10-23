using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private Button _buttonResume;
    [SerializeField] private Button _buttonMainMenu;
    [SerializeField] private Button _buttonPause;
    [SerializeField] private GameObject _winBannerUI;
    [SerializeField] private Button _buttonPlayAgain;
    [SerializeField] private Button _buttonReturnToMainMenu;

    private bool _isPaused = false;

    private void Start()
    {
        _pauseMenuUI.SetActive(false);
        _buttonPause.onClick.AddListener(TogglePause);
        _buttonResume.onClick.AddListener(ResumeGame);
        _buttonMainMenu.onClick.AddListener(ReturnToMainMenu);

        if (_winBannerUI != null)
        {
            _buttonPlayAgain.onClick.AddListener(PlayAgain);
            _buttonReturnToMainMenu.onClick.AddListener(ReturnToMainMenu);
        }
    }

    private void TogglePause()
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

    private void PauseGame()
    {
        _buttonPause.gameObject.SetActive(false);
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
    }

    private void ResumeGame()
    {
        _buttonPause.gameObject.SetActive(true);
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}