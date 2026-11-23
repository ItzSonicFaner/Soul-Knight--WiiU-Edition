using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Characters : MonoBehaviour
{
    public static Characters Instance { get; private set; }

    [Header("Characters")]
    [SerializeField] private CharacterData[] _characters;
    public int _characterId;
    public int _characterUpgradeId;

    [Header("Weapons")]
    [SerializeField] private GameObject[] _weapons;
    public int _weaponId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (CharacterData _character in _characters)
        {
            _character.InitializeData();
            StartCoroutine(Init(_character));
        }
    }

    private IEnumerator Init(CharacterData _character)
    {
        yield return new WaitUntil(() => _character.GetUpgrades().Count == _character.GetCharacter().GetCharacterUpgrades().Length);
        if (PlayerPrefs.HasKey(_character.GetCharacter().GetName() + "_Upgrade"))
        {
            _character._upgradeId = PlayerPrefs.GetInt(_character.GetCharacter().GetName() + "_Upgrade");
            for (int _i = 1; _i <= _character._upgradeId; _i++)
                _character.GetUpgrades()[_i].SetIsHaving(true);
        }

        if (_character.GetUpgrades().Count > 0)
            LoadUpgrades(_character.GetCharacter().GetId());
    }

    public void ChooseCharacter(int _newCharacterId)
    {
        CharacterSelection _characterSelection = FindObjectOfType<CharacterSelection>();

        _characterSelection._characterId = _newCharacterId;
        _characterSelection._characterUpgradeId = _characters[_newCharacterId]._upgradeId;
        _characterSelection.LoadCharacter(-1);
    }

    public void SetData(Player _player, int _health, int _armor, int _mana)
    {
        _player.SetMaxHealth(_health);
        _player.SetMaxArmor(_armor);
        _player.SetMaxMana(_mana);

        _player.SetHealth(_health);
        _player.SetArmor(_armor);
        _player.SetMana(_mana);

        foreach (UIManager _ui in FindObjectsOfType<UIManager>())
        {
            if (_ui.GetUIType() != UIManager.UIType.Gems)
            {
                _ui.SetPlayer(_player);
                _ui.LoadValues();
            }
        }
    }

    public void ChangeWeapon(int _newWeaponId)
    {
        _weaponId = _newWeaponId;
    }

    public void LoadUpgrades(int _newCharacterId)
    {
        RestoreToDefaults(_newCharacterId);
        StartCoroutine(UpgradesLoading(_newCharacterId));
    }

    private IEnumerator UpgradesLoading(int _newCharacterId)
    {
        CharacterData _characterData = null;
        foreach (CharacterData _someCharacterData in _characters)
        {
            if (_someCharacterData.GetCharacter().GetId() == _newCharacterId)
            {
                _characterData = _someCharacterData;
                break;
            }
        }

        yield return new WaitUntil(() => _characterData.GetUpgrades().Count == _characterData.GetCharacter().GetCharacterUpgrades().Length);
        for (int _i = 1; _i <= _characterData._upgradeId; _i++)
            _characterData.GetUpgrades()[_i].SetIsHaving(true);

        PlayerPrefs.SetInt(_characterData.GetCharacter().GetName() + "_Upgrade", _characterData._upgradeId);
    }

    public void RestoreToDefaults(int _newCharacterId)
    {
        foreach (CharacterData _someCharacterData in _characters)
        {
            if (_someCharacterData.GetCharacter().GetId() == _newCharacterId)
            {
                _someCharacterData.InitializeData();
                break;
            }
        }
    }

    public GameObject[] GetWeapons()
    {
        return _weapons;
    }

    public CharacterData[] GetCharacters()
    {
        return _characters;
    }
}

[Serializable]
public class CharacterData
{
    [Header("Character")]
    [SerializeField] private Character _character;
    [SerializeField] private List<Upgrade> _upgrades = new List<Upgrade>();
    public int _upgradeId;

    public void InitializeData()
    {
        _upgrades.Clear();
        if (_character != null)
        {
            foreach (CharacterUpgrade _upgrade in _character.GetCharacterUpgrades())
                _upgrades.Add(new Upgrade());

            _upgrades[0].SetIsHaving(true);
        }
    }

    public Character GetCharacter()
    {
        return _character;
    }

    public List<Upgrade> GetUpgrades()
    {
        return _upgrades;
    }
}

[Serializable]
public class Upgrade
{
    [Header("Upgrade")]
    [SerializeField] private bool _isHaving;

    public void SetIsHaving(bool _value)
    {
        _isHaving = _value;
    }

    public bool GetIsHaving()
    {
        return _isHaving;
    }
}