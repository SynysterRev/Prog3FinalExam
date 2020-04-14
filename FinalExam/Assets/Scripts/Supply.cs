using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : MonoBehaviour
{
    [SerializeField] Material[] mats;
    Ressources typeSupply;
    int IDSpawningRoom;
    public bool IsInTheHold = false;
    // Start is called before the first frame update

    public void Initialize(int _idRoom, Ressources _type)
    {
        IDSpawningRoom = _idRoom;
        typeSupply = _type;
        GetComponent<MeshRenderer>().material = mats[(int)_type];
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
