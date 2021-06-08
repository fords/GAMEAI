using System;
using System.Collections.Generic;

namespace Priority_Queue
{
    /// <summary>
    /// The IPriorityQueue interface.  This is mainly here for purists, and in case I decide to add more implementations later.
    /// For speed purposes, it is actually recommended that you *don't* access the priority queue through this interface, since the JIT can
    /// (theoretically?) optimize method calls from concrete-types slightly better.
    /// </summary>
    public interface IPriorityQueue<TItem, in TPriority> : IEnumerable<TItem>
        where TPriority : IComparable<TPriority>
    {
        /// <summary>
        /// Enqueue a pathNode to the priority queue.  Lower values are placed in front. Ties are broken by first-in-first-out.
        /// See implementation for how duplicates are handled.
        /// </summary>
        void Enqueue(TItem pathNode, TPriority priority);

        /// <summary>
        /// Removes the head of the queue (pathNode with minimum priority; ties are broken by order of insertion), and returns it.
        /// </summary>
        TItem Dequeue();

        /// <summary>
        /// Removes every pathNode from the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns whether the given pathNode is in the queue.
        /// </summary>
        bool Contains(TItem pathNode);

        /// <summary>
        /// Removes a pathNode from the queue.  The pathNode does not need to be the head of the queue.  
        /// </summary>
        void Remove(TItem pathNode);

        /// <summary>
        /// Call this method to change the priority of a pathNode.  
        /// </summary>
        void UpdatePriority(TItem pathNode, TPriority priority);

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// </summary>
        TItem First { get; }

        /// <summary>
        /// Returns the number of nodes in the queue.
        /// </summary>
        int Count { get; }
    }
}
