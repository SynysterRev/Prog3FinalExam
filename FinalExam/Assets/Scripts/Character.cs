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
    bool[] IDAlreadyUsed;

    Transform[] usedDiePositions;
    bool isMyTurn;
    public bool WantToMove;
    public bool WantToMovePlane;
    int indexDieMove;
    int id;
    int numberRoll;
    int numberDieUsed;

    bool lerpNeeded;
    Vector3 nextPosition;
    float timer;

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
        usedDiePositions = _usedDiePositions;
        colorCharact = (ColorCharacter)characterProfession;
        InitDice();
        IDAlreadyUsed = new bool[6];
        for (int i = 0; i < 6; ++i)
        {
            IDAlreadyUsed[i] = false;
        }
    }

    /* public bool RollDice()
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
                     int idDie = GetUnusedID();
                     dice[i].Initialize(id, idDie);
                     dice[i].OnStop += MoveDieOutOfBoard;
                 }
                 position.x = Random.Range(-20.0f, 20.0f);
                 position.z = Random.Range(-5.0f, 5.0f);
             }
             numberRoll--;
             return numberRoll > 0;
         }
         return false;
     }*/

    void InitDice()
    {
        GameObject go;
        Vector3 position = Vector3.zero + Vector3.up * 101.0f;
        for (int i = 0; i < numberDiceToRoll; ++i)
        {
            go = Instantiate(prefabDie, position, Quaternion.identity);
            if (go.GetComponent<Die>())
            {
                dice.Add(go.GetComponent<Die>());
                dice[i].Initialize(id, i);
                dice[i].OnStop += MoveDieOutOfBoard;
            }
        }
    }

    public bool RollDice()
    {
        if (isMyTurn && numberRoll > 0)
        {
            //DeleteNotLockedDice();
            //GameObject go;
            Vector3 position = Vector3.zero + Vector3.up * 4.0f;
            position.x = Random.Range(-20.0f, 20.0f);
            position.z = Random.Range(-5.0f, 5.0f);
            for (int i = 0; i < numberDiceToRoll; ++i)
            {
                if (dice[i] != null && !dice[i].isLocked && !dice[i].isUseToMove && !dice[i].HasBeenUsed)
                {
                    dice[i].RollDie(position);
                    position.x = Random.Range(-20.0f, 20.0f);
                    position.z = Random.Range(-5.0f, 5.0f);
                }
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
                IDAlreadyUsed[dice[i].id] = false;
                dice.RemoveAt(i);
                Destroy(go);
                i--;
            }
        }
    }

    void DeleteNotUseDice()
    {
        Vector3 position = Vector3.zero + Vector3.up * 101.0f;
        for (int i = 0; i < dice.Count; ++i)
        {
            if (dice[i] != null && !dice[i].isUseForSupply)
            {
                dice[i].ResetNoneSupplyDie(position);
            }
        }
    }

    void DiceManagement()
    {
        if (!WantToMovePlane && !Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
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
                        if (diceUsedOnSupply.Contains(dice[indexDieMove]))
                        {
                            dice[index].AddedToListToPossibleSupply(false);
                            diceUsedOnSupply.Remove(dice[index]);
                        }
                        WantToMove = dice[index].UseForMovement(false);
                            Debug.Log(WantToMove);
                    }
                    else if (indexDieMove == index)
                    {
                        indexDieMove = -1;
                        WantToMove = dice[index].UseForMovement(false);
                        Debug.Log(WantToMove);
                    }
                }
            }
        }

        if (!WantToMove && !Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Die die = hit.collider.GetComponent<Die>();
                if (dice.Contains(die) && die.IsUpperFacePlane())
                {
                    int index = dice.IndexOf(die);
                    if (indexDieMove == -1)
                    {
                        indexDieMove = index;
                        if (diceUsedOnSupply.Contains(dice[indexDieMove]))
                        {
                            dice[index].AddedToListToPossibleSupply(false);
                            diceUsedOnSupply.Remove(dice[index]);
                        }
                        WantToMovePlane = dice[index].UseForMovement(true);
                        Debug.Log(WantToMovePlane);
                    }
                    else if (indexDieMove == index)
                    {
                        indexDieMove = -1;
                        WantToMovePlane = dice[index].UseForMovement(true);
                        Debug.Log(WantToMovePlane);
                    }
                }
            }
        }

        if (!WantToMove && !WantToMovePlane)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
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
                                dice[index].AddedToListToPossibleSupply(true);
                                diceUsedOnSupply.Add(dice[index]);
                                Debug.Log("dé ajouté");
                            }
                            else
                            {
                                dice[index].AddedToListToPossibleSupply(false);
                                diceUsedOnSupply.Remove(dice[index]);
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (board.rooms[IDCurrentRoom].isHold)
                {
                    board.rooms[IDCurrentRoom].LockDieForHold(diceUsedOnSupply);
                }
                else
                {
                    board.rooms[IDCurrentRoom].LockDiceForSupply(diceUsedOnSupply);
                }
                diceUsedOnSupply.Clear();
            }
        }
    }

    public bool IsDieInSupplyList()
    {
        return diceUsedOnSupply.Count > 0;
    }
    public void MovePlayer(Vector3 _position, int _idRoom)
    {
        if (WantToMove)
        {
            nextPosition = _position;
            lerpNeeded = true;
            //transform.position = _position;
            WantToMove = false;
            dice[indexDieMove].gameObject.layer = 2;
            dice[indexDieMove].ReturnDieToOwner();
            IDAlreadyUsed[dice[indexDieMove].id] = false;
            indexDieMove = -1;
            IDCurrentRoom = _idRoom;
        }
    }

    public void DeleteDiceUsedToMovePlane()
    {
        if (WantToMovePlane)
        {
            WantToMovePlane = false;
            dice[indexDieMove].gameObject.layer = 2;
            dice[indexDieMove].ReturnDieToOwner();
            IDAlreadyUsed[dice[indexDieMove].id] = false;
            indexDieMove = -1;
        }
    }

    void MoveDieOutOfBoard(int _idDie)
    {
        dice[_idDie].MoveDice(usedDiePositions[_idDie].position, false);
    }

    public void EndTurn()
    {
        if (isMyTurn)
        {
            isMyTurn = false;
            WantToMove = false;
            WantToMovePlane = false;
            DeleteNotUseDice();
            numberRoll = 3;
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
        if(lerpNeeded)
        {
            timer += Time.deltaTime * 5.0f;
            transform.position = Vector3.Lerp(transform.position, nextPosition, timer);
            if (timer >= 1.0f)
            {
                lerpNeeded = false;
                timer = 0.0f;
            }
        }

        if (isMyTurn && !board.TimeUp)
        {
            DiceManagement();
        }
    }
}

/*public class SpecialEffect
{
    public class 
}*/
