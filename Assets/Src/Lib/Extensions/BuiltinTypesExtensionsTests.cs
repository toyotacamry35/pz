using System;
using UnityEngine;

namespace Uins.Extensions
{
    public class BuiltinTypesExtensionsTests : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("BuiltinTypesExtensions Tests ...", gameObject);

            AssertAreEqual(4f.SafeDivide(1), 4);

            AssertAreEqual(4f.SafeDivide(0, 42), 42);

            AssertAreEqual(4f.SafeDivide(0), float.MaxValue);

            AssertAreEqual(0f.SafeDivide(0), 0);

            AssertAreEqual(4.5f.SafeDivide(123.4f), 4.5f / 123.4f);

            AssertAreEqual(600f.SafeDivide(1000), 0.6f);


            AssertAreEqual(0f.SafeRatio(1), 0);

            AssertAreEqual(1f.SafeRatio(0), 1);

            AssertAreEqual(1000f.SafeRatio(0), 1);

            AssertAreEqual(-0.1f.SafeRatio(0), -1);

            AssertAreEqual(0f.SafeRatio(0), 0);

            AssertAreEqual(2f.SafeRatio(4), 0.5f);

            AssertAreEqual(4f.SafeRatio(2, 1.5f), 1.5f);

//            AssertAreEqual(4f.SafeRatio(2), 1);

            Debug.Log("... tests completed", gameObject);
        }

        private void AssertAreEqual(float one, float expected)
        {
            if (!Mathf.Approximately(one, expected))
                LogError($"Expected {expected}, but was: {one}");
        }

        private void LogError(string msg)
        {
            Debug.LogError(msg, gameObject);
        }
    }
}