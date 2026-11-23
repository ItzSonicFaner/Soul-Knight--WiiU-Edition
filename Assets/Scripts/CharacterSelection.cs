using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [Header("Character")]
    public int _characterId;
    public int _characterUpgradeId;

    [Header("StatsUI")]
    [SerializeField] private Text _name;
    [SerializeField] private Text _health;
    [SerializeField] private Text _armor;
    [SerializeField] private Text _mana;
    [SerializeField] private Text _criticalChance;

    [Header("SkillUI")]
    [SerializeField] private Text _skillName;
    [SerializeField] private Text _skillDescription;
    [SerializeField] private Image _skillIcon;

    [Header("UpgradeUI")]
    [SerializeField] private Text _cost;
    [SerializeField] private GameObject _upgradeButton;
    [SerializeField] private Image[] _stars;
    [SerializeField] private Color32 _available = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 _unavailable = new Color32(75, 75, 75, 255);

    [Header("GUISwitcher")]
    [SerializeField] private GameObject _gamepadGUI;
    [SerializeField] private GameObject _textBar;
    [SerializeField] private GameObject _chosenCharacter;

    [Header("Characters Objects")]
    [SerializeField] private CharacterObjects[] _charactersObjects;

    [Header("Player")]
    [SerializeField] private GameObject _player;

    [Header("Camera")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _gamepadCamera;
    [SerializeField] private Vector3 _cameraStartPosition;
    [SerializeField] private Vector3 _gamepadCameraStartPosition;

    [Header("Components")]
    private Characters _characters;
    private Base _base;

    private void Start()
    {
        _characters = Characters.Instance;
        _base = FindObjectOfType<Base>();
        _gamepadCamera.transform.position = _cameraStartPosition;
    }

    public void LoadCharacter(int _newCharacterId)
    {
        if (_newCharacterId >= 0)
        {
            _characters.ChooseCharacter(_newCharacterId);
            return;
        }

        Vector3 _cameraPosition = Vector3.zero;
        CharacterObjects _characterObject = null;
        foreach (CharacterObjects _someCharacterObject in _charactersObjects)
        {
            if (_someCharacterObject.GetCharacter().GetId() == _characterId)
            {
                _characterObject = _someCharacterObject;
                _cameraPosition = new Vector3(_someCharacterObject.GetCharacterObject().transform.position.x,
                    _someCharacterObject.GetCharacterObject().transform.position.y + 0.9f, -10.0f);
                break;
            }
        }

        Character _character = null;
        CharacterData _characterData = null;
        foreach (CharacterData _someCharacter in _characters.GetCharacters())
        {
            if (_someCharacter.GetCharacter().GetId() == _characterId)
            {
                _character = _someCharacter.GetCharacter();
                _characterData = _someCharacter;
                break;
            }
        }
        CharacterUpgrade _characterUpgrade = _character.GetCharacterUpgrades()[_characterUpgradeId];

        _name.text = _character.GetName();
        _health.text = _characterUpgrade.GetHealth().ToString();
        _armor.text = _characterUpgrade.GetArmor().ToString();
        _mana.text = _characterUpgrade.GetMana().ToString();
        _criticalChance.text = _characterUpgrade.GetCriticalChance().ToString();

        _skillName.text = _character.GetSkillName();
        _skillDescription.text = _character.GetSkillDescription();
        _skillIcon.sprite = _character.GetSkillIcon();

        if (_characterUpgradeId + 1 < _characterData.GetUpgrades().Count)
            _cost.text = _characterUpgrade.GetCost().ToString();
        else
            _upgradeButton.SetActive(false);

        _characterObject.GetCharacterObject().SetActive(true);
        _characterObject.GetCharacterUI().SetActive(false);

        for (int _i = 0; _i < _stars.Length; _i++)
        {
            if (_i + 1 <= _characterUpgradeId)
                _stars[_i].color = _available;
            else
                _stars[_i].color = _unavailable;
        }

        _textBar.SetActive(false);
        _chosenCharacter.SetActive(true);

        _camera.transform.position = _cameraPosition;
        _camera.orthographicSize = 1.75f;

        _gamepadCamera.transform.position = _cameraPosition;
        _gamepadCamera.orthographicSize = 1.75f;
    }

    public void Back()
    {
        CharacterObjects _characterObject = null;
        foreach (CharacterObjects _someCharacterObject in _charactersObjects)
        {
            if (_someCharacterObject.GetCharacter().GetId() == _characterId)
            {
                _characterObject = _someCharacterObject;
                break;
            }
        }

        _characterObject.GetCharacterObject().SetActive(false);
        _characterObject.GetCharacterUI().SetActive(true);

        _textBar.SetActive(true);
        _chosenCharacter.SetActive(false);

        _upgradeButton.SetActive(true);

        _camera.transform.position = _cameraStartPosition;
        _camera.orthographicSize = 6.0f;

        _gamepadCamera.transform.position = _cameraStartPosition;
        _gamepadCamera.orthographicSize = 6.0f;
    }

    public void Upgrade()
    {
        Character _character = null;
        CharacterData _characterData = null;
        foreach (CharacterData _someCharacter in _characters.GetCharacters())
        {
            if (_someCharacter.GetCharacter().GetId() == _characterId)
            {
                _character = _someCharacter.GetCharacter();
                _characterData = _someCharacter;
                break;
            }
        }
        CharacterUpgrade _characterUpgrade = _character.GetCharacterUpgrades()[_characterUpgradeId];

        if (_base._gems < _characterUpgrade.GetCost())
            return;

        if (_characterUpgradeId + 1 < _characterData.GetUpgrades().Count)
        {
            _base._gems -= _characterUpgrade.GetCost();
            PlayerPrefs.SetInt("Gems", _base._gems);

            foreach (UIManager _ui in FindObjectsOfType<UIManager>())
            {
                if (_ui.GetUIType() == UIManager.UIType.Gems)
                    _ui.LoadValues();
            }

            _characterData._upgradeId++;
            _characterUpgradeId++;
            _characterUpgrade = _character.GetCharacterUpgrades()[_characterUpgradeId];

            _characters.LoadUpgrades(_characterId);

            for (int _i = 0; _i < _stars.Length; _i++)
            {
                if (_i + 1 <= _characterUpgradeId)
                    _stars[_i].color = _available;
                else
                    _stars[_i].color = _unavailable;
            }

            _name.text = _character.GetName();
            _health.text = _characterUpgrade.GetHealth().ToString();
            _armor.text = _characterUpgrade.GetArmor().ToString();
            _mana.text = _characterUpgrade.GetMana().ToString();
            _criticalChance.text = _characterUpgrade.GetCriticalChance().ToString();
        }

        if (_characterUpgradeId + 1 >= _characterData.GetUpgrades().Count)
        {
            _upgradeButton.SetActive(false);
            return;
        }

        _cost.text = _characterUpgrade.GetCost().ToString();
    }

    public void Select()
    {
        Character _character = null;
        foreach (CharacterData _someCharacter in _characters.GetCharacters())
        {
            if (_someCharacter.GetCharacter().GetId() == _characterId)
            {
                _character = _someCharacter.GetCharacter();
                break;
            }
        }
        CharacterUpgrade _characterUpgrade = _character.GetCharacterUpgrades()[_characterUpgradeId];

        CharacterObjects _characterObject = null;
        foreach (CharacterObjects _someCharacterObject in _charactersObjects)
        {
            if (_someCharacterObject.GetCharacter().GetId() == _characterId)
            {
                _characterObject = _someCharacterObject;
                break;
            }
        }
        Inventory _inventory = transform.parent.GetComponent<Inventory>();

        _characters._characterId = _characterId;
        _characters._characterUpgradeId = _characterUpgradeId;
        _characters._weaponId = _characterUpgrade.GetWeaponId();

        _characterObject.GetCharacterObject().transform.parent.gameObject.SetActive(false);
        _characters.SetData(_player.GetComponent<Player>(), _characterUpgrade.GetHealth(), _characterUpgrade.GetArmor(), _characterUpgrade.GetMana());
        _player.SetActive(true);

        _player.transform.position = _characterObject.GetCharacterObject().transform.parent.position;

        foreach (Slot _slot in _inventory.GetSlots())
        {
            if (_slot._weaponId < 0 && !_slot._locked)
            {
                _slot.SaveData(_characterUpgrade.GetWeaponId());
                break;
            }
        }

        foreach (Slot _slot in _inventory.GetSlots())
        {
            if (_slot._weaponId < 0 && !_slot._locked)
            {
                _slot.SaveData(2);
                break;
            }
        }
        _inventory.UpdateSlots();

        _camera.transform.position = _cameraStartPosition;
        _camera.orthographicSize = 6.0f;

        _gamepadCamera.transform.position = _gamepadCameraStartPosition;
        _gamepadCamera.orthographicSize = 6.0f;

        _gamepadGUI.SetActive(true);
        gameObject.SetActive(false);

        ButtonsHandler _buttonsHandler = FindObjectOfType<ButtonsHandler>();
        _buttonsHandler._selectingCharacter = false;
        _buttonsHandler.SwitchMode("Awake");

        foreach(CharacterObjects _someCharacterObject in _charactersObjects)
        {
            if (_someCharacterObject != _characterObject)
            {
                _someCharacterObject.GetCharacterObject().SetActive(true);
                _someCharacterObject.GetCharacterUI().SetActive(false);
            }
        }
    }
}

[Serializable]
public class CharacterObjects
{
    [Header("Character")]
    [SerializeField] private Character _character;
    [SerializeField] private GameObject _characterObject;
    [SerializeField] private GameObject _characterUI;

    public Character GetCharacter()
    {
        return _character;
    }

    public GameObject GetCharacterObject()
    {
        return _characterObject;
    }

    public GameObject GetCharacterUI()
    {
        return _characterUI;
    }
}