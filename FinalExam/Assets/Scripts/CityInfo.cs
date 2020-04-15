using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInfo
{
    string cityName;
    Ressources[] ressourcesNeeded = new Ressources[4];
    ColorCity colorCity;

    public CityInfo(string _name, Ressources[] _RessourcesNeeded, ColorCity _colorCity)
    {
        cityName = _name;
        ressourcesNeeded = _RessourcesNeeded;
        colorCity = _colorCity;
    }

    public string CityName { get => cityName; }
    public Ressources[] RessourcesNeeded { get => ressourcesNeeded; }
    public ColorCity ColorCity { get => colorCity; }
}
