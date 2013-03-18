using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections
{
    /// <summary>
    /// A weak reference to a an object of a specific type. The item may still be garbage collected even while a weak reference is held.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedWeakReference<T> where T : class
    {
        private WeakReference Reference;

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get
            {
                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public T Target
        {
            get
            {
                return Reference.Target as T;
            }
            set
            {
                Reference.Target = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object referenced by this weak reference is tracked after finalisation.
        /// </summary>
        /// <value><c>true</c> if the object referenced by this weak reference is tracked after finalisation; otherwise, <c>false</c>.</value>
        public bool TrackResurrection
        {
            get
            {
                return Reference.TrackResurrection;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedWeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public TypedWeakReference(T target)
        {
            Reference = new WeakReference(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedWeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> the object referenced by this weak reference is tracked after finalisation.</param>
        public TypedWeakReference(T target, bool trackResurrection)
        {
            Reference = new WeakReference(target, trackResurrection);
        }
    }
}
