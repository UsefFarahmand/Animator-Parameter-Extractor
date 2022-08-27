#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorParametersEditor : EditorWindow
{
	public List<AnimatorController> animatorControllers;
	private SerializedObject so;
	private const string Title = "Create Reflection File";

	private void OnEnable()
	{
		FindAllAnimatorControllers();
		so = new SerializedObject(this);
	}

	private void FindAllAnimatorControllers()
	{
		string[] paths = Directory.GetFiles("Assets", "*.controller", SearchOption.AllDirectories);
		object[] data = Array.ConvertAll<string, object>(paths, input => AssetDatabase.LoadAssetAtPath(input, typeof(AnimatorController)));
		animatorControllers = new List<AnimatorController>(Array.ConvertAll(data, input => input as AnimatorController));
	}

	private void OnDisable()
	{
		EditorUtility.ClearProgressBar();
		so.Dispose();
	}

	private void OnGUI()
	{
		so.Update();

		SerializedProperty property = so.FindProperty("animatorControllers");
		EditorGUILayout.PropertyField(property, new GUIContent("Animator Controller", EditorGUIUtility.IconContent("d_NetworkAnimator Icon").image));

		so.ApplyModifiedProperties();

		if (GUILayout.Button(new GUIContent("Find All", "Find all animator controllers!"), GUI.skin.button))
		{
			FindAllAnimatorControllers();
		}
		EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);

		bool disable =
			animatorControllers.Count == 0 ||
			animatorControllers.Exists(x => x == null) ||
			animatorControllers.Count != animatorControllers.Distinct().Count();

		if (disable) HelpBox();
		EditorGUI.BeginDisabledGroup(disable);
		if (GUILayout.Button("Create File", GUI.skin.button))
		{
			EditorUtility.ClearProgressBar();

			int option = EditorUtility.DisplayDialogComplex("Output type",
			"Specify the output type:",
			"Enum",
			"String",
			"Cancel");

			switch (option)
			{
				// Enum.
				case 0:
					EnumReflection(
						EditorUtility.SaveFilePanel(
							"Save file",
							"Assets",
							"AnimatorParameter",
							"cs"
							)
						);
					break;

				// String.
				case 1:
					StringReflection(
						EditorUtility.SaveFilePanel(
							"Save file",
							"Assets",
							"AnimatorParameter",
							"cs"
							)
						);
					break;

				// Cancel.
				case 2:
					break;

				default:
					Debug.LogError("Unrecognized option.");
					break;
			}


		}
		EditorGUI.EndDisabledGroup();
	}

	private void HelpBox()
	{
		if (animatorControllers.Count == 0)
		{
			EditorGUILayout.HelpBox("The List is Empty!", MessageType.Error, true);
			return;
		}

		if (animatorControllers.Exists(x => x == null))
		{
			EditorGUILayout.HelpBox("There is a null reference inside the List!", MessageType.Warning, true);
		}

		if (animatorControllers.Count != animatorControllers.Distinct().Count())
		{
			EditorGUILayout.HelpBox("There is duplicate reference inside the List!", MessageType.Warning, true);
		}
	}

	private async void StringReflection(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			await using (FileStream stream = File.Create(path, 4096, FileOptions.Asynchronous))
			{
				var bytes = GetTextToByte(SetClassDeclaration(fileName));
				EditorUtility.DisplayProgressBar(Title, "Initialize...", 1f);
				await stream.WriteAsync(bytes, 0, bytes.Length);

				for (int i = 0; i < animatorControllers.Count; i++)
				{
					string info = $"Create Parameters : {animatorControllers[i].name} ";

					bytes = GetTextToByte(SetSubClassDeclaration(animatorControllers[i].name));
					EditorUtility.DisplayProgressBar(Title, info, 0f);
					await stream.WriteAsync(bytes, 0, bytes.Length);
					Thread.Sleep(250);

					AnimatorControllerParameter[] parameters = animatorControllers[i].parameters;

					for (int j = 0; j < parameters.Length; j++)
					{
						//summery
						bytes = GetTextToByte(SetValueSummary(parameters[j].type.ToString()));
						await stream.WriteAsync(bytes, 0, bytes.Length);
						Thread.Sleep(200);
						//variable
						bytes = GetTextToByte(SetParamDeclaration(parameters[j].name));
						EditorUtility.DisplayProgressBar(Title, info, (float)j / parameters.Length);
						await stream.WriteAsync(bytes, 0, bytes.Length);
						Thread.Sleep(200);
					}

					bytes = GetTextToByte(CloseSubClassBrace());
					EditorUtility.DisplayProgressBar(Title, info, 1);
					await stream.WriteAsync(bytes, 0, bytes.Length);
					Thread.Sleep(250);
				}

				bytes = GetTextToByte(CloseClassBrace());
				EditorUtility.DisplayProgressBar(Title, "Close File...", 1);
				await stream.WriteAsync(bytes, 0, bytes.Length);
				stream.Close();
				await stream.DisposeAsync();
			}

			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog(Title, $"Create Animator Reflection \nPath : {Path.GetRelativePath("Assets", path)}", "Okay");
			AssetDatabase.Refresh();
		}
	}

	private async void EnumReflection(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			await using (FileStream stream = File.Create(path, 4096, FileOptions.Asynchronous))
			{
				var bytes = GetTextToByte(SetClassDeclaration(fileName));
				EditorUtility.DisplayProgressBar(Title, "Initialize...", 1f);
				await stream.WriteAsync(bytes, 0, bytes.Length);

				Dictionary<AnimatorControllerParameterType, List<string>> parametersName = new Dictionary<AnimatorControllerParameterType, List<string>>();
				foreach (var val in Enum.GetValues(typeof(AnimatorControllerParameterType)).Cast<AnimatorControllerParameterType>())
				{
					parametersName.Add(val, new List<string>());
				}

				for (int i = 0; i < animatorControllers.Count; i++)
				{
					foreach (var val in parametersName)
					{
						val.Value.Clear();
					}

					string info = $"Create Parameters : {animatorControllers[i].name} ";

					bytes = GetTextToByte(SetEnum(animatorControllers[i].name));
					EditorUtility.DisplayProgressBar(Title, info, 0f);
					await stream.WriteAsync(bytes, 0, bytes.Length);
					Thread.Sleep(250);

					AnimatorControllerParameter[] parameters = animatorControllers[i].parameters;
					
					for (int j = 0; j < parameters.Length; j++)
					{
						var key = parameters[j].name;
						if (parametersName[parameters[j].type].Contains(key)) continue;
						parametersName[parameters[j].type].Add(key);

						//summery
						bytes = GetTextToByte(SetValueSummary(parameters[j].type.ToString()));
						await stream.WriteAsync(bytes, 0, bytes.Length);
						Thread.Sleep(200);
						//value
						bytes = GetTextToByte(SetEnumValue(parameters[j].name));
						await stream.WriteAsync(bytes, 0, bytes.Length);
						Thread.Sleep(200);
						//end enum
					}
					bytes = GetTextToByte(CloseEnumBrace());
					EditorUtility.DisplayProgressBar(Title, info, 1);
					await stream.WriteAsync(bytes, 0, bytes.Length);
					Thread.Sleep(200);
					//foreach (var type in parametersName)
					//{
					//	if (type.Value.Count == 0) continue;
					//	var typeInfo = $"{info} ({type.Key})";
					//	bytes = GetTextToByte(SetEnum(type.Key.ToString()));
					//	await stream.WriteAsync(bytes, 0, bytes.Length);
					//	Thread.Sleep(200);
					//	for (int k = 0; k < type.Value.Count; k++)
					//	{
					//		string para = type.Value[k];
					//		bytes = GetTextToByte(SetEnumValue(para));
					//		EditorUtility.DisplayProgressBar(Title, typeInfo, (float)k / type.Value.Count);
					//		await stream.WriteAsync(bytes, 0, bytes.Length);
					//		Thread.Sleep(200);
					//	}
					//	bytes = GetTextToByte(CloseEnumBrace());
					//	EditorUtility.DisplayProgressBar(Title, info, 1);
					//	await stream.WriteAsync(bytes, 0, bytes.Length);
					//	Thread.Sleep(200);
					//}

					//bytes = GetTextToByte(CloseSubClassBrace());
					//EditorUtility.DisplayProgressBar(Title, info, 1);
					//await stream.WriteAsync(bytes, 0, bytes.Length);
					//Thread.Sleep(250);
				}

				bytes = GetTextToByte(CloseClassBrace());
				EditorUtility.DisplayProgressBar(Title, "Close File...", 1);
				await stream.WriteAsync(bytes, 0, bytes.Length);
				stream.Close();
				await stream.DisposeAsync();
			}

			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog(Title, $"Create Animator Reflection \nPath : {Path.GetRelativePath("Assets", path)}", "Okay");
			AssetDatabase.Refresh();
		}
	}

	private string SetClassDeclaration(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"public static class {str} " + "\n{ \n";
	}

	private string SetSubClassDeclaration(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"\tpublic static class {str} " + "\n{ \n";
	}

	private string SetParamDeclaration(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"\t\tpublic const string {str} = \"{str}\";\n";
	}

	private string SetEnum(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"\tpublic enum {str} " + " \n\t{ \n";
	}

	private string SetEnumValue(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"\t\t{str},\n";
	}

	private string SetEnumSummary(string text)
	{
		return $"\t/// <summary>\n\t/// Animator Controller: {text}\n\t/// </summary>\n";
	}

	private string SetValueSummary(string text)
	{
		return $"\t\t/// <summary>\n\t\t/// Parameter Type: {text}\n\t\t/// </summary>\n";
	}

	private string CloseClassBrace() => "}\n";

	private string CloseSubClassBrace() => "\t}\n\n";

	private string CloseEnumBrace() => "\t}\n";

	private byte[] GetTextToByte(string text)
	{
		return Encoding.Default.GetBytes(text);
	}

	[MenuItem("Tool/Animator Parameter Creation")]
	public static void OpenMenu() => GetWindow<AnimatorParametersEditor>("Animator Parameter Creation");

	private string ReplaceNotValidChar(string text)
	{
		text = text.Replace(' ', '_');
		text = text.Replace('-', '_');
		text = text.Replace('@', '_');
		text = text.Replace('[', '_');
		text = text.Replace(']', '_');
		text = text.Replace('(', '_');
		text = text.Replace(')', '_');
		return text;
	}
}

#endif