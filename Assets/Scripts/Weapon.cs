using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WiiU;

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField]
    private GameObject _weapon;
    [SerializeField]
    private AudioSource _shootSound;
    [SerializeField]
    private int _id;
    [SerializeField]
    private int _manaDecreaseAmount;
    [SerializeField]
    private float _maxReloadTime;
    [SerializeField]
    private float _reloadTime;
    [SerializeField]
    private float _idleShootAngle;

    [Header("Bullet")]
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private Transform _bulletSpawnPoint;
    [SerializeField]
    private int _bulletDamage;
    [SerializeField]
    private float _bulletSpeed;
    [SerializeField]
    private float _bulletLifetime;

    [Header("Components")]
    private Player _player;

    private void Awake()
    {
        _player = transform.parent.parent.GetComponent<Player>();
        _reloadTime = _maxReloadTime;
    }

    private void Start()
    {
        _weapon = transform.parent.gameObject;
        UnityEngine.WiiU.AudioSourceOutput.Assign(_shootSound, AudioOutput.GamePad);
    }

    private void Update()
    {
        if (_reloadTime > 0)
            _reloadTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_player.GetEnemy() != null)
        {
            Vector2 _direction = (_player.GetEnemy().transform.position - _weapon.transform.position).normalized;

            float _x = _player.GetIsFlipped() ? _direction.x * -1 : _direction.x;
            float _y = _player.GetIsFlipped() ? _direction.y * -1 : _direction.y;
            float _angle = Mathf.Atan2(_y, _x) * Mathf.Rad2Deg;

            _weapon.transform.rotation = Quaternion.Euler(0, 0, _angle);
        }
    }

    public void CreateBullet(bool _flipped, Vector2 _input)
    {
        GameObject _bullet = Instantiate(_bulletPrefab);
        Bullet _bulletComponent = _bullet.GetComponent<Bullet>();

        _bullet.transform.position = _bulletSpawnPoint.position;
        _bulletComponent._bulletSpeed = _bulletSpeed;
        _bulletComponent._bulletLifetime = _bulletLifetime;
        _bulletComponent._idleShootAngle = _idleShootAngle;
        _bulletComponent._bulletDamage = _bulletDamage;
        _bulletComponent._shooter = Bullet.Shooter.Player;

        if (_input.x == 0 && _input.y == 0)
            _bulletComponent._standing = true;

        if (_weapon)
            _bulletComponent._direction = _flipped ? -_weapon.transform.right : _weapon.transform.right;
        else
            Destroy(_bullet);
    }

    public GameObject GetWeapon()
    {
        return _weapon;
    }

    public AudioSource GetShootSound()
    {
        return _shootSound;
    }

    public void SetMaxReloadTime(float _value)
    {
        _maxReloadTime = _value;
    }

    public void SetReloadTime(float _value)
    {
        _reloadTime = _value;
    }

    public int GetId()
    {
        return _id;
    }

    public float GetReloadTime()
    {
        return _reloadTime;
    }

    public float GetMaxReloadTime()
    {
        return _maxReloadTime;
    }

    public int GetManaDecreaseAmount()
    {
        return _manaDecreaseAmount;
    }
}
