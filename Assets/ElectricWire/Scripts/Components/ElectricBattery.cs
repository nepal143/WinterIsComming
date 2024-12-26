
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricBatteryJsonData
    {
        public float accumulatedEnergySec;

        public ElectricBatteryJsonData(float newAccumulatedEnergySec)
        {
            accumulatedEnergySec = newAccumulatedEnergySec;
        }
    }

    public class ElectricBattery : ElectricComponent, ISaveJsonData
    {
        public float maxEnergy = 36f;
        public GameObject energyGauge;
        // Max fill on input before turn battery off and maybe damage
        public float maxFill = 2f;
        // Receive damage if overfill?
        public int damageOverFill = 5;
        // Max drain on output before turn battery off and maybe damage
        public float maxDrain = 2f;
        // Receive damage if drain more than max
        public int damageOverDrain = 5;

        public float accumulatedEnergySec = 0f;

        private bool lastEnergized = false;
        private bool lastOn = false;

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
            string jsonData = JsonUtility.ToJson(new ElectricBatteryJsonData(accumulatedEnergySec));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricBatteryJsonData electricBatteryJsonData = JsonUtility.FromJson<ElectricBatteryJsonData>(jsonData);
            if (electricBatteryJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            accumulatedEnergySec = electricBatteryJsonData.accumulatedEnergySec;

            StartManagement();
        }

        #endregion

        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            // Activate object connected to the output
            ActivateOutput();
        }

        public override float IsDrainEnergy(int index)
        {
            // Battery generate energy, so we do not calculate all other component connected to the battery output in the drain
            return accumulatedEnergySec < maxEnergy ? drainEnergy : 0f;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // Battery do not need energize this way
        }

        #endregion

        private void OnDisable()
        {
            // Cancel battery energy management
            CancelInvoke();
        }

        private void StartEnergyManagement()
        {
            // Cancel energy management
            CancelInvoke();
            // Start battery energy management
            InvokeRepeating(nameof(ManageEnergy), 1f, 1f);
        }

        private void ManageEnergy()
        {
            // If we have energy we are energized
            GetSetIsEnergized = accumulatedEnergySec > 0f;
            // If we are energized we are on
            GetSetIsOn = IsEnergized();

            if (IsEnergized() != lastEnergized || IsOn() != lastOn)
            {
                lastEnergized = IsEnergized();
                lastOn = IsOn();

                ActivateOutput();
            }

            // If connected to something
            if (IsWireConnected(false, 0))
            {
                float theDrain = wireOutput[0].GetComponent<WireControl>().wireConnectorInput.IsDrainEnergy();
                // Decrease time
                accumulatedEnergySec -= theDrain;

                if (theDrain > maxDrain)
                {
                    // If we pass maxDrain range, turn battery off
                    GetSetIsOn = false;

                    ActivateOutput();

                    // TODO : Damage battery
                }

                if (accumulatedEnergySec < 0f)
                {
                    accumulatedEnergySec = 0f;
                    // If we drop under, turn battery off
                    GetSetIsOn = false;

                    ActivateOutput();
                }
            }

            // If something connected to
            if (IsWireConnected(true, 0))
            {
                // If connected is Energized?
                if (wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsEnergized() && wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsOn())
                {
                    float theDrain = wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsGenerateEnergy();

                    if (theDrain > maxFill)
                    {
                        // If we pass maxFill range, turn battery off
                        GetSetIsOn = false;

                        ActivateOutput();

                        // TODO : Damage battery
                    }
                    else
                    {
                        // Accumulate time
                        accumulatedEnergySec += theDrain;

                        if (accumulatedEnergySec > maxEnergy)
                            accumulatedEnergySec = maxEnergy;
                    }
                }
            }

            SetEnergyGauge();
        }

        private void SetEnergyGauge()
        {
            if (energyGauge != null)
            {
                float percent = (accumulatedEnergySec != 0f && maxEnergy != 0f) ? accumulatedEnergySec / maxEnergy : 0f;
                energyGauge.transform.localScale = new Vector3(percent, 1f, 1f);
            }
        }
    }
}
