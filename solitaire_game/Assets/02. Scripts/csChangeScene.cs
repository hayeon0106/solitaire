using UnityEngine;
using System.Collections;

public class csChangeScene : MonoBehaviour {

    public void SceneTrans_title()
    {
        Application.LoadLevel("01-Title");
    }

	public void SceneTrans_GameScene()
    {
        Application.LoadLevel("02-Game");
    }

    public void SceneTrans_End()
    {
        Application.LoadLevel("03-FinishGame");
    }

    public void GameEixt()
    {
        Application.Quit();
    }
}
