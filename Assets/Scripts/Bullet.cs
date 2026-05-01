using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum Shooter
    {
        Enemy,
        Player
    }

    [Header("Bullet")]
    public GameObject _effectPrefab;
    public Vector3 _direction;
    public int _bulletDamage;
    public float _bulletSpeed;
    public float _bulletLifetime;
    public float _idleShootAngle;
    public bool _flip;
    public bool _standing;
    public Shooter _shooter;

    private void Start()
    {
        float _randomAngle = Random.Range(_idleShootAngle * -1, _idleShootAngle);
        float _z = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

        _z += _standing ? _randomAngle : 0.0f;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, _z);

        float _radians = _z * Mathf.Deg2Rad;
        _direction = new Vector3(Mathf.Cos(_radians), Mathf.Sin(_radians), 0f).normalized;
    }

    private void Update()
    {
        if (_bulletLifetime > 0)
            _bulletLifetime -= Time.deltaTime;

        if (_bulletLifetime <= 0)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += _direction * _bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Wall"))
        {
            if (_effectPrefab)
            {
                GameObject _effect = Instantiate(_effectPrefab);
                _effect.transform.position = transform.position;
            }

            Destroy(gameObject);
        }
        else if(_collision.gameObject.CompareTag("Enemy") && _shooter != Shooter.Enemy)
        {
            _collision.gameObject.GetComponent<Enemy>().TakeDamage(_bulletDamage);
            Destroy(gameObject);
        }
        else if (_collision.gameObject.CompareTag("Player") && _shooter != Shooter.Player)
        {
            _collision.gameObject.GetComponent<Player>().TakeDamage(_bulletDamage);
            Destroy(gameObject);
        }
    }
}
