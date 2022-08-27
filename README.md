# Animator-Parameter-Extractor
A Unity Editor Tool that create a C# script with in input a list of Animator Controller
![MainImage](/Readme%20Images/Animator%20Parameter%20Extractor.png)
___

### Objective

Create a unique class that contain all the animators parameters.

___

## How to Use

Use the tool is quite simple, as long as you have the Editor Script _'AnimatorParametersEditor.cs'_.

### Open

Let's start with shortcut, above on the Navigation Board e follow the path **'Tool/Animator Parameter Creation'**

![Shortcut](/Readme%20Images/Shortcut.png)
___

### Editor Window

When the Editor Window is open it automatically search all Animator Controllers inside the Assets Folder and SubFolders.
After the search create a list of Animation Controller so you can immediately start the Reflection.

![Editor](/Readme%20Images/Editor.png)

> ⚠️ _<span style="color:yellow">Warning!</span> If you modify the list there are 3 condition you have tu fulfill before you click!_
> 1. _You cannot have **Null Reference** in the list._
> 2. _You cannot have **Duplicate** instance in the list._
> 3. _The list cannot be **Empty**._

___

### Save File and Start Reflection

You now click the Button **'Create File'** and it will open Output type Dialog, you can choose whether your output type is enum or string, then open a Save Dialog Panel, when you save the file in the directory you choose it begin the reflection.
<br> It will appear a Progress Bar that indicate the state of the file, at the end of procedure it show a Message Box Content the relative path of the file.

### File

This is the Generate File at the end of the Reflection.
- [Enum type](/Assets/Animator%20Parameter%20Extractor/Example/AnimatorParameter.cs)
``` C#
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
```

- [String type](/Assets/Animator%20Parameter%20Extractor/Example/AnimatorParameterString.cs)
``` C#
public static class AnimatorParameter
{
    public static class Enemy
    {
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Death = "Death";
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Attack = "Attack";
        /// <summary>
        /// Parameter Type: Bool
        /// </summary>
        public const string Scream = "Scream";
    }

    public static class Evil_Wizard
    {
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Death = "Death";
        /// <summary>
        /// Parameter Type: Int
        /// </summary>
        public const string Life = "Life";
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Attack = "Attack";
        /// <summary>
        /// Parameter Type: Int
        /// </summary>
        public const string Speed = "Speed";
        /// <summary>
        /// Parameter Type: Bool
        /// </summary>
        public const string IsCasting = "IsCasting";
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Evil_Laugh = "Evil_Laugh";
    }

    public static class Hero
    {
        /// <summary>
        /// Parameter Type: Float
        /// </summary>
        public const string Life = "Life";
        /// <summary>
        /// Parameter Type: Float
        /// </summary>
        public const string Speed = "Speed";
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Attack = "Attack";
        /// <summary>
        /// Parameter Type: Bool
        /// </summary>
        public const string IsJumping = "IsJumping";
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Wave = "Wave";
    }

    public static class NPC
    {
        /// <summary>
        /// Parameter Type: Bool
        /// </summary>
        public const string IsTalking = "IsTalking";
        /// <summary>
        /// Parameter Type: Trigger
        /// </summary>
        public const string Surprised = "Surprised";
    }
}

```
___

### Usage Example
- Enum type
![Example](/Readme%20Images/Example%20of%20using%20enum.png)

- String type
![Example](/Readme%20Images/Example%20of%20using%20string.png)
