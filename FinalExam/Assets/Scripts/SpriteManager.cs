using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private static SpriteManager instance;
    public static SpriteManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] Material[] matCard;
    [SerializeField] Sprite[] suppliesSprite;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public Sprite[] SuppliesSprite { get => suppliesSprite; }

    public Material[] MatCard { get => matCard; }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
