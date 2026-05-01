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

    [Header("Components")]
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    public void SwitchMode(string _type)
    {
        if (_type.ToLower() == "gamepad")
        {
            _camera.targetDisplay = 1;
            _gamepadCamera.targetDisplay = 0;

            _gamepadUI.SetActive(true);
            _tvUI.SetActive(true);

            WiiU.AudioSourceOutput.Unassign(_player.GetShootSound(), WiiU.AudioOutput.GamePad);
            WiiU.AudioSourceOutput.Assign(_player.GetShootSound(), WiiU.AudioOutput.TV);
            WiiU.AudioSourceOutput.SetMainAudioOutput(WiiU.AudioOutput.GamePad);
        }
        else if (_type.ToLower() == "tv")
        {
            _camera.targetDisplay = 0;
            _gamepadCamera.targetDisplay = 1;

            _gamepadUI.SetActive(false);
            _tvUI.SetActive(false);

            WiiU.AudioSourceOutput.Unassign(_player.GetShootSound(), WiiU.AudioOutput.TV);
            WiiU.AudioSourceOutput.Assign(_player.GetShootSound(), WiiU.AudioOutput.GamePad);
            WiiU.AudioSourceOutput.SetMainAudioOutput(WiiU.AudioOutput.TV);
        }
    }
}