using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room : MonoBehaviour
{
    [SerializeField] GameObject supplyPrefab;
    public int idRoom;
    public string nameRoom;
    public Ressources typeSupply;
    public Transform[] positionPlayer;
    public Transform[] positionRessources;
    public Transform[] positionDices;
    public Section[] RessourcesRequired;
    public Die[] lockedDice = new Die[5];
    public int[] IDNeighbourRoom;
    public Ressources[] ressourcesHold = new Ressources[9];
    public bool isHold;
    public bool isWaste;
    public int numberWaste;
    public Supply[] supplies;
    GameObject wasteCoin;
    BoxCollider boxCollider;

    private void Start()
    {
        if (typeSupply != Ressources.none)
        {
            supplies = new Supply[4];
            GameObject go;
            for (int i = 0; i < 4; ++i)
            {
                go = Instantiate(supplyPrefab, positionRessources[i].position, Quaternion.identity);
                if (go.GetComponent<Supply>())
                {
                    supplies[i] = go.GetComponent<Supply>();
                    supplies[i].Initialize(idRoom, typeSupply);
                }
            }
        }
        if(isWaste)
        {
            wasteCoin = Instantiate(supplyPrefab, positionRessources[0].position, Quaternion.identity);
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