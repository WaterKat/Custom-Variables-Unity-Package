using UnityEngine;
using System;

namespace WaterKat.CustomVariables
{
    [Serializable]
    public abstract class CustomVariable<T> : ScriptableObject
    {
        public T Value;
    }
}