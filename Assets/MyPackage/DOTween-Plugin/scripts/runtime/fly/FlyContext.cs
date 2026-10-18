using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    /// <summary>Runtime parameters of one flying item.</summary>
    public struct FlyContext
    {
        /// <summary>The object that flies.</summary>
        public Transform Target;

        /// <summary>Where it starts (position and rotation). Defaults to Target itself.</summary>
        public Transform Origin;

        /// <summary>Where it flies to. Followed every frame, so it may move.</summary>
        public Transform Destination;

        public FlyContext(Transform target, Transform origin, Transform destination)
        {
            Target = target;
            Origin = origin;
            Destination = destination;
        }
    }
}
