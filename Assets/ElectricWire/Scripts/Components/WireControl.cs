
//(c8

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class WireJsonData
    {
        public List<Vector3> segments;

        public WireJsonData(List<Vector3> newSegments)
        {
            segments = newSegments;
        }
    }

    public class WireControl : MonoBehaviour, ISaveJsonData
    {
        [HideInInspector] public GameObject instantiatedFrom;

        [Header("If you do not want this component save")]
        public bool skipSave = false;
        [Header("If you do not want this component have undo/redo")]
        public bool skipUndoRedo = false;

        public LineRenderer lineRenderer;

        public string segmentsPosition;

        // temp position while loading
        public List<Vector3> segmentsT;
        private int checkConnectionTry = 0;
        private int checkConnectionMaxTry = 5;
        private float checkConnectionSecTry = 1f;

        // Wire connections
        public WireConnector wireConnectorInput;
        public WireConnector wireConnectorOutput;
        private WireConnector wireConnectorTemp;
        private bool firstIsInput = false;

        public void Start()
        {
            // When start with already placed prefabs in the scene
            if (wireConnectorInput != null && wireConnectorOutput != null)
            {
                wireConnectorInput.ConnectWire(gameObject);
                wireConnectorOutput.ConnectWire(gameObject);
            }
        }

        private void OnDisable()
        {
            // TODO : Remove connected to?
        }

        public string GetJsonPosition()
        {
            string jsonData = JsonUtility.ToJson(new PositionJsonData(gameObject.name.Replace("(Clone)", ""), transform.position, transform.rotation));
            return jsonData;
        }

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new WireJsonData(segmentsT));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            WireJsonData wireJsonData = JsonUtility.FromJson<WireJsonData>(jsonData);
            if (wireJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            SetupLine(wireJsonData.segments, true);
        }

        private void ReSetupLine()
        {
            checkConnectionTry++;
            if (checkConnectionTry < checkConnectionMaxTry)
            {
                SetupLine(segmentsT, true);
            }
            else
            {
                Debug.LogWarning("Wire: " + gameObject.name + " position: " + gameObject.transform.position + " did not find a connector, object removed!");
                Destroy(gameObject);
            }
        }

        public Vector3 SetupLine(List<Vector3> segments, bool isLoading = false)
        {
            segmentsT = segments;

            // Find connection at each end
            FindConnectionAtEachEnd();

            // Do we have a connection at each end?
            if (wireConnectorInput == null || wireConnectorOutput == null)
            {
                // When loading from database, check number of time for a late spawn of connector
                if (isLoading)
                    Invoke(nameof(ReSetupLine), checkConnectionSecTry);
                return Vector3.zero;
            }

            // Wire start is at the output position
            if (!isLoading)
                transform.position = firstIsInput ? wireConnectorOutput.transform.position : wireConnectorInput.transform.position;

            lineRenderer.positionCount = segmentsT.Count;
            for (int i = 0; i < segmentsT.Count; i++)
                lineRenderer.SetPosition(i, transform.InverseTransformPoint(segmentsT[i]));

            for (int i = 0; i < segmentsT.Count - 1; i++)
            {
                var seg = new GameObject("seg" + i);
                var newCollider = seg.AddComponent<BoxCollider>();
                newCollider.isTrigger = true;
                newCollider.center = new Vector3(0f, 0f, 0.05f);
                newCollider.size = new Vector3(0.05f, 0.05f, 0.1f);
                seg.transform.position = segmentsT[i];
                seg.transform.SetParent(transform, true);
                // Make the collider follow wire, object start position is at positionTwo
                seg.transform.rotation = Quaternion.LookRotation((segmentsT[i + 1] - segmentsT[i]).normalized);
                seg.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(segmentsT[i + 1], segmentsT[i]) * 10f);
            }
            
            // Connect wire
            wireConnectorInput.ConnectWire(gameObject);
            wireConnectorOutput.ConnectWire(gameObject);

            // Return the second target, to make the collider correctly
            // And making wire position at a connector help for looting wire
            return firstIsInput ? wireConnectorOutput.transform.position : wireConnectorInput.transform.position;
        }

        private void FindConnectionAtEachEnd()
        {
            // Find connection at each end
            Collider[] hitColliders = Physics.OverlapSphere(segmentsT[0], 0.01f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                wireConnectorTemp = hitColliders[i].GetComponent<WireConnector>();
                if (wireConnectorTemp != null)
                {
                    if (wireConnectorTemp.isInput)
                    {
                        firstIsInput = true;
                        wireConnectorInput = wireConnectorTemp;
                    }
                    else
                        wireConnectorOutput = wireConnectorTemp;
                    break;
                }
            }
            hitColliders = Physics.OverlapSphere(segmentsT[segmentsT.Count - 1], 0.01f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                wireConnectorTemp = hitColliders[i].GetComponent<WireConnector>();
                if (wireConnectorTemp != null)
                {
                    if (wireConnectorTemp.isInput)
                        wireConnectorInput = wireConnectorTemp;
                    else
                        wireConnectorOutput = wireConnectorTemp;
                    break;
                }
            }

            wireConnectorTemp = null;
        }

        public void DisconnectWire()
        {
            if (wireConnectorInput != null)
                wireConnectorInput.DisconnectWire();
            if (wireConnectorOutput != null)
                wireConnectorOutput.DisconnectWire();

            Destroy(gameObject);
        }

        public void DisableWire()
        {
            if (wireConnectorInput != null)
                wireConnectorInput.DisconnectWire();
            if (wireConnectorOutput != null)
                wireConnectorOutput.DisconnectWire();

            wireConnectorInput = null;
            wireConnectorOutput = null;

            gameObject.SetActive(false);
        }

        public void ReSetupWire()
        {
            FindConnectionAtEachEnd();

            if (wireConnectorInput == null || wireConnectorOutput == null)
                return;

            // Connect wire
            wireConnectorInput.ConnectWire(gameObject);
            wireConnectorOutput.ConnectWire(gameObject);
        }
    }
}
