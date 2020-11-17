using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor.Interactors.TextTree
{
    /// <summary>
    /// A node of a tree that allows for quickly searching for values with string keys.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FastTextSearchNode<T>
    {
        private readonly Dictionary<char, List<T>> _items = new Dictionary<char, List<T>>();
        private readonly Dictionary<char, FastTextSearchNode<T>> _nodes = new Dictionary<char, FastTextSearchNode<T>>();

        /// <summary>
        /// Add an item to this node of the tree.
        /// </summary>
        /// <param name="key">The key of the new item.</param>
        /// <param name="item">The item to add.</param>
        public void Add(string key, T item)
        {
            if (key.Length == 0)
            {
                return;
            }
            Add(new ArraySegment<char>(key.ToCharArray()), item);
        }

        /// <summary>
        /// Gets all values corresponding to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The list of values that match the given key.</returns>
        public IEnumerable<T> GetMatches(string key)
        {
            if (key.Length == 0)
            {
                return GetAll();
            }
            return Get(new ArraySegment<char>(key.ToCharArray()));
        }

        /// <summary>
        /// Get all values in this node.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return _items.Values
                .SelectMany(v => v)
                .Concat(_nodes.Values.SelectMany(n => n.GetAll()));
        }

        private void Add(ArraySegment<char> key, T item)
        {
            if (key.Count == 1)
            {
                Add(key[0], item);
            }
            else
            {
                var node = GetNode(key[0]);
                node.Add(IncrementArraySegment(key), item);
            }
        }

        private ArraySegment<char> IncrementArraySegment(ArraySegment<char> arraySegment) =>
            new ArraySegment<char>(arraySegment.Array, arraySegment.Offset + 1, arraySegment.Count - 1);

        private IEnumerable<T> Get(ArraySegment<char> key)
        {
            if (key.Count == 1)
            {
                return Get(key[0]);
            }
            var node = GetNode(key[0]);
            return node.Get(IncrementArraySegment(key));
        }

        private FastTextSearchNode<T> GetNode(char key)
        {
            if (_nodes.TryGetValue(key, out var node))
            {
                return node;
            }
            var newNode = new FastTextSearchNode<T>();
            _nodes.Add(key, newNode);
            return newNode;
        }

        private void Add(char key, T item)
        {
            if (_items.TryGetValue(key, out var items))
            {
                items.Add(item);
            }
            else
            {
                _items.Add(key, new List<T> { item });
            }
        }

        private IEnumerable<T> Get(char key)
        {
            if (!_items.ContainsKey(key))
            {
                _items.Add(key, new List<T>());
            }
            return _items[key].Concat(GetNode(key).GetAll());
        }
    }
}