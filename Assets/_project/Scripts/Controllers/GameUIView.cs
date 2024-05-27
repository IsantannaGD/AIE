using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIView : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;

    [SerializeField] private GameObject _generalRulesPanel;
    [SerializeField] private GameObject _selectWormPanel;

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameOverPanel;

    [SerializeField] private Button _pausePanelReturnButton;
    [SerializeField] private Button _pausePanelMainMenuButton;
    [SerializeField] private Button _pausePanelExitButton;

    [SerializeField] private Button _gameOverPanelMainMenuButton;
    [SerializeField] private Button _gameOverPanelExitButton;

    [SerializeField] private TextMeshProUGUI _leftInputDisplay;
    [SerializeField] private TextMeshProUGUI _rightInputDisplay;
    [SerializeField] private TextMeshProUGUI _playerNameDisplay;
    [SerializeField] private TextMeshProUGUI _wormDescriptionDisplay;
    [SerializeField] private TextMeshProUGUI _timerDisplay;

    [SerializeField] private GameObject[] _preSetsDisplay = new GameObject[3];
    [SerializeField] private string[] _preSetsDescriptions = new string[3];

    [SerializeField] private BodyPartType[][] _bodyPartsToSelect = new BodyPartType[3][];
    [SerializeField] private int _preSetIndex;

    [SerializeField] private int _playerCount;
    [SerializeField] private bool _newPlayerSelect;

    private void Start()
    {
        Initializations();
    }

    private void Update()
    {
        if (_newPlayerSelect)
        {
            if (Input.GetKeyDown(_inputManager.CurrentLeftInput))
            {
                MoveSelectionToLeft();
            }
            else if (Input.GetKeyDown(_inputManager.CurrentRightInput))
            {
                MoveSelectionToRight();
            }
        }
    }

    private void Initializations()
    {
        GameManager.OnGameStart += StartGameCallback;
        GameManager.OnRegisterNewPlayer += NewPlayerSelectCallback;
        GameManager.OnResetPlayerList += ResetPlayerCallback;
        GameManager.OnGamePause += GamePausedCallback;
        GameManager.OnGameOver += GameOverCallback;

        _pausePanelReturnButton.onClick.AddListener(ReturnToGameCallback);
        _pausePanelMainMenuButton.onClick.AddListener(GoToMainMenuCallback);
        _pausePanelExitButton.onClick.AddListener(ExitGameCallback);

        _gameOverPanelMainMenuButton.onClick.AddListener(GoToMainMenuCallback);
        _gameOverPanelExitButton.onClick.AddListener(ExitGameCallback);

        SetupPreSetBodyParts();
    }

    private void StartGameCallback()
    {
        _generalRulesPanel.SetActive(false);
    }

    private void GamePausedCallback()
    {
        _pausePanel.SetActive(!_pausePanel.activeInHierarchy);
    }

    private void SetupPreSetBodyParts()
    {
        BodyPartType[] a = new BodyPartType[3];
        a[0] = BodyPartType.BatteringRam;
        a[1] = BodyPartType.Regular;
        a[2] = BodyPartType.Regular;

        BodyPartType[] b = new BodyPartType[3];
        b[0] = BodyPartType.EnginePower;
        b[1] = BodyPartType.Regular;
        b[2] = BodyPartType.Regular;

        BodyPartType[] c = new BodyPartType[3];
        c[0] = BodyPartType.Regular;
        c[1] = BodyPartType.Regular;
        c[2] = BodyPartType.Regular;

        _bodyPartsToSelect[0] = a;
        _bodyPartsToSelect[1] = b;
        _bodyPartsToSelect[2] = c;
    }

    private void NewPlayerSelectCallback(char l, char r)
    {
        _leftInputDisplay.text = $"{l}";
        _rightInputDisplay.text = $"{r}";

        _playerCount++;
        _playerNameDisplay.text = $"Player 0{_playerCount}";

        _selectWormPanel.SetActive(true);
        _newPlayerSelect = true;

        GameManager.OnInitialSetupSelected?.Invoke(_bodyPartsToSelect[_preSetIndex], _playerCount);
    }

    private void ResetPlayerCallback()
    {
        _playerCount = 0;
        _selectWormPanel.SetActive(false);
        _newPlayerSelect = false;
    }

    private void MoveSelectionToLeft()
    {
        if (_preSetIndex <= 0)
        { return; }

        _preSetsDisplay[_preSetIndex].SetActive(false);

        _preSetIndex--;
        _wormDescriptionDisplay.text = _preSetsDescriptions[_preSetIndex];
        _preSetsDisplay[_preSetIndex].SetActive(true);

        GameManager.OnInitialSetupSelected?.Invoke(_bodyPartsToSelect[_preSetIndex], _playerCount);
    }

    private void MoveSelectionToRight()
    {
        if (_preSetIndex >= 2)
        { return; }

        _preSetsDisplay[_preSetIndex].SetActive(false);

        _preSetIndex++;
        _wormDescriptionDisplay.text = _preSetsDescriptions[_preSetIndex];
        _preSetsDisplay[_preSetIndex].SetActive(true);

        GameManager.OnInitialSetupSelected?.Invoke(_bodyPartsToSelect[_preSetIndex], _playerCount);
    }

    private void ReturnToGameCallback()
    {
        GameManager.OnGamePause?.Invoke();
    }

    private void GoToMainMenuCallback()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    private void ExitGameCallback()
    {
        Application.Quit();
    }

    private void GameOverCallback()
    {
        _gameOverPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= StartGameCallback;
        GameManager.OnRegisterNewPlayer -= NewPlayerSelectCallback;
        GameManager.OnResetPlayerList -= ResetPlayerCallback;
        GameManager.OnGameOver -= GameOverCallback;
    }
}
