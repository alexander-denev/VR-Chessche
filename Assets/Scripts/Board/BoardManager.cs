using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = TILE_SIZE / 2;

    // Camera
    private Camera cam;

    // List of Chessman prefabs
    public List<GameObject> ChessmanPrefabs;
    // List of chessmans being on the board
    private List<GameObject> ActiveChessmans;
    // Array of the chessmans present on the particular board cell
    public Chessman[,] Chessmans{ set; get; }
    // Currently Selected Chessman
    public Chessman SelectedChessman;
    // Kings
    public Chessman WhiteKing;
    public Chessman BlackKing;
    public Chessman WhiteRook1;
    public Chessman WhiteRook2;
    public Chessman BlackRook1;
    public Chessman BlackRook2;

    // Allowed moves
    public bool[,] allowedMoves;
    // EnPassant move
    public int[] EnPassant { set; get; }

    // Variable to store turn
    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        cam = FindFirstObjectByType<Camera>();
        ActiveChessmans = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        EnPassant = new int[2] { -1, -1 };


        // Spawning all chessmans on the board
        SpawnAllChessmans();
    }

    private void Update()
    {
        // // If it is NPC's turn : Black
        if(!isWhiteTurn)
        {
            // NPC will make a move
            ChessAI.Instance.NPCMove();
            isWhiteTurn = true;
        }
    }

    public void MoveChessman(Chessman chessman, int x, int y)
    {
        bool[,] allowedMoves = chessman.PossibleMoves();

        if(allowedMoves[x,y])
        {
            Chessman opponent = Chessmans[x, y];

            if(opponent != null)
            {
                // Capture an opponent piece
                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);

            }
            // -------EnPassant Move Manager------------
            // If it is an EnPassant move than Destroy the opponent
            if (EnPassant[0] == x && EnPassant[1] == y && chessman.GetType() == typeof(Pawn))
            {
                if(isWhiteTurn)
                    opponent = Chessmans[x, y + 1];
                else
                    opponent = Chessmans[x, y - 1];

                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);

            }

            // Reset the EnPassant move
            EnPassant[0] = EnPassant[1] = -1;

            // Set EnPassant available for opponent
            if(chessman.GetType() == typeof(Pawn))
            {
                //-------Promotion Move Manager------------
                if (y == 7)
                {
                    ActiveChessmans.Remove(chessman.gameObject);
                    Destroy(chessman.gameObject);
                    SpawnChessman(10, new Vector3(x, 0, y));
                    chessman = Chessmans[x, y];
                }
                if (y == 0)
                {
                    ActiveChessmans.Remove(chessman.gameObject);
                    Destroy(chessman.gameObject);
                    SpawnChessman(4, new Vector3(x, 0, y));
                    chessman = Chessmans[x, y];
                }
                //-------Promotion Move Manager Over-------
                
                if (chessman.currentY == 1 && y == 3)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y - 1;
                }
                if (chessman.currentY == 6 && y == 4)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y + 1;
                }
            }
            // -------EnPassant Move Manager Over-------

            // -------Castling Move Manager------------
            // If the selectef chessman is King and is trying Castling move which needs two steps
            if(chessman.GetType() == typeof(King) && System.Math.Abs(x - chessman.currentX) == 2)
            {
                // King Side (towards (0, 0))
                if(x - chessman.currentX < 0)
                {
                    // Moving Rook1
                    Chessmans[x + 1, y] = Chessmans[x - 1, y];
                    Chessmans[x - 1, y] = null;
                    Chessmans[x + 1, y].SetPosition(x + 1, y);

                    XRSocketInteractor socket = BoardSockets.Instance.VrChessSockets[x + 1, y].GetComponent<XRSocketInteractor>();
                    XRGrabInteractable piece = Chessmans[x + 1, y].GetComponent<XRGrabInteractable>();
                    socket.interactionManager.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)piece);

                    Chessmans[x + 1, y].isMoved = true;
                }
                // Queen side (away from (0, 0))
                else
                {
                    // Moving Rook2
                    Chessmans[x - 1, y] = Chessmans[x + 2, y];
                    Chessmans[x + 2, y] = null;
                    Chessmans[x - 1, y].SetPosition(x - 1, y);

                    XRSocketInteractor socket = BoardSockets.Instance.VrChessSockets[x - 1, y].GetComponent<XRSocketInteractor>();
                    XRGrabInteractable piece = Chessmans[x - 1, y].GetComponent<XRGrabInteractable>();
                    socket.interactionManager.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)piece);

                    Chessmans[x - 1, y].isMoved = true;
                }
                // Note : King will move as a chessman by this function later
            }
            // -------Castling Move Manager Over-------

            Chessmans[chessman.currentX, chessman.currentY] = null;
            Chessmans[x, y] = chessman;
            chessman.SetPosition(x, y);
            chessman.isMoved = true;
            isWhiteTurn = !isWhiteTurn;
        }

        // ------- King Check Alert Manager -----------
        // Is it Check to the King
        // If now White King is in Check
        if(isWhiteTurn)
        {
            if(WhiteKing.InDanger())
                BoardHighlights.Instance.SetTileCheck(WhiteKing.currentX, WhiteKing.currentY);
        }
        // If now Black King is in Check
        else
        {
            if(BlackKing.InDanger())
                BoardHighlights.Instance.SetTileCheck(BlackKing.currentX, BlackKing.currentY);
        }
        // ------- King Check Alert Manager Over ----

       
        // Check if it is Checkmate
        IsCheckmate();
    }

    private void SpawnChessman(int index, Vector3 position)
    {
        GameObject ChessmanObject = Instantiate(ChessmanPrefabs[index], position, ChessmanPrefabs[index].transform.rotation) as GameObject;
        ChessmanObject.transform.SetParent(this.transform);
        ActiveChessmans.Add(ChessmanObject);

        int x = (int)(position.x);
        int y = (int)(position.z);
        Chessmans[x, y] = ChessmanObject.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);

    }
    
    private void SpawnAllChessmans()
    {
        // Spawn White Pieces
        // Rook1
        SpawnChessman(0, new Vector3(0, 0, 7));
        // Knight1
        SpawnChessman(1, new Vector3(1, 0, 7));
        // Bishop1
        SpawnChessman(2, new Vector3(2, 0, 7));
        // King
        SpawnChessman(3, new Vector3(3, 0, 7));
        // Queen
        SpawnChessman(4, new Vector3(4, 0, 7));
        // Bishop2
        SpawnChessman(2, new Vector3(5, 0, 7));
        // Knight2
        SpawnChessman(1, new Vector3(6, 0, 7));
        // Rook2
        SpawnChessman(0, new Vector3(7, 0, 7));
        // Pawns
        for(int i=0; i<8; i++)
        {
            SpawnChessman(5, new Vector3(i, 0, 6));
        }

        // Spawn Black Pieces
        // Rook1
        SpawnChessman(6, new Vector3(0, 0, 0));
        // Knight1
        SpawnChessman(7, new Vector3(1, 0, 0));
        // Bishop1
        SpawnChessman(8, new Vector3(2, 0, 0));
        // King
        SpawnChessman(9, new Vector3(3, 0, 0));
        // Queen
        SpawnChessman(10, new Vector3(4, 0, 0));
        // Bishop2
        SpawnChessman(8, new Vector3(5, 0, 0));
        // Knight2
        SpawnChessman(7, new Vector3(6, 0, 0));
        // Rook2
        SpawnChessman(6, new Vector3(7, 0, 0));
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, new Vector3(i, 0, 1));
        }

        WhiteKing = Chessmans[3, 7];
        BlackKing = Chessmans[3, 0];

        WhiteRook1 = Chessmans[0, 7];
        WhiteRook2 = Chessmans[7, 7];
        BlackRook1 = Chessmans[0, 0];
        BlackRook2 = Chessmans[7, 0];
    }

    public void EndGame()
    {
        if (!isWhiteTurn)
            Debug.Log("White team wins");
        else
            Debug.Log("Black team wins");

        foreach (GameObject go in ActiveChessmans)
            Destroy(go);

        // New Game
        isWhiteTurn = true;
        BoardHighlights.Instance.DisableAllHighlights();
        SpawnAllChessmans();
    }

    private void IsCheckmate()
    {
        bool hasAllowedMove = false;
        foreach(GameObject chessman in ActiveChessmans)
        {
            if(chessman.GetComponent<Chessman>().isWhite != isWhiteTurn)
                continue;

            bool[,] allowedMoves = chessman.GetComponent<Chessman>().PossibleMoves();

            for(int x=0; x<8; x++)
            {
                for(int y=0; y<8; y++)
                {
                    if(allowedMoves[x, y])
                    {
                        hasAllowedMove = true;
                        break;
                    }
                }
                if(hasAllowedMove) break;
            }
        }

        if(!hasAllowedMove) 
        {
            BoardHighlights.Instance.HighlightCheckmate(isWhiteTurn);

            Debug.Log("CheckMate");

            Debug.Log("Average Response Time of computer (in seconds): " + (ChessAI.Instance.averageResponseTime/1000.0));

            // Display Game Over Menu
            GameOver.Instance.GameOverMenu();

            // EndGame();
        }
    }
}
