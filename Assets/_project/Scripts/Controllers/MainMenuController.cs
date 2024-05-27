using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _singlePlayerPlay;
    [SerializeField] private Button _multiPlayerPlay;
    [SerializeField] private Button _exitGame;

    private void Start()
    {
        Initializations();
    }

    private void Initializations()
    {
        _singlePlayerPlay.onClick.AddListener(GoToSinglePlayerGame);
        _multiPlayerPlay.onClick.AddListener(GoToMultiplayerGame);
        _exitGame.onClick.AddListener(ExitGame);
    }

    private void GoToSinglePlayerGame()
    {
        GameManager.Instance.SetGameMode(GameMode.SinglePlayer);
        SceneManager.LoadScene("Game");
    }

    private void GoToMultiplayerGame()
    {
        GameManager.Instance.SetGameMode(GameMode.LocalMultiPlayer);
        SceneManager.LoadScene("Game");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
