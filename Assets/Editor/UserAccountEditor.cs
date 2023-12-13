using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


[CustomEditor(typeof(UserAccount))]
public class UserAccountEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//if (GUILayout.Button("Test Register User"))
		//{
		//	TestRegisterUser((UserAccount)target);
		//}
	}

	//public void TestRegisterUser(UserAccount target)
	//{
	//	
	//}
}

