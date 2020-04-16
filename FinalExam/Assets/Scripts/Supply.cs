using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : MonoBehaviour
{
    [SerializeField] Material[] mats;
    Ressources typeSupply;
    Room spawningRoom;
    public int IDSpawningRoom;
    public bool IsInTheHold = false;
    int idPlaceInRoom;
    // Start is called before the first frame update

    public Ressources TypeSupply { get => typeSupply; }

    public void Initialize(int _idRoom, Ressources _type, Room _spawningRoom, int _idSpawn)
    {
        IDSpawningRoom = _idRoom;
        typeSupply = _type;
        spawningRoom = _spawningRoom;
        idPlaceInRoom = _idSpawn;
        GetComponent<MeshRenderer>().material = mats[(int)_type];
    }

    public void MoveSupply(Vector3 _position, bool _toHold, bool _toSpawningRoom)
    {
        IsInTheHold = _toHold;
        transform.position = _position;
        if (_toSpawningRoom)
        {
            transform.position = spawningRoom.positionRessources[idPlaceInRoom].position;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
