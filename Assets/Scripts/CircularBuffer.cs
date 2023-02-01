using System.Collections.Generic;

namespace Gameplay.Core.Camera
{
    public class CircularBuffer<T>
    {
        private LinkedList<T> _linked = null;
        private LinkedListNode<T> _node = null;

        public CircularBuffer(IEnumerable<T> enumerable)
        {
            _linked = new LinkedList<T>(enumerable);
            _node = _linked.First;
        }

        public T Value => _node.Value;

        public CircularBuffer<T> Next()
        {
            if(_linked.Count == 0) return null;
            if (_node == _linked.Last)
                _node = _linked.First;
            else
                _node = _node.Next;

            return this;
        }
    }
}