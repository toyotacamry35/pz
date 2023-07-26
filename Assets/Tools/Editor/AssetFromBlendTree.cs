using System.Collections.Generic;
using UnityEditor; 
using UnityEngine; 
using UnityEditor.Animations;

namespace Assets.Tools
{
	public class CreateBlendtreeAsset : MonoBehaviour
	{
		[MenuItem("Tools/Asset from Blendtree")]
		static void CreateBlendtree()
		{
			if (Selection.objects.Length == 0)
				Debug.LogError("No objects selected");

			foreach (var obj in Selection.objects)
				CreateBlendtree(obj);
		}

		static void CreateBlendtree(Object @object)
		{
			string path = "Assets/";

			string currentPath = AssetDatabase.GetAssetPath(@object);
			if (currentPath != null)
			{
				path = currentPath;
			}

			string rootBlendTreeName = null;

			var blendTree = @object as BlendTree;
			if (blendTree)
			{
				rootBlendTreeName = blendTree.name;
			}
			else
			{
				var animState = @object as AnimatorState;
				if (animState)
				{
					blendTree = animState.motion as BlendTree;
					if (blendTree)
						rootBlendTreeName = animState.name;
				}
			}

			if (blendTree)
			{
				var list = new List<BlendTree>();
				CopyBlendTree(blendTree, list);

				var itr = list.GetEnumerator();
				if (itr.MoveNext())
				{
					var rootBlendTree = itr.Current;
					var assetPath =
						AssetDatabase.GenerateUniqueAssetPath(path.Replace(".controller", "") + "_" + rootBlendTreeName + ".asset");
					AssetDatabase.CreateAsset(rootBlendTree, assetPath);
					while (itr.MoveNext())
						AssetDatabase.AddObjectToAsset(itr.Current, assetPath);
					AssetDatabase.ImportAsset(assetPath);
				}
			}
			else
			{
				Debug.LogError($"{@object.name} is not a blend tree");
			}
		}

		static BlendTree CopyBlendTree(BlendTree blendTree, List<BlendTree> trees)
		{
			var blendTreeCopy = Instantiate<BlendTree>(blendTree);
			blendTreeCopy.name = blendTree.name;
			trees.Add(blendTreeCopy);
			var children = blendTreeCopy.children;
			var newChildren = new ChildMotion[blendTreeCopy.children.Length];
			for (var i = 0; i < children.Length; i++)
			{
				var childBlendTree = children[i].motion as BlendTree;
				newChildren[i] = children[i];
				if (childBlendTree)
					newChildren[i].motion = CopyBlendTree(childBlendTree, trees);
			}

			blendTreeCopy.children = newChildren;
			return blendTreeCopy;
		}
	}
}
