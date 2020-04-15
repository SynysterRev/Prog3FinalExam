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

public enum ColorCharacter
{
    green, 
    grey,
    red,
    blue
}

public class Character : MonoBehaviour
{
    [SerializeField] GameObject prefabDie;
    List<Die> dice = new List<Die>();
    List<Die> diceUsedOnSupply = new List<Die>();
    BoardGame board;
    ColorCharacter colorCharact;
    int numberDiceToRoll;
    public string nameCharacter;
    string descriptionEffect;
    public CharacterProfession characterProfession;
    public int idRoomSpawn;
    public int IDCurrentRoom;

    Transform[] usedDiePositions;
    bool isMyTurn;
    public bool WantToMove;
    int indexDieMove;
    int id;
    int numberRoll;
    int numberDieUsed;

    bool canUseDiceForSupply;

    public ColorCharacter ColorCharact { get => colorCharact; }

    public void Initialize(Vector3 _position, BoardGame _board, int _id, Transform[] _usedDiePositions)
    {
        transform.position = _position;
        IDCurrentRoom = idRoomSpawn;
        isMyTurn = false;
        numberDiceToRoll = 6;
        numberRoll = 3;
        board = _board;
        id = _id;
        indexDieMove = -1;
        numberDieUsed = 0;
        usedDiePositions = _usedDiePositions;
        colorCharact = (ColorCharacter)characterProfession;
    }

    public bool RollDice()
    {
        if (isMyTurn && numberRoll > 0)
        {
            DeleteNotLockedDice();
            GameObject go;
            Vector3 position = Vector3.zero + Vector3.up * 4.0f;
            position.x = Random.Range(-20.0f, 20.0f);
            position.z = Random.Range(-5.0f, 5.0f);
            int totalDice = dice.Count;
            for (int i = totalDice; i < numberDiceToRoll; ++i)
            {
                go = Instantiate(prefabDie, position, Quaternion.identity);
                if (go.GetComponent<Die>())
                {
                    dice.Add(go.GetComponent<Die>());
                    dice[i].Initialize(id, i);
                    int idDie = i;
                    dice[i].OnStop += MoveDieOutOfBoard;
                }
                position.x = Random.Range(-20.0f, 20.0f);
                position.z = Random.Range(-5.0f, 5.0f);
            }
            numberRoll--;
            return numberRoll > 0;
        }
        return false;
    }

    void DeleteNotLockedDice()
    {
        for (int i = 0; i < dice.Count; ++i)
        {
            if (!dice[i].isLocked && !dice[i].isUseToMove && dice[i] != null && !dice[i].HasBeenUsed)
            {
                GameObject go = dice[i].gameObject;
                dice.RemoveAt(i);
                Destroy(go);
                numberDieUsed--;
                i--;
            }
        }
    }

    void DeleteNotUseDice()
    {
        for (int i = 0; i < dice.Count; ++i)
        {
            if (!dice[i].isUseForSupply && dice[i] != null)
            {
                GameObject go = dice[i].gameObject;
                dice.RemoveAt(i);
                Destroy(go);
                i--;
            }
        }
    }

    void DiceManagement()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Die die = hit.collider.GetComponent<Die>();
                if (dice.Contains(die))
                {
                    int index = dice.IndexOf(die);
                    if (indexDieMove == -1)
                    {                   
                        indexDieMove = index;
                        WantToMove = dice[index].UseForMovement();
                        Debug.Log(WantToMove);
                    }
                    else if (indexDieMove == index)
                    {
                        indexDieMove = -1;
                        WantToMove = dice[index].UseForMovement();
                        Debug.Log(WantToMove);
                    }
                }
            }
        }

        if (!WantToMove)
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Die die = hit.collider.GetComponent<Die>();
                    if (dice.Contains(die))
                    {
                        int index = dice.IndexOf(die);
                        dice[index].UnlockLockDice();
                    }
                }
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Die die = hit.collider.GetComponent<Die>();
                    if (dice.Contains(die))
                    {
                        if (die.IsCorrectSupply(board.rooms[IDCurrentRoom].typeSupply))
                        {
                            int index = dice.IndexOf(die);
                            if (!diceUsedOnSupply.Contains(dice[index]))
                            {
                                diceUsedOnSupply.Add(dice[index]);
                                Debug.Log("dé ajouté");
                            }
                            else
                            {
                                diceUsedOnSupply.Remove(dice[index]);
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                board.rooms[IDCurrentRoom].LockDiceForSupply(diceUsedOnSupply);
                diceUsedOnSupply.Clear();
            }
        }
    }

    public void MovePlayer(Vector3 _position, int _idRoom)
    {
        if (WantToMove)
        {
            transform.position = _position;
            WantToMove = false;
            dice[indexDieMove].HasBeenUsed = true;
            dice[indexDieMove].gameObject.layer = 2;
            dice[indexDieMove].ReturnDieToOwner();
            indexDieMove = -1;
            IDCurrentRoom = _idRoom;
        }
    }

    void MoveDieOutOfBoard(int _idDie)
    {
        dice[_idDie].MoveDice(usedDiePositions[numberDieUsed].position, false);
        numberDieUsed++;
    }

    public void EndTurn()
    {
        if (isMyTurn)
        {
            isMyTurn = false;
            DeleteNotUseDice();
            numberRoll = 3;
            numberDieUsed = 0;
            diceUsedOnSupply.Clear();
        }
    }

    public void ActivateTurn()
    {
        if (!isMyTurn)
        {
            isMyTurn = true;
            DeleteNotUseDice();
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMyTurn)
        {
            DiceManagement();
        }
    }
}

/*public class SpecialEffect
{
    public class 
}*/
