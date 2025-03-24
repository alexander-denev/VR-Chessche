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

        previewObject = new GameObject("Hover Preview Object");
        previewObject.transform.SetParent(transform);
        previewObject.transform.localScale = Vector3.one;
        previewObject.transform.localPosition = Vector3.zero;
        previewObject.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        previewObject.AddComponent<MeshRenderer>().material = hoverPreviewMaterial;
        previewObject.SetActive(false);

        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnUngrab);
        grabInteractable.firstHoverEntered.AddListener(OnStartHovering);
        grabInteractable.lastHoverExited.AddListener(OnEndHovering);
        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
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
        }
    }

    private void OnUngrab(SelectExitEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor && !currentHoveringSocket)
        {
            XRSocketInteractor currentSocket = BoardSockets.Instance.VrChessSockets[chessman.currentX, chessman.currentY].GetComponent<XRSocketInteractor>();
            currentSocket.interactionManager.SelectEnter((IXRSelectInteractor)currentSocket, (IXRSelectInteractable)grabInteractable);
        }
    }

    // HOVER ============================================================================================================

    private void OnStartHovering(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor && !(args.interactableObject as XRGrabInteractable).isSelected)
        {
            previewObject.SetActive(true);
        }
    }

    private void OnEndHovering(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            previewObject.SetActive(false);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor eventSocket)
        {
            if (eventSocket = GetClosestSocket())
            {
            }
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        //if (args.interactorObject is XRSocketInteractor eventSocket)
        //{
        //    if (!args.isCanceled)
        //    {
        //        currentHoveringSocket = null;
        //    }
        //}
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
