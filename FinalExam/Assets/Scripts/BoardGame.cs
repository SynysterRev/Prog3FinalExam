using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public delegate void DelegatePlayer();
    public event DelegatePlayer OnNoMoreDice;

    [SerializeField] public Room[] rooms;
    [SerializeField] Transform[] citiesPosition;
    [SerializeField] GameObject prefabPlane;
    [SerializeField] GameObject coinTimePrefab;
    [SerializeField] List<GameObject> prefabPlayer;
    [SerializeField] Transform[] cardsCitiesPosition;
    [SerializeField] Transform pileOfCards;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform[] timeCoinPositions;
    [SerializeField] Transform[] UsedDiePositions;
    [SerializeField] float timePerRound;
    [SerializeField] int holdRoomID;

    [SerializeField] GameObject HiddenCard;

    CityInfo[] cities;
    List<Card> cardsDeck;
    List<Card> citiesToSave;
    List<Card> currentCitiesToSave;
    GameObject[] coinTime;
    GameObject plane;
    Character[] players;

    CityInfo cityPlane;

    public float timerRound;
    public int planeCity;
    int numberPlayers = 2;
    int numberCitiesToSave = 5;
    int numberCoinTimerLeft;
    int idCharacterTurn;
    bool isGamePaused;
    bool timeUp;
    bool isGameOver;
    bool isGameWin;

    bool lerpNeeded;
    Vector3 nextPosition;
    float timer;

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
        for (int i = 0; i < rooms.Length; ++i)
        {
            rooms[i].OnLaunch += PauseGame;
            rooms[i].OnStopLaunch += StopPauseGame;
            rooms[i].OnWasteMax += OnWasteMax;
        }
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
                players[i].Initialize(rooms[players[i].idRoomSpawn].positionPlayer[i].position, this, i, UsedDiePositions);
                players[i].OnWantToMove += DisplayHelpMovementPlayer;
                players[i].OnWantToMoveStop += StopDisplayHelpMovementPlayer;
                players[i].OnNoMoreDice += NoMoreDice;
            }
            prefabPlayer.RemoveAt(randomCharacter[i]);
        }
        idCharacterTurn = numberPlayers - 1;
        players[idCharacterTurn].ActivateTurn();
    }

    void NoMoreDice()
    {
        OnNoMoreDice();
    }

    void PauseGame()
    {
        isGamePaused = true;
        OnPauseGame();
    }

    void StopPauseGame()
    {
        isGamePaused = false;
        OnEndPauseGame();
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
        planeCity = randomStartCity;
        cityPlane = cities[randomStartCity];
        citiesToSave = new List<Card>();
        currentCitiesToSave = new List<Card>();
        int[] randomCities = new int[numberCitiesToSave];
        Card city;
        //take 5 random city and remove them from the deck
        for (int i = 0; i < numberCitiesToSave; ++i)
        {
            randomCities[i] = Random.Range(0, cardsDeck.Count);
            if (randomCities[i] != randomStartCity)
            {
                city = cardsDeck[randomCities[i]];
                cardsDeck.Remove(city);
                citiesToSave.Add(city);
            }
            else
            {
                i--;
            }
        }
        //destroy the deck we don't need it anymore
        for (int i = 0; i < cardsDeck.Count; ++i)
        {
            Destroy(cardsDeck[i].gameObject);
        }

        //put 2 cities on the board
        for (int i = 0; i < 2; ++i)
        {
            citiesToSave[i].MoveCard(cardsCitiesPosition[citiesToSave[i].IDCard].position);
            //citiesToSave[i].transform.position = cardsCitiesPosition[citiesToSave[i].IDCard].position;
            citiesToSave[i].transform.rotation = cardsCitiesPosition[citiesToSave[i].IDCard].rotation;
            currentCitiesToSave.Add(citiesToSave[i]);
            citiesToSave.RemoveAt(i);
        }

        GameObject go = Instantiate(HiddenCard);
        //put the 3 others in the draw pile
        Vector3 position = pileOfCards.position;
        for (int i = 0; i < 3; ++i)
        {
            citiesToSave[i].MoveCard(position);
            position.y += 0.2f;
            citiesToSave[i].transform.rotation = pileOfCards.rotation;
        }
        position.y += 0.2f;
        go.transform.position = position;
        go.transform.rotation = pileOfCards.rotation;
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

    void DisplayHelpMovementPlayer()
    {
        if (players[idCharacterTurn].WantToMove)
        {
            for (int i = 0; i < rooms[players[idCharacterTurn].IDCurrentRoom].IDNeighbourRoom.Length; ++i)
            {
                rooms[rooms[players[idCharacterTurn].IDCurrentRoom].IDNeighbourRoom[i]].ActivateOutline(true);
            }
        }
    }

    void StopDisplayHelpMovementPlayer()
    {
        if (!players[idCharacterTurn].WantToMove)
        {
            for (int i = 0; i < rooms[players[idCharacterTurn].IDCurrentRoom].IDNeighbourRoom.Length; ++i)
            {
                rooms[rooms[players[idCharacterTurn].IDCurrentRoom].IDNeighbourRoom[i]].ActivateOutline(false);
            }
        }
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

    void GetOneCoinTime()
    {
        coinTime[numberCoinTimerLeft] = Instantiate(coinTimePrefab, timeCoinPositions[numberCoinTimerLeft].position, Quaternion.identity);
        numberCoinTimerLeft++;
    }

    void DeleteSavedCityAndAddNewOne()
    {
        for (int i = 0; i < currentCitiesToSave.Count; ++i)
        {
            if (currentCitiesToSave[i].cityInfo.CityName == cityPlane.CityName)
            {
                GameObject go = currentCitiesToSave[i].gameObject;
                currentCitiesToSave.RemoveAt(i);
                Destroy(go);
            }
        }
        if (citiesToSave.Count > 0)
        {
            citiesToSave[0].MoveCard(cardsCitiesPosition[citiesToSave[0].IDCard].position);
            citiesToSave[0].transform.rotation = cardsCitiesPosition[citiesToSave[0].IDCard].rotation;
            currentCitiesToSave.Add(citiesToSave[0]);
            citiesToSave.RemoveAt(0);
        }

        if (citiesToSave.Count == 0 && currentCitiesToSave.Count == 0)
        {
            timeUp = true;
            isGameWin = true;
            OnGameWin();
        }
    }

    public ColorCharacter GetCharacterColor()
    {
        return players[idCharacterTurn].ColorCharact;
    }

    bool CheckIfPlaneIsOnCityToSave()
    {
        for (int i = 0; i < currentCitiesToSave.Count; ++i)
        {
            if (currentCitiesToSave[i].cityInfo.CityName == cityPlane.CityName)
                return true;
        }
        return false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void OnWasteMax()
    {
        timeUp = true;
        isGameOver = true;
        OnGameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (lerpNeeded)
        {
            timer += Time.deltaTime;
            plane.transform.position = Vector3.Lerp(plane.transform.position, nextPosition, timer);
            if (timer >= 1.0f)
            {
                lerpNeeded = false;
                timer = 0.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            citiesToSave.Clear();
            currentCitiesToSave.Clear();
            DeleteSavedCityAndAddNewOne();
        }
        if (!timeUp && !isGamePaused && !isGameOver)
        {
            if (Input.GetMouseButtonDown(0) && players[idCharacterTurn].WantToMove)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Room room = hit.collider.GetComponent<Room>();
                    if (room && room.IsRoomNeighbour(players[idCharacterTurn].IDCurrentRoom) && players[idCharacterTurn].IDCurrentRoom != room.idRoom)
                    {
                        players[idCharacterTurn].MovePlayer(room.positionPlayer[idCharacterTurn].position, room.idRoom);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) && players[idCharacterTurn].WantToMovePlane)
            {
                if (planeCity == 0) planeCity = citiesPosition.Length - 1;
                else planeCity--;
                lerpNeeded = true;
                //plane.transform.position = citiesPosition[planeCity].position;
                nextPosition = citiesPosition[planeCity].position;
                plane.transform.rotation = citiesPosition[planeCity].rotation;
                cityPlane = cities[planeCity];
                players[idCharacterTurn].DeleteDiceUsedToMovePlane();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && players[idCharacterTurn].WantToMovePlane)
            {
                if (planeCity == citiesPosition.Length - 1) planeCity = 0;
                else planeCity++;
                lerpNeeded = true;
                nextPosition = citiesPosition[planeCity].position;
                // plane.transform.position = citiesPosition[planeCity].position;
                plane.transform.rotation = citiesPosition[planeCity].rotation;
                cityPlane = cities[planeCity];
                players[idCharacterTurn].DeleteDiceUsedToMovePlane();
            }

            if (Input.GetKeyDown(KeyCode.Return) && !players[idCharacterTurn].IsDieInSupplyList())
            {
                if(rooms[players[idCharacterTurn].IDCurrentRoom].isWaste)
                {
                    rooms[players[idCharacterTurn].IDCurrentRoom].UseSupplyOnWaste();
                }
                else if (rooms[players[idCharacterTurn].IDCurrentRoom].isHold && CheckIfPlaneIsOnCityToSave() && rooms[players[idCharacterTurn].IDCurrentRoom].IsDieLockHold())
                {
                    if (rooms[players[idCharacterTurn].IDCurrentRoom].DeliverSupplies(cityPlane.RessourcesNeeded))
                    {
                        GetOneCoinTime();
                        DeleteSavedCityAndAddNewOne();
                    }
                }
                else 
                {
                    if (rooms[holdRoomID].HasEnoughSpaceInHold())
                    {
                        List<Supply> supplies = rooms[players[idCharacterTurn].IDCurrentRoom].TransfertSuppliesToHold();
                        rooms[holdRoomID].AddSuppliesToHold(supplies);
                    }
                }
            }
            timerRound = Mathf.Clamp(timerRound - Time.deltaTime, 0.0f, timePerRound);
            if (timerRound <= 0.0f)
            {
                if (numberCoinTimerLeft > 0)
                {
                    timeUp = true;
                    OnTimeUp();
                }
                else
                {
                    timeUp = true;
                    isGameOver = true;
                    OnGameOver();
                }
            }
        }
    }
}
