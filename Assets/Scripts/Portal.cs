using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WiiU;

public class Portal : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _finishLevel;

    [Header("Settings")]
    [SerializeField] private bool _isGame;
    private bool _gotReward;

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
            _finishLevel.SetActive(true);
            _collision.GetComponent<Player>().SetCanAttack(false);
        }
    }

    private void OnTriggerStay2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            if(Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene("Game");

                    if (_gotReward == false && _isGame == true)
                    {
                        _gotReward = true;
                        _base._gems += 50;
                        PlayerPrefs.SetInt("Gems", _base._gems);
                    }
                }
            }
            else
            {
                if (GamePad.access.state.IsTriggered(GamePadButton.A))
                {
                    SceneManager.LoadScene("Game");

                    if (_gotReward == false && _isGame == true)
                    {
                        _gotReward = true;
                        _base._gems += 50;
                        PlayerPrefs.SetInt("Gems", _base._gems);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            _finishLevel.SetActive(false);
            _collision.GetComponent<Player>().SetCanAttack(true);
        }
    }
}
