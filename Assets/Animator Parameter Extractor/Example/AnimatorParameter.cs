using UnityEngine;

public static class AnimatorParameter
{ 
	public enum Enemy  
	{ 
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Death,
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Attack,
		/// <summary>
		/// Parameter Type: Bool
		/// </summary>
		Scream,
	}
}

public class EnemyController : MonoBehaviour
{
	public Animator _animator;

	public void Death()
    {
		_animator.SetTrigger(AnimatorParameterString.Enemy.Death);
    }
}