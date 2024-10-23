using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Button _buttonCompetition;
    [SerializeField] private Button _buttonSettings;
    [SerializeField] private Button _buttonCreators;
    [SerializeField] private Button _buttonQuit;

    private void Start()
    {
        _buttonPlay.onClick.AddListener(StartGame);
        _buttonCompetition.onClick.AddListener(PlaceholderAction);
        _buttonSettings.onClick.AddListener(PlaceholderAction);
        _buttonCreators.onClick.AddListener(PlaceholderAction);
        _buttonQuit.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("SoloGameScene");
    }

    private void PlaceholderAction()
    {
        Debug.Log("Действие пока не реализовано");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}