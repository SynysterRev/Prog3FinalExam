using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGame : MonoBehaviour
{
    public delegate void DelegatePauseGame();
    public event DelegatePauseGame OnPauseGame;
    public event DelegatePauseGame OnEndPauseGame;

    public delegate void DelegateTimeUp();
    public event DelegateTimeUp OnTimeUp;
    public event DelegateTimeUp OnTimeUpEnd;

    public delegate void DelegateEndGame();
    public event DelegateEndGame OnGameOver;
    public event DelegateEndGame OnGameWin;

    [SerializeField] Room[] rooms;
    [SerializeField] Transform[] citiesPosition;
    [SerializeField] GameObject prefabPlane;
    [SerializeField] GameObject coinTimePrefab;
    [SerializeField] List<GameObject> prefabPlayer;
    [SerializeField] Transform[] cardsCitiesPosition;
    [SerializeField] Transform pileOfCards;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform[] timeCoinPositions;
    [SerializeField] float timePerRound;
    CityInfo[] cities;
    List<Card> cardsDeck;
    List<Card> citiesToSave;
    GameObject[] coinTime;
    GameObject plane;
    Character[] players;
    int numberPlayers = 2;
    int numberCitiesToSave = 5;
    public float timerRound;
    int numberCoinTimerLeft;
    int idCharacterTurn;
    bool isGamePaused;
    bool timeUp;
    bool isGameOver;
    bool isGameWin;

    public bool TimeUp { get => timeUp; }

    void InitializeBoard()
    {
        CreateCardDeck();
        CreatePlayer();

        coinTime = new GameObject[9];
        timerRound = timePerRound;
        numberCoinTimerLeft = 3;
        for (int i = 0; i < numberCoinTimerLeft; ++i)
        {
            coinTime[i] = Instantiate(coinTimePrefab, timeCoinPositions[i].position, Quaternion.identity);
        }
        isGamePaused = false;
        timeUp = false;
    }

    void CreatePlayer()
    {
        players = new Character[numberPlayers];
        int[] randomCharacter = new int[numberPlayers];
        GameObject go;
        //pick one character among the 4 available and remove it
        for (int i = 0; i < numberPlayers; ++i)
        {
            randomCharacter[i] = Random.Range(0, prefabPlayer.Count);
            go = Instantiate(prefabPlayer[randomCharacter[i]]);
            if (go.GetComponent<Character>())
            {
                players[i] = go.GetComponent<Character>();
                //spawn character at the correct place
                players[i].Initialize(rooms[players[i].idRoomSpawn].positionPlayer[i].position);
            }
            prefabPlayer.RemoveAt(randomCharacter[i]);
        }
        idCharacterTurn = numberPlayers - 1;
        players[idCharacterTurn].ActivateTurn();
    }

    void CreateCardDeck()
    {
        //create a fulldeck
        cardsDeck = new List<Card>();
        for (int i = 0; i < 24; ++i)
        {
            cardsDeck.Add(Instantiate(cardPrefab).GetComponent<Card>());
            cardsDeck[i].IDCard = i;
        }

        //get the database and pick on random city where to begin and remove it from the deck
        cities = CitiesDataBase.Cities;
        int randomStartCity = Random.Range(0, cardsDeck.Count);
        plane = Instantiate(prefabPlane, citiesPosition[randomStartCity].position, citiesPosition[randomStartCity].rotation);

        citiesToSave = new List<Card>();
        int[] randomCities = new int[numberCitiesToSave];
        Card city;
        //take 5 random city and remove them from the deck
        for (int i = 0; i < numberCitiesToSave; ++i)
        {
            randomCities[i] = Random.Range(0, cardsDeck.Count);
            city = cardsDeck[randomCities[i]];
            cardsDeck.Remove(city);
            citiesToSave.Add(city);
        }
        //destroy the deck we don't need it anymore
        for (int i = 0; i < cardsDeck.Count; ++i)
        {
            Destroy(cardsDeck[i].gameObject);
        }

        //put 2 cities on the board
        for (int i = 0; i < 2; ++i)
        {
            citiesToSave[i].transform.position = cardsCitiesPosition[citiesToSave[i].IDCard].position;
            citiesToSave[i].transform.rotation = cardsCitiesPosition[citiesToSave[i].IDCard].rotation;
        }

        //put the 3 others in the draw pile
        Vector3 position = pileOfCards.position;
        for (int i = 2; i < 5; ++i)
        {
            citiesToSave[i].transform.position = position;
            position.y += 0.1f;
            citiesToSave[i].transform.rotation = pileOfCards.rotation;
        }
    }

    public bool RollDice()
    {
        return players[idCharacterTurn].RollDice();
    }

    public void EndTurn()
    {
        players[idCharacterTurn].EndTurn();
        idCharacterTurn--;
        if (idCharacterTurn < 0)
            idCharacterTurn = numberPlayers - 1;
        players[idCharacterTurn].ActivateTurn();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
    }

    public void UseCoinTime()
    {
        if (timeUp)
        {
            if (numberCoinTimerLeft > 0)
            {
                Destroy(coinTime[numberCoinTimerLeft - 1]);
                numberCoinTimerLeft--;
                timerRound = timePerRound;
                OnTimeUpEnd();
                timeUp = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeUp && !isGamePaused)
        {
            timerRound = Mathf.Clamp(timerRound - Time.deltaTime, 0.0f, 120.0f);
            if (timerRound <= 0.0f)
            {
                if (numberCoinTimerLeft > 0)
                {
                    timeUp = true;
                    OnTimeUp();
                }
                else
                {
                    isGameOver = true;
                    OnGameOver();
                }
            }
        }
    }
}
