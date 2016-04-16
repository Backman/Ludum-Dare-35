using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditorInternal;
using System.IO;

namespace UnityEditor
{
	public class CreateAnimationEditor : EditorWindow
	{
		private List<Sprite> _sprites = new List<Sprite>();
		private static CreateAnimationEditor _window;
		private ReorderableList _list;
		private string _animationName;
		private Vector2 _scrollPos;

		[MenuItem("Tools/Create Sprite Animation")]
		private static void OpenWindow()
		{
			_window = GetWindow<CreateAnimationEditor>("Create Animation");
		}

		private void SetupReorderableList()
		{
			_list = new ReorderableList(_sprites, typeof(Sprite));
			_list.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				var sprite = (Sprite)_list.list[index];
				rect.y += 2f;
				var height = EditorGUIUtility.singleLineHeight;
				sprite = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, height),
					sprite, typeof(Sprite), false);
			};

			_list.drawHeaderCallback = (rect) =>
			{
				EditorGUI.LabelField(rect, "Sprites");
			};

			_list.onSelectCallback = (list) =>
			{
				var sprite = (Sprite)list.list[list.index];
				if (sprite)
				{
					EditorGUIUtility.PingObject(sprite);
				}
			};

			_list.onCanRemoveCallback = (list) =>
			{
				return list.count > 1;
			};

			_list.onRemoveCallback = (list) =>
			{
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
			};

			_list.onAddCallback = (list) =>
			{
				var index = list.count;
				list.list.Add(null);
				list.index = index;
				var sprite = (Sprite)list.list[index];

			};
		}

		private void OnGUI()
		{
			if (_list == null)
			{
				SetupReorderableList();
			}

			_scrollPos = GUILayout.BeginScrollView(_scrollPos);
			EditorGUILayout.Separator();


			//ReorderableListGUI.Title("Sprites");
			//ReorderableListGUI.ListField(_sprites, DrawSpriteItem);
			_list.DoLayoutList();


			_animationName = EditorGUILayout.TextField("Name", _animationName);

			var objects = Selection.objects;
			List<Sprite> sprites = new List<Sprite>();
			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i].GetType() != typeof(Sprite))
				{
					continue;
				}
				var sprite = (Sprite)objects[i];
				if (sprite != null)
				{
					sprites.Add(sprite);
				}
			}

			GUI.enabled = sprites.Count > 0;
			if (GUILayout.Button("Add Selected Sprites"))
			{
				_sprites.Clear();
				var sort = new Dictionary<int, Sprite>();
				var spriteIndex = new List<int>();
				var sortedSprites = new List<Sprite>();
				foreach (var sprite in sprites)
				{
					var split = sprite.name.Split('_');
					var index = int.Parse(split[split.Length - 1]);
					spriteIndex.Add(index);
					sort.Add(index, sprite);
				}
				spriteIndex.Sort();
				foreach (var index in spriteIndex)
				{
					sortedSprites.Add(sort[index]);
				}

				_sprites.AddRange(sortedSprites);
			}

			GUI.enabled = _sprites.Count > 0 && !_sprites.Any(s => s == null) && !string.IsNullOrEmpty(_animationName);

			if (GUILayout.Button("Create"))
			{
				var path = EditorUtility.SaveFilePanel("Save Animation",
					Application.dataPath, _animationName, "anim");
				if (!string.IsNullOrEmpty(path))
				{
					CreateAnimation(_sprites, path);
				}
			}
			GUI.enabled = _sprites.Count > 0;
			if (GUILayout.Button("Clear"))
			{
				_sprites.Clear();
			}

			GUILayout.EndScrollView();

			Repaint();
		}

		private Sprite DrawSpriteItem(Rect rect, Sprite sprite)
		{
			return EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), sprite, typeof(Sprite), false) as Sprite;
		}

		private static void CreateAnimation(List<Sprite> sprites, string path)
		{
			var frameCount = sprites.Count;
			var frameLength = 1f / 30f;

			var name = Path.GetFileNameWithoutExtension(path);
			var clip = new AnimationClip();
			clip.frameRate = 30f;
			clip.name = name;
			//AnimationUtility.GetAnimationClipSettings(clip);

			EditorCurveBinding curveBinding = new EditorCurveBinding();
			curveBinding.type = typeof(SpriteRenderer);
			curveBinding.propertyName = "m_Sprite";

			ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameCount];

			for (int i = 0; i < frameCount; i++)
			{
				ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
				kf.time = i * frameLength;
				kf.value = sprites[i];
				keyFrames[i] = kf;
			}

			AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

			var startIndex = path.IndexOf("Assets/");
			AssetDatabase.CreateAsset(clip, path.Substring(startIndex));
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
		}
	}
}