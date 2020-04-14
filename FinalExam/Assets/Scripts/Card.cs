using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CityInfo cityInfo;
    public int IDCard;
    // Start is called before the first frame update
    void Start()
    {
        cityInfo = CitiesDataBase.Cities[IDCard];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
