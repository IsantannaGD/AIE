using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [SerializeField] private int _submitCounter;
    [SerializeField] private int _resetCounter;

    [SerializeField] private bool _keyOneRegistered;
    [SerializeField] private bool _keyTwoRegistered;
    [SerializeField] private string _keyOneRegisteredString;
    [SerializeField] private string _keyTwoRegisteredString;
    [SerializeField] private int _keysCount;

    [SerializeField] private float _holdTimer;

    [SerializeField] private bool _cooldown;

    [SerializeField] private string _currentLeftInput;
    [SerializeField] private string _currentRightInput;

    public string CurrentLeftInput => _currentLeftInput;
    public string CurrentRightInput => _currentRightInput;

    private void Update()
    {
        if (!GameManager.Instance.GameStarted)
        {
            if (Input.GetKeyDown("enter"))
            {
                _submitCounter++;
            
                if (_submitCounter == 2)
                {
                    GameManager.OnGameStart?.Invoke();
                }
            };

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _resetCounter++;

                if (_resetCounter == 2)
                {
                    GameManager.OnResetPlayerList?.Invoke();
                    _currentLeftInput = string.Empty;
                    _currentRightInput = string.Empty;
                    _resetCounter = 0;
                }
            }

            if (Input.anyKey)
            {
                if (string.IsNullOrWhiteSpace(Input.inputString) || _cooldown)
                    return;

                if (!_keyOneRegistered && Input.inputString != _keyTwoRegisteredString)
                {
                    _keyOneRegisteredString = Input.inputString.Filter(true, true, false, false, false).ToLower();

                    if (string.IsNullOrWhiteSpace(_keyOneRegisteredString))
                        return;

                    _keyOneRegistered = true;
                    _keysCount++;
                }
                else if (!_keyTwoRegistered && Input.inputString != _keyOneRegisteredString)
                {
                    _keyTwoRegisteredString = Input.inputString.Filter(true, true, false, false, false).ToLower();

                    if (string.IsNullOrWhiteSpace(_keyTwoRegisteredString))
                        return;

                    _keyTwoRegistered = true;
                    _keysCount++;
                }
            }

            if (_keyOneRegistered && Input.GetKeyUp(_keyOneRegisteredString))
            {
                _keysCount--;
                _keyOneRegistered = false;
                _keyOneRegisteredString = string.Empty;
                _holdTimer = 0;
            }

            if (_keyTwoRegistered && Input.GetKeyUp(_keyTwoRegisteredString))
            {
                _keysCount--;
                _keyTwoRegistered = false;
                _keyTwoRegisteredString = string.Empty;
                _holdTimer = 0;
            }

            if (_keysCount == 2)
            {
                _holdTimer += Time.deltaTime;

                if (_holdTimer > 1f)
                {
                    GameManager.OnRegisterNewPlayer?.Invoke(_keyOneRegisteredString[0], _keyTwoRegisteredString[0]);
                    ResetCounters();
                }
            }
        }

        if (!GameManager.Instance.GameOver && GameManager.Instance.GameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !_cooldown)
            {
                GameManager.OnGamePause?.Invoke();
                _cooldown = true;
                StartCoroutine(CooldownRoutine());
            }
        }
    }

    private void ResetCounters()
    {
        _currentLeftInput = _keyOneRegisteredString;
        _currentRightInput = _keyTwoRegisteredString;

        _cooldown = true;
        _keysCount = 0;
        _keyOneRegistered = false;
        _keyTwoRegistered = false;
        _keyOneRegisteredString = string.Empty;
        _keyTwoRegisteredString = string.Empty;
        _holdTimer = 0f;

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(2f);
        _cooldown = false;
    }
}
