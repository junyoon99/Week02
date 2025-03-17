using TMPro;
using UnityEngine;

public class Warning : MonoBehaviour
{
	EnemyBase parentEnemy;
	EnemyBase.State currentState;
	TextMeshPro warningText;
	void Start()
	{
		transform.parent.TryGetComponent<EnemyBase>(out parentEnemy);
		TryGetComponent<TextMeshPro>(out warningText);
	}

	void Update()
	{
		if (currentState != parentEnemy.currentState) 
		{
			currentState = parentEnemy.currentState;
			switch (currentState)
			{
				case EnemyBase.State.idle:
					warningText.text = "...";
					break;
				case EnemyBase.State.foundPlayer:
					warningText.text = "!";
					warningText.color = Color.white;
					break;
				case EnemyBase.State.stuned:
					warningText.text = "";
					warningText.color = Color.white;
					break;
				case EnemyBase.State.attack:
					warningText.text = "<b>!!<b>";
					warningText.color = Color.red;
					break;
			}
		}
	}
}
