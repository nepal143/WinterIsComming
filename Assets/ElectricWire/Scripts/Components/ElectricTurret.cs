
//(c8

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricTurretJsonData
    {
        public bool isOn;

        public ElectricTurretJsonData(bool newIsOn)
        {
            isOn = newIsOn;
        }
    }

    public class ElectricTurret : ElectricComponent, ISaveJsonData
    {
        public Transform turretBarrel;
        public Transform laserShoot;
        public bool isShowingLaser = false;
        public bool activated = false;
        public List<Transform> targets = new List<Transform>();

        public AudioSource turretShootAudioSource;

        private void OnEnable()
        {
            activated = false;
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricTurretJsonData(GetSetIsOn));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricTurretJsonData electricTurretJsonData = JsonUtility.FromJson<ElectricTurretJsonData>(jsonData);
            if (electricTurretJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            TurningOnOff(electricTurretJsonData.isOn);
        }

        #endregion

        private void TurningOnOff(bool newIsOn)
        {
            GetSetIsOn = newIsOn;
        }

        #region IWire interface

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            // We unenergize by ourself
            GetSetIsEnergized = false;
        }

        #endregion

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
                TurningOnOff(!IsOn());
        }

        #region Turret

        public IEnumerator TurretLoop()
        {
            activated = true;

            while (targets.Count > 0)
            {
                if (targets[0] != null)
                {
                    Vector3 targetPostition = new Vector3(targets[0].position.x, targets[0].GetComponent<TurretTarget>().targetPoint.position.y, targets[0].position.z);
                    turretBarrel.LookAt(targetPostition);
                    //turretRotor.localEulerAngles = new Vector3(0.0f, turretBarrel.localEulerAngles.y, 0.0f);
                    float distance = Vector3.Distance(turretBarrel.position, targetPostition);
                    StartCoroutine(LookAndShootAtTarget(distance * 2));

                    // Damage the target
                    TurretTarget target = targets[0].GetComponent<TurretTarget>();
                    if (target != null)
                        target.AttackByTurret();

                    yield return new WaitForSeconds(1f);
                }
                if (targets.Count > 0)
                    targets.RemoveAt(0);
            }

            activated = false;
        }

        private IEnumerator LookAndShootAtTarget(float distance)
        {
            if (!isShowingLaser)
            {
                isShowingLaser = true;
                laserShoot.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, distance));
                laserShoot.GetComponent<Renderer>().enabled = true;
            }

            turretShootAudioSource.Play();

            yield return new WaitForSeconds(0.05f);
            laserShoot.GetComponent<Renderer>().enabled = false;
            isShowingLaser = false;
            yield return null;
        }

        #endregion
    }
}
