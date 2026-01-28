using System;
using UnityEngine;

public static class NoiseBus
{
    public struct NoiseEvent
    {
        public Vector3 position;
        public float radius;
        public float intensity;
        public string source; // "Walk", "Sprint", "Jump", "Grab", etc.

        public NoiseEvent(Vector3 position, float radius, float intensity, string source)
        {
            this.position = position;
            this.radius = radius;
            this.intensity = intensity;
            this.source = source;
        }
    }

    public static event Action<NoiseEvent> OnNoiseEmitted;

    public static void Emit(Vector3 position, float radius, float intensity, string source)
    {
        var e = new NoiseEvent(position, radius, intensity, source);
        OnNoiseEmitted?.Invoke(e);

        Debug.Log($"[NOISE] {source} | radius={radius:0.0} | intensity={intensity:0.00} | pos={position}");
    }
}
