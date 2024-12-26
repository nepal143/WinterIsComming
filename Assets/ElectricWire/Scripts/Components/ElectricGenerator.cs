
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricGeneratorJsonData
    {
        public bool isOn;

        public ElectricGeneratorJsonData(bool newIsOn)
        {
            isOn = newIsOn;
        }
    }

    public class ElectricGenerator : ElectricComponent, ISaveJsonData
    {
        // Receive damage if drain more than max
        public int damageOverDrain = 5;

        // Max overload attempt before declare overload
        public int maxOverloadChance = 1;
        // Time before return on after overload, 0 = manual return
        public float breakerTime = 0f;

        private int actualRetryChance = 0;

        private void Start()
        {
            // When start with already placed prefabs in the scene
            StartManagement();
        }

        public override void StartManagement()
        {
            // Start energy management
            StartEnergyManagement();
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricGeneratorJsonData(GetSetIsOn));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricGeneratorJsonData electricGeneratorJsonData = JsonUtility.FromJson<ElectricGeneratorJsonData>(jsonData);
            if (electricGeneratorJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }
            
            SetOnOff(electricGeneratorJsonData.isOn);

            StartManagement();
        }

        #endregion

        public bool SwitchOnOff()
        {
            return SetOnOff(!GetSetIsOn);
        }

        private bool SetOnOff(bool state)
        {
            GetSetIsEnergized = state;
            GetSetIsOn = state;

            if (GetSetIsOn)
                StartEnergyManagement();
            else
                CancelInvoke();

            ActivateOutput();

            return GetSetIsOn;
        }

        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            // Activate object connected to the output
            ActivateOutput();
        }

        public override float IsDrainEnergy(int index)
        {
            // Generator generate energy, so we do not calculate all other component connected to the generator output in the drain
            return IsOn() ? drainEnergy : 0f;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // Generator do not need energize this way
        }

        #endregion

        private void OnDisable()
        {
            // Cancel energy management
            CancelInvoke();
        }

        private void StartEnergyManagement()
        {
            // Cancel energy management
            CancelInvoke();
            // Start energy management
            InvokeRepeating(nameof(ManageEnergy), 1f, 1f);
        }

        private void ManageEnergy()
        {
            // If connected to something
            if (IsWireConnected(false, 0))
            {
                float theDrain = wireOutput[0].GetComponent<WireControl>().wireConnectorInput.IsDrainEnergy();

                if (theDrain > IsGenerateEnergy(-1))
                {
                    if (actualRetryChance >= maxOverloadChance)
                    {
                        actualRetryChance = 0;

                        // If we pass IsGenerateEnergy range, turn off
                        GetSetIsOn = false;

                        ActivateOutput();

                        // TODO : Damage?

                        // Breaker time is use to turn back on
                        if (breakerTime > 0)
                            Invoke(nameof(JumpBreaker), breakerTime);
                    }
                    else
                        actualRetryChance++;
                }
                else
                    actualRetryChance = 0;
            }
            else
                actualRetryChance = 0;
        }

        private void JumpBreaker()
        {
            GetSetIsOn = true;

            ActivateOutput();
        }

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
                SwitchOnOff();
        }
    }
}
