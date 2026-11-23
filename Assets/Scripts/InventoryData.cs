using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData : MonoBehaviour
{
    [Header("Slots")]
    public List<SlotData> _slotsData = new List<SlotData>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

[Serializable]
public class SlotData
{
    [Header("Slot Data")]
    public int _slotId;
    public int _weaponId;
}
