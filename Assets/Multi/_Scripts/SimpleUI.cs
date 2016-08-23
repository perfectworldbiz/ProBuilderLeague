using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.UI;

public class SimpleUI : PunBehaviour {

    bool escMenuOn = false;
    public Vector2 WidthAndHeight;
    Image crosshair;
    Text hitText;
    Text shotText;
    [HideInInspector]
    public PlayerStats localStats;

    void Awake()
    {
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        hitText = GameObject.Find("HitText").GetComponent<Text>();
        shotText = GameObject.Find("ShotText").GetComponent<Text>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escMenuOn = !escMenuOn;
            
            if (escMenuOn)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                CloseESC();
            }
            crosshair.enabled = !escMenuOn;
        }
        if (escMenuOn)
        {
            if (Input.GetKeyDown(KeyCode.Y))
                ExitGame();
            if (Input.GetKeyDown(KeyCode.N))
                CloseESC();
        }
        if (localStats != null)
        {
            hitText.text = "Scored hits: " + localStats.scoredHits;
            shotText.text = "Times shot: " + localStats.timesShot;
        }
    }
    void CloseESC()
    {
        escMenuOn = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void ExitGame()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
    void OnGUI()
    {
        if (escMenuOn)
        {
            Rect content = new Rect((Screen.width - this.WidthAndHeight.x) / 2, (Screen.height - this.WidthAndHeight.y) / 2, this.WidthAndHeight.x, this.WidthAndHeight.y);
            GUI.Box(content, "Exit game?");
            GUILayout.BeginArea(content);

            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("(y)es"))
                ExitGame();
            if (GUILayout.Button("(n)o"))
                CloseESC();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
