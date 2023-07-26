using System;
using UnityEngine;

[Serializable]
public class NormalizedAnimationCurve
{
	[SerializeField]
	private AnimationCurve _curve = new AnimationCurve();

	[SerializeField]
	private float _amplitude = 1;

	[SerializeField]
	private float _duration = 1;

	public Keyframe[] Keys { get { return _curve.keys; }}

	public float Evaluate(float t)
	{
		return _curve.Evaluate(t / _duration) * _amplitude;
	}

	public float EvaluateNormalized(float t)
	{
		return _curve.Evaluate(t);
	}

	public float GetAmplitude()
	{
		return _amplitude;
	}

	public float GetDuration()
	{
		return _duration;
	}
}