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
	public enum Evil_Wizard  
	{ 
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Death,
		/// <summary>
		/// Parameter Type: Int
		/// </summary>
		Life,
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Attack,
		/// <summary>
		/// Parameter Type: Int
		/// </summary>
		Speed,
		/// <summary>
		/// Parameter Type: Bool
		/// </summary>
		IsCasting,
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Evil_Laugh,
	}
	public enum Hero  
	{ 
		/// <summary>
		/// Parameter Type: Float
		/// </summary>
		Life,
		/// <summary>
		/// Parameter Type: Float
		/// </summary>
		Speed,
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Attack,
		/// <summary>
		/// Parameter Type: Bool
		/// </summary>
		IsJumping,
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Wave,
	}
	public enum NPC  
	{ 
		/// <summary>
		/// Parameter Type: Bool
		/// </summary>
		IsTalking,
		/// <summary>
		/// Parameter Type: Trigger
		/// </summary>
		Surprised,
	}
}
