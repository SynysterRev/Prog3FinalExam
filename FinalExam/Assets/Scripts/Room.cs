using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room : MonoBehaviour
{
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
    BoxCollider collider;
}

[System.Serializable]
public class Section
{
    public int numberRessourcesRequired;
    public int numberRessourcesReward;
    public bool isCompleted;
}