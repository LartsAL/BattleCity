using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class TimeUtils
    {
        public static void StopTime()
        {
            Time.timeScale = 0.0f;
        }
        
        public static void ResumeTime()
        {
            Time.timeScale = 1.0f;
        }
        
        public static IEnumerator WaitAndDo(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }
}