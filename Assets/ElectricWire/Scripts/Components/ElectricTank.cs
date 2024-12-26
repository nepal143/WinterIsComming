
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricTankJsonData
    {
        public float accumulatedEnergySec;

        public ElectricTankJsonData(float newAccumulatedEnergySec)
        {
            accumulatedEnergySec = newAccumulatedEnergySec;
        }
    }

    public class ElectricTank : ElectricComponent, ISaveJsonData
    {
        public float maxEnergy = 36f;
        public GameObject energyGauge;
        // Max fill on input before turn tank off and maybe damage
        public float maxFill = 2f;
        // Receive damage if overfill?
        public int damageOverFill = 0;
        // Max drain on output before turn tank off and maybe damage
        public float maxDrain = 2f;
        // Receive damage if drain more than max
        public int damageOverDrain = 0;

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
            // Start tank (energy) management
            StartTankManagement();
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricTankJsonData(accumulatedEnergySec));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricTankJsonData electricTankJsonData = JsonUtility.FromJson<ElectricTankJsonData>(jsonData);
            if (electricTankJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            accumulatedEnergySec = electricTankJsonData.accumulatedEnergySec;

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
            // Tank generate energy, so we do not calculate all other component connected to the tank output in the drain
            return accumulatedEnergySec < maxEnergy ? drainEnergy : 0f;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // Tank do not need energize this way
        }

        #endregion

        private void OnDisable()
        {
            // Cancel tank energy management
            CancelInvoke();
        }

        private void StartTankManagement()
        {
            // Cancel tank energy management
            CancelInvoke();
            // Start battetankry energy management
            InvokeRepeating(nameof(ManageEnergy), 1f, 1f);
        }

        private void ManageEnergy()
        {
            // If something connected to
            if (IsWireConnected(true, 0))
            {
                // If connected is Energized?
                if (wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsEnergized() && wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsOn())
                {
                    float theDrain = wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsGenerateEnergy();

                    if (theDrain > maxFill)
                    {
                        // If we pass maxFill range, turn tank off
                        GetSetIsOn = false;

                        ActivateOutput();

                        // TODO : Damage tank
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

            // If we have energy we are energized
            GetSetIsEnergized = accumulatedEnergySec > 0f;
            // If we are energized we are on
            GetSetIsOn = IsEnergized();

            if (GetSetIsEnergized != lastEnergized || GetSetIsOn != lastOn)
            {
                lastEnergized = GetSetIsEnergized;
                lastOn = GetSetIsOn;

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
                    // If we pass maxDrain range, turn tank off
                    GetSetIsOn = false;

                    ActivateOutput();

                    // TODO : Damage tank
                }

                if (accumulatedEnergySec < 0f)
                {
                    accumulatedEnergySec = 0f;
                    // If we drop under, turn tank off
                    GetSetIsOn = false;

                    ActivateOutput();
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
