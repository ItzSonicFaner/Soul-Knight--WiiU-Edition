using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIType
    {
        Gems,
        Health,
        Armor,
        Mana
    }

    [Header("UI")]
    [SerializeField] private UIType _uiType;

    [Header("Components")]
    private Base _base;
    private Characters _characters;
    private Player _player;

    private void Start()
    {
        SearchPlayer();
        _base = FindObjectOfType<Base>();
        _characters = Characters.Instance;

        LoadValues();
    }

    public void SetPlayer(Player _newPlayer)
    {
        _player = _newPlayer;
    }

    public void SearchPlayer()
    {
        _player = FindObjectOfType<Player>();
    }

    public void LoadValues()
    {
        if(_uiType == UIType.Gems)
        {
            Text _text = GetComponent<Text>();
            _text.text = _base._gems.ToString();
        }
        else if(_uiType == UIType.Health || _uiType == UIType.Armor || _uiType == UIType.Mana)
        {
            Image _fill = GetComponent<Image>();
            Text _text = _fill.GetComponentInChildren<Text>();

            if (_uiType == UIType.Health)
            {
                _fill.fillAmount = (float)_player.GetHealth() / (float)_player.GetMaxHealth();
                _text.text = _player.GetHealth() + "/" + _player.GetMaxHealth();
            }
            else if (_uiType == UIType.Armor)
            {
                _fill.fillAmount = (float)_player.GetArmor() / (float)_player.GetMaxArmor();
                _text.text = _player.GetArmor() + "/" + _player.GetMaxArmor();
            }
            else if (_uiType == UIType.Mana)
            {
                _fill.fillAmount = (float)_player.GetMana() / (float)_player.GetMaxMana();
                _text.text = _player.GetMana() + "/" + _player.GetMaxMana();
            }
        }
    }

    public UIType GetUIType()
    {
        return _uiType;
    }
}
