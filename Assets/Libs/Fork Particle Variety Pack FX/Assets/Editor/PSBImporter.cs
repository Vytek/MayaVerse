/******************************************************************************
 Disclaimer Notice:
 This file is provided as is with no warranties of any kind and is
 provided without any obligation on Fork Particle, Inc. to assist in 
 its use or modification. Fork Particle, Inc. will not, under any
 circumstances, be liable for any lost revenue or other damages arising 
 from the use of this file.
 
 (c) Copyright 2017 Fork Particle, Inc. All rights reserved.
******************************************************************************/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
public class PSBImporter : AssetImporter
{	
	public const string PSBExtension = ".psb";
	public const string AssetExtension = ".asset";
	public const string PrefabExtension = ".prefab";

	public static void Import(string assetPath)
	{
		GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("", HideFlags.HideInHierarchy);
		string prefabFilePath = GetPrefabPath(assetPath);
		GameObject prefab = PrefabUtility.CreatePrefab(prefabFilePath, gameObject, ReplacePrefabOptions.ReplaceNameBased);

		PSBAsset asset = ScriptableObject.CreateInstance<PSBAsset>();
		asset.name = Path.GetFileNameWithoutExtension(assetPath);
		asset.Load (assetPath);
		AssetDatabase.AddObjectToAsset(asset, prefabFilePath);
		 
		ForkParticleEffect particleSystem = prefab.AddComponent<ForkParticleEffect>();
		particleSystem.PSBFile = asset;

        MeshRenderer mesh = prefab.AddComponent<MeshRenderer>();
        mesh.receiveShadows = false;
        mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mesh.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        prefab.AddComponent<MeshFilter>();

		GameObject.DestroyImmediate(gameObject);
	}

	public static void Delete(string assetPath)
	{
		string prefabFilePath = GetPrefabPath(assetPath);
		AssetDatabase.DeleteAsset(prefabFilePath);
	}

	public static bool IsPSBFile(string assetPath)
	{
		return assetPath.EndsWith(PSBExtension, StringComparison.OrdinalIgnoreCase);
	}

	public static string GetAssetPath(string assetPath)
	{
		return Path.ChangeExtension(assetPath, AssetExtension);
	}

	public static string GetPrefabPath(string assetPath)
	{
		return Path.ChangeExtension(assetPath, PrefabExtension);
	}
}
#endif