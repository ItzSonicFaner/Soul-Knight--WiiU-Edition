using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Character")]
public class Character : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] private string _name;
    [SerializeField] private int _id;
    [SerializeField] private RuntimeAnimatorController _animation;
    [SerializeField] private CharacterUpgrade[] _characterUpgrades;

    [Header("Skill")]
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillDescription;
    [SerializeField] private AudioClip _skillSound;
    [SerializeField] private GameObject _skillEffect;
    [SerializeField] private Sprite _skillIcon;

    public string GetName()
    {
        return _name;
    }

    public int GetId()
    {
        return _id;
    }

    public string GetSkillName()
    {
        return _skillName;
    }

    public string GetSkillDescription()
    {
        return _skillDescription;
    }

    public AudioClip GetSkillSound()
    {
        return _skillSound;
    }

    public RuntimeAnimatorController GetAnimation()
    {
        return _animation;
    }

    public Sprite GetSkillIcon()
    {
        return _skillIcon;
    }

    public GameObject GetSkillEffect()
    {
        return _skillEffect;
    }

    public CharacterUpgrade[] GetCharacterUpgrades()
    {
        return _characterUpgrades;
    }
}

[Serializable]
public class CharacterUpgrade
{
    [Header("Stats")]
    [SerializeField] private int _health;
    [SerializeField] private int _armor;
    [SerializeField] private int _mana;
    [SerializeField] private int _criticalChance;
    [SerializeField] private int _weaponId;
    [SerializeField] private float _skillReload;
    [SerializeField] private float _skillDuration;
    [SerializeField] private int _cost;

    public int GetHealth()
    {
        return _health;
    }

    public int GetArmor()
    {
        return _armor;
    }

    public int GetMana()
    {
        return _mana;
    }

    public int GetCriticalChance()
    {
        return _criticalChance;
    }

    public int GetWeaponId()
    {
        return _weaponId;
    }

    public float GetSkillReload()
    {
        return _skillReload;
    }

    public float GetSkillDuration()
    {
        return _skillDuration;
    }

    public int GetCost()
    {
        return _cost;
    }
}