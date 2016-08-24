using UnityEngine;
using System.Collections;

public class SimpleSQL_UI : MonoBehaviour {

    public Vector2 WidthAndHeight;
    string result = "none";

    void RunQuery()
    {
        result = GetComponent<SQL_Controller>().SimpleQuery("select * from Artykuly2");
    }


    void OnGUI()
    {
        Rect content = new Rect((Screen.width - this.WidthAndHeight.x) / 2, (Screen.height - this.WidthAndHeight.y) / 2, this.WidthAndHeight.x, this.WidthAndHeight.y);
        GUI.Box(content, "SQL Kontrola");
        GUILayout.BeginArea(content);
        GUILayout.Space(30);

        if (GUILayout.Button("RunQuery"))
            RunQuery();
        GUILayout.Label(result);
        GUILayout.EndArea();
    }
}
