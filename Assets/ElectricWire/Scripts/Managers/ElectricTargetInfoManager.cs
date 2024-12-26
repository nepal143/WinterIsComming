
//(c8

using UnityEngine;
using UnityEngine.EventSystems;

namespace ElectricWire
{
    public class ElectricTargetInfoManager : MonoBehaviour
    {
        private int targetInfoRange = 20;

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
                Physics.Raycast(mouseRay, out hit, targetInfoRange, Physics.DefaultRaycastLayers);
            }
            else
                Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, targetInfoRange, Physics.DefaultRaycastLayers);

            if (hit.transform != null && Vector3.Distance(Camera.main.transform.position, hit.point) <= targetInfoRange)
            {
                ElectricComponent electricComponent = hit.transform.GetComponent<ElectricComponent>();
                if (electricComponent != null)
                {
                    ElectricManager.electricManager.SetText("[Drain from output] " + electricComponent.IsDrainEnergy() + "\n" +
                                                            "[Generate from input] " + electricComponent.IsGenerateEnergy() + "\n" +
                                                            "[Component drain] " + electricComponent.drainEnergy + "\n" +
                                                            "[Component generate] " + electricComponent.generateEnergy);
                }
                else
                {
                    WireControl wireControl = hit.transform.GetComponentInParent<WireControl>();
                    if (wireControl != null)
                    {
                        ElectricManager.electricManager.SetText("[Wire Input] " + wireControl.wireConnectorInput.GetComponentInParent<ElectricComponent>().name.Replace("(Clone)", "") + "\n" +
                                                                "[Wire Output] " + wireControl.wireConnectorOutput.GetComponentInParent<ElectricComponent>().name.Replace("(Clone)", ""));
                    }
                    else
                        ElectricManager.electricManager.SetText("[INFO]");
                }
            }

            // Stop
            if (Input.GetMouseButtonDown(1))
                ElectricManager.electricManager.StopPlacement();
        }
    }
}
