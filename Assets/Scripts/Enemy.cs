using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _health;
    [SerializeField] private float _speed;
    [SerializeField] private float _distanceFromPlayer;
    [SerializeField] private float _playersRange;
    [SerializeField] private float _chasingRange;
    [SerializeField] private LayerMask _playersLayer;
    private bool _flipped;
    public bool _searchPlayer;

    [Header("Weapon")]
    [SerializeField] private GameObject _weapon;
    [SerializeField] private float _maxReloadTime;
    [SerializeField] private float _reloadTime;

    [Header("Bullet")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private int _bulletDamage;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletLifetime;

    [Header("Components")]
    private RoomPrefab _room;
    //private Rigidbody2D _rigidbody;
    //private Animator _animator;

    [Header("Player")]
    private Player _player;

    private void Awake()
    {
        //_rigidbody = GetComponent<Rigidbody2D>();
        //_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_searchPlayer)
        {
            Collider2D[] _players = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y + 0.5f), _playersRange, _playersLayer);

            if (_players.Length > 0 && _player == null)
                _player = _players[UnityEngine.Random.Range(0, _players.Length)].gameObject.GetComponent<Player>();
            else if (_players.Length == 0 && _player != null)
            {
                _players = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y + 0.5f), _chasingRange + _playersRange, _playersLayer);

                if (_players.Length == 0)
                    _player = null;
            }
        }

        if (_reloadTime > 0)
            _reloadTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            float _distance = Vector2.Distance(transform.position, _player.transform.position);
            Vector2 _direction = (_player.transform.position - _weapon.transform.position).normalized;

            float _x = _flipped ? _direction.x * -1 : _direction.x;
            float _y = _flipped ? _direction.y * -1 : _direction.y;
            float _angle = Mathf.Atan2(_y, _x) * Mathf.Rad2Deg;

            _weapon.transform.rotation = Quaternion.Euler(0, 0, _angle);

            if ((_direction.x < 0 && !_flipped) || (_direction.x > 0 && _flipped))
                Flip();

            if (_distance > _distanceFromPlayer)
                transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
            else
                Shoot();
        }
    }

    public void Shoot()
    {
        if (_reloadTime > 0)
            return;

        GameObject _bullet = Instantiate(_bulletPrefab);
        Bullet _bulletComponent = _bullet.GetComponent<Bullet>();

        _bullet.transform.position = _bulletSpawnPoint.position;
        _bulletComponent._bulletSpeed = _bulletSpeed;
        _bulletComponent._bulletLifetime = _bulletLifetime;
        _bulletComponent._bulletDamage = _bulletDamage;

        _bulletComponent._direction = _flipped ? -_weapon.transform.right : _weapon.transform.right;
        _reloadTime = _maxReloadTime;
    }

    private void Flip()
    {
        Vector3 _newScale = transform.localScale;
        _newScale.x *= -1;
        transform.localScale = _newScale;
        _flipped = !_flipped;
    }

    public void TakeDamage(int _value)
    {
        if (!_searchPlayer)
            return;

        _health -= _value;
        _health = Mathf.Clamp(_health, 0, _maxHealth);

        if (_health <= 0)
        {
            _room.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetRoom(RoomPrefab _newRoom)
    {
        _room = _newRoom;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 0, 0, 100);
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 0.5f, 0.0f), _playersRange);

        Gizmos.color = new Color32(0, 255, 0, 100);
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 0.5f, 0.0f), _chasingRange + _playersRange);
    }
}
