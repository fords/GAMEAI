using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Priority_Queue
{
    /// <summary>
    /// A copy of StablePriorityQueue which also has generic priority-type
    /// </summary>
    /// <typeparam name="TItem">The values in the queue.  Must extend the GenericPriorityQueueNode class</typeparam>
    /// <typeparam name="TPriority">The priority-type.  Must extend IComparable&lt;TPriority&gt;</typeparam>
    public sealed class GenericPriorityQueue<TItem, TPriority> : IFixedSizePriorityQueue<TItem, TPriority>
        where TItem : GenericPriorityQueueNode<TPriority>
        where TPriority : IComparable<TPriority>
    {
        private int _numNodes;
        private TItem[] _nodes;
        private long _numNodesEverEnqueued;
        private readonly Comparison<TPriority> _comparer;

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        public GenericPriorityQueue(int maxNodes) : this(maxNodes, Comparer<TPriority>.Default) { }

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        /// <param name="comparer">The comparer used to compare TPriority values.</param>
        public GenericPriorityQueue(int maxNodes, IComparer<TPriority> comparer) : this(maxNodes, comparer.Compare) { }

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        /// <param name="comparer">The comparison function to use to compare TPriority values</param>
        public GenericPriorityQueue(int maxNodes, Comparison<TPriority> comparer)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
            }
#endif

            _numNodes = 0;
            _nodes = new TItem[maxNodes + 1];
            _numNodesEverEnqueued = 0;
            _comparer = comparer;
        }

        /// <summary>
        /// Returns the number of nodes in the queue.
        /// O(1)
        /// </summary>
        public int Count
        {
            get
            {
                return _numNodes;
            }
        }

        /// <summary>
        /// Returns the maximum number of items that can be enqueued at once in this queue.  Once you hit this number (ie. once Count == MaxSize),
        /// attempting to enqueue another item will cause undefined behavior.  O(1)
        /// </summary>
        public int MaxSize
        {
            get
            {
                return _nodes.Length - 1;
            }
        }

        /// <summary>
        /// Removes every pathNode from the queue.
        /// O(n) (So, don't do this often!)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            Array.Clear(_nodes, 1, _numNodes);
            _numNodes = 0;
        }

        /// <summary>
        /// Returns (in O(1)!) whether the given pathNode is in the queue.
        /// If pathNode is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(pathNode) has been called
        /// O(1)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(TItem pathNode)
        {
#if DEBUG
            if(pathNode == null)
            {
                throw new ArgumentNullException("pathNode");
            }
            if (pathNode.Queue != null && !Equals(pathNode.Queue))
            {
                throw new InvalidOperationException("pathNode.Contains was called on a pathNode from another queue.  Please call originalQueue.ResetNode() first");
            }
            if (pathNode.QueueIndex < 0 || pathNode.QueueIndex >= _nodes.Length)
            {
                throw new InvalidOperationException("pathNode.QueueIndex has been corrupted. Did you change it manually?");
            }
#endif

            return (_nodes[pathNode.QueueIndex] == pathNode);
        }

        /// <summary>
        /// Enqueue a pathNode to the priority queue.  Lower values are placed in front. Ties are broken by first-in-first-out.
        /// If the queue is full, the result is undefined.
        /// If the pathNode is already enqueued, the result is undefined.
        /// If pathNode is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(pathNode) has been called
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Enqueue(TItem pathNode, TPriority priority)
        {
#if DEBUG
            if(pathNode == null)
            {
                throw new ArgumentNullException("pathNode");
            }
            if(_numNodes >= _nodes.Length - 1)
            {
                throw new InvalidOperationException("Queue is full - pathNode cannot be added: " + pathNode);
            }
            if (pathNode.Queue != null && !Equals(pathNode.Queue))
            {
                throw new InvalidOperationException("pathNode.Enqueue was called on a pathNode from another queue.  Please call originalQueue.ResetNode() first");
            }
            if (Contains(pathNode))
            {
                throw new InvalidOperationException("Node is already enqueued: " + pathNode);
            }
            pathNode.Queue = this;
#endif

            pathNode.Priority = priority;
            _numNodes++;
            _nodes[_numNodes] = pathNode;
            pathNode.QueueIndex = _numNodes;
            pathNode.InsertionIndex = _numNodesEverEnqueued++;
            CascadeUp(pathNode);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeUp(TItem pathNode)
        {
            //aka Heapify-up
            int parent;
            if (pathNode.QueueIndex > 1)
            {
                parent = pathNode.QueueIndex >> 1;
                TItem parentNode = _nodes[parent];
                if(HasHigherPriority(parentNode, pathNode))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _nodes[pathNode.QueueIndex] = parentNode;
                parentNode.QueueIndex = pathNode.QueueIndex;

                pathNode.QueueIndex = parent;
            }
            else
            {
                return;
            }
            while(parent > 1)
            {
                parent >>= 1;
                TItem parentNode = _nodes[parent];
                if(HasHigherPriority(parentNode, pathNode))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                _nodes[pathNode.QueueIndex] = parentNode;
                parentNode.QueueIndex = pathNode.QueueIndex;

                pathNode.QueueIndex = parent;
            }
            _nodes[pathNode.QueueIndex] = pathNode;
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeDown(TItem pathNode)
        {
            //aka Heapify-down
            int finalQueueIndex = pathNode.QueueIndex;
            int childLeftIndex = 2 * finalQueueIndex;

            // If leaf pathNode, we're done
            if(childLeftIndex > _numNodes)
            {
                return;
            }

            // Check if the left-child is higher-priority than the current pathNode
            int childRightIndex = childLeftIndex + 1;
            TItem childLeft = _nodes[childLeftIndex];
            if(HasHigherPriority(childLeft, pathNode))
            {
                // Check if there is a right child. If not, swap and finish.
                if(childRightIndex > _numNodes)
                {
                    pathNode.QueueIndex = childLeftIndex;
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    _nodes[childLeftIndex] = pathNode;
                    return;
                }
                // Check if the left-child is higher-priority than the right-child
                TItem childRight = _nodes[childRightIndex];
                if(HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            // Not swapping with left-child, does right-child exist?
            else if(childRightIndex > _numNodes)
            {
                return;
            }
            else
            {
                // Check if the right-child is higher-priority than the current pathNode
                TItem childRight = _nodes[childRightIndex];
                if(HasHigherPriority(childRight, pathNode))
                {
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                // Neither child is higher-priority than current, so finish and stop.
                else
                {
                    return;
                }
            }

            while(true)
            {
                childLeftIndex = 2 * finalQueueIndex;

                // If leaf pathNode, we're done
                if(childLeftIndex > _numNodes)
                {
                    pathNode.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = pathNode;
                    break;
                }

                // Check if the left-child is higher-priority than the current pathNode
                childRightIndex = childLeftIndex + 1;
                childLeft = _nodes[childLeftIndex];
                if(HasHigherPriority(childLeft, pathNode))
                {
                    // Check if there is a right child. If not, swap and finish.
                    if(childRightIndex > _numNodes)
                    {
                        pathNode.QueueIndex = childLeftIndex;
                        childLeft.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        _nodes[childLeftIndex] = pathNode;
                        break;
                    }
                    // Check if the left-child is higher-priority than the right-child
                    TItem childRight = _nodes[childRightIndex];
                    if(HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        childLeft.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        childRight.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if(childRightIndex > _numNodes)
                {
                    pathNode.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = pathNode;
                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current pathNode
                    TItem childRight = _nodes[childRightIndex];
                    if(HasHigherPriority(childRight, pathNode))
                    {
                        childRight.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        pathNode.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = pathNode;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherPriority(pathNode, pathNode) (ie. both arguments the same pathNode) will return false
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherPriority(TItem higher, TItem lower)
        {
            var cmp = _comparer(higher.Priority, lower.Priority);
            return (cmp < 0 || (cmp == 0 && higher.InsertionIndex < lower.InsertionIndex));
        }

        /// <summary>
        /// Removes the head of the queue (pathNode with minimum priority; ties are broken by order of insertion), and returns it.
        /// If queue is empty, result is undefined
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public TItem Dequeue()
        {
#if DEBUG
            if(_numNodes <= 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            }

            if(!IsValidQueue())
            {
                throw new InvalidOperationException("Queue has been corrupted (Did you update a pathNode priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same pathNode to two different queues?)");
            }
#endif

            TItem returnMe = _nodes[1];
            //If the pathNode is already the last pathNode, we can remove it immediately
            if(_numNodes == 1)
            {
                _nodes[1] = null;
                _numNodes = 0;
                return returnMe;
            }

            //Swap the pathNode with the last pathNode
            TItem formerLastNode = _nodes[_numNodes];
            _nodes[1] = formerLastNode;
            formerLastNode.QueueIndex = 1;
            _nodes[_numNodes] = null;
            _numNodes--;

            //Now bubble formerLastNode (which is no longer the last pathNode) down
            CascadeDown(formerLastNode);
            return returnMe;
        }

        /// <summary>
        /// Resize the queue so it can accept more nodes.  All currently enqueued nodes are remain.
        /// Attempting to decrease the queue size to a size too small to hold the existing nodes results in undefined behavior
        /// O(n)
        /// </summary>
        public void Resize(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            }

            if (maxNodes < _numNodes)
            {
                throw new InvalidOperationException("Called Resize(" + maxNodes + "), but current queue contains " + _numNodes + " nodes");
            }
#endif

            TItem[] newArray = new TItem[maxNodes + 1];
            int highestIndexToCopy = Math.Min(maxNodes, _numNodes);
            Array.Copy(_nodes, newArray, highestIndexToCopy + 1);
            _nodes = newArray;
        }

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// If the queue is empty, behavior is undefined.
        /// O(1)
        /// </summary>
        public TItem First
        {
            get
            {
#if DEBUG
                if(_numNodes <= 0)
                {
                    throw new InvalidOperationException("Cannot call .First on an empty queue");
                }
#endif

                return _nodes[1];
            }
        }

        /// <summary>
        /// This method must be called on a pathNode every time its priority changes while it is in the queue.  
        /// <b>Forgetting to call this method will result in a corrupted queue!</b>
        /// Calling this method on a pathNode not in the queue results in undefined behavior
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void UpdatePriority(TItem pathNode, TPriority priority)
        {
#if DEBUG
            if(pathNode == null)
            {
                throw new ArgumentNullException("pathNode");
            }
            if (pathNode.Queue != null && !Equals(pathNode.Queue))
            {
                throw new InvalidOperationException("pathNode.UpdatePriority was called on a pathNode from another queue");
            }
            if (!Contains(pathNode))
            {
                throw new InvalidOperationException("Cannot call UpdatePriority() on a pathNode which is not enqueued: " + pathNode);
            }
#endif

            pathNode.Priority = priority;
            OnNodeUpdated(pathNode);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void OnNodeUpdated(TItem pathNode)
        {
            //Bubble the updated pathNode up or down as appropriate
            int parentIndex = pathNode.QueueIndex >> 1;

            if(parentIndex > 0 && HasHigherPriority(pathNode, _nodes[parentIndex]))
            {
                CascadeUp(pathNode);
            }
            else
            {
                //Note that CascadeDown will be called if parentNode == pathNode (that is, pathNode is the root)
                CascadeDown(pathNode);
            }
        }

        /// <summary>
        /// Removes a pathNode from the queue.  The pathNode does not need to be the head of the queue.  
        /// If the pathNode is not in the queue, the result is undefined.  If unsure, check Contains() first
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Remove(TItem pathNode)
        {
#if DEBUG
            if(pathNode == null)
            {
                throw new ArgumentNullException("pathNode");
            }
            if (pathNode.Queue != null && !Equals(pathNode.Queue))
            {
                throw new InvalidOperationException("pathNode.Remove was called on a pathNode from another queue");
            }
            if (!Contains(pathNode))
            {
                throw new InvalidOperationException("Cannot call Remove() on a pathNode which is not enqueued: " + pathNode);
            }
#endif

            //If the pathNode is already the last pathNode, we can remove it immediately
            if(pathNode.QueueIndex == _numNodes)
            {
                _nodes[_numNodes] = null;
                _numNodes--;
                return;
            }

            //Swap the pathNode with the last pathNode
            TItem formerLastNode = _nodes[_numNodes];
            _nodes[pathNode.QueueIndex] = formerLastNode;
            formerLastNode.QueueIndex = pathNode.QueueIndex;
            _nodes[_numNodes] = null;
            _numNodes--;

            //Now bubble formerLastNode (which is no longer the last pathNode) up or down as appropriate
            OnNodeUpdated(formerLastNode);
        }

        /// <summary>
        /// By default, nodes that have been previously added to one queue cannot be added to another queue.
        /// If you need to do this, please call originalQueue.ResetNode(pathNode) before attempting to add it in the new queue
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void ResetNode(TItem pathNode)
        {
#if DEBUG
            if (pathNode == null)
            {
                throw new ArgumentNullException("pathNode");
            }
            if (pathNode.Queue != null && !Equals(pathNode.Queue))
            {
                throw new InvalidOperationException("pathNode.ResetNode was called on a pathNode from another queue");
            }
            if (Contains(pathNode))
            {
                throw new InvalidOperationException("pathNode.ResetNode was called on a pathNode that is still in the queue");
            }

            pathNode.Queue = null;
#endif

            pathNode.QueueIndex = 0;
        }


        public IEnumerator<TItem> GetEnumerator()
        {
#if NET_VERSION_4_5 // ArraySegment does not implement IEnumerable before 4.5
            IEnumerable<TItem> e = new ArraySegment<TItem>(_nodes, 1, _numNodes);
            return e.GetEnumerator();
#else
            for(int i = 1; i <= _numNodes; i++)
                yield return _nodes[i];
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// <b>Should not be called in production code.</b>
        /// Checks to make sure the queue is still in a valid state.  Used for testing/debugging the queue.
        /// </summary>
        public bool IsValidQueue()
        {
            for(int i = 1; i < _nodes.Length; i++)
            {
                if(_nodes[i] != null)
                {
                    int childLeftIndex = 2 * i;
                    if(childLeftIndex < _nodes.Length && _nodes[childLeftIndex] != null && HasHigherPriority(_nodes[childLeftIndex], _nodes[i]))
                        return false;

                    int childRightIndex = childLeftIndex + 1;
                    if(childRightIndex < _nodes.Length && _nodes[childRightIndex] != null && HasHigherPriority(_nodes[childRightIndex], _nodes[i]))
                        return false;
                }
            }
            return true;
        }
    }
}
