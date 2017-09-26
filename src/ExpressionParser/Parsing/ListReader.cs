using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser.Parsing
{
    /// <summary>
    /// A simple reader for an IList. 
    /// </summary>
    /// <typeparam name="TItem">The type of the item in the list.</typeparam>
    public class ListReader<TItem>
    {
        IList<TItem> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListReader{TItem}"/> class.
        /// </summary>
        /// <param name="list">The list to read.</param>
        /// <exception cref="System.ArgumentNullException">list</exception>
        public ListReader(IList<TItem> list)
        {
            if (list == null) { throw new ArgumentNullException("list"); }

            _list = list;
        }

        private int _position;
        /// <summary>
        /// Gets or sets the position of the reader. This is the 0-based index.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public int Position
        {
            get { return _position; }
            set
            {
                if (value < 0 || value > _list.Count)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _position = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the reader has reached the end of list.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is eol; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnd { get { return _position >= _list.Count; } }

        /// <summary>
        /// Reads the current item in the list and moves the <see cref="Position" /> forward.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public TItem Read()
        {
            return _list[Position++];
        }

        /// <summary>
        /// Peeks the current item in the list without moving the <see cref="Position"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public TItem Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// Peeks the item in the list without moving the <see cref="Position" />.
        /// </summary>
        /// <param name="offset">The offset from current position.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public TItem Peek(int offset)
        {
            // let list throw the exception.
            return _list[Position + offset];
        }
    }
}
