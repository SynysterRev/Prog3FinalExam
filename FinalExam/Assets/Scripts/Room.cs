using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room : MonoBehaviour
{
    public delegate void DelegateIsLaunchingDieWaste();
    public event DelegateIsLaunchingDieWaste OnLaunch;
    public event DelegateIsLaunchingDieWaste OnStopLaunch;

    public delegate void DelegateWasteMaxReach();
    public event DelegateWasteMaxReach OnWasteMax;

    [SerializeField] GameObject supplyPrefab;
    [SerializeField] GameObject outlineHelp;
    [SerializeField] Shader shader;
    [SerializeField] int idRoomWaste;
    Material outlineToChange;
    public int idRoom;
    public string nameRoom;
    public Ressources typeSupply;
    public Transform[] positionPlayer;
    public Transform[] positionRessources;
    public Transform[] positionDices;
    public Section[] RessourcesRequired;
    public Die[] lockedDice = new Die[5];
    public int[] IDNeighbourRoom;
    public bool isWaste;
    public int numberWaste;
    public Supply[] supplies;

    List<Die> DieStop = new List<Die>();

    int numberWasteTmp;

    //"normal room"
    int numberSuppliesTransferable;
    int numberSuppliesLeft;
    public int numberDieLocked;

    //"hold room"
    public bool isHold;
    public Supply[] ressourcesHold;
    int numberRessourcesHold;

    //wasteRoom
    bool launchDice;
    int numberDieStop;
    int numberWastePossiblyDelete;
    GameObject wasteCoin;

    bool lerpNeeded;
    Vector3 nextPosition;
    float timer;

    BoardGame board;

    BoxCollider boxCollider;

    private void Start()
    {
        if (typeSupply != Ressources.none && !isHold)
        {
            supplies = new Supply[4];
            GameObject go;
            for (int i = 0; i < 4; ++i)
            {
                go = Instantiate(supplyPrefab, positionRessources[i].position, Quaternion.identity);
                if (go.GetComponent<Supply>())
                {
                    supplies[i] = go.GetComponent<Supply>();
                    supplies[i].Initialize(idRoom, typeSupply, this, i);
                }
            }
        }
        numberSuppliesLeft = 4;
        if (isWaste)
        {
            wasteCoin = Instantiate(supplyPrefab, positionRessources[0].position, Quaternion.identity);
        }
        numberDieLocked = 0;
        numberRessourcesHold = 0;
        if (isHold)
        {
            ressourcesHold = new Supply[9];
        }
        outlineToChange = outlineHelp.GetComponent<Renderer>().sharedMaterial = new Material(shader);
        outlineToChange.SetFloat("_Thickness", 6.0f);
        outlineToChange.SetColor("_Color", Color.green);
        outlineHelp.SetActive(false);
        board = FindObjectOfType<BoardGame>();
    }

    public bool HasEnoughRessources(int _numberOfRessources)
    {
        bool enoughRessources = false;
        int totalRessources = _numberOfRessources;
        for (int i = 0; i < RessourcesRequired.Length; ++i)
        {
            if (!RessourcesRequired[i].isCompleted)
            {
                if (totalRessources >= RessourcesRequired[i].numberRessourcesRequired)
                {
                    totalRessources -= RessourcesRequired[i].numberRessourcesRequired;
                    RessourcesRequired[i].isCompleted = true;
                    enoughRessources = true;
                }
                else
                {
                    return enoughRessources;
                }
            }
        }
        return enoughRessources;
    }

    public void ActivateOutline(bool _activate)
    {
        outlineHelp.SetActive(_activate);
    }

    public void LockDiceForSupply(List<Die> _dieList)
    {
        if (_dieList.Count == 0) return;
        int totalRessources = _dieList.Count;
        int totalRessourcesUsed = 0;
        for (int i = 0; i < RessourcesRequired.Length; ++i)
        {
            if (!RessourcesRequired[i].isCompleted)
            {
                //enough supplies left and enough dice
                if (numberSuppliesLeft >= RessourcesRequired[i].numberRessourcesReward && totalRessources >= RessourcesRequired[i].numberRessourcesRequired)
                {
                    totalRessources -= RessourcesRequired[i].numberRessourcesRequired;
                    int max = totalRessourcesUsed + RessourcesRequired[i].numberRessourcesRequired;
                    //die lock will ignore raycast and move it to the correct place
                    //also add die to our lockdice list to remember which die we have
                    for (int j = totalRessourcesUsed; j < max; ++j)
                    {
                        _dieList[j].gameObject.layer = 2;
                        lockedDice[numberDieLocked] = _dieList[j];
                        _dieList[j].LockForSupply(typeSupply);
                        _dieList[j].MoveDice(positionDices[numberDieLocked].position, true);
                        _dieList[j].OnStopForWaste += StopWaste;
                        numberDieLocked++;
                        totalRessourcesUsed++;
                    }
                    //if section is complete then we can use it to get supply
                    RessourcesRequired[i].isCompleted = true;
                    numberSuppliesTransferable = RessourcesRequired[i].numberRessourcesReward;
                }
                else
                {
                    return;
                }
            }
        }
    }

    public void LockDieForWaste(List<Die> _dieList)
    {
        if (isWaste)
        {
            if (_dieList.Count == 0) return;
            Debug.Log(_dieList.Count);
            int totalRessources = _dieList.Count;
            int totalRessourcesUsed = 0;
            for (int i = 0; i < RessourcesRequired.Length; ++i)
            {
                if (!RessourcesRequired[i].isCompleted)
                {
                    if (totalRessources >= RessourcesRequired[i].numberRessourcesRequired)
                    {
                        totalRessources -= RessourcesRequired[i].numberRessourcesRequired;
                        int max = totalRessourcesUsed + RessourcesRequired[i].numberRessourcesRequired;
                        //die lock will ignore raycast and move it to the correct place
                        //also add die to our lockdice list to remember which die we have
                        for (int j = totalRessourcesUsed; j < max; ++j)
                        {
                            _dieList[j].gameObject.layer = 2;
                            lockedDice[numberDieLocked] = _dieList[j];
                            _dieList[j].LockForSupplyWaste();
                            _dieList[j].MoveDice(positionDices[numberDieLocked].position, true);
                            numberDieLocked++;
                            totalRessourcesUsed++;
                        }
                        //if section is complete then we can use it to get supply
                        RessourcesRequired[i].isCompleted = true;
                        numberWastePossiblyDelete = RessourcesRequired[i].numberRessourcesReward;
                    }
                    else
                    {
                        return;
                    }

                    /*  else
                      {
                          return;
                      }*/
                }
            }
        }
    }

    public void LockDieForHold(List<Die> _dieList)
    {
        if (_dieList.Count > 0)
        {
            _dieList[0].gameObject.layer = 2;
            lockedDice[numberDieLocked] = _dieList[0];
            _dieList[0].LockForSupply(typeSupply);
            _dieList[0].MoveDice(positionDices[numberDieLocked].position, true);
            numberDieLocked++;
        }
    }

    public bool IsDieLockHold()
    {
        if (isHold)
        {
            if (lockedDice[0] != null && lockedDice[0].IsUpperFacePlane())
            {
                return true;
            }
        }
        return false;
    }

    public int GetNumberSupplyAvailableForHold()
    {
        int number = 0;
        for (int i = 0; i < RessourcesRequired.Length; ++i)
        {
            if (RessourcesRequired[i].isCompleted && numberSuppliesLeft >= RessourcesRequired[i].numberRessourcesReward)
            {
                number += RessourcesRequired[i].numberRessourcesReward;
            }
            else
            {
                return number;
            }
        }
        return number;
    }

    public List<Supply> TransfertSuppliesToHold()
    {
        if (numberSuppliesTransferable == 0)
        {
            for (int i = 0; i < numberDieLocked; ++i)
            {
                lockedDice[i].ReturnDieToOwner();
                lockedDice[i] = null;
            }
            for (int i = 0; i < RessourcesRequired.Length; ++i)
            {
                RessourcesRequired[i].isCompleted = false;
            }
            numberDieLocked = 0;
            return null;
        }
        List<Supply> suppliesList = new List<Supply>();
        if (!isHold)
        {
            int cpt = Mathf.Clamp(numberSuppliesLeft - numberSuppliesTransferable, 0, 4);
            for (int i = numberSuppliesLeft; i > cpt; --i)
            {
                suppliesList.Add(supplies[i - 1]);
                numberSuppliesTransferable--;
                numberSuppliesLeft--;
            }
            RollDiceForWaste();
            /*for (int i = 0; i < numberDieLocked; ++i)
            {
                lockedDice[i].ReturnDieToOwner();
                lockedDice[i] = null;
            }*/
            //numberDieLocked = 0;
            for (int i = 0; i < RessourcesRequired.Length; ++i)
            {
                RessourcesRequired[i].isCompleted = false;
            }
        }
        return suppliesList;
    }

    void RollDiceForWaste()
    {
        OnLaunch();
        launchDice = true;
        Vector3 position = Vector3.zero + Vector3.up * 4.0f;
        position.x = Random.Range(-20.0f, 20.0f);
        position.z = Random.Range(-5.0f, 5.0f);
        for (int i = 0; i < numberDieLocked; ++i)
        {
            lockedDice[i].RollDie(position);
            position.x = Random.Range(-20.0f, 20.0f);
            position.z = Random.Range(-5.0f, 5.0f);
        }
    }

    void StopWaste(bool _isTrash, Die _die)
    {
        if (!DieStop.Contains(_die))
        {
            if (_isTrash)
                numberWasteTmp++;
            numberDieStop++;
            DieStop.Add(_die);
        }
    }

    public void AddSuppliesToHold(List<Supply> suppliesList)
    {
        if (suppliesList == null) return;
        if (isHold && numberRessourcesHold < 9)
        {
            for (int i = 0; i < suppliesList.Count; ++i)
            {
                for (int j = 0; j < ressourcesHold.Length; ++j)
                {
                    if (ressourcesHold[j] == null)
                    {
                        ressourcesHold[j] = suppliesList[i];
                        suppliesList[i].MoveSupply(positionRessources[j].position, true, false);
                        numberRessourcesHold++;
                        break;
                    }
                }
            }
        }
    }

    public bool HasEnoughSpaceInHold()
    {
        return numberRessourcesHold < 9;
    }

    public bool DeliverSupplies(Ressources[] _suppliesNeeded)
    {
        if (isHold)
        {
            bool[] supplyAlreadyUsed = new bool[9] { false, false, false, false, false, false, false, false, false };
            bool[] CanBeDelivered = new bool[_suppliesNeeded.Length];
            int[] idSupply = new int[_suppliesNeeded.Length];
            for (int i = 0; i < CanBeDelivered.Length; ++i)
                CanBeDelivered[i] = false;

            for (int i = 0; i < _suppliesNeeded.Length; ++i)
            {
                for (int j = 0; j < ressourcesHold.Length; ++j)
                {
                    if (!supplyAlreadyUsed[j])
                    {
                        if (_suppliesNeeded[i] == ressourcesHold[j].TypeSupply)
                        {
                            CanBeDelivered[i] = true;
                            supplyAlreadyUsed[j] = true;
                            idSupply[i] = j;
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < CanBeDelivered.Length; ++i)
                if (!CanBeDelivered[i]) return false;

            for (int i = 0; i < idSupply.Length; ++i)
            {
                ressourcesHold[idSupply[i]].MoveSupply(Vector3.zero, false, true);
                ressourcesHold[idSupply[i]] = null;
                numberRessourcesHold--;
            }
            for (int i = 0; i < numberDieLocked; ++i)
            {
                lockedDice[i].ReturnDieToOwner();
                lockedDice[i] = null;
            }
            numberDieLocked = 0;
            return true;
        }
        return false;
    }

    public void UseSupplyOnWaste()
    {
        MoveCoinWaste(numberWastePossiblyDelete);
        for (int i = 0; i < numberDieLocked; ++i)
        {
            lockedDice[i].ReturnDieToOwner();
            lockedDice[i] = null;
        }
        numberDieLocked = 0;
        for (int i = 0; i < RessourcesRequired.Length; ++i)
        {
            RessourcesRequired[i].isCompleted = false;
        }
    }

    public void AddDieLock(Die _lockedDie)
    {
        if (numberDieLocked < lockedDice.Length && !CheckDieAlreadyLocked(_lockedDie))
        {
            _lockedDie.gameObject.layer = 2;
            lockedDice[numberDieLocked] = _lockedDie;
            _lockedDie.MoveDice(positionDices[numberDieLocked].position, true);
            numberDieLocked++;
        }
    }

    bool CheckDieAlreadyLocked(Die _die)
    {
        for (int i = 0; i < numberDieLocked; ++i)
        {
            if (lockedDice[i] == _die) return true;
        }
        return false;
    }

    public bool IsRoomNeighbour(int _idRoom)
    {
        for (int i = 0; i < IDNeighbourRoom.Length; ++i)
        {
            if (_idRoom == IDNeighbourRoom[i]) return true;
        }
        return false;
    }

    void MoveCoinWaste(int _number)
    {
        if (isWaste)
        {
            lerpNeeded = true;
            numberWaste = Mathf.Clamp(numberWaste + _number, 0, positionRessources.Length - 1);
            nextPosition = positionRessources[numberWaste].position;
            // wasteCoin.transform.position = positionRessources[numberWaste].position;
            /*  if (numberWaste == positionRessources.Length - 1)
              {
                  OnWasteMax();
              }*/
        }
    }

    private void Update()
    {

        if (lerpNeeded && isWaste)
        {
            timer += Time.deltaTime * 5.0f;
            wasteCoin.transform.position = Vector3.Lerp(wasteCoin.transform.position, nextPosition, timer);
            if (timer >= 1.0f)
            {
                lerpNeeded = false;
                timer = 0.0f;
                if (numberWaste == positionRessources.Length - 1)
                {
                    OnWasteMax();
                }
            }
        }


        if (launchDice)
        {
            if (numberDieStop == numberDieLocked)
            {
                launchDice = false;
                board.rooms[idRoomWaste].MoveCoinWaste(numberWasteTmp);
                numberWasteTmp = 0;
                numberDieStop = 0;
                DieStop.Clear();
                OnStopLaunch();
                for (int i = 0; i < numberDieLocked; ++i)
                {
                    lockedDice[i].OnStopForWaste -= StopWaste;
                    lockedDice[i].ReturnDieToOwner();
                    lockedDice[i] = null;
                }
                numberDieLocked = 0;
            }
        }
    }
}

[System.Serializable]
public class Section
{
    public int numberRessourcesRequired;
    public int numberRessourcesReward;
    public bool isCompleted;
}