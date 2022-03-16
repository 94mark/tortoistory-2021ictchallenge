using UnityEngine;
using UnityEditor;

public class ObjectsCreator : ScriptableWizard
{
    public GameObject Prefab;
    public Transform OriginT;

    public int PrefabsToInstantiate = 10;
    public float Radius = 1.0f;
    public float MaxHeight = 1.0f;

    public Vector2 RangeScale = Vector2.one;
    
    [MenuItem("Tools/Objects Creator")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ObjectsCreator>("Create GO");
    }

    private void OnWizardCreate()
    {
        if (!Prefab || !OriginT)
            return;

        for (int i = 0; i < PrefabsToInstantiate; i++)
        {
            Vector3 RandomLocation = Random.insideUnitSphere * Radius;
            RandomLocation.y *= MaxHeight;

            Vector3 NewLocation = OriginT.position + RandomLocation;
            GameObject GO = PrefabUtility.InstantiatePrefab(Prefab) as GameObject;
            GO.transform.position = NewLocation;
            GO.transform.localScale = Vector3.one * Random.Range(RangeScale.x, RangeScale.y);
        }
    }

    private void OnWizardOtherButton()
    {

    }
}
