
//(c8

using UnityEngine;
using UnityEngine.EventSystems;

namespace ElectricWire
{
    public class ElectricRemoveManager : MonoBehaviour
    {
        private int removeRange = 20;
        
        private void Update()
        {
            // Do nothing if over ui
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // If UI selected do not do nothing
            if (EventSystem.current.currentSelectedGameObject != null)
                return;

            RaycastHit hit;
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(mouseRay, out hit, removeRange, Physics.DefaultRaycastLayers);
            }
            else
                Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, removeRange, Physics.DefaultRaycastLayers);

            if (hit.transform != null && Vector3.Distance(Camera.main.transform.position, hit.point) <= removeRange)
            {
                ElectricComponent electricComponent = hit.transform.GetComponent<ElectricComponent>();
                if (electricComponent != null)
                {
                    ElectricManager.electricManager.SetText("[REMOVE OBJECT] " + hit.transform.name.Replace("(Clone)", ""));
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Undo/Redo listing
                        if (ElectricManager.electricManager.electricUndoRedo != null)
                        {
                            if (!electricComponent.skipUndoRedo)
                            {
                                ElectricManager.electricManager.electricUndoRedo.AddToUndoRedo(false, hit.transform.gameObject, electricComponent.instantiatedFrom, hit.transform.position, hit.transform.rotation);
                                electricComponent.StructureDisabled();
                            }
                            else
                                electricComponent.StructureDestroyed();
                        }
                        else
                            electricComponent.StructureDestroyed();
                    }
                }
                else
                {
                    WireControl wireControl = hit.transform.GetComponentInParent<WireControl>();
                    if (wireControl != null)
                    {
                        ElectricManager.electricManager.SetText("[REMOVE OBJECT] " + hit.transform.parent.name.Replace("(Clone)", ""));
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Undo/Redo listing
                            if (ElectricManager.electricManager.electricUndoRedo != null)
                            {
                                if (!wireControl.skipUndoRedo)
                                {
                                    ElectricManager.electricManager.electricUndoRedo.AddToUndoRedo(false, wireControl.gameObject, wireControl.instantiatedFrom, wireControl.segmentsT, hit.transform.position, hit.transform.rotation);
                                    wireControl.DisableWire();
                                }
                                else
                                    wireControl.DisconnectWire();
                            }
                            else
                                wireControl.DisconnectWire();
                        }
                    }
                    else
                        ElectricManager.electricManager.SetText("[REMOVE OBJECT]");
                }
            }

            // Stop
            if (Input.GetMouseButtonDown(1))
                ElectricManager.electricManager.StopPlacement();
        }
    }
}
