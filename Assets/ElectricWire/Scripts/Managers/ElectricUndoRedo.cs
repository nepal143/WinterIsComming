
//(c8

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class UndoRedoData
    {
        public bool isAdded = false;
        public GameObject acualObject;
        public GameObject objectName;
        public List<Vector3> segments;
        public Vector3 position;
        public Quaternion rotation;

        public UndoRedoData(bool newIsAdded, GameObject newActualObject, GameObject newObjectName, List<Vector3> newSegments, Vector3 newPosition, Quaternion newRotation)
        {
            isAdded = newIsAdded;
            acualObject = newActualObject;
            objectName = newObjectName;
            segments = newSegments;
            position = newPosition;
            rotation = newRotation;
        }
    }

    public class ElectricUndoRedo : MonoBehaviour
    {
        public int maxUndoRedo = 1000;
        private int actualActionPosition = -1;
        private List<UndoRedoData> actionList = new List<UndoRedoData>();

        public void ClearUndoRedo()
        {
            actualActionPosition = -1;
            actionList = new List<UndoRedoData>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Undo ... will be CTRL-Z

                if (actualActionPosition < 0)
                    return;

                if (!actionList[actualActionPosition].isAdded)
                {
                    if (actionList[actualActionPosition].segments == null)
                    {
                        actionList[actualActionPosition].acualObject.SetActive(true);
                        actionList[actualActionPosition].acualObject.GetComponent<ElectricComponent>().StartManagement();
                        actionList[actualActionPosition].acualObject.GetComponent<ElectricComponent>().ReconnectAllConnector();
                    }
                    else
                    {
                        actionList[actualActionPosition].acualObject.SetActive(true);
                        actionList[actualActionPosition].acualObject.GetComponent<WireControl>().SetupLine(actionList[actualActionPosition].segments, false);
                    }
                }
                else
                {
                    ElectricComponent electricComponent = actionList[actualActionPosition].acualObject.transform.GetComponent<ElectricComponent>();
                    if (electricComponent != null)
                    {
                        electricComponent.StructureDisabled();
                    }
                    else
                    {
                        WireControl wireControl = actionList[actualActionPosition].acualObject.transform.GetComponentInParent<WireControl>();
                        if (wireControl != null)
                            wireControl.DisableWire();
                    }
                }

                actualActionPosition--;

                if (actualActionPosition < -1)
                    actualActionPosition = -1;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Redo ... will be CTRL-Y

                actualActionPosition++;

                if (actualActionPosition >= actionList.Count)
                {
                    actualActionPosition = actionList.Count - 1;
                    return;
                }

                if (!actionList[actualActionPosition].isAdded)
                {
                    ElectricComponent electricComponent = actionList[actualActionPosition].acualObject.transform.GetComponent<ElectricComponent>();
                    if (electricComponent != null)
                    {
                        electricComponent.StructureDisabled();
                    }
                    else
                    {
                        WireControl wireControl = actionList[actualActionPosition].acualObject.transform.GetComponentInParent<WireControl>();
                        if (wireControl != null)
                            wireControl.DisableWire();
                    }
                }
                else
                {
                    if (actionList[actualActionPosition].segments == null)
                    {
                        actionList[actualActionPosition].acualObject.SetActive(true);
                        actionList[actualActionPosition].acualObject.GetComponent<ElectricComponent>().StartManagement();
                        actionList[actualActionPosition].acualObject.GetComponent<ElectricComponent>().ReconnectAllConnector();
                    }
                    else
                    {
                        actionList[actualActionPosition].acualObject.SetActive(true);
                        actionList[actualActionPosition].acualObject.GetComponent<WireControl>().SetupLine(actionList[actualActionPosition].segments, false);
                    }
                }
            }
        }

        public void AddToUndoRedo(bool isAdded, GameObject spawnedObject, GameObject referencePrefab, Vector3 position, Quaternion rotation)
        {
            AddToUndoRedo(isAdded, spawnedObject, referencePrefab, null, position, rotation);
        }

        public void AddToUndoRedo(bool isAdded, GameObject spawnedObject, GameObject referencePrefab, List<Vector3> segments, Vector3 position, Quaternion rotation)
        {
            actualActionPosition++;

            if (actualActionPosition < actionList.Count)
                actionList.RemoveRange(actualActionPosition, actionList.Count - actualActionPosition);

            actionList.Add(new UndoRedoData(isAdded, spawnedObject, referencePrefab, segments, position, rotation));

            if (actionList.Count > maxUndoRedo)
            {
                actualActionPosition--;
                actionList.RemoveAt(0);
            }
        }
    }
}
