using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    public void Start()
    {
        ParticleSystem _this = GetComponent<ParticleSystem>();
        Destroy(gameObject, _this.main.duration + _this.main.startLifetime.constantMax);
    }
}
