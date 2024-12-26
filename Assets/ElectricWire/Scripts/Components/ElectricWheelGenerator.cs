
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricWheelGeneratorJsonData
    {
        public float delay;

        public ElectricWheelGeneratorJsonData(float newDelay)
        {
            delay = newDelay;
        }
    }

    public class ElectricWheelGenerator : ElectricComponent, ISaveJsonData
    {
        public Animator animator;

        // Receive damage if drain more than max
        public int damageOverDrain = 5;

        // Max overload attempt before declare overload
        public int maxOverloadChance = 1;
        // Time before return on after overload, 0 = manual return
        public float breakerTime = 0f;

        private int actualRetryChance = 0;

        private bool lastEnergized = false;
        private bool lastOn = false;

        private bool isRotating = false;

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
            string jsonData = JsonUtility.ToJson(new ElectricWheelGeneratorJsonData(0));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricSolarPanelJsonData electricSolarPanelJsonData = JsonUtility.FromJson<ElectricSolarPanelJsonData>(jsonData);
            if (electricSolarPanelJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

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
            // SolarPanel generate energy, so we do not calculate all other component connected to the solarpanel output in the drain
            return IsOn() ? drainEnergy : 0f;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // SolarPanel do not need energize this way
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
            // If we rotating we are energized
            GetSetIsEnergized = isRotating;
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

        public void RotateRotator(bool isForward, bool goIdle = false)
        {
            if (!goIdle)
            {
                animator.SetInteger("forwardreverse", isForward ? 1 : 2);
                isRotating = true;
            }
            else
            {
                animator.SetInteger("forwardreverse", 0);
                isRotating = false;
            }
        }
    }
}
