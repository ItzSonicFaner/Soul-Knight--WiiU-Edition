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
    [SerializeField] private float _skillReload;
    [SerializeField] private float _armorRegenerationTime;
    [SerializeField] private float _armorMaxRegenerationTime;
    [SerializeField] private LayerMask _enemiesLayer;
    [SerializeField] private Character _character;

    [Header("Movement")]
    [SerializeField] private Transform _camera;
    [SerializeField] private float _cameraFollowSpeed;
    private GameObject _secondWeapon;
    private Vector2 _input;
    private int _lastMoveDirection;
    private float _horizontal;
    private float _vertical;
    private bool _flipLocked;
    private bool _flipped;
    private bool _canAttack;

    [Header("Enemy")]
    private Enemy _enemy;

    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Weapon _currentWeapon;
    private Characters _characters;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _canAttack = true;

        _camera.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, -10.0f);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _characters = Characters.Instance;

        foreach (CharacterData _someCharacter in _characters.GetCharacters())
        {
            if (_someCharacter.GetCharacter().GetId() == _characters._characterId)
            {
                _character = _someCharacter.GetCharacter();
                break;
            }
        }

        CharacterUpgrade _characterUpgrade = _character.GetCharacterUpgrades()[_characters._characterUpgradeId];
        _animator.runtimeAnimatorController = _character.GetAnimation();
        _characters.SetData(this, _characterUpgrade.GetHealth(), _characterUpgrade.GetArmor(), _characterUpgrade.GetMana());

        foreach (GameObject _otherWeapon in _characters.GetWeapons())
        {
            if (_otherWeapon.GetComponent<Weapon>().GetId() == _characters._weaponId)
            {
                GameObject _newWeapon = Instantiate(_otherWeapon, transform.Find("Weapon"));
                _currentWeapon = _newWeapon.GetComponent<Weapon>();
                break;
            }
        }
    }

    public void ChangeWeapon(int _newWeaponId)
    {
        if (_currentWeapon.GetId() == _newWeaponId)
            return;
        StartCoroutine(StopSkill(true, _character.GetSkillName()));

        foreach (Transform _previousWeapon in transform.Find("Weapon"))
            Destroy(_previousWeapon.gameObject);
        _currentWeapon = null;
        _characters.ChangeWeapon(_newWeaponId);

        foreach (GameObject _otherWeapon in _characters.GetWeapons())
        {
            if (_otherWeapon.GetComponent<Weapon>().GetId() == _characters._weaponId)
            {
                GameObject _newWeapon = Instantiate(_otherWeapon, transform.Find("Weapon"));
                _currentWeapon = _newWeapon.GetComponent<Weapon>();
                break;
            }
        }
    }

    public void Ability(string _name)
    {
        GameObject _skillEffect = null;
        if (_name.ToLower() == "dual wield")
        {
            GameObject _otherWeapon = transform.Find("OtherWeapon").gameObject;
            GameObject _mainWeapon = transform.Find("Weapon").GetChild(0).gameObject;
            Characters _characters = Characters.Instance;

            foreach (GameObject _weapon in _characters.GetWeapons())
            {
                if (_weapon.GetComponent<Weapon>().GetId() == _mainWeapon.GetComponent<Weapon>().GetId())
                {
                    GameObject _newOtherWeapon = Instantiate(_weapon, _otherWeapon.transform);
                    _secondWeapon = _newOtherWeapon;
                    _secondWeapon.GetComponent<Weapon>().SetMaxReloadTime(0.05f);
                    _secondWeapon.GetComponent<SpriteRenderer>().sortingOrder = 0;
                    break;
                }
            }

            _skillEffect = Instantiate(_character.GetSkillEffect(), transform.Find("SkillEffect"));
            if (_character.GetSkillSound())
            {
                _skillEffect.AddComponent<AudioSource>().clip = _character.GetSkillSound();
                _skillEffect.GetComponent<AudioSource>().Play();
            }
        }
        StartCoroutine(StopSkill(false , _name, _skillEffect));
    }

    private IEnumerator StopSkill(bool _instant, string _skillName, GameObject _skillEffect = null)
    {
        if (!_instant)
            yield return new WaitForSeconds(_characters.GetCharacters()[_characters._characterId].GetCharacter().GetCharacterUpgrades()[_characters._characterUpgradeId].GetSkillDuration());
        if (_skillName.ToLower() == "dual wield")
        {
            _skillReload = _characters.GetCharacters()[_characters._characterId].GetCharacter().GetCharacterUpgrades()[_characters._characterUpgradeId].GetSkillReload();
            Destroy(_secondWeapon);

            if (_skillEffect)
                Destroy(_skillEffect);
            else if (!_skillEffect && transform.Find("SkillEffect").childCount > 0)
                Destroy(transform.Find("SkillEffect").GetChild(0).gameObject);
        }
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
                StartCoroutine(Shoot());

            if (Input.GetKeyDown(KeyCode.Q) && _skillReload <= 0)
                Ability(_character.GetSkillName());
        }
        else
        {
            GamePadState _gamePad = GamePad.access.state;
            _input.x = _gamePad.lStick.x;
            _input.y = _gamePad.lStick.y;

            if (_gamePad.IsPressed(GamePadButton.A) || _gamePad.IsPressed(GamePadButton.B))
                StartCoroutine(Shoot());

            if ((_gamePad.IsPressed(GamePadButton.Y) || _gamePad.IsPressed(GamePadButton.X)) && _skillReload <= 0)
                Ability(_character.GetSkillName());
        }

        if (_skillReload > 0)
            _skillReload -= Time.deltaTime;

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
            {
                _currentWeapon.transform.parent.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(_y, _x) * Mathf.Rad2Deg);

                if (_secondWeapon)
                    _secondWeapon.transform.parent.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(_y, _x) * Mathf.Rad2Deg);
            }
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

    public void SetCanAttack(bool _value)
    {
        _canAttack = _value;
    }

    public void Flip()
    {
        Vector3 _newScale = transform.localScale;
        _newScale.x *= -1;
        transform.localScale = _newScale;
        _flipped = !_flipped;
    }

    public IEnumerator Shoot(Weapon _weaponToShoot = null)
    {
        if (!_weaponToShoot)
            _weaponToShoot = _currentWeapon;

        if (_weaponToShoot.GetReloadTime() > 0 || _mana <= 0 || (_canAttack == false && (_weaponToShoot && _secondWeapon && _weaponToShoot != _secondWeapon.GetComponent<Weapon>())))
            yield break;

        _canAttack = false;
        if (_weaponToShoot.GetComponent<Animator>())
            _weaponToShoot.GetComponent<Animator>().SetTrigger("Attack");

        if (!Application.isEditor)
        {
            byte[] _pattern = { 0xF0, 0xF0, 0x0F };
            GamePad.access.ControlMotor(_pattern, _pattern.Length * 8);
        }

        _weaponToShoot.GetShootSound().Play();

        if (_weaponToShoot.GetManaDecreaseAmount() > 0)
        {
            _mana -= _weaponToShoot.GetManaDecreaseAmount();
            _mana = Mathf.Clamp(_mana, 0, _maxMana);
        }

        _weaponToShoot.CreateBullet(_flipped, _input);
        _weaponToShoot.SetReloadTime(_weaponToShoot.GetMaxReloadTime());
        _armorRegenerationTime = _armorMaxRegenerationTime;

        if (_secondWeapon && (_weaponToShoot && _secondWeapon && _weaponToShoot != _secondWeapon.GetComponent<Weapon>()))
        {
            yield return new WaitForSeconds(_currentWeapon.GetMaxReloadTime());
            if (_secondWeapon)
                StartCoroutine(Shoot(_secondWeapon.GetComponent<Weapon>()));
        }
        else
        {
            if (_weaponToShoot && _secondWeapon && _weaponToShoot == _secondWeapon.GetComponent<Weapon>())
                yield return new WaitForSeconds(_currentWeapon.GetMaxReloadTime());
            _canAttack = true;
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
        }
        else if (_armor > 0)
        {
            _armor -= _value;
            _armor = Mathf.Clamp(_armor, 0, _maxArmor);
        }

        _armorRegenerationTime = _armorMaxRegenerationTime;
    }

    public void SetMaxHealth(int _value)
    {
        _maxHealth = _value;
    }

    public void SetMaxArmor(int _value)
    {
        _maxArmor = _value;
    }

    public void SetMaxMana(int _value)
    {
        _maxMana = _value;
    }

    public void SetHealth(int _value)
    {
        _health = _value;
    }

    public void SetArmor(int _value)
    {
        _armor = _value;
    }

    public void SetMana(int _value)
    {
        _mana = _value;
    }

    public void AddHealth(int _value)
    {
        _health += _value;
    }

    public void AddArmor(int _value)
    {
        _armor += _value;
    }

    public void AddMana(int _value)
    {
        _mana += _value;
    }

    public void AddMaxHealth(int _value)
    {
        _maxHealth += _value;
    }

    public void AddMaxArmor(int _value)
    {
        _maxArmor += _value;
    }

    public void AddMaxMana(int _value)
    {
        _maxMana += _value;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public int GetHealth()
    {
        return _health;
    }

    public int GetMaxArmor()
    {
        return _maxArmor;
    }

    public int GetArmor()
    {
        return _armor;
    }

    public int GetMaxMana()
    {
        return _maxMana;
    }

    public int GetMana()
    {
        return _mana;
    }

    public void SetIsFlipLocked(bool _value)
    {
        _flipLocked = _value;
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public Vector2 GetInput()
    {
        return _input;
    }

    public Transform GetCamera()
    {
        return _camera;
    }

    public Enemy GetEnemy()
    {
        return _enemy;
    }

    public bool GetIsFlipped()
    {
        return _flipped;
    }

    public bool GetIsFlipLocked()
    {
        return _flipLocked;
    }
}
