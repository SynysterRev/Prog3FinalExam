using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Button rollDice;
    [SerializeField] Button nextTurn;
    [SerializeField] Button coinTime;
    [SerializeField] Text timer;
    [SerializeField] Text playerTurn;
    BoardGame board;
    bool canRollDice;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardGame>();
        board.OnPauseGame += OnPauseGame;
        board.OnEndPauseGame += OnEndPauseGame;

        board.OnTimeUp += OnTimeUp;
        board.OnTimeUpEnd += OnTimeUpEnd;

        board.OnGameOver += OnPauseGame;
        coinTime.interactable = false;
        canRollDice = true;
        playerTurn.text = board.GetCharacterColor().ToString() + " player";
        switch(board.GetCharacterColor())
        {
            case ColorCharacter.green:
                playerTurn.color = Color.green;
                break;
            case ColorCharacter.grey:
                playerTurn.color = Color.grey;
                break;
            case ColorCharacter.red:
                playerTurn.color = Color.red;
                break;
            case ColorCharacter.blue:
                playerTurn.color = Color.blue;
                break;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        timer.text = Mathf.CeilToInt(board.timerRound).ToString();
    }

    public void EndTurnCharacter()
    {
        if (!board.TimeUp)
        {
            board.EndTurn();
            canRollDice = true;
            rollDice.interactable = true;
            playerTurn.text = board.GetCharacterColor().ToString() + " player";
            switch (board.GetCharacterColor())
            {
                case ColorCharacter.green:
                    playerTurn.color = Color.green;
                    break;
                case ColorCharacter.grey:
                    playerTurn.color = Color.grey;
                    break;
                case ColorCharacter.red:
                    playerTurn.color = Color.red;
                    break;
                case ColorCharacter.blue:
                    playerTurn.color = Color.blue;
                    break;
            }
        }
    }

    public void AskRollDice()
    {
        if (canRollDice && !board.TimeUp)
        {
            canRollDice = board.RollDice();
        }
        if (!canRollDice) rollDice.interactable = false;
    }

    public void UseCoinTime()
    {
        if (board.TimeUp)
        {
            board.UseCoinTime();
        }
    }

    void OnPauseGame()
    {
        rollDice.interactable = false;
        nextTurn.interactable = false;
        coinTime.interactable = false;
    }

    void OnEndPauseGame()
    {
        rollDice.interactable = canRollDice;
        nextTurn.interactable = true;
        coinTime.interactable = board.TimeUp;
    }

    void OnTimeUp()
    {
        rollDice.interactable = false;
        nextTurn.interactable = false;
        coinTime.interactable = true;
    }

    void OnTimeUpEnd()
    {
        rollDice.interactable = canRollDice;
        nextTurn.interactable = true;
        coinTime.interactable = false;
    }
}
