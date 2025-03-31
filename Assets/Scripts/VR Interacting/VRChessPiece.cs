using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRChessPiece : MonoBehaviour
{
    private Chessman chessman;
    private XRGrabInteractable grabInteractable;

    private GameObject previewObject;
    [SerializeField] Material hoverPreviewMaterial;

    private XRSocketInteractor CurrentSocket
    {
        get { 
            return BoardSockets.Instance.VrChessSockets[chessman.currentX, chessman.currentY].GetComponent<XRSocketInteractor>(); 
        }
    }
    private XRSocketInteractor currentHoveringSocket;
    private bool isHovering;

    private Quaternion SnappedRotation
    {
        get
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            float snappedYRotation = Mathf.Round(currentRotation.y / 90) * 90;
            return Quaternion.Euler(0, snappedYRotation, 0);
        }
    }

    private void Awake()
    {
        chessman = GetComponent<Chessman>();

        previewObject = new GameObject("HoverPreviewObject_" + gameObject.name);
        previewObject.transform.localScale = transform.localScale;
        previewObject.transform.position = transform.position;
        previewObject.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        previewObject.AddComponent<MeshRenderer>().material = hoverPreviewMaterial;
        previewObject.SetActive(false);

        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnUngrab);
        grabInteractable.firstHoverEntered.AddListener(OnStartHovering);
        grabInteractable.lastHoverExited.AddListener(OnEndHovering);
    }

    private void Update()
    {
        HoverUpdate();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor socketInteractor) 
        {
            socketInteractor.transform.rotation = SnappedRotation;

            VRChessSocket vrSocket = socketInteractor.GetComponent<VRChessSocket>();
            if (chessman.currentX != vrSocket.x || chessman.currentY != vrSocket.y)
            {
                BoardManager.Instance.MoveChessman(chessman, vrSocket.x, vrSocket.y);
            }
        } else
        {
            BoardHighlights.Instance.HighlightPossibleMoves(chessman);

            previewObject.transform.position = CurrentSocket.transform.position;
            previewObject.SetActive(true);
        }
    }

    private void OnUngrab(SelectExitEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor)
        {
            previewObject.SetActive(false);
            BoardHighlights.Instance.DisableAllButYellowHighlights();

            if (currentHoveringSocket)
            {
                currentHoveringSocket.interactionManager.SelectEnter((IXRSelectInteractor)currentHoveringSocket, (IXRSelectInteractable)grabInteractable);
            }
            else
            {
                CurrentSocket.interactionManager.SelectEnter((IXRSelectInteractor)CurrentSocket, (IXRSelectInteractable)grabInteractable);
            }
            currentHoveringSocket = null;
        }
    }

    // HOVER ============================================================================================================

    private void HoverUpdate()
    {
        if (!isHovering) { return; }

        XRSocketInteractor closestSocketInteractor = GetClosestSocket();
        VRChessSocket closestVRSocket = closestSocketInteractor.GetComponent<VRChessSocket>();

        if (chessman.PossibleMoves()[closestVRSocket.x, closestVRSocket.y]) 
        {
            currentHoveringSocket = closestSocketInteractor;
        } else
        {
            currentHoveringSocket = CurrentSocket;
        }
        previewObject.transform.SetPositionAndRotation(currentHoveringSocket.transform.position, SnappedRotation);
    }

    private void OnStartHovering(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor eventSocket && !eventSocket.IsSelecting(grabInteractable))
        {
            isHovering = true;
        }
    }

    private void OnEndHovering(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            isHovering = false;
            currentHoveringSocket = CurrentSocket;
            previewObject.transform.position = CurrentSocket.transform.position;
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
