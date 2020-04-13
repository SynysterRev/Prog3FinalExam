using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGame : MonoBehaviour
{
    [SerializeField] Room[] rooms;
    CitiesDataBase cities;
    [SerializeField] GameObject prefabPlane;
    [SerializeField] GameObject prefabPlayer;
    GameObject plane;
    float timerRound;
    int numberPieceTimerLeft;

    void InitializeBoard()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
