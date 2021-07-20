using UnityEngine;
using System;

namespace WaterKat.CustomVariables
{
    [Serializable]
    public abstract class CustomVariable<T> : ScriptableObject
    {
        [SerializeField] private T value;
        public T Value
        {
            get { return value; }
            set { this.value = value; OnValueChanged?.Invoke(this.value); }
        }

        public event CustomVariableEventHandler OnValueChanged;
        public delegate void CustomVariableEventHandler(T _newValue);
    }
}