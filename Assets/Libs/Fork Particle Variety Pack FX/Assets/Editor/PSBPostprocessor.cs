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
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 
public class PSBPostprocessor: AssetPostprocessor
{
	protected static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		foreach(string assetPath in importedAssets)
		{
			if (PSBImporter.IsPSBFile(assetPath))
				PSBImporter.Import(assetPath);
		}

		foreach (string assetPath in deletedAssets)
		{
			if (PSBImporter.IsPSBFile(assetPath))
				PSBImporter.Delete(assetPath);
		}

		foreach (string assetPath in movedFromPath)
		{
			if (PSBImporter.IsPSBFile(assetPath))
				PSBImporter.Delete(assetPath);
		}
	}
}
#endif