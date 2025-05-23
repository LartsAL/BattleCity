using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class TimeUtils
    {
        public static IEnumerator WaitAndDo(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }
}