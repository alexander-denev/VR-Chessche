using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRChessInteractable : XRGrabInteractable
{
    private Chessman chessman;
    private readonly List<XRSocketInteractor> hoveringSockets = new();
    private XRSocketInteractor currentClosestSocket;

    protected override void Awake()
    {
        base.Awake();
        chessman = GetComponent<Chessman>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
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

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (hoveringSockets.Count == 0)
        {
            XRSocketInteractor currentSocket = BoardSockets.Instance.VrChessSockets[chessman.currentX, chessman.currentY].GetComponent<XRSocketInteractor>();
            currentSocket.interactionManager.SelectEnter((IXRSelectInteractor)currentSocket, (IXRSelectInteractable)this); //FIXME: already selecting error
        }
    }

    // HOVER ============================================================================================================

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (args.interactorObject is XRSocketInteractor socket)
        {
            hoveringSockets.Add(socket);
            if (socket != GetClosestSocket())
            {
                socket.interactionManager.HoverCancel((IXRHoverInteractor)socket, (IXRHoverInteractable)this);
            }
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (args.interactorObject is XRSocketInteractor socket)
        {
            hoveringSockets.Remove(socket);
        }
    }

    private XRSocketInteractor GetClosestSocket()
    {
        XRSocketInteractor closest = null;
        float minDistance = float.MaxValue;

        foreach (var socket in BoardSockets.Instance.VrChessSockets)
        {
            float distance = Vector3.Distance(transform.position, socket.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = socket.GetComponent<XRSocketInteractor>();
            }
        }

        return closest;
    
    }
}
