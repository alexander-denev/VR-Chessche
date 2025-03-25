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

    private XRSocketInteractor currentHoveringSocket;

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
            BoardHighlights.Instance.DisableAllHighlights();

            int socketX = (int)socketInteractor.transform.position.x;
            int socketY = (int)socketInteractor.transform.position.y;
            if (chessman.currentX != socketX || chessman.currentY != socketY)
            {
                BoardManager.Instance.MoveChessman(chessman, socketX, socketY);
            }
        } else
        {
            BoardHighlights.Instance.SetTileYellow(chessman.currentX, chessman.currentY);
            BoardHighlights.Instance.HighlightPossibleMoves(chessman);

            previewObject.transform.position = chessman.transform.position;
            previewObject.SetActive(true);
        }
    }

    private void OnUngrab(SelectExitEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor)
        {
            previewObject.SetActive(false);
            if (currentHoveringSocket)
            {
                currentHoveringSocket.interactionManager.SelectEnter((IXRSelectInteractor)currentHoveringSocket, (IXRSelectInteractable)grabInteractable);
            }
            else
            {
                XRSocketInteractor currentSocket = BoardSockets.Instance.VrChessSockets[chessman.currentX, chessman.currentY].GetComponent<XRSocketInteractor>();
                currentSocket.interactionManager.SelectEnter((IXRSelectInteractor)currentSocket, (IXRSelectInteractable)grabInteractable);
            }
        }
    }

    // HOVER ============================================================================================================

    private void HoverUpdate()
    {
        if (previewObject.activeSelf)
        {
            XRSocketInteractor closestSocket = GetClosestSocket();
            currentHoveringSocket = closestSocket;
            previewObject.transform.position = closestSocket.transform.position;
        }
    }

    private void OnStartHovering(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor eventSocket && !eventSocket.IsSelecting(grabInteractable))
        {
            previewObject.SetActive(true);
        }
    }

    private void OnEndHovering(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            previewObject.SetActive(false);
            currentHoveringSocket = null;
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
