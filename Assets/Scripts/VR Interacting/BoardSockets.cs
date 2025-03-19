using UnityEngine;

public class BoardSockets : MonoBehaviour
{
    public static BoardSockets Instance { get; set; }

    [SerializeField] GameObject VrChessSocketPrefab;

    public GameObject[,] VrChessSockets = new GameObject[8, 8];

    private void Start()
    {
        Instance = this;
        PlaceAllTiles();
    }

    public void PlaceAllTiles()
    {
        GameObject tile;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tile = Instantiate(VrChessSocketPrefab);
                tile.transform.position = new Vector3(i, 0.0001f, j);
                tile.transform.SetParent(this.transform);
                VrChessSockets[i, j] = tile;
                
            }
        }
    }
}
