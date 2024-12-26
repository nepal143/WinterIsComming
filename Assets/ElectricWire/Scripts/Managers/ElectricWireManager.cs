
//(c8

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ElectricWire
{
    public class ElectricWireManager : MonoBehaviour
    {
        // Make connector glow
        public bool makeConnectorGlow = true;
        // Distance from player to be able connect a wire
        public float connectMaxRange = 3;
        // Longest wire from point to point
        public float wireMaxRange = 8;
        // Max number of segments in a wire
        public float wireMaxSegment = 5;
        public bool continueWireAfterPlacement = true;

        private Color canBuildColor = Color.cyan;
        private Color cantBuildColor = Color.red;
        private int buildRange = 10;

        private GameObject wirePrefab;
        private bool isPlacingWire = false;
        private int numberOfSegment = 0;
        private Transform targetOne;
        private bool targetOneIsInput = false;
        private Transform targetTwo;
        private List<Vector3> wireSegments = new List<Vector3>();
        private Vector3 targetSegment;
        private GameObject preview;
        private int actualSegmentPosition = 1;

        public void Startup(GameObject newWireGameObject)
        {
            wirePrefab = newWireGameObject;
        }

        private void Update()
        {
            RaycastHit hit;
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(mouseRay, out hit, buildRange, Physics.DefaultRaycastLayers);
            }
            else
                Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, buildRange, Physics.DefaultRaycastLayers);

            // If hit connector make it glow
            if (hit.transform != null && makeConnectorGlow)
            {
                WireConnector wireConnector = hit.transform.GetComponent<WireConnector>();
                if (wireConnector != null)
                    wireConnector.EnableGlow();
            }

            // If we have a preview update it
            if (preview != null)
            {
                // Cancel placing wire
                if (Input.GetMouseButtonDown(1))
                {
                    isPlacingWire = false;
                    Destroy(preview);
                    return;
                }

                if (Vector3.Distance(Camera.main.transform.position, hit.point) <= buildRange)
                    preview.GetComponent<LineRenderer>().SetPosition(actualSegmentPosition, hit.point);
                else
                {
                    Vector3 inFront = Camera.main.transform.position + Camera.main.transform.forward * buildRange;
                    preview.GetComponent<LineRenderer>().SetPosition(actualSegmentPosition, inFront);
                }

                bool canBuild = CanBuildThere(hit);
                foreach (Renderer renderer in preview.GetComponentsInChildren<Renderer>())
                    renderer.material.color = canBuild ? canBuildColor : cantBuildColor;
            }

            // Start placing wire
            if (Input.GetMouseButtonDown(0))
            {
                // Do nothing if over ui
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                if (CanBuildThere(hit))
                    PlaceWire();
                return;
            }

            // Stop placing wire
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(preview);
                ElectricManager.electricManager.StopPlacement();
                return;
            }
        }

        private void PlaceWire()
        {
            // If not placing a wire, start placing one
            if (!isPlacingWire)
            {
                isPlacingWire = true;
                numberOfSegment = 0;
                targetTwo = null;
                wireSegments = new List<Vector3>();
                wireSegments.Add(targetOne.position);
                preview = new GameObject("preview");
                preview.transform.position = targetOne.position;
                LineRenderer lineRenderer = preview.AddComponent<LineRenderer>();
                lineRenderer.useWorldSpace = true;
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, targetOne.position);
                lineRenderer.SetPosition(1, targetOne.position);
                // Reset segment position on new wire
                actualSegmentPosition = 1;
            }
            else
            {
                // Make segment if we do not have a second connector as target
                if (targetTwo == null)
                {
                    preview.GetComponent<LineRenderer>().SetPosition(actualSegmentPosition, targetSegment);
                    preview.GetComponent<LineRenderer>().positionCount++;
                    actualSegmentPosition++;
                    // We set again the new at this position or we see at position 0,0,0 until update
                    preview.GetComponent<LineRenderer>().SetPosition(actualSegmentPosition, targetSegment);
                    numberOfSegment++;
                    wireSegments.Add(targetSegment);
                    return;
                }

                isPlacingWire = false;
                wireSegments.Add(targetTwo.position);
                Destroy(preview);

                // Create wire
                GameObject go = Instantiate(wirePrefab);
                go.GetComponent<WireControl>().SetupLine(wireSegments);
                go.transform.parent = ElectricManager.electricManager.allConstructionParent.transform;

                // Undo/Redo listing
                if (ElectricManager.electricManager.electricUndoRedo != null)
                {
                    WireControl wireControl = go.GetComponent<WireControl>();
                    if (wireControl != null && !wireControl.skipUndoRedo)
                    {
                        wireControl.instantiatedFrom = wirePrefab;
                        ElectricManager.electricManager.electricUndoRedo.AddToUndoRedo(true, go, wirePrefab, wireSegments, preview.transform.position, preview.transform.rotation);
                    }
                }

                if (!continueWireAfterPlacement)
                    ElectricManager.electricManager.StopPlacement();
            }
        }

        public bool CanBuildThere(RaycastHit hit)
        {
            // TODO : Why need this ??
            //if (hit.point == Vector3.zero) return false;

            if (hit.transform == null) return false;

            var wireConnector = hit.transform.GetComponent<WireConnector>();

            if (!isPlacingWire)
                targetOne = null;

            targetTwo = null;

            // Move point little bit on normal
            // Use to place wire little bit over the raycast position
            // And use for the check path line cast, or the linecast will find the object at hit.point
            Vector3 newHitPoint = hit.point + hit.normal * 0.01f;

            // Check if the path from last position and this position is clear
            // ex.: do not make wire pass thru wall, ...
            if (targetOne != null)
            {
                // Check distance from last point
                if (Vector3.Distance(wireSegments[wireSegments.Count - 1], hit.point) > wireMaxRange)
                    return false;

                if (wireConnector != null)
                {
                    // On connector we use raycast to find it, because line cast can say false in certain condition
                    Vector3 heading = hit.point - wireSegments[wireSegments.Count - 1];
                    if (Physics.Raycast(wireSegments[wireSegments.Count - 1], heading.normalized, out RaycastHit hit2, connectMaxRange, Physics.DefaultRaycastLayers))
                    {
                        if (hit2.transform != wireConnector.transform)
                            return false;
                    }
                }
                else if (Physics.Linecast(wireSegments[wireSegments.Count - 1], newHitPoint, Physics.DefaultRaycastLayers))
                    return false;
            }

            bool foundAttach = false;
            if (wireConnector != null)
            {
                // If this connect has no wire connection
                if (!wireConnector.IsWireConnected())
                {
                    if (!isPlacingWire)
                    {
                        // If we are not already placing wire
                        targetOne = wireConnector.transform;
                        targetOneIsInput = wireConnector.isInput;
                        foundAttach = true;
                    }
                    else
                    {
                        // If not trying connect a loop on same object and not connecting input to input
                        if (targetOne.parent.gameObject != wireConnector.transform.parent.gameObject && targetOneIsInput != wireConnector.isInput)
                        {
                            targetTwo = wireConnector.transform;
                            foundAttach = true;
                        }
                    }
                }
            }
            else
            {
                // If not a connector and we already in place wire mode add segment
                if (isPlacingWire)
                {
                    if (numberOfSegment < wireMaxSegment)
                    {
                        // Use modified position
                        targetSegment = newHitPoint;
                        foundAttach = true;
                    }
                }
            }

            if (!foundAttach)
                return false;

            // linecast to make sure that nothing is between us and the build preview
            return !Physics.Linecast(Camera.main.transform.position, newHitPoint, Physics.DefaultRaycastLayers);
        }
    }
}
