public static class AnimatorParameter { 
	public static class Enemy { 
		public enum Bool  
		{ 
			Scream,
		}
		public enum Trigger  
		{ 
			Death,
			Attack,
		}
	}

	public static class Evil_Wizard { 
		public enum Int  
		{ 
			Life,
			Speed,
		}
		public enum Bool  
		{ 
			IsCasting,
		}
		public enum Trigger  
		{ 
			Death,
			Attack,
			Evil_Laugh,
		}
	}

	public static class Hero { 
		public enum Float  
		{ 
			Life,
			Speed,
		}
		public enum Bool  
		{ 
			IsJumping,
		}
		public enum Trigger  
		{ 
			Attack,
			Wave,
		}
	}

	public static class NPC { 
		public enum Bool  
		{ 
			IsTalking,
		}
		public enum Trigger  
		{ 
			Surprised,
		}
	}
}


public static class AnimatorParameterString
{
	public static class NPC
	{
		public const string IsTalking = "IsTalking";
	}
}