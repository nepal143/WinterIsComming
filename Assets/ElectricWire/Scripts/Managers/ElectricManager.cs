
//(c8


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElectricWire
{
    [Serializable]
    public class PositionJsonData
    {
        public string objectName;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float rotationW;

        public PositionJsonData(string newObjectName, Vector3 newPosition, Quaternion newRotation)
        {
            objectName = newObjectName;
            positionX = newPosition.x;
            positionY = newPosition.y;
            positionZ = newPosition.z;
            rotationX = newRotation.x;
            rotationY = newRotation.y;
            rotationZ = newRotation.z;
            rotationW = newRotation.w;
        }
    }

    public class ElectricManager : MonoBehaviour
    {
        public static ElectricManager electricManager;
        public ElectricUndoRedo electricUndoRedo;

        public Text infoText;

        public GameObject targetInfoManager;
        public GameObject removeManager;
        public GameObject wireManager;
        public GameObject componentManager;

        private GameObject actualManager;
        [HideInInspector] public GameObject allConstructionParent;

        private void Awake()
        {
            if (electricManager != null && electricManager != this) Destroy(gameObject); else electricManager = this;

            allConstructionParent = new GameObject();
            allConstructionParent.name = "AllConstruction";
        }

        #region SaveLoadAllJsonData

        public void SaveAllJsonData()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            string dateTime = DateTime.Now.ToString("MM/dd/yyyy HH-mm-ss").Replace(" ", "_");
            string fileName = SceneManager.GetActiveScene().name + "_" + dateTime;
            UISaveMenu.uiSaveMenu.SaveMenu(fileName);
        }

        public void WriteAllJsonData(string fileName)
        {
            // Each object contain 1 line for prefabname, position, rotation
            // And a second line is for data

            fileName += ".ewdata";
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            StreamWriter writer = new StreamWriter(path, false);

            // Write components
            ElectricComponent[] electricComponents = allConstructionParent.GetComponentsInChildren<ElectricComponent>();
            for (int i = 0; i < electricComponents.Length; i++)
            {
                if (electricComponents[i].skipSave)
                    continue;

                writer.WriteLine(electricComponents[i].GetJsonPosition());
                var componentData = electricComponents[i].GetComponent<ISaveJsonData>();
                if (componentData != null)
                    writer.WriteLine(componentData.GetJsonData());
                else
                    writer.WriteLine("");
            }

            // Write wires
            WireControl[] wireControls = allConstructionParent.GetComponentsInChildren<WireControl>();
            for (int i = 0; i < wireControls.Length; i++)
            {
                if (wireControls[i].skipSave)
                    continue;

                writer.WriteLine(wireControls[i].GetJsonPosition());
                var wireData = wireControls[i].GetComponent<ISaveJsonData>();
                if (wireData != null)
                    writer.WriteLine(wireData.GetJsonData());
                else
                    writer.WriteLine("");
            }

            writer.Close();

            // If menu visible refresh
            if (UILoadMenu.uiLoadMenu.panel.activeSelf)
                LoadAllJsonData();
        }

        public void LoadAllJsonData()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            string path = Application.streamingAssetsPath;
            string[] fileNames = Directory.GetFiles(path, "*.ewdata");
            UILoadMenu.uiLoadMenu.LoadMenu(fileNames);
        }

        public void SpawnWithJsonData(string path)
        {
            ClearAllJsonData();

            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                PositionJsonData positionJsonData = JsonUtility.FromJson<PositionJsonData>(reader.ReadLine());
                GameObject newObject = Resources.Load("Components/" + positionJsonData.objectName) as GameObject;
                GameObject newGameObject = Instantiate(newObject);
                newGameObject.transform.position = new Vector3(positionJsonData.positionX, positionJsonData.positionY, positionJsonData.positionZ);
                newGameObject.transform.rotation = new Quaternion(positionJsonData.rotationX, positionJsonData.rotationY, positionJsonData.rotationZ, positionJsonData.rotationW);
                newGameObject.transform.parent = allConstructionParent.transform;
                var objectData = newGameObject.GetComponent<ISaveJsonData>();
                if (objectData != null)
                    objectData.SetupFromJsonData(reader.ReadLine());
                else
                    reader.ReadLine();
            }
            reader.Close();
        }

        public void ClearAllJsonData()
        {
            // Remove all components already in scene
            DestroyImmediate(allConstructionParent);
            allConstructionParent = null;
            allConstructionParent = new GameObject();
            allConstructionParent.name = "AllConstruction";

            if (electricUndoRedo != null)
                electricUndoRedo.ClearUndoRedo();
        }

        public void RemoveSaveFile(string fileName)
        {
            File.Delete(fileName);
#if UNITY_EDITOR
            File.Delete(fileName + ".meta");
#endif
        }

        #endregion

        public void OnClickTargetInfoButtonUI()
        {
            if (actualManager != null)
                Destroy(actualManager);

            actualManager = Instantiate(targetInfoManager);
            SetText("[INFO]");
        }

        public void OnClickRemoveButtonUI()
        {
            if (actualManager != null)
                Destroy(actualManager);

            actualManager = Instantiate(removeManager);
            SetText("[REMOVE OBJECT]");
        }

        public void OnClickWireButtonUI(GameObject objectPrefab)
        {
            if (actualManager != null)
                Destroy(actualManager);

            actualManager = Instantiate(wireManager);
            actualManager.GetComponent<ElectricWireManager>().Startup(objectPrefab);
            SetText("Build: " + objectPrefab.name);
        }

        public void OnClickComponentButtonUI(GameObject objectPrefab)
        {
            if (actualManager != null)
                Destroy(actualManager);

            actualManager = Instantiate(componentManager);
            actualManager.GetComponent<ElectricComponentManager>().Startup(objectPrefab);
            SetText("Build: " + objectPrefab.name);
        }

        public void StopPlacement()
        {
            SetText("");
            Destroy(actualManager);
        }

        public void SetText(string newText)
        {
            // Background
            if (infoText != null)
            {
                infoText.text = newText;
                // Foreground
                if (infoText.transform.childCount > 0 && infoText.transform.GetChild(0) != null)
                    infoText.transform.GetChild(0).GetComponent<Text>().text = newText;
            }
        }

        public bool CanTriggerComponent()
        {
            return !EventSystem.current.IsPointerOverGameObject() ? actualManager == null : false;
        }
    }
}
