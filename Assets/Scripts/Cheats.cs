using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [Header("UI Managers")]
    private UIManager[] _uiManagers;

    private void Awake()
    {
        _uiManagers = FindObjectsOfType<UIManager>();
    }

    public void GainGems(int _amount)
    {
        Base _base = FindObjectOfType<Base>();
        _base._gems += _amount;
        PlayerPrefs.SetInt("Gems", _base._gems);

        foreach (UIManager _ui in _uiManagers)
        {
            if (_ui.GetUIType() == UIManager.UIType.Gems)
            {
                _ui.LoadValues();
                break;
            }
        }
    }
}
