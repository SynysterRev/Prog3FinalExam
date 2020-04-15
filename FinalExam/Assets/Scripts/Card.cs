using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CityInfo cityInfo;
    public int IDCard;
    [SerializeField] SpriteRenderer[] spriteSuppliesNeeded;
    [SerializeField] TextMesh[] nameCity;
    // Start is called before the first frame update
    void Start()
    {
        cityInfo = CitiesDataBase.Cities[IDCard];
        for (int i = 0; i < nameCity.Length; ++i)
            nameCity[i].text = cityInfo.CityName;

        for (int i = 0; i < cityInfo.RessourcesNeeded.Length; ++i)
        {
            spriteSuppliesNeeded[i].sprite = SpriteManager.Instance.SuppliesSprite[(int)cityInfo.RessourcesNeeded[i]];
        }
        if (GetComponent<MeshRenderer>().material)
            GetComponent<MeshRenderer>().material = SpriteManager.Instance.MatCard[(int)cityInfo.ColorCity];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
