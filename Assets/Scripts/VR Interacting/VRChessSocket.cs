using UnityEngine;

public class VRChessSocket : MonoBehaviour
{
    public int currentX;
    public int currentY;

    private void Start()
    {
        currentX = (int)transform.position.x;
        currentY = (int)transform.position.z;
    }

    public void Select()
    {
        BoardManager.Instance.SelectChessman(currentX, currentY);
    }
}
