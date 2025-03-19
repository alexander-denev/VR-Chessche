using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRChessInteractable : MonoBehaviour
{
    private Chessman chessman;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        chessman = GetComponent<Chessman>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor socketInteractor) 
        {
            VRChessSocket socket = socketInteractor.GetComponent<VRChessSocket>();
            BoardManager.Instance.SelectChessman(socket.x, socket.y);
        } else
        {
            BoardManager.Instance.SelectChessman(chessman.currentX, chessman.currentY);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            Debug.Log(gameObject.name + " released by socket!");
        }
        else
        {
            BoardManager.Instance.DeselectChessman();
        }
    }
}
