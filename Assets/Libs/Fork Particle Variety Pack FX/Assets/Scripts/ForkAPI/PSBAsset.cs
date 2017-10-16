/******************************************************************************
 Disclaimer Notice:
 This file is provided as is with no warranties of any kind and is
 provided without any obligation on Fork Particle, Inc. to assist in 
 its use or modification. Fork Particle, Inc. will not, under any
 circumstances, be liable for any lost revenue or other damages arising 
 from the use of this file.
 
 (c) Copyright 2017 Fork Particle, Inc. All rights reserved.
******************************************************************************/

using UnityEngine;
using System;
using System.IO;

public class PSBAsset : ScriptableObject
{
	public byte[] Data { get; protected set; }
	public char[] sPSBName;
	public int nDataSize = 0;
	public string sPSBPath;

	public void Load(string psbFile)
	{
		sPSBPath = psbFile;
		Data = File.ReadAllBytes (psbFile);
		sPSBName = this.name.ToCharArray();
		nDataSize = Data.Length;
	}
}