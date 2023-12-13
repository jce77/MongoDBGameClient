using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MongoDB))]
public class MongoDBEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//if (GUILayout.Button("Does Nothing"))
		//{
		//	TestAddHighScore((MongoDB)target);
		//}
	}

	//public void TestAddHighScore(MongoDB target)
	//{
	//	target.StartCoroutine(target.AddHighscore(target.addScoreTestName, target.addScoreTestScore));
	//}



}
