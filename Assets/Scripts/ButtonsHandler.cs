using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using WiiU = UnityEngine.WiiU;

public class ButtonsHandler : MonoBehaviour
{
    [Header("Camera's")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _gamepadCamera;

    [Header("UI")]
    [SerializeField] private GameObject _gamepadUI;
    [SerializeField] private GameObject _tvUI;

    [Header("Settings")]
    public bool _selectingCharacter;

    [Header("Components")]
    private Player _player;
    private Base _base;

    private void Awake()
    {
        GetPlayer();

        _base = FindObjectOfType<Base>();
        SwitchMode("Awake");
    }

    public void GetPlayer()
    {
        _player = FindObjectOfType<Player>();
    }

    public void SwitchMode(string _type)
    {
        if ((!_gamepadUI && !_tvUI) || (!_camera && !_gamepadCamera) || _selectingCharacter)
            return;

        if (_player == null)
            GetPlayer();

        if (_type.ToLower() == "gamepad" || (_base._mode == Base.Mode.GamePad && _type.ToLower() == "awake"))
        {
            _camera.targetDisplay = 1;
            _gamepadCamera.targetDisplay = 0;

            _gamepadUI.SetActive(true);
            _tvUI.SetActive(true);

            _base._mode = Base.Mode.GamePad;
            WiiU.AudioSourceOutput.SetMainAudioOutput(WiiU.AudioOutput.GamePad);
        }
        else if (_type.ToLower() == "tv")
        {
            _camera.targetDisplay = 0;
            _gamepadCamera.targetDisplay = 1;

            _gamepadUI.SetActive(false);
            _tvUI.SetActive(false);

            _base._mode = Base.Mode.TV;
            WiiU.AudioSourceOutput.SetMainAudioOutput(WiiU.AudioOutput.TV);
        }
    }

    public void Leave()
    {
        SceneManager.LoadScene("Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Lobby");
    }


    public static void StaticStartGame()
    {
        SceneManager.LoadScene("Lobby");
    }
}