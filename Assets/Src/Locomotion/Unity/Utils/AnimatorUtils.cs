using UnityEngine;

namespace Src.Locomotion.Unity
{
    public static class AnimatorUtils
    {
        public static bool IsParameterExists(this Animator animator, string name)
        {
            foreach (var param in animator.parameters)
                if (param.name == name)
                    return true;
            return false;
        }
        
        public static bool IsParameterExists(this Animator animator, int nameHash)
        {
            foreach (var param in animator.parameters)
                if (param.nameHash == nameHash)
                    return true;
            return false;
        }
    }
}