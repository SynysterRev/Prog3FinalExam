using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesDataBase
{
    public static CityInfo[] Cities = new CityInfo[24]
    {
          new CityInfo("Tokyo", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.food, Ressources.food}, ColorCity.red ),
         new CityInfo("Seoul", new Ressources[]{Ressources.food, Ressources.food, Ressources.power, Ressources.power }, ColorCity.red ),
         new CityInfo("HongKong", new Ressources[]{Ressources.water, Ressources.water, Ressources.food, Ressources.food}, ColorCity.red ),
         new CityInfo("Bangkok", new Ressources[]{Ressources.power, Ressources.power, Ressources.firstAid, Ressources.firstAid }, ColorCity.red ),
         new CityInfo("Delhi", new Ressources[]{Ressources.power, Ressources.power, Ressources.power, Ressources.food}, ColorCity.grey ),
         new CityInfo("Karachi", new Ressources[]{Ressources.food, Ressources.food, Ressources.food, Ressources.water}, ColorCity.grey ),
         new CityInfo("Ritadh", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.firstAid, Ressources.power}, ColorCity.grey  ),
         new CityInfo("Cairo", new Ressources[]{Ressources.water, Ressources.water, Ressources.water, Ressources.vaccine},ColorCity.grey  ),
         new CityInfo("Istanbul", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.vaccine, Ressources.firstAid}, ColorCity.grey  ),
         new CityInfo("Moscow", new Ressources[]{Ressources.water, Ressources.water, Ressources.water, Ressources.power}, ColorCity.grey  ),
         new CityInfo("Essen", new Ressources[]{Ressources.firstAid, Ressources.power, Ressources.food, Ressources.water}, ColorCity.blue  ),
         new CityInfo("Paris", new Ressources[]{Ressources.power, Ressources.food, Ressources.water, Ressources.firstAid}, ColorCity.blue   ),
         new CityInfo("London", new Ressources[]{Ressources.water, Ressources.food, Ressources.vaccine, Ressources.firstAid}, ColorCity.blue   ),
         new CityInfo("Madrid", new Ressources[]{Ressources.water, Ressources.water, Ressources.vaccine, Ressources.firstAid}, ColorCity.blue   ),
         new CityInfo("Montreal", new Ressources[]{Ressources.firstAid, Ressources.vaccine, Ressources.power, Ressources.food}, ColorCity.blue   ),
         new CityInfo("Atlanta", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.food, Ressources.power}, ColorCity.blue   ),
         new CityInfo("LosAngeles", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.water, Ressources.firstAid}, ColorCity.yellow ),
         new CityInfo("Mexico", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.power, Ressources.vaccine} , ColorCity.yellow),
         new CityInfo("Bogota", new Ressources[]{Ressources.water, Ressources.water, Ressources.food, Ressources.vaccine}, ColorCity.yellow ),
         new CityInfo("SaoPaulo", new Ressources[]{Ressources.power, Ressources.power, Ressources.firstAid, Ressources.food} , ColorCity.yellow),
         new CityInfo("Lagos", new Ressources[]{Ressources.food, Ressources.food, Ressources.power, Ressources.water} , ColorCity.yellow),
         new CityInfo("Johannesburg", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.food, Ressources.power}, ColorCity.yellow ),
         new CityInfo("Sydney", new Ressources[]{Ressources.firstAid, Ressources.firstAid, Ressources.vaccine, Ressources.food} , ColorCity.red),
         new CityInfo("Manila", new Ressources[]{Ressources.vaccine, Ressources.vaccine, Ressources.water, Ressources.water} , ColorCity.red),
    };
}
