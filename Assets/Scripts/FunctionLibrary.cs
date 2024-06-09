using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);

    public enum FunctionName
    {
        MobiusStrip,
        MultiWave,
        SphericalHarmonics,
        DNAHelix,
        Torus,
        torusKnot,
    }

    private static readonly Function[] functions = { MobiusStrip, MultiWave, SphericalHarmonics, DNAHelix, Torus , TorusKnot};

    public static int FunctionCount => functions.Length;

    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    public static FunctionName GetNextFunctionName(FunctionName name)
    {
        return (int)name < functions.Length - 1 ? name + 1 : 0;
    }

    public static FunctionName GetRandomFunctionNameOtherThan(FunctionName name)
    {
        var choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == name ? 0 : choice; 
    }

    public static Vector3 Morph(
        float u, float v, float t, Function from, Function to, float progress
    )
    {
        return Vector3.LerpUnclamped(
            from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress)
        );
    }
    
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        var d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        var r = 0.9f + 0.1f * Sin(PI * (12f * u + 8f * v + t));
        var s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        var r1 = 0.7f + 0.1f * Sin(PI * (8f * u + 0.5f * t));
        var r2 = 0.15f + 0.05f * Sin(PI * (16f * u + 8f * v + 3f * t));
        var s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 MobiusStrip(float u, float v, float t)
    {
        float radius = 1.0f + 0.5f * v * Cos(0.5f * PI * u);
        Vector3 p;
        p.x = radius * Cos(PI * u + 0.5f * t);
        p.y = radius * Sin(PI * u + 0.5f * t);
        p.z = v * Sin(0.5f * PI * u + t);
        return p;
    }
    public static Vector3 TorusKnot(float u, float v, float t)
    {
        float r1 = 1f + 0.3f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.2f + 0.1f * Sin(PI * (12f * v + t));
        float angle = 2f * PI * u;
        Vector3 p;
        p.x = (r1 + r2 * Cos(2f * PI * v)) * Cos(angle);
        p.y = (r1 + r2 * Cos(2f * PI * v)) * Sin(angle);
        p.z = r2 * Sin(2f * PI * v);
        return p;
    }
    public static Vector3 SphericalHarmonics(float u, float v, float t)
    {
        float theta = PI * u;
        float phi = 2 * PI * v;
        float r = 0.5f + 0.5f * Sin(4 * phi + t) * Cos(2 * theta + t);
        
        Vector3 p;
        p.x = r * Sin(theta) * Cos(phi);
        p.y = r * Sin(theta) * Sin(phi);
        p.z = r * Cos(theta);
        return p;
    }
    
    public static Vector3 DNAHelix(float u, float v, float t)
    {
        float offset = PI; // Offset one of the helices by 180 degrees
        float radius = 0.5f;
        float angle1 = 2f * PI * u;
        float angle2 = 2f * PI * u + offset; // Add the offset to the angle for the second helix
        float height = u * 2f; 
        
        Vector3 p1;
        p1.x = radius * Cos(angle1);
        p1.y = height;
        p1.z = radius * Sin(angle1);

        Vector3 p2;
        p2.x = radius * Cos(angle2);
        p2.y = height;
        p2.z = radius * Sin(angle2);

        Vector3 p = Vector3.Lerp(p1, p2, v);

        float rotationSpeed = 50f; // Speed of rotation
        p = Quaternion.Euler(0f, rotationSpeed * t, 0f) * p;

        return p;
    }

}