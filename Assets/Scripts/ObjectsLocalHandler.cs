using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsLocalHandler : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private string _type;
    private float _timer = 5;

    private void Update()
    {
        if (_type.ToLower() == "loading")
        {
            if (_timer > 0)
                _timer -= Time.deltaTime;

            if (_timer <= 0)
                gameObject.SetActive(false);
        }
    }
}
