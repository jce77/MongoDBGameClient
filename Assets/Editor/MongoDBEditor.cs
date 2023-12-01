using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TopDownRPGDemo
{
    [CustomEditor(typeof(MongoDB))]
    public class MongoDBEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Test Add High Score"))
            {
                TestAddHighScore((MongoDB)target);
            }
            if (GUILayout.Button("Test Add Random High Score"))
            {
                TestAddRandomHighScore((MongoDB)target);
            }
            if (GUILayout.Button("Test Print High Scores"))
            {
                TestPrintHighScores((MongoDB)target);
            }

        }

        public void TestAddHighScore(MongoDB target)
        {
            target.StartCoroutine(target.AddHighscore(target.addScoreTestName, target.addScoreTestScore));
        }

        public void TestAddRandomHighScore(MongoDB target)
        {
            target.StartCoroutine(target.AddHighscore("Player" + Random.Range(1, 100), Random.Range(1000, 15000)));
        }

        public void TestPrintHighScores(MongoDB target)
        {
            target.StartCoroutine(target.ShowHighscores());
        }


	}

}
