using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CityInfo cityInfo;
    public int IDCard;
    [SerializeField] SpriteRenderer[] spriteSuppliesNeeded;
    [SerializeField] TextMesh[] nameCity;

    bool lerpNeeded;
    Vector3 nextPosition;
    float timer;
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

    public void MoveCard(Vector3 _position)
    {
        nextPosition = _position;
        lerpNeeded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpNeeded)
        {
            timer += Time.deltaTime * 5.0f;
            transform.position = Vector3.Lerp(transform.position, nextPosition, timer);
            if (timer >= 1.0f)
            {
                lerpNeeded = false;
                timer = 0.0f;
            }
        }
    }
}
