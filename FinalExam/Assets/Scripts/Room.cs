using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room : MonoBehaviour
{
    [SerializeField] GameObject supplyPrefab;
    [SerializeField] Room holdRoom;
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

    //"normal room"
    int numberSuppliesTransferable;
    int numberSuppliesLeft;
    int numberDieLocked;

    //"hold room"
    public bool isHold;
    public Supply[] ressourcesHold;
    int numberRessourcesHold;

    GameObject wasteCoin;
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

    public void LockDiceForSupply(List<Die> _dieList)
    {
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
                numberDieLocked = 0;
            }
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
        return suppliesList;
    }

    public void AddSuppliesToHold(List<Supply> suppliesList)
    {
        if (suppliesList == null) return;
        if (isHold)
        {
            for (int i = 0; i < suppliesList.Count; ++i)
            {
                if (numberRessourcesHold < 9)
                {
                    ressourcesHold[numberRessourcesHold] = suppliesList[i];
                    suppliesList[i].MoveSupply(positionRessources[numberRessourcesHold].position, true, false);
                    numberRessourcesHold++;
                }
            }
        }
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

    private void Update()
    {

    }
}

[System.Serializable]
public class Section
{
    public int numberRessourcesRequired;
    public int numberRessourcesReward;
    public bool isCompleted;
}