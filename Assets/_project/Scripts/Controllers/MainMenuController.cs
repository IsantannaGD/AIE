using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _playGameButton;
    [SerializeField] private Button _exitGame;

    private void Start()
    {
        Initializations();
    }

    private void Initializations()
    {
        _playGameButton.onClick.AddListener(PlayGameCallback);
        _exitGame.onClick.AddListener(ExitGame);
    }

    private void PlayGameCallback()
    {
        SceneManager.LoadScene("Game");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
