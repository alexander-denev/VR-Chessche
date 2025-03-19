using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRChessInteractable : MonoBehaviour
{
    private Chessman chessman;
    private XRGrabInteractable grabInteractable;
    private bool isOverSocket;

    private void Awake()
    {
        chessman = GetComponent<Chessman>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.hoverEntered.AddListener(OnHover);
        grabInteractable.hoverExited.AddListener(OnUnhover);
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
            BoardHighlights.Instance.DisableAllHighlights();
            VRChessSocket socket = socketInteractor.GetComponent<VRChessSocket>();
            if (chessman.currentX != socket.x || chessman.currentY != socket.y)
            {
                BoardManager.Instance.MoveChessman(chessman, socket.x, socket.y);
            }
        } else
        {
            BoardHighlights.Instance.SetTileYellow(chessman.currentX, chessman.currentY);
            BoardHighlights.Instance.HighlightPossibleMoves(chessman);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor socketInteractor)
        {
            
        } else
        {
            if (!isOverSocket)
            {
                grabInteractable.interactionManager.SelectEnter(args.interactorObject, grabInteractable);
            } 
        }
    }
    private void OnHover(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            isOverSocket = true;
        }
    }

    private void OnUnhover(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            isOverSocket = false;
        }
    }
}
