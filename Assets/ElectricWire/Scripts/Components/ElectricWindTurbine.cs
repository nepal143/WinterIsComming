
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricWindTurbineJsonData
    {
        public float delay;

        public ElectricWindTurbineJsonData(float newDelay)
        {
            delay = newDelay;
        }
    }

    public class ElectricWindTurbine : ElectricComponent, ISaveJsonData
    {
        // Animations
        public Animator animator;
        public bool workWithoutWind = false;
        public GameObject energyGauge;
        public string windName = "WindZone";
        public float alignAngle = 0.65f;
        public float minWindForce = 0.2f;
        public float maxWindForce = 2f;

        // Receive damage if drain more than max
        public int damageOverDrain = 5;
        // Max overload attempt before declare overload
        public int maxOverloadChance = 1;
        // Time before return on after overload, 0 = manual return
        public float breakerTime = 0f;

        private int actualRetryChance = 0;

        private bool lastEnergized = false;
        private bool lastOn = false;

        private float totalPower = 0f;
        private WindZone windGameObject;

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
            string jsonData = JsonUtility.ToJson(new ElectricWindTurbineJsonData(0));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricWindTurbineJsonData electricWindTurbineJsonData = JsonUtility.FromJson<ElectricWindTurbineJsonData>(jsonData);
            if (electricWindTurbineJsonData == null)
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
            // Wind Turbine generate energy, so we do not calculate all other component connected to the wind turbine output in the drain
            return IsOn() ? drainEnergy : 0f;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // Wind Turbine do not need energize this way
        }

        #endregion

        private void OnDisable()
        {
            lastEnergized = false;
            lastOn = false;

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
            // Find wind
            if (!workWithoutWind && windGameObject == null)
            {
                GameObject newWind = GameObject.Find(windName);
                if (newWind != null)
                {
                    windGameObject = newWind.GetComponent<WindZone>();
                }
                else
                {
                    workWithoutWind = true;
                    Debug.LogWarning("No wind found in scene.. wind turbine will always work.");
                }
            }

            float windStrength = 1f;
            float dot = 90f;

            if (!workWithoutWind)
            {
                windStrength = windGameObject.windMain;
                dot = Vector3.Dot(transform.forward, windGameObject.transform.forward);
            }

            totalPower = (windStrength >= minWindForce && windStrength <= maxWindForce) ? windStrength / ((maxWindForce - minWindForce) / 2) : 0f;
            if (totalPower > 1f)
                totalPower = 1f;
            SetEnergyGauge();

            // Make energized if wind is aligned with turbine and have enough strength.. not too much
            if (dot > alignAngle || dot < -alignAngle)
                GetSetIsEnergized = totalPower > 0f;
            else
                GetSetIsEnergized = false;

            // If we are energized, we are on
            GetSetIsOn = IsEnergized();

            // Set animator speed
            animator.speed = totalPower;

            if (GetSetIsEnergized != lastEnergized || GetSetIsOn != lastOn)
            {
                lastEnergized = GetSetIsEnergized;
                lastOn = GetSetIsOn;

                animator.SetBool("IsOn", GetSetIsOn);

                ActivateOutput();
            }

            // If connected to something
            if (IsWireConnected(false, 0))
            {
                float theDrain = wireOutput[0].GetComponent<WireControl>().wireConnectorInput.IsDrainEnergy();

                if (theDrain > IsGenerateEnergy(-1) * totalPower)
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
