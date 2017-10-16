﻿//
// Copyright 2017 Valve Corporation. All rights reserved. Subject to the following license:
// https://valvesoftware.github.io/steam-audio/license.html
//

using System;
using System.Runtime.InteropServices;

namespace SteamAudio
{
    //
    // Phonon API functions.
    //
    public static class PhononCore
    {
        //
        // Functions for OpenCL compute devices.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateComputeDevice(GlobalContext globalContext, ComputeDeviceType deviceType, int numComputeUnits, [In, Out] ref IntPtr device);

        [DllImport("phonon")]
        public static extern void iplDestroyComputeDevice([In, Out] ref IntPtr device);

        //
        // Scene export callback functions.
        //

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LoadSceneProgressCallback(float progress);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FinalizeSceneProgressCallback(float progress);

        //
        // Functions for scene export.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateScene(GlobalContext globalContext, IntPtr computeDevice, SimulationSettings simulationSettings, int numMaterials, [In, Out] ref IntPtr scene);

        [DllImport("phonon")]
        public static extern void iplDestroyScene([In, Out] ref IntPtr scene);

        [DllImport("phonon")]
        public static extern void iplSetSceneMaterial(IntPtr scene, int materialIndex, Material material);

        [DllImport("phonon")]
        public static extern Error iplCreateStaticMesh(IntPtr scene, int numVertices, int numTriangles, [In, Out] ref IntPtr staticMesh);

        [DllImport("phonon")]
        public static extern void iplDestroyStaticMesh([In, Out] ref IntPtr staticMesh);

        [DllImport("phonon")]
        public static extern void iplSetStaticMeshVertices(IntPtr scene, IntPtr staticMesh, Vector3[] vertices);

        [DllImport("phonon")]
        public static extern void iplSetStaticMeshTriangles(IntPtr scene, IntPtr staticMesh, Triangle[] triangles);

        [DllImport("phonon")]
        public static extern void iplSetStaticMeshMaterials(IntPtr scene, IntPtr staticMesh, int[] materialIndices);

        [DllImport("phonon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void iplFinalizeScene(IntPtr scene, FinalizeSceneProgressCallback progressCallback);

        [DllImport("phonon")]
        public static extern int iplSaveFinalizedScene(IntPtr scene, [In, Out] byte[] data);

        [DllImport("phonon", CallingConvention = CallingConvention.Cdecl)]
        public static extern Error iplLoadFinalizedScene(GlobalContext globalContext, SimulationSettings simulationSettings, byte[] data, int size, IntPtr computeDevice, LoadSceneProgressCallback progressCallback, [In, Out] ref IntPtr scene);

        [DllImport("phonon")]
        public static extern void iplDumpSceneToObjFile(IntPtr scene, byte[] fileName);

        //
        // Custom ray tracer callback functions.
        //

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ClosestHitCallback(Vector3 origin, Vector3 direction, float minDistance, float maxDistance, [In, Out] ref float hitDistance, [In, Out] ref Vector3 hitNormal, [In, Out] ref int hitMaterialIndex, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AnyHitCallback(Vector3 origin, Vector3 direction, float minDistance, float maxDistance, [In, Out] ref int hitExists, IntPtr userData);

        //
        // Functions for setting up a custom ray tracer.
        //

        [DllImport("phonon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void iplSetRayTracerCallbacks(IntPtr scene, ClosestHitCallback closetHitCallback, AnyHitCallback anyHitCallback, IntPtr userData);

        //
        // Functions to setup Environment.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateEnvironment(GlobalContext globalContext, IntPtr computeDevice, SimulationSettings simulationSettings, IntPtr scene, IntPtr probeManager, [In, Out] ref IntPtr environment);

        [DllImport("phonon")]
        public static extern void iplDestroyEnvironment([In, Out] ref IntPtr environment);

        [DllImport("phonon")]
        public static extern void iplSetNumBounces(IntPtr environment, int numBounces);

        //
        // Functions related to Audio Buffers.
        //

        [DllImport("phonon")]
        public static extern void iplMixAudioBuffers(int numBuffers, AudioBuffer[] inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplInterleaveAudioBuffer(AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplDeinterleaveAudioBuffer(AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplConvertAudioBufferFormat(AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern Error iplCreateAmbisonicsRotator(int order, [In, Out] ref IntPtr rotator);

        [DllImport("phonon")]
        public static extern void iplDestroyAmbisonicsRotator([In, Out] ref IntPtr rotator);

        [DllImport("phonon")]
        public static extern void iplSetAmbisonicsRotation(IntPtr rotator, Quaternion quaternion);

        [DllImport("phonon")]
        public static extern void iplRotateAmbisonicsAudioBuffer(IntPtr rotator, AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern Vector3 iplCalculateRelativeDirection(Vector3 sourcePosition, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp);

        //
        // Functions for HRTF based 3D audio processing.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateBinauralRenderer(GlobalContext globalContext, RenderingSettings renderingSettings, HRTFParams hrtfParams, [In, Out] ref IntPtr renderer);

        [DllImport("phonon")]
        public static extern void iplDestroyBinauralRenderer([In, Out] ref IntPtr renderer);

        [DllImport("phonon")]
        public static extern Error iplCreatePanningEffect(IntPtr renderer, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyPanningEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplApplyPanningEffect(IntPtr effect, AudioBuffer inputAudio, Vector3 direction, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplFlushPanningEffect(IntPtr effect);

        [DllImport("phonon")]
        public static extern Error iplCreateBinauralEffect(IntPtr renderer, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyBinauralEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplApplyBinauralEffect(IntPtr effect, AudioBuffer inputAudio, Vector3 direction, HRTFInterpolation interpolation, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplFlushBinauralEffect(IntPtr effect);

        [DllImport("phonon")]
        public static extern Error iplCreateVirtualSurroundEffect(IntPtr renderer, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyVirtualSurroundEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplApplyVirtualSurroundEffect(IntPtr effect, AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplFlushVirtualSurroundEffect(IntPtr effect);

        [DllImport("phonon")]
        public static extern Error iplCreateAmbisonicsPanningEffect(IntPtr renderer, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyAmbisonicsPanningEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplApplyAmbisonicsPanningEffect(IntPtr effect, AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplFlushAmbisonicsPanningEffect(IntPtr effect);

        [DllImport("phonon")]
        public static extern Error iplCreateAmbisonicsBinauralEffect(IntPtr renderer, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyAmbisonicsBinauralEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplApplyAmbisonicsBinauralEffect(IntPtr effect, AudioBuffer inputAudio, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplFlushAmbisonicsBinauralEffect(IntPtr effect);

        //
        // Functions for Environment renderer.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateEnvironmentalRenderer(GlobalContext globalContext, IntPtr environment,
            RenderingSettings renderingSettings, AudioFormat outputFormat, IntPtr threadCreateCallback,
            IntPtr threadDestroyCallback, [In, Out] ref IntPtr renderer);

        [DllImport("phonon")]
        public static extern void iplDestroyEnvironmentalRenderer([In, Out] ref IntPtr renderer);

        //
        // Direct Sound.
        //

        [DllImport("phonon")]
        public static extern DirectSoundPath iplGetDirectSoundPath(IntPtr renderer, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, Vector3 sourcePosition, float sourceRadius, OcclusionMode occlusionMode, OcclusionMethod occlusionMethod);

        //
        // Direct Sound Effect.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateDirectSoundEffect(IntPtr renderer, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyDirectSoundEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplApplyDirectSoundEffect(IntPtr effect, AudioBuffer inputAudio, DirectSoundPath directSoundPath, DirectSoundEffectOptions directSoundEffectOptions, AudioBuffer outputAudio);

        [DllImport("phonon")]
        public static extern void iplFlushDirectSoundEffect(IntPtr effect);

        //
        // Convolution Effect.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateConvolutionEffect(IntPtr renderer, byte[] name, SimulationType simulationType, AudioFormat inputFormat, AudioFormat outputFormat, [In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplDestroyConvolutionEffect([In, Out] ref IntPtr effect);

        [DllImport("phonon")]
        public static extern void iplSetConvolutionEffectName(IntPtr effect, byte[] name);

        [DllImport("phonon")]
        public static extern void iplSetDryAudioForConvolutionEffect(IntPtr effect, Vector3 sourcePosition, AudioBuffer dryAudio);

        [DllImport("phonon")]
        public static extern void iplGetWetAudioForConvolutionEffect(IntPtr effect, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, AudioBuffer wetAudio);

        [DllImport("phonon")]
        public static extern void iplGetMixedEnvironmentalAudio(IntPtr renderer, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, AudioBuffer mixedWetAudio);

        [DllImport("phonon")]
        public static extern void iplFlushConvolutionEffect(IntPtr effect);

        //
        // Acoustic Probes callback.
        //

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ProbePlacementProgressCallback(float progress);

        //
        // Functions for creating and managing Acoustic Probes for baking.
        //

        [DllImport("phonon")]
        public static extern Error iplCreateProbeBox(IntPtr scene, float[] boxLocalToWorldTransform, ProbePlacementParameters placementParams, ProbePlacementProgressCallback progressCallback, [In, Out] ref IntPtr probeBox);

        [DllImport("phonon")]
        public static extern void iplDestroyProbeBox([In, Out] ref IntPtr probeBox);

        [DllImport("phonon")]
        public static extern int iplGetProbeSpheres(IntPtr probeBox, [In, Out] Sphere[] probeSpheres);

        [DllImport("phonon")]
        public static extern int iplSaveProbeBox(IntPtr probeBox, [In, Out] Byte[] data);

        [DllImport("phonon")]
        public static extern Error iplLoadProbeBox(Byte[] data, int size, [In, Out] ref IntPtr probeBox);

        [DllImport("phonon")]
        public static extern Error iplCreateProbeBatch([In, Out] ref IntPtr probeBatch);

        [DllImport("phonon")]
        public static extern void iplDestroyProbeBatch([In, Out] ref IntPtr probeBatch);

        [DllImport("phonon")]
        public static extern void iplAddProbeToBatch(IntPtr probeBatch, IntPtr probeBox, int probeIndex);

        [DllImport("phonon")]
        public static extern void iplFinalizeProbeBatch(IntPtr probeBatch);

        [DllImport("phonon")]
        public static extern int iplSaveProbeBatch(IntPtr probeBatch, [In, Out] Byte[] data);

        [DllImport("phonon")]
        public static extern Error iplLoadProbeBatch(Byte[] data, int size, [In, Out] ref IntPtr probeBatch);

        [DllImport("phonon")]
        public static extern Error iplCreateProbeManager([In, Out] ref IntPtr probeManager);

        [DllImport("phonon")]
        public static extern void iplDestroyProbeManager([In, Out] ref IntPtr probeManager);

        [DllImport("phonon")]
        public static extern void iplAddProbeBatch(IntPtr probeManager, IntPtr probeBatch);

        [DllImport("phonon")]
        public static extern void iplRemoveProbeBatch(IntPtr probeManager, IntPtr probeBatch);

        //
        // Baking related callback.
        //

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BakeProgressCallback(float progress);

        //
        // Functions for baking environmental effects.
        //

        [DllImport("phonon")]
        public static extern void iplBakeReverb(IntPtr environment, IntPtr probeBox, BakingSettings bakingSettings, BakeProgressCallback progressCallback);

        [DllImport("phonon")]
        public static extern void iplBakePropagation(IntPtr environment, IntPtr probeBox, Sphere sourceInfluence, byte[] sourceName, BakingSettings bakingSettings, BakeProgressCallback progressCallback);

        [DllImport("phonon")]
        public static extern void iplBakeStaticListener(IntPtr environment, IntPtr probeBox, Sphere listenerInfluence, byte[] listenerName, BakingSettings bakingSettings, BakeProgressCallback progressCallback);

        [DllImport("phonon")]
        public static extern void iplCancelBake();

        [DllImport("phonon")]
        public static extern void iplDeleteBakedDataByName(IntPtr probeBox, byte[] sourceName);

        [DllImport("phonon")]
        public static extern int iplGetBakedDataSizeByName(IntPtr probeBox, byte[] sourceName);

        //
        // Functions for generating IRs for analysis and visualization.

        [DllImport("phonon")]
        public static extern Error iplCreateSimulationData(SimulationSettings simulationSettings, RenderingSettings renderingSettings, [In, Out] ref IntPtr simulationData);

        [DllImport("phonon")]
        public static extern void iplDestroySimulationData([In, Out] ref IntPtr simulationData);

        [DllImport("phonon")]
        public static extern int iplGetNumIrSamples(IntPtr simulationData);

        [DllImport("phonon")]
        public static extern int iplGetNumIrChannels(IntPtr simulationData);

        [DllImport("phonon")]
        public static extern void iplGenerateSimulationData(IntPtr simulationData, IntPtr environment, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, Vector3[] sourcePositions);

        [DllImport("phonon")]
        public static extern void iplGetSimulationResult(IntPtr simulationData, int sourceIndex, int channel, float[] buffer);
    }

    public static class PhononUnityNative
    {
        [DllImport("audioplugin_phonon")]
        public static extern void iplUnityResetAudioEngine();

        [DllImport("audioplugin_phonon")]
        public static extern void iplUnitySetEnvironment(SimulationSettings simulationSettings, IntPtr environment);

        [DllImport("audioplugin_phonon")]
        public static extern void iplUnityResetEnvironment();

        [DllImport("audioplugin_phonon")]
        public static extern void iplUnitySetListener(Vector3 position, Vector3 ahead, Vector3 up);
    }
}
