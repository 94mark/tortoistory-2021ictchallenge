using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenameGO : ScriptableWizard
{

    public bool bRemoveSuffix;
    public int SuffixToRemove;

    public bool bRemoveIndex;
    public int IndexToRemove;

    public GameObject[] GOToRename;

    [MenuItem("Tools/Rename Game Objects")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<RenameGO>("Rename GO", "Rename");
    }

    private void OnWizardCreate()
    {
        if (GOToRename == null || GOToRename.Length == 0)
            return;

        for (int i = 0; i < GOToRename.Length; i++)
        {
            if (bRemoveIndex)
            {
                GOToRename[i].name = GOToRename[i].name.Remove(IndexToRemove);
            }
        }
    }

    private void OnWizardOtherButton()
    {

    }
}
