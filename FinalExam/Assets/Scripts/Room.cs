using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room
{
    public int idRoom;
    public string nameRoom;
    public Ressources typeSupply;
    public Vector3[] positionPlayer;
    public Vector3[] positionRessources;
    public Section[] RessourcesRequired;
    public Die[] lockedDice = new Die[5];
    public Ressources[] ressourcesHold = new Ressources[9];
}

[System.Serializable]
public class Section
{
    public int numberRessourcesRequired;
    public int numberRessourcesReward;
    public bool isCompleted;
}