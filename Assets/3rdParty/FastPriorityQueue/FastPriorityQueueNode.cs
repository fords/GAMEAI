using System;

namespace Priority_Queue
{
    public class FastPriorityQueueNode
    {
        /// <summary>
        /// The Priority to insert this pathNode at.  Must be set BEFORE adding a pathNode to the queue (ideally just once, in the pathNode's constructor).
        /// Should not be manually edited once the pathNode has been enqueued - use queue.UpdatePriority() instead
        /// </summary>
        public float Priority { get; protected internal set; }

        /// <summary>
        /// Represents the current position in the queue
        /// </summary>
        public int QueueIndex { get; internal set; }

#if DEBUG
        /// <summary>
        /// The queue this pathNode is tied to. Used only for debug builds.
        /// </summary>
        public object Queue { get; internal set; }
#endif
    }
}
