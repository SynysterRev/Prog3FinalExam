using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInfo
{
    string cityName;
    Ressources[] RessourcesNeeded = new Ressources[4];

    public CityInfo(string _name, Ressources[] _RessourcesNeeded)
    {
        cityName = _name;
        RessourcesNeeded = _RessourcesNeeded;
    }
}
