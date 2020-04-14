using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterProfession
{
    techician,
    flightPlanner,
    analyst,
    engineer
}

public class Character : MonoBehaviour
{
    [SerializeField] GameObject prefabDie;
    List<Die> dice = new List<Die>();
    int numberDiceToRoll;
    public string nameCharacter;
    string descriptionEffect;
    public CharacterProfession characterProfession;
    public int idRoomSpawn;
    int IDCurrentRoom;
    bool isMyTurn;
    int id;

    public void Initialize(Vector3 _position)
    {
        transform.position = _position;
        IDCurrentRoom = idRoomSpawn;
        isMyTurn = false;
        numberDiceToRoll = 6;
    }

    public void RollDice()
    {
        DeleteNotLockedDice();
        GameObject go;
        Vector3 position = Vector3.zero + Vector3.up * 4.0f;
        position.x = Random.Range(-20.0f, 20.0f);
        position.z = Random.Range(-5.0f, 5.0f);
        for (int i = dice.Count; i < dice.Count + numberDiceToRoll; ++i)
        {
            go = Instantiate(prefabDie, position, Quaternion.identity);
            if (go.GetComponent<Die>())
            {
                dice.Add(go.GetComponent<Die>());
                dice[i].Initialize(id);
            }
            position.x = Random.Range(-20.0f, 20.0f);
            position.z = Random.Range(-5.0f, 5.0f);
        }
    }

    void DeleteNotLockedDice()
    {
        for (int i = 0; i < dice.Count; ++i)
        {
            if(!dice[i].isLocked)
            {
                GameObject go = dice[i].gameObject;
                dice.RemoveAt(i);
                Destroy(go);
            }
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RollDice();
        }
    }
}

/*public class SpecialEffect
{
    public class 
}*/
