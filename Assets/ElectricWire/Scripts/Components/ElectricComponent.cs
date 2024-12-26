
//(c8

using System;
using UnityEngine;
using UnityEngine.Events;

namespace ElectricWire
{
    // Use by electric component to activate things when energized or toggled on/off
    [Serializable] public class UnityEventBool : UnityEvent<bool> { }

    [Serializable]
    public class ElectricComponentJsonData
    {
        public string componentName;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float rotationW;

        public ElectricComponentJsonData(string newComponentName, Vector3 newPosition, Quaternion newRotation)
        {
            componentName = newComponentName;
            positionX = newPosition.x;
            positionY = newPosition.y;
            positionZ = newPosition.z;
            rotationX = newRotation.x;
            rotationY = newRotation.y;
            rotationZ = newRotation.z;
            rotationW = newRotation.w;
        }
    }

    public class ElectricComponent : MonoBehaviour, IWire
    {
        [HideInInspector] public GameObject instantiatedFrom;

        [Header("If you do not want this component save")]
        public bool skipSave = false;
        [Header("If you do not want this component have undo/redo")]
        public bool skipUndoRedo = false;
        public bool _isEnergized = false;
        public bool _isOn = false;

        public bool GetSetIsEnergized
        {
            get { return _isEnergized; }
            set
            {
                _isEnergized = value;
                onEnergized.Invoke(IsEnergized());

                // Do the On stuff if isOn
                if (IsOn())
                    TurnOnOff();
            }
        }

        public bool GetSetIsOn
        {
            get { return _isOn; }
            set
            {
                _isOn = value;
                TurnOnOff();
            }
        }

        // This component drain energy, how much
        public float drainEnergy = 0f;
        // This component generate energy, how much
        public float generateEnergy = 0f;

        [Header("Events")]
        public UnityEventBool onEnergized;
        public UnityEventBool onTurnOnOff;
        public UnityEventBool onTurnOnOffOnlyWhenEnergized;

        // Connector
        public GameObject[] wireConnectorInput;
        public GameObject[] wireConnectorOutput;
        // WireConnection
        [HideInInspector] public GameObject[] wireInput;
        [HideInInspector] public GameObject[] wireOutput;
        // Connector connected (for client)
        [HideInInspector] public bool[] wireInputIsConnected;
        [HideInInspector] public bool[] wireOutputIsConnected;

        public virtual void Awake()
        {
            for (int i = 0; i < wireConnectorInput.Length; i++)
                wireConnectorInput[i].GetComponent<WireConnector>().SetupInit(true, i);
            for (int i = 0; i < wireConnectorOutput.Length; i++)
                wireConnectorOutput[i].GetComponent<WireConnector>().SetupInit(false, i);

            wireInput = new GameObject[wireConnectorInput.Length];
            wireOutput = new GameObject[wireConnectorOutput.Length];
            wireInputIsConnected = new bool[wireConnectorInput.Length];
            wireOutputIsConnected = new bool[wireConnectorOutput.Length];
        }

        public string GetJsonPosition()
        {
            // By default return the position
            string jsonData = JsonUtility.ToJson(new PositionJsonData(gameObject.name.Replace("(Clone)", ""), transform.position, transform.rotation));
            return jsonData;
        }

        public void TurnOnOff()
        {
            // For stuff activated when turn on, example switch button?
            onTurnOnOff.Invoke(IsOn());
            // For stuff that must only be activated if energized, example light?
            TurnOnOffOnlyWhenEnergized();
        }

        public void TurnOnOffOnlyWhenEnergized()
        {
            onTurnOnOffOnlyWhenEnergized.Invoke(IsEnergized() && IsOn());
        }

        public void SetIsEnergize(bool newIsEnergized)
        {
            GetSetIsEnergized = newIsEnergized;

            // Do the On stuff if isOn
            if (IsOn())
                TurnOnOff();
        }

        public void SetIsOn(bool newIsOn)
        {
            GetSetIsOn = newIsOn;
            TurnOnOff();
        }

        public virtual void ReconnectAllConnector()
        {
            for (int i = 0; i < wireConnectorInput.Length; i++)
            {
                Collider[] hitColliders = Physics.OverlapSphere(wireConnectorInput[i].transform.position, 0.01f);
                for (int ii = 0; ii < hitColliders.Length; ii++)
                {
                    WireControl wireControlTemp = hitColliders[ii].GetComponentInParent<WireControl>();
                    if (wireControlTemp != null)
                    {
                        // TODO : Find why found 4 seg0
                        //print("Input: " + ii + " : " + hitColliders[i].name);
                        wireControlTemp.ReSetupWire();
                        break;
                    }
                }
            }
            for (int i = 0; i < wireConnectorOutput.Length; i++)
            {
                Collider[] hitColliders = Physics.OverlapSphere(wireConnectorOutput[i].transform.position, 0.01f);
                for (int ii = 0; ii < hitColliders.Length; ii++)
                {
                    WireControl wireControlTemp = hitColliders[ii].GetComponentInParent<WireControl>();
                    if (wireControlTemp != null)
                    {
                        // TODO : Find why found 4 seg0
                        //print("Output: " + ii + " : " + hitColliders[i].name);
                        wireControlTemp.ReSetupWire();
                        break;
                    }
                }
            }
        }

        public virtual void StartManagement()
        {

        }

        public virtual void StructureDestroyed()
        {
            for (int i = 0; i < wireInput.Length; i++)
            {
                if (wireInput[i] != null)
                    wireInput[i].GetComponent<WireControl>().wireConnectorOutput.DisconnectWire();
            }
            for (int i = 0; i < wireOutput.Length; i++)
            {
                if (wireOutput[i] != null)
                    wireOutput[i].GetComponent<WireControl>().wireConnectorInput.DisconnectWire();
            }

            Destroy(gameObject);
        }

        public virtual void StructureDisabled()
        {
            for (int i = 0; i < wireInput.Length; i++)
            {
                if (wireInput[i] != null)
                    wireInput[i].GetComponent<WireControl>().wireConnectorOutput.DisconnectWire();
            }
            for (int i = 0; i < wireOutput.Length; i++)
            {
                if (wireOutput[i] != null)
                    wireOutput[i].GetComponent<WireControl>().wireConnectorInput.DisconnectWire();
            }

            gameObject.SetActive(false);
        }

        #region IWire interface

        public virtual void ConnectWire(GameObject wire, bool isInput, int index)
        {
            if (isInput)
            {
                wireInput[index] = wire;
                wireInputIsConnected[index] = true;
            }
            else
            {
                wireOutput[index] = wire;
                wireOutputIsConnected[index] = true;
            }
        }

        public virtual void DisconnectWire(bool isInput, int index)
        {
            if (isInput)
            {
                wireInput[index] = null;
                wireInputIsConnected[index] = false;
            }
            else
            {
                wireOutput[index] = null;
                wireOutputIsConnected[index] = false;
            }
        }

        public virtual bool IsWireConnected(bool isInput, int index)
        {
            if (isInput)
                return wireInputIsConnected.Length > index && wireInputIsConnected[index];
            else
                return wireOutputIsConnected.Length > index && wireOutputIsConnected[index];
        }

        public virtual bool IsEnergized()
        {
            return GetSetIsEnergized;
        }

        public virtual bool IsOn()
        {
            return GetSetIsOn;
        }

        public virtual float IsGenerateEnergy(int index = -1)
        {
            // This is how much energy this component drain
            float thisComponentDrainEnergy = IsOn() ? drainEnergy : 0f;
            // This is the total generated energy read from the input of this component
            // And the energy generated by this component is added first
            float totalGenerateEnergy = IsOn() ? generateEnergy : 0f;

            // If this component is On and do not generate energy by itself
            if (IsOn() && totalGenerateEnergy == 0f)
            {
                // Check each input to see if we receive generated energy
                for (int i = 0; i < wireInput.Length; i++)
                {
                    if (IsWireConnected(true, i))
                        totalGenerateEnergy += wireInput[i].GetComponent<WireControl>().wireConnectorOutput.IsGenerateEnergy();
                }

                // Check each output to see it we send generated energy
                for (int i = 0; i < wireOutput.Length; i++)
                {
                    // Do not calculate the output
                    if (i != index && IsWireConnected(false, i))
                        totalGenerateEnergy -= wireOutput[i].GetComponent<WireControl>().wireConnectorInput.IsDrainEnergy();
                }

                // If this component drain energy, remove it from the total generated energy
                totalGenerateEnergy -= thisComponentDrainEnergy;

                if (totalGenerateEnergy < 0f)
                    totalGenerateEnergy = 0f;
            }

            return IsOn() ? totalGenerateEnergy : 0f;
        }

        public virtual float IsDrainEnergy(int index = -1)
        {
            float totalDrainEnergy = IsOn() ? drainEnergy : 0f;

            for (int i = 0; i < wireOutput.Length; i++)
            {
                if (IsWireConnected(false, i))
                    totalDrainEnergy += wireOutput[i].GetComponent<WireControl>().wireConnectorInput.IsDrainEnergy();
            }

            return IsOn() ? totalDrainEnergy : 0f;
        }

        public virtual void ActivateByWire(string data)
        {
            SetIsOn(data == "1" ? true : false);
        }

        public virtual void EnergizeByWire(bool onOff, int index)
        {
            SetIsEnergize(onOff);
        }

        #endregion

        // Specialize data transmission
        public virtual void ActivateDataOutput(string data)
        {
            // Activating output mean activating the input of the other end of the wire
            // By default a component activate all output if he are energized and on
            // Override to make different behaviour
            if (IsEnergized() && IsOn())
            {
                for (int i = 0; i < wireOutput.Length; i++)
                {
                    if (wireOutput[i] != null)
                        wireOutput[i].GetComponent<WireControl>().wireConnectorInput.ActivateByWire(data);
                }
            }
        }

        // Standard energized activation
        public virtual void ActivateOutput()
        {
            // Activating output mean activating the input of the other end of the wire
            // By default a component activate all output if he are energized and on
            // Override to make different behaviour
            for (int i = 0; i < wireOutput.Length; i++)
            {
                if (wireOutput[i] != null)
                    wireOutput[i].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(IsEnergized() && IsOn());
            }
        }
    }
}
