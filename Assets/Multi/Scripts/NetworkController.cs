using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class NetworkController : MonoBehaviour {

    public static NetworkController cont;
    NetworkManager mgr;
	NetworkMatch networkMatch;
    public bool listMatches = false;

	void Start()
	{
        cont = this;
        mgr = MyNetworkManager.mgr;
		StartMatchmaking ();
	}
	void StartMatchmaking ()
	{
		mgr.StartMatchMaker ();	
		networkMatch = gameObject.AddComponent<NetworkMatch> ();
	}


    void OnGUI()
	{
        int spacing = 24;
        int ypos = 40;
        if (!listMatches)
        {
            if (GUILayout.Button("Create match"))
            {
                networkMatch.CreateMatch("new room", 4, true, "", "", "", 0, 1, mgr.OnMatchCreate);
            }
            if (GUILayout.Button("List matches"))
            {
                networkMatch.ListMatches(0, 10, "", false, 0, 1, mgr.OnMatchList);
            }
        }
        else
        {
            Debug.Log(mgr.matches.Count + " is the number of matches");
            foreach (var match in mgr.matches)
            {
                if (GUILayout.Button("Join Match:" + match.name))
                {
                    mgr.matchName = match.name;
                    mgr.matchSize = (uint)match.currentSize;
                    mgr.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 1, mgr.OnMatchJoined);
                }
                Debug.Log(match.name);
                ypos += spacing;
            }
        }
    }
}