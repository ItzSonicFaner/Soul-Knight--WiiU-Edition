using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private Room[] _rooms;
    [SerializeField] private GameObject _portal;
    //[SerializeField] private bool _random;

    [Header("Patterns")]
    [SerializeField] private Pattern[] _patterns;

    [Header("Enemies")]
    [SerializeField] private GameObject[] _levelKnightEnemies;

    private void Awake()
    {
        int _patternId = UnityEngine.Random.Range(0, _patterns.Length);

        for (int _i = 0; _i < _patterns[_patternId].GetLines().Length; _i++)
        {
            for (int _j = 0; _j < _patterns[_patternId].GetLines()[_i].GetLines().Length; _j++)
            {
                Line _line = _patterns[_patternId].GetLines()[_i].GetLines()[_j];

                for (int _a = 0; _a < _rooms.Length; _a++)
                {
                    if (_line.GetValue().Length > 0)
                    {
                        if (_line.GetValue()[0] == _rooms[_a].GetId()[0])
                        {
                            GameObject _room = null;

                            if (_line.GetValue().Length > 1)
                            {
                                if (_line.GetValue().Length == 2)
                                    _room = Instantiate(_rooms[_a].GetRooms()[int.Parse(_line.GetValue()[1].ToString())]);
                                else if (_line.GetValue().Length > 2)
                                {
                                    string _resultedNumber = "";

                                    for (int _b = 1; _b < _line.GetValue().Length; _b++)
                                    {
                                        _resultedNumber += _line.GetValue()[1];

                                        if (_b + 1 == _line.GetValue().Length)
                                            _room = Instantiate(_rooms[_a].GetRooms()[int.Parse(_resultedNumber)]);
                                    }
                                }
                            }
                            else if (_line.GetValue().Length == 1)
                                _room = Instantiate(_rooms[_a].GetRooms()[0]);

                            Vector2 _roomPosition = _room.transform.position;
                            _roomPosition.x += 35 * _j;
                            _roomPosition.y -= 35 * _i;
                            _room.transform.position = _roomPosition;
                            _room.GetComponent<RoomPrefab>()._roomType = _line.GetRoomType();

                            if (_line.GetRoomType() == RoomType.Spawn)
                                _room.GetComponent<RoomPrefab>().SetSpawnpoint(_roomPosition);
                            else if (_line.GetRoomType() == RoomType.Fight)
                                _room.GetComponent<RoomPrefab>().Spawn();
                            else if (_line.GetRoomType() == RoomType.End)
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
    [SerializeField] private HorizontalLine[] _lines;

    public HorizontalLine[] GetLines()
    {
        return _lines;
    }
}

[Serializable]
public class HorizontalLine
{
    [Header("Lines")]
    [SerializeField] private string _name;
    [SerializeField] private Line[] _lines;

    public string GetName()
    {
        return _name;
    }

    public Line[] GetLines()
    {
        return _lines;
    }
}

[Serializable]
public class Line
{
    [Header("Line")]
    [SerializeField] private string _name;
    [SerializeField] private string _value;
    [SerializeField] private RoomType _roomType;

    public string GetName()
    {
        return _name;
    }

    public string GetValue()
    {
        return _value;
    }

    public RoomType GetRoomType()
    {
        return _roomType;
    }
}

public enum RoomType
{
    Fight,
    Spawn,
    Chest,
    End
}