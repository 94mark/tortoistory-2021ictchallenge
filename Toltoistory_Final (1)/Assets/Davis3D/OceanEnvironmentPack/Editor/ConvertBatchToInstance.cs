using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConvertBatchToInstance : ScriptableWizard
{

    public GameObject ParentGO;
    public GameObject GOToPlace;

    [MenuItem("Tools/Convert Batch To Instances")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ConvertBatchToInstance>("Convert GO", "Convert");
    }

    private void OnWizardCreate()
    {
        if (!ParentGO || !GOToPlace)
            return;

        GameObject NewParent;
        NewParent = new GameObject(ParentGO.name + "_GO");
        NewParent.transform.localPosition = Vector3.zero;
        NewParent.transform.localRotation = Quaternion.identity;
        NewParent.transform.localScale = Vector3.one;

        List<GameObject> ParentsToDestroy = new List<GameObject>();
        int ChildCount = ParentGO.transform.childCount;

        for (int i = 0; i < ChildCount; i++)
        {
            Transform CurrentT = ParentGO.transform.GetChild(i);
            ParentsToDestroy.Add(CurrentT.gameObject);
            string ParentName = CurrentT.name;

            GameObject GO = PrefabUtility.InstantiatePrefab(GOToPlace) as GameObject;
            GO.transform.parent = NewParent.transform;
            GO.name = ParentName;
            GO.transform.localScale = CurrentT.localScale;
            GO.transform.localRotation = CurrentT.localRotation;
            GO.transform.localPosition = CurrentT.localPosition;
        }
    }

    private void OnWizardOtherButton()
    {

    }
}
