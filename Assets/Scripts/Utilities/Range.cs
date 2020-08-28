using UnityEngine;

[System.Serializable]
public struct Range
{
    public float min;
    public float max;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public static implicit operator Range(float value)
    {
        return new Range() { max = value, min = value };
    }

    public float Lerp(float t)
    {
        return Mathf.Lerp(this.min, this.max, t);
    }

    public float InverseLerp(float value)
    {
        return Mathf.InverseLerp(this.min, this.max, value);
    }

    public float Clamp(float value)
    {
        return Mathf.Clamp(value, this.min, this.max);
    }
}