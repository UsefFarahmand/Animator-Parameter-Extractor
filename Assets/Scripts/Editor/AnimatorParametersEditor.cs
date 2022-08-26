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

		bool disable =
			animatorControllers.Count == 0 ||
			animatorControllers.Exists(x => x == null) ||
			animatorControllers.Count != animatorControllers.Distinct().Count();

		if (disable) HelpBox();


		if (GUILayout.Button(new GUIContent("Find All", "Find all animator controllers!"), GUI.skin.button))
		{
			FindAllAnimatorControllers();
		}

		EditorGUI.BeginDisabledGroup(disable);
		if (GUILayout.Button("Create File", GUI.skin.button))
		{
			EditorUtility.ClearProgressBar();
			Reflection(
				EditorUtility.SaveFilePanel(
					"Save file",
					"Assets",
					"AnimatorParameter",
					"cs"
					)
				);
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

	private async void Reflection(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			await using (FileStream stream = File.Create(path, 4096, FileOptions.Asynchronous))
			{
				byte[] bytes = GetTextToByte("using UnityEngine;\n\n");
				EditorUtility.DisplayProgressBar(Title, "Initialize...", 0);
				await stream.WriteAsync(bytes, 0, bytes.Length);

				bytes = GetTextToByte(SetClassDeclaration(fileName));
				EditorUtility.DisplayProgressBar(Title, "Initialize...", 1f);
				await stream.WriteAsync(bytes, 0, bytes.Length);

				Dictionary<AnimatorControllerParameterType, List<string>> parametersName = new Dictionary<AnimatorControllerParameterType, List<string>>();
				foreach (var val in Enum.GetValues(typeof(AnimatorControllerParameterType)).Cast<AnimatorControllerParameterType>())
				{
					parametersName.Add(val, new List<string>());
				}

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
						var key = parameters[j].name;
						if (parametersName[parameters[j].type].Contains(parameters[j].name)) continue;
						parametersName[parameters[j].type].Add(parameters[j].name);
					}

					foreach (var type in parametersName)
					{
						var typeInfo = $"{info} ({type.Key})";
						bytes = GetTextToByte(CreateEnum(type.Key.ToString()));
						await stream.WriteAsync(bytes, 0, bytes.Length);
						Thread.Sleep(200);
						for (int k = 0; k < type.Value.Count; k++)
						{
							string para = type.Value[k];
							bytes = GetTextToByte(CreateEnumValue(para));
							EditorUtility.DisplayProgressBar(Title, typeInfo, (float)k / type.Value.Count);
							await stream.WriteAsync(bytes, 0, bytes.Length);
							Thread.Sleep(200);
						}
						bytes = GetTextToByte(CloseEnumBrace());
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

	private string SetClassDeclaration(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"public static class {str} " + "{ \n";
	}

	private string SetSubClassDeclaration(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"\tpublic static class {str} " + "{ \n";
	}

	private string CreateEnum(string name)
	{
		string str = ReplaceNotValidChar(name);
		return $"\t\tpublic enum {str} " + " \n\t\t{ \n";
	}

	private string CreateEnumValue(string text)
	{
		string str = ReplaceNotValidChar(text);
		return $"\t\t\t{str},\n";
	}

	private string CloseClassBrace() => "}\n";

	private string CloseSubClassBrace() => "\t}\n\n";

	private string CloseEnumBrace() => "\t\t}\n";

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