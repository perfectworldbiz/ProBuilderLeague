using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

public class MyNetworkManager : NetworkManager {

	public static MyNetworkManager mgr;

	void Awake()
	{
		if (mgr == null)
			mgr = this;
		else if (mgr != this)
			Destroy (gameObject);
	}

	public override void OnMatchCreate (bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo matchInfo)
	{
		base.OnMatchCreate (success, extendedInfo, matchInfo);
		Debug.Log ("Match created");
	}
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);

        if (!success)
            Debug.Log("MatchList failed");
        else
        {
            Debug.Log("MatchList passed");
            NetworkController.cont.listMatches = true;
        }
    }
    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
    }
}
