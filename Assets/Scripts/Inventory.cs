using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private Slot[] _slots;

    private void Awake()
    {
        if (!FindObjectOfType<ButtonsHandler>()._selectingCharacter)
            LoadSlots();
    }

    public void LoadSlots()
    {
        GameObject _inventoryData = null;
        InventoryData _inventoryDataComponent = null;

        if (GameObject.Find("_inventoryData"))
        {
            _inventoryData = GameObject.Find("_inventoryData");
            _inventoryDataComponent = _inventoryData.GetComponent<InventoryData>();
        }
        else
            return;

        foreach(SlotData _slotData in _inventoryDataComponent._slotsData)
        {
            for (int _i = 0; _i < _slots.Length; _i++)
            {
                if (_slots[_i].GetSlotId() == _slotData._slotId)
                    _slots[_i]._weaponId = _slotData._weaponId;
            }
        }

        UpdateSlots();
    }

    public void UpdateSlots()
    {
        foreach(Slot _slot in _slots)
        {
            if (_slot._locked)
                _slot.GetSlot().SetActive(false);
            else
                _slot.GetSlot().SetActive(true);

            for (int _i = 0; _i < Characters.Instance.GetWeapons().Length; _i++)
            {
                if (Characters.Instance.GetWeapons()[_i].GetComponent<Weapon>().GetId() == _slot._weaponId)
                {
                    _slot.GetSlotIcon().color = new Color32(255, 255, 255, 255);
                    _slot.GetSlotIcon().sprite = Characters.Instance.GetWeapons()[_i].GetComponent<SpriteRenderer>().sprite;
                    break;
                }

                if (_i + 1 == Characters.Instance.GetWeapons().Length)
                    _slot.GetSlotIcon().color = new Color32(255, 255, 255, 0);
            }
            _slot.GetSlotIcon().SetNativeSize();
        }
    }

    public void SelectButton(GameObject _button)
    {
        foreach (Slot _slot in _slots)
        {
            if (_slot.GetSlot() == _button)
            {
                Player _player = FindObjectOfType<Player>();
                _player.ChangeWeapon(_slot._weaponId);
                break;
            }
        }
    }

    public Slot[] GetSlots()
    {
        return _slots;
    }
}

[Serializable]
public class Slot
{
    [Header("Slot")]
    [SerializeField] private int _slotId;
    [SerializeField] private Image _slotIcon;
    [SerializeField] private GameObject _slot;
    public int _weaponId;
    public bool _locked;

    public int GetSlotId()
    {
        return _slotId;
    }

    public Image GetSlotIcon()
    {
        return _slotIcon;
    }

    public GameObject GetSlot()
    {
        return _slot;
    }

    public void SaveData(int _newWeaponId)
    {
        GameObject _inventoryData = null;
        InventoryData _inventoryDataComponent = null;

        if (!GameObject.Find("_inventoryData"))
        {
            _inventoryData = GameObject.Instantiate(new GameObject());
            _inventoryData.name = "_inventoryData";
            _inventoryDataComponent = _inventoryData.AddComponent<InventoryData>();
        }
        else
        {
            _inventoryData = GameObject.Find("_inventoryData");
            _inventoryDataComponent = _inventoryData.GetComponent<InventoryData>();
        }

        if (_inventoryDataComponent._slotsData.Count > 0)
        {
            for (int _i = 0; _i < _inventoryDataComponent._slotsData.Count; _i++)
            {
                if (_inventoryDataComponent._slotsData[_i]._slotId == _slotId)
                {
                    _inventoryDataComponent._slotsData[_i]._weaponId = _newWeaponId;
                    break;
                }

                if (_i + 1 == _inventoryDataComponent._slotsData.Count)
                {
                    SlotData _newSlotData = new SlotData();
                    _newSlotData._slotId = _slotId;
                    _newSlotData._weaponId = _newWeaponId;
                    _inventoryDataComponent._slotsData.Add(_newSlotData);
                }
            }
        }
        else
        {
            SlotData _newSlotData = new SlotData();
            _newSlotData._slotId = _slotId;
            _newSlotData._weaponId = _newWeaponId;
            _inventoryDataComponent._slotsData.Add(_newSlotData);
        }

        _weaponId = _newWeaponId;
    }
}