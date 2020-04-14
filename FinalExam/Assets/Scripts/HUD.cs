using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    BoardGame board;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardGame>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AskRollDice()
    {
        board.RollDice();
    }
}
