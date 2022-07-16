using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
	public static class Util
	{
		public static Vector3Int ScreenSize { get { return Vector3Int.right * Screen.width + Vector3Int.up * Screen.height; } }

		public static Vector3 ScreenCenter { get { return Vector3.right * ScreenSize.x * .5f + Vector3.up * ScreenSize.y * .5f; } }

		public static bool Chance(float chanceOfSuccess)
		{
			return Random.Range(0f, 1f) <= chanceOfSuccess;
		}

		public static void TryAction(System.Action action)
		{
			if (action != null)
				action();
		}

		public static void TryAction(System.Action<float> action, float parameter)
		{
			if (action != null)
				action(parameter);
		}

		public static IEnumerator _Transition(float duration, System.Action<float> callback)
		{
			float mult = 1 / duration;
			float state = 0;
			while (state < 1f)
			{
				callback(state);
				yield return null;
				state = Mathf.Clamp(state + Time.deltaTime * mult, 0f, 1f);
			}
			callback(state);
		}
	}

	public static class UMath
	{
		public static float Normalize(float value, float min, float max)
		{
			return (value - min) / (max - min);
		}

		public static float DivideByZero(float dividend, float divider)
		{
			try
			{
				return dividend / divider;
			}
			catch (System.DivideByZeroException)
			{
				return float.PositiveInfinity;
			}
		}
	}
	
	class UTime
	{
		public static string FormatTime(double time)
		{
			var mm = (int)(time / 60);
			var ss = (int)(time - mm * 60);

			return FormatNum(mm) + ":" + FormatNum(ss);

			string FormatNum(int num)
			{
				return (num > 9 ? "" : "0") + num.ToString();
			}
		}
	}

	static class UCurve
	{
		static float doublePi { get { return Mathf.PI * 2f; } }

		public static AnimationCurve bell = new AnimationCurve(
			new Keyframe(0f, 0f),
			new Keyframe(.5f, 1f),
			new Keyframe(1f, 0f)
			);

		public static AnimationCurve sine = new AnimationCurve(
			new Keyframe(0f, 0f, doublePi, doublePi),
			new Keyframe(.25f, 1f, 0f, 0f),
			new Keyframe(.75f, -1f, 0f, 0f),
			new Keyframe(1f, 0f, doublePi, doublePi)
			);
	}

	class Timer
	{
		private float timeFromStart = Mathf.NegativeInfinity;
		private float duration;
		protected bool isUnscaled = false;
		private float CurrentTime { get { return isUnscaled ? Time.unscaledTime : Time.time; } }

		public bool IsUnscaled { get { return isUnscaled; } }
		public float Completion { get { return UMath.Normalize(ElapsedTime, 0f, duration); } }
		public float RemainingTime { get { return duration - ElapsedTime; } }
		public float ElapsedTime { get { return Time.time - timeFromStart; } }

		public bool IsExpired
		{
			get { return CurrentTime - timeFromStart > duration; }
		}

		public void Set(float duration)
		{
			timeFromStart = CurrentTime;
			this.duration = duration;
		}
	}

	class UnscaledTimer : Timer
	{
		public UnscaledTimer()
		{
			this.isUnscaled = true;
		}
	}

	public class OneTimeEvent
	{
		bool performed = false;

		System.Func<bool> Condition;
		System.Action Callback;

		public OneTimeEvent(System.Func<bool> condition, System.Action callback)
		{
			this.Condition = condition;
			this.Callback = callback;
		}

		public void Evaluate()
		{
			if (!performed && Condition())
			{
				Callback();
				performed = true;
			}
		}

		public void Reset()
		{
			performed = false;
		}
	}
}