using System;

namespace WaterKat.CustomVariables
{
    [Serializable]
    public abstract class CustomReference<T>
    {
        public bool UseConstant = true;
        public T ConstantValue;
        public CustomVariable<T> Variable;

        public T Value
        {
            get 
            { 
                return UseConstant ? ConstantValue : Variable.Value; 
            }
            set
            {
                if (UseConstant)
                {
                    ConstantValue = value;
                }
                else
                {
                    Variable.Value = value;
                }
            }
        }
    }
}