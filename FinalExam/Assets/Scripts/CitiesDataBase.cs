using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesDataBase
{
    CityInfo[] Cities;
    public CitiesDataBase()
    {
        Cities = new CityInfo[24]
        {
         new CityInfo("Tokyo", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.food, Ressources.food} ),
         new CityInfo("Seoul", new Ressources[]{Ressources.food, Ressources.food, Ressources.power, Ressources.power } ),
         new CityInfo("HongKong", new Ressources[]{Ressources.water, Ressources.water, Ressources.food, Ressources.food} ),
         new CityInfo("Bangkok", new Ressources[]{Ressources.power, Ressources.power, Ressources.firstAid, Ressources.firstAid } ),
         new CityInfo("Delhi", new Ressources[]{Ressources.power, Ressources.power, Ressources.power, Ressources.food} ),
         new CityInfo("Karachi", new Ressources[]{Ressources.food, Ressources.food, Ressources.food, Ressources.water} ),
         new CityInfo("Ritadh", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.firstAid, Ressources.power} ),
         new CityInfo("Cairo", new Ressources[]{Ressources.water, Ressources.water, Ressources.water, Ressources.vaccine} ),
         new CityInfo("Istanbul", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.vaccine, Ressources.firstAid} ),
         new CityInfo("Moscow", new Ressources[]{Ressources.water, Ressources.water, Ressources.water, Ressources.power} ),
         new CityInfo("Essen", new Ressources[]{Ressources.firstAid, Ressources.power, Ressources.food, Ressources.water} ),
         new CityInfo("Paris", new Ressources[]{Ressources.power, Ressources.food, Ressources.water, Ressources.firstAid} ),
         new CityInfo("London", new Ressources[]{Ressources.water, Ressources.food, Ressources.vaccine, Ressources.firstAid} ),
         new CityInfo("Madrid", new Ressources[]{Ressources.water, Ressources.water, Ressources.vaccine, Ressources.firstAid} ),
         new CityInfo("Montreal", new Ressources[]{Ressources.firstAid, Ressources.vaccine, Ressources.power, Ressources.food} ),
         new CityInfo("Atlanta", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.food, Ressources.power} ),
         new CityInfo("LosAngeles", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.water, Ressources.firstAid} ),
         new CityInfo("Mexico", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.power, Ressources.vaccine} ),
         new CityInfo("Bogota", new Ressources[]{Ressources.water, Ressources.water, Ressources.food, Ressources.vaccine} ),
         new CityInfo("SaoPaulo", new Ressources[]{Ressources.power, Ressources.power, Ressources.firstAid, Ressources.food} ),
         new CityInfo("Lagos", new Ressources[]{Ressources.food, Ressources.food, Ressources.power, Ressources.water} ),
         new CityInfo("Johannesburg", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.food, Ressources.power} ),
         new CityInfo("Sydney", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.vaccine, Ressources.food} ),
         new CityInfo("Manila", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.water, Ressources.water} ),
        };
    }
}
