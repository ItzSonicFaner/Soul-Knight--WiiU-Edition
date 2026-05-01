using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WiiU;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _health;
    [SerializeField] private int _maxArmor;
    [SerializeField] private int _armor;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _mana;
    [SerializeField] private float _speed;
    [SerializeField] private float _enemiesRange;
    [SerializeField] private float _armorRegenerationTime;
    [SerializeField] private float _armorMaxRegenerationTime;
    [SerializeField] private LayerMask _enemiesLayer;

    [Header("Movement")]
    [SerializeField] private Transform _camera;
    [SerializeField] private float _cameraFollowSpeed;
    private Vector2 _input;
    private int _lastMoveDirection;
    private float _horizontal;
    private float _vertical;
    private bool _flipLocked;
    private bool _flipped;

    [Header("Weapon")]
    [SerializeField] private GameObject _weapon;
    [SerializeField] private AudioSource _shootSound;
    [SerializeField] private int _manaDecreaseAmount;
    [SerializeField] private float _maxReloadTime;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _idleShootAngle;

    [Header("Bullet")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private int _bulletDamage;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletLifetime;

    [Header("UI")]
    [SerializeField] private Image[] _healthFill;
    [SerializeField] private Image[] _armorFill;
    [SerializeField] private Image[] _manaFill;
    [SerializeField] private Text[] _healthText;
    [SerializeField] private Text[] _armorText;
    [SerializeField] private Text[] _manaText;

    [Header("Enemy")]
    private Enemy _enemy;

    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _health = _maxHealth;
        _armor = _maxArmor;
        _mana = _maxMana;

        UpdateHealth();
        UpdateArmor();
        UpdateMana();

        _camera.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, -10.0f);
    }

    private void Start()
    {
        UnityEngine.WiiU.AudioSourceOutput.Assign(_shootSound, AudioOutput.GamePad);
    }

    private void Update()
    {
        Collider2D[] _enemies = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y + 0.5f), _enemiesRange, _enemiesLayer);

        if (_enemies.Length > 0 && _enemy == null)
            _enemy = _enemies[UnityEngine.Random.Range(0, _enemies.Length)].gameObject.GetComponent<Enemy>();
        else if (_enemies.Length == 0 && _enemy != null)
            _enemy = null;

        float _minDistance = Mathf.Infinity;

        foreach (Collider2D _enemyCollider in _enemies)
        {
            Vector2 _directionToEnemy = _enemyCollider.transform.position - transform.position;

            if (!_flipped && _directionToEnemy.x < 0) continue;
            if (_flipped && _directionToEnemy.x > 0) continue;

            float _distance = _directionToEnemy.sqrMagnitude;

            if (_distance < _minDistance)
            {
                _minDistance = _distance;
            }
        }

        if (Application.isEditor)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            if (Input.GetMouseButton(0))
                Shoot();
        }
        else
        {
            GamePadState _gamePad = GamePad.access.state;
            _input.x = _gamePad.lStick.x;
            _input.y = _gamePad.lStick.y;

            if(_gamePad.IsPressed(GamePadButton.A))
                Shoot();
        }

        if (_reloadTime > 0)
            _reloadTime -= Time.deltaTime;

        if (_armor < _maxArmor)
        {
            if (_armorRegenerationTime > 0)
                _armorRegenerationTime -= Time.deltaTime;
            else if (_armorRegenerationTime <= 0)
            {
                _armor++;
                _armor = Mathf.Clamp(_armor, 0, _maxArmor);

                if (_armor < _maxArmor)
                    _armorRegenerationTime = 1.5f;

                UpdateArmor();
            }
        }
        else if (_armor == _maxArmor)
            _armorRegenerationTime = _armorMaxRegenerationTime;
    }

    private void FixedUpdate()
    {
        Vector3 _targetPosition = Vector3.zero;

        if (_enemy != null)
        {
            Vector3 _playerPosition = transform.position;
            Vector3 _enemyPosition = _enemy.transform.position;
            Vector3 _center = (_playerPosition + _enemyPosition) / 2f;

            _targetPosition = new Vector3(_center.x, _center.y + 0.5f, -10.0f);
        }
        else
            _targetPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, -10.0f);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, _targetPosition, _cameraFollowSpeed * Time.deltaTime);

        if (_enemy != null)
        {
            Vector2 _direction = (_enemy.transform.position - _weapon.transform.position).normalized;

            float _x = _flipped ? _direction.x * -1 : _direction.x;
            float _y = _flipped ? _direction.y * -1 : _direction.y;
            float _angle = Mathf.Atan2(_y, _x) * Mathf.Rad2Deg;

            _weapon.transform.rotation = Quaternion.Euler(0, 0, _angle);

            float _directionToEnemyX = _enemy.transform.position.x - transform.position.x;
            bool _shouldFlip = (!_flipped && _directionToEnemyX < 0) || (_flipped && _directionToEnemyX > 0);

            if (_shouldFlip)
            {
                if (_input.x == 0 || Mathf.Sign(_input.x) != Mathf.Sign(_directionToEnemyX))
                {
                    if (!_flipLocked)
                    {
                        Flip();
                        _flipLocked = true;
                    }
                }
            }
        }

        if (_input.x != 0)
        {
            int _currentDirection = _input.x > 0 ? 1 : -1;

            if (_currentDirection != _lastMoveDirection)
            {
                _flipLocked = false;
                _lastMoveDirection = _currentDirection;
            }
        }

        if (_input.x != 0 || _input.y != 0)
        {
            _animator.SetBool("isWalking", true);

            float _x = _flipped ? _input.x * -1 : _input.x;
            float _y = _flipped ? _input.y * -1 : _input.y;

            if (_enemy == null)
                _weapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(_y, _x) * Mathf.Rad2Deg);
        }
        else
            _animator.SetBool("isWalking", false);

        if (!_flipLocked)
        {
            if ((_input.x < 0 && !_flipped) || (_input.x > 0 && _flipped))
                Flip();
        }

        _rigidbody.velocity = _input * _speed;
    }

    private void Flip()
    {
        Vector3 _newScale = transform.localScale;
        _newScale.x *= -1;
        transform.localScale = _newScale;
        _flipped = !_flipped;
    }

    public void Shoot()
    {
        if (_reloadTime > 0 || _mana <= 0)
            return;

        _animator.SetTrigger("Attack");

        GameObject _bullet = Instantiate(_bulletPrefab);
        Bullet _bulletComponent = _bullet.GetComponent<Bullet>();

        if (!Application.isEditor)
        {
            byte[] _pattern = { 0xF0, 0xF0, 0x0F };
            GamePad.access.ControlMotor(_pattern, _pattern.Length * 8);
        }

        _shootSound.Play();

        if (_manaDecreaseAmount > 0)
        {
            _mana -= _manaDecreaseAmount;
            _mana = Mathf.Clamp(_mana, 0, _maxMana);
            UpdateMana();
        }

        _bullet.transform.position = _bulletSpawnPoint.position;
        _bulletComponent._bulletSpeed = _bulletSpeed;
        _bulletComponent._bulletLifetime = _bulletLifetime;
        _bulletComponent._idleShootAngle = _idleShootAngle;
        _bulletComponent._bulletDamage = _bulletDamage;
        _bulletComponent._shooter = Bullet.Shooter.Player;

        if (_input.x == 0 && _input.y == 0)
            _bulletComponent._standing = true;

        _bulletComponent._direction = _flipped ? -_weapon.transform.right : _weapon.transform.right;
        _reloadTime = _maxReloadTime;
        _armorRegenerationTime = _armorMaxRegenerationTime;
    }

    private void UpdateHealth()
    {
        for (int _i = 0; _i < _healthFill.Length; _i++)
        {
            _healthFill[_i].fillAmount = (float)_health / (float)_maxHealth;
            _healthText[_i].text = _health + "/" + _maxHealth;
        }
    }

    private void UpdateArmor()
    {
        for (int _i = 0; _i < _armorFill.Length; _i++)
        {
            _armorFill[_i].fillAmount = (float)_armor / (float)_maxArmor;
            _armorText[_i].text = _armor + "/" + _maxArmor;
        }
    }

    private void UpdateMana()
    {
        for (int _i = 0; _i < _manaFill.Length; _i++)
        {
            _manaFill[_i].fillAmount = (float)_mana / (float)_maxMana;
            _manaText[_i].text = _mana + "/" + _maxMana;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 0, 0, 100);
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 0.5f, 0.0f), _enemiesRange);
    }

    public void TakeDamage(int _value)
    {
        if (_armor == 0)
        {
            _health -= _value;
            _health = Mathf.Clamp(_health, 0, _maxHealth);
            UpdateHealth();
        }
        else if (_armor > 0)
        {
            _armor -= _value;
            _armor = Mathf.Clamp(_armor, 0, _maxArmor);
            UpdateArmor();
        }

        _armorRegenerationTime = _armorMaxRegenerationTime;
    }

    public Transform GetCamera()
    {
        return _camera;
    }

    public AudioSource GetShootSound()
    {
        return _shootSound;
    }
}
