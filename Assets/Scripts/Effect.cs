using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [Header("Effect")]
    public GameObject _particleEffectPrefab;

    private void Start()
    {
        GameObject _particleEffect = Instantiate(_particleEffectPrefab);
        _particleEffect.transform.position = transform.position;
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
