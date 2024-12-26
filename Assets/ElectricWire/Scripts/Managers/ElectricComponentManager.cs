
//(c8

using UnityEngine;
using UnityEngine.EventSystems;

namespace ElectricWire
{
    public class ElectricComponentManager : MonoBehaviour
    {
        public int buildRange = 100;
        public bool continueBuildAfterPlacement = true;

        private float rotationez = 0f;
        private GameObject preview;
        private GameObject lastPrefabUse;

        public void Startup(GameObject newComponentGameObject)
        {
            lastPrefabUse = newComponentGameObject;
            preview = Instantiate(newComponentGameObject);
            preview.layer = LayerMask.NameToLayer("Ignore Raycast");
            SetChildLayers(preview.transform, LayerMask.NameToLayer("Ignore Raycast"));
        }

        private void OnDisable()
        {
            Destroy(preview);
        }
        
        private void Update()
        {
            // If we have a preview update it
            if (preview != null)
            {
                // Cancel placement
                if (Input.GetMouseButtonDown(1))
                {
                    ElectricManager.electricManager.StopPlacement();
                    return;
                }

                RaycastHit hit;
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(mouseRay, out hit, buildRange, Physics.DefaultRaycastLayers);
                }
                else
                    Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, buildRange, Physics.DefaultRaycastLayers);

                if (Vector3.Distance(Camera.main.transform.position, hit.point) <= buildRange)
                {
                    float mouseScrollDelta = Input.GetAxis("Mouse ScrollWheel");
                    if (mouseScrollDelta != 0.0f)
                        rotationez += mouseScrollDelta;

                    preview.transform.position = hit.point;
                    preview.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                    preview.transform.Rotate(0f, rotationez * 100.0f, 0f);
                }
                else
                {
                    Vector3 inFront = Camera.main.transform.position + Camera.main.transform.forward * buildRange;
                    preview.transform.position = inFront;
                }
            }

            // Place component
            if (Input.GetMouseButtonDown(0))
            {
                // Do nothing if over ui
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                PlaceComponent();
                return;
            }
        }

        private void PlaceComponent()
        {
            GameObject go = Instantiate(lastPrefabUse, preview.transform.position, preview.transform.rotation, ElectricManager.electricManager.allConstructionParent.transform);

            // Undo/Redo listing
            if (ElectricManager.electricManager.electricUndoRedo != null)
            {
                ElectricComponent electricComponent = go.GetComponent<ElectricComponent>();
                if (electricComponent != null && !electricComponent.skipUndoRedo)
                {
                    electricComponent.instantiatedFrom = lastPrefabUse;
                    ElectricManager.electricManager.electricUndoRedo.AddToUndoRedo(true, go, lastPrefabUse, preview.transform.position, preview.transform.rotation);
                }
            }

            Destroy(preview);
            preview = null;

            if (continueBuildAfterPlacement)
                ElectricManager.electricManager.OnClickComponentButtonUI(lastPrefabUse);
            else
                ElectricManager.electricManager.StopPlacement();
        }

        public void SetChildLayers(Transform t, int layer)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                child.gameObject.layer = layer;
                SetChildLayers(child, layer);
            }
        }
    }
}
