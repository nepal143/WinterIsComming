
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricSolarPanelJsonData
    {
        public float delay;

        public ElectricSolarPanelJsonData(float newDelay)
        {
            delay = newDelay;
        }
    }

    public class ElectricSolarPanel : ElectricComponent, ISaveJsonData
    {
        // Receive damage if drain more than max
        public int damageOverDrain = 5;

        public GameObject energyGauge;
        public bool directionalSun = true;
        public string sunName = "Sun/Directional Light";
        public float alignAngle = 0.65f;
        public Transform solarPanel;
        public GameObject[] rayTest;

        // Max overload attempt before declare overload
        public int maxOverloadChance = 1;
        // Time before return on after overload, 0 = manual return
        public float breakerTime = 0f;

        private int actualRetryChance = 0;

        private bool lastEnergized = false;
        private bool lastOn = false;

        private float totalPower = 0f;
        private GameObject sunGameObject;

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
            string jsonData = JsonUtility.ToJson(new ElectricSolarPanelJsonData(0));
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
            // Find sun
            if (sunGameObject == null)
                sunGameObject = GameObject.Find(sunName);

            Vector3 direction = -sunGameObject.transform.forward;

            // For each ray error, diminish solar panel effiency
            int totalGoodRay = 0;
            foreach (var item in rayTest)
            {
                if (!directionalSun)
                    direction = sunGameObject.transform.position - item.transform.position;

                // Make a ray from each position to the sun to find obstruction
                if (!Physics.Raycast(item.transform.position, direction, out RaycastHit hit, Mathf.Infinity))
                {
                    totalGoodRay++;
                    Debug.DrawRay(item.transform.position, direction, Color.green, 1f);
                }
                else
                {
                    Debug.DrawRay(item.transform.position, direction, Color.red, 1f);
                }
            }

            direction = -sunGameObject.transform.forward;
            if (!directionalSun)
                direction = sunGameObject.transform.position - solarPanel.position;

            // Without float cast, it return integer
            totalPower = (totalGoodRay != 0f && rayTest.Length != 0f) ? (float)totalGoodRay / (float)rayTest.Length : 0f;
            SetEnergyGauge();

            // Angle 0 = max power... 90+ = no power
            float angle = Vector3.Angle(direction, solarPanel.forward);

            // Make energized if sun is aligned with panel and no obstruction
            if (angle < alignAngle)
                GetSetIsEnergized = totalPower > 0f;
            else
                GetSetIsEnergized = false;

            // If we are energized we are on
            GetSetIsOn = IsEnergized();

            if (GetSetIsEnergized != lastEnergized || GetSetIsOn != lastOn)
            {
                lastEnergized = GetSetIsEnergized;
                lastOn = GetSetIsOn;

                ActivateOutput();
            }

            // If connected to something
            if (GetSetIsOn && IsWireConnected(false, 0))
            {
                float theDrain = wireOutput[0].GetComponent<WireControl>().wireConnectorInput.IsDrainEnergy();

                if (theDrain > IsGenerateEnergy(-1) * totalPower)
                {
                    if (actualRetryChance >= maxOverloadChance)
                    {
                        actualRetryChance = 0;

                        // If we pass IsGenerateEnergy range, turn solarpanel off
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

        private void SetEnergyGauge()
        {
            if (energyGauge != null)
                energyGauge.transform.localScale = new Vector3(totalPower, 1f, 1f);
        }

        private void OnMouseOver()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
            {
                // Rotate object 5 degres
                if (Input.GetMouseButtonDown(0))
                    transform.Rotate(0f, 5f, 0f);

                if (Input.GetMouseButtonDown(1))
                    transform.Rotate(0f, -5f, 0f);
            }
        }
    }
}
