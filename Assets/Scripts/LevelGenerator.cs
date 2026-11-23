using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private Room[] _rooms;
    [SerializeField] private GameObject _portal;
    //private List<string> _letters = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    //private List<string> _numbers = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    //private List<string> _types = new List<string>() { "!", "#", "$", "?" };
    //[SerializeField] private bool _random;

    [Header("Patterns")]
    [SerializeField] private Pattern[] _patterns;

    [Header("Enemies")]
    [SerializeField] private GameObject[] _levelKnightEnemies;

    private void Awake()
    {
        int _patternId = UnityEngine.Random.Range(0, _patterns.Length);

        for (int _i = 0; _i < _patterns[_patternId]._lines.Length; _i++)
        {
            string _letterId = _patterns[_patternId]._lines[_i]._letterId;
            string[] _letterIds = _letterId.Split('|');

            string _numberId = _patterns[_patternId]._lines[_i]._numberId;
            string[] _numberIds = _numberId.Split('|');

            string _roomType = _patterns[_patternId]._lines[_i]._roomType;
            string[] _roomTypes = _roomType.Split('|');

            for (int _j = 0; _j < _letterIds.Length; _j++ )
            {
                for (int _a = 0; _a < _rooms.Length; _a++)
                {
                    if (_letterIds[_j] == _rooms[_a].GetId())
                    {
                        GameObject _room = Instantiate(_rooms[_a].GetRooms()[int.Parse(_numberIds[_j])]);
                        RoomType _convertedRoomType = RoomType.Fight;

                        switch(_roomTypes[_j])
                        {
                            case "!":
                                _convertedRoomType = RoomType.Spawn;
                                break;
                            case "#":
                                _convertedRoomType = RoomType.Fight;
                                break;
                            case "?":
                                _convertedRoomType = RoomType.End;
                                break;
                        }

                        Vector2 _roomPosition = _room.transform.position;
                        _roomPosition.x += 35 * _j;
                        _roomPosition.y -= 35 * _i;
                        _room.transform.position = _roomPosition;
                        _room.GetComponent<RoomPrefab>()._roomType = _convertedRoomType;

                        if (_convertedRoomType == RoomType.Spawn)
                            _room.GetComponent<RoomPrefab>().SetSpawnpoint(_roomPosition);
                        else if (_convertedRoomType == RoomType.Fight)
                            _room.GetComponent<RoomPrefab>().Spawn();
                        else if (_convertedRoomType == RoomType.End)
                        {
                            GameObject _newPortal = Instantiate(_portal);
                            _newPortal.transform.SetParent(_room.transform);
                            _newPortal.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
                        }
                    }
                }
            }
        }
    }

    public Room[] GetRooms()
    {
        return _rooms;
    }

    public Pattern[] GetPatterns()
    {
        return _patterns;
    }

    public GameObject[] GetEnemies()
    {
        return _levelKnightEnemies;
    }
}

[Serializable]
public class Room
{
    [Header("Room")]
    [SerializeField] private string _id;
    [SerializeField] private GameObject[] _roomPrefabs;

    public string GetId()
    {
        return _id;
    }

    public GameObject[] GetRooms()
    {
        return _roomPrefabs;
    }
}

[Serializable]
public class Pattern
{
    [Header("Lines")]
    public Line[] _lines;
}

[Serializable]
public class Line
{
    [Header("Line")]
    public string _letterId;
    public string _numberId;
    public string _roomType;
}

public enum RoomType
{
    Spawn,
    Fight,
    End
}