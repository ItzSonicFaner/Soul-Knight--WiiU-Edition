using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefab : MonoBehaviour
{
    [Header("Room")]
    public RoomType _roomType;
    [SerializeField] private int _maxWaves;
    [SerializeField] private int _minEnemies;
    [SerializeField] private int _maxEnemies;
    [SerializeField] private Vector2 _spawnZone;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject[] _fences;

    [Header("Waves")]
    private bool _fightStarted;
    private bool _fightEnded;
    private int _currentWave = 1;
    private List<GameObject> _enemies = new List<GameObject>();

    [Header("Player")]
    private Player _player;
    private LevelGenerator _levelGenerator;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _levelGenerator = FindObjectOfType<LevelGenerator>();
    }

    private void FixedUpdate()
    {
        if (_enemies.Count == 0 && _currentWave == _maxWaves && !_fightEnded)
        {
            _fightEnded = true;
            UnlockFences();
        }
        else if (_enemies.Count == 0 && _currentWave < _maxWaves && !_fightEnded && _roomType == RoomType.Fight)
        {
            _currentWave++;
            Spawn(true);
        }
    }

    public void Spawn(bool _searchPlayer = false)
    {
        int _enemiesToSpawn = UnityEngine.Random.Range(_minEnemies, _maxEnemies);

        for (int _i = 0; _i < _enemiesToSpawn; _i++)
        {
            GameObject _enemy = Instantiate(_levelGenerator.GetEnemies()[UnityEngine.Random.Range(0, _levelGenerator.GetEnemies().Length)]);
            Vector3 _spawnPosition = transform.position;
            _spawnPosition.x += UnityEngine.Random.Range((_spawnZone.x / 2) * -1, (_spawnZone.x / 2));
            _spawnPosition.y += UnityEngine.Random.Range((_spawnZone.y / 2) * -1, (_spawnZone.y / 2)) + 0.75f;
            _enemy.transform.position = _spawnPosition;
            _enemy.GetComponent<Enemy>().SetRoom(this);
            _enemy.GetComponent<Enemy>()._searchPlayer = _searchPlayer;
            _enemies.Add(_enemy);
        }
    }

    public void SetSpawnpoint(Vector3 _position)
    {
        _player.transform.position = _position;
        _player.GetCamera().position = new Vector3(_position.x, _position.y + 0.5f, -10.0f);
    }

    public void LockFences()
    {
        for (int _i = 0; _i < _fences.Length; _i++)
        {
            _fences[_i].SetActive(true);
        }
    }

    public void UnlockFences()
    {
        for (int _i = 0; _i < _fences.Length; _i++)
        {
            _fences[_i].SetActive(false);
        }
    }

    public void RemoveEnemy(GameObject _enemy)
    {
        _enemies.Remove(_enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 _position = transform.position;
        _position.y += 0.75f;

        Gizmos.color = new Color32(255, 0, 0, 100);
        Gizmos.DrawCube(_position, _spawnZone);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_roomType == RoomType.Fight)
        {
            if (_collision.CompareTag("Player") && !_fightStarted)
            {
                for (int _i = 0; _i < _enemies.Count; _i++)
                    _enemies[_i].GetComponent<Enemy>()._searchPlayer = true;

                LockFences();

                _fightStarted = true;
            }
        }
    }
}
