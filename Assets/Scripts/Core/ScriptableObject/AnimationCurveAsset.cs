using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatedCurve", menuName = "Curve/AnimatedCurve")]
public class AnimationCurveAsset : ScriptableObject
{
	public AnimationCurve	m_AnimationCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
}
