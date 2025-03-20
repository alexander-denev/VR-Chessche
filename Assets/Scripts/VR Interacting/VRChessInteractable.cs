using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRChessInteractable : XRGrabInteractable
{
    private Chessman chessman;
    private readonly List<XRSocketInteractor> hoveringSockets = new();
    private XRSocketInteractor currentHoveringSocket;

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
        if (!currentHoveringSocket)
        {
            XRSocketInteractor currentSocket = BoardSockets.Instance.VrChessSockets[chessman.currentX, chessman.currentY].GetComponent<XRSocketInteractor>();
            currentSocket.interactionManager.SelectEnter((IXRSelectInteractor)currentSocket, (IXRSelectInteractable)this); //FIXME: already selecting error
        }
    }

    // HOVER ============================================================================================================

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (args.interactorObject is XRSocketInteractor eventSocket)
        {
            if (eventSocket != currentHoveringSocket)
            {
                if (currentHoveringSocket != null)
                {
                    currentHoveringSocket.interactionManager.HoverCancel((IXRHoverInteractor)currentHoveringSocket, (IXRHoverInteractable)this);
                }
                currentHoveringSocket = eventSocket;
            }
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (args.interactorObject is XRSocketInteractor eventSocket)
        {
            if (!args.isCanceled)
            {
                currentHoveringSocket = null;
            }
        }
    }

}
