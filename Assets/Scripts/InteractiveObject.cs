using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WiiU;

public class InteractiveObject : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _gui;
    [SerializeField] private GameObject _menu;

    [Header("Settings")]
    [SerializeField] private string _objectType;

    [Header("Camera")]
    [SerializeField] private Camera _gamepadCamera;

    [Header("Components")]
    private Base _base;

    private void Start()
    {
        _base = FindObjectOfType<Base>();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            _gui.SetActive(true);
            _collision.GetComponent<Player>().SetCanAttack(false);
        }
    }

    private void OnTriggerStay2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    switch(_objectType)
                    {
                        case "Start":
                            SceneManager.LoadScene("Game");
                            break;
                        case "Continue":
                            SceneManager.LoadScene("Game");
                            _base._gems += 50;
                            PlayerPrefs.SetInt("Gems", _base._gems);
                            break;
                        case "Fridge":
                            _menu.SetActive(true);
                            break;
                    }
                }
            }
            else
            {
                if (GamePad.access.state.IsTriggered(GamePadButton.A))
                {
                    switch (_objectType)
                    {
                        case "Start":
                            SceneManager.LoadScene("Game");
                            break;
                        case "Continue":
                            SceneManager.LoadScene("Game");
                            _base._gems += 50;
                            PlayerPrefs.SetInt("Gems", _base._gems);
                            break;
                        case "Fridge":
                            _menu.SetActive(true);
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            _gui.SetActive(false);
            _collision.GetComponent<Player>().SetCanAttack(true);
        }
    }
}
