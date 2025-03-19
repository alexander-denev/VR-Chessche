using UnityEngine;

public class VRChessSocket : MonoBehaviour
{
    public int x;
    public int y;

    private void Start()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.z;
    }

    public void Select()
    {
        BoardManager.Instance.SelectChessman(x, y);
    }
}
