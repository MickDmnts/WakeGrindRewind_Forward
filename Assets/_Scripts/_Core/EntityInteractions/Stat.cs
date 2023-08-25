using UnityEngine;

namespace WGR.Entities
{
    /* CLASS DOCUMENTATION *\
     * [Must Know]
     * 1. Stat is a generic helper class that is used in the Entity class for further value modifications.
     */
    [System.Serializable]
    public class Stat<T>
    {
        [SerializeField] private T value;

        public void SetValue(T value)
        {
            this.value = value;
        }

        public T GetValue()
        { return this.value; }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }

}