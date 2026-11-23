using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public enum Mode
    {
        TV,
        GamePad
    }

    [Header("Currencies")]
    public int _gems;

    [Header("Mode")]
    public Mode _mode;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Gems"))
            _gems = PlayerPrefs.GetInt("Gems");
    }
}
