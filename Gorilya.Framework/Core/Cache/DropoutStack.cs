using Gorilya.Framework.Core.Cache.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Gorilya.Framework.Core.Cache
{
    internal class DropoutStack<T>
    {
        private int maximumCapacity;
        private List<T> items;

        /// <summary>
        /// Creates a DropoutStack using the Default Maximum Capacity.
        /// </summary>
        public DropoutStack()
        {
            this.maximumCapacity = CacheConstants.Defaults.DropoutStackMaxCapacity;
            this.items = new List<T>();
        }

        /// <summary>
        /// Creates a DropoutStack.
        /// </summary>
        /// <param name="maximumCapacity">The maximum capacity of the DropoutStack.</param>
        public DropoutStack(int maximumCapacity)
        {
            this.maximumCapacity = maximumCapacity;
            this.items = new List<T>();
        }

        /// <summary>
        /// Creates a DropoutStack populated with the List of Items provided with the Default Maximum Capacity.
        /// </summary>
        /// <remarks>If the List contains more items than the Maximum Capacity allows,
        /// n number of the Lowest items will be removed from the List</remarks>
        /// <param name="items">List of Items to be added to the DropoutStack.</param>
        public DropoutStack(List<T> items)
        {
            this.maximumCapacity = CacheConstants.Defaults.DropoutStackMaxCapacity;
            this.items = items;

            if (items.Count > maximumCapacity)
            {
                var shortenedCapacity = items.Count - maximumCapacity;
                RemoveOldData(shortenedCapacity);
            }
        }

        /// <summary>
        /// Creates a DropoutStack populated with the List of Items and Maximum Capacity provided.
        /// </summary>
        /// <remarks>If the List contains more items than the Maximum Capacity allows,
        /// n number of the Lowest items will be removed from the List</remarks>
        /// <param name="items">List of Items to be added to the DropoutStack.</param>
        /// <param name="maximumCapacity">The maximum capacity of the DropoutStack.</param>
        public DropoutStack(List<T> items, int maximumCapacity)
        {
            this.maximumCapacity = maximumCapacity;
            this.items = items;

            if (items.Count > maximumCapacity)
            {
                var shortenedCapacity = items.Count - maximumCapacity;
                RemoveOldData(shortenedCapacity);
            }
        }

        /// <summary>
        /// Returns the Stack as a List of Items.
        /// </summary>
        public List<T> ToList
        {
            get
            {
                return items.ToList();
            }
        }

        /// <summary>
        /// Search the Stack and Retrieves the List of Items that matches the Search Predicate.
        /// </summary>
        /// <param name="searchPredicate">The LINQ Predicate to use when searching.</param>
        /// <returns>Returns the List of Items that Matches the Search Predicate.</returns>
        public List<T> GetListFromStack(Func<T, bool> searchPredicate)
        {
            var item = items.Where(searchPredicate).ToList();
            return item;
        }

        /// <summary>
        /// Search the Stack and Retrieves the First Item that matches the Search Predicate.
        /// </summary>
        /// <param name="searchPredicate">The LINQ Predicate to use when searching.</param>
        /// <returns>Returns the First Item that Matches the Search Predicate.</returns>
        public T GetItemFromStack(Func<T, bool> searchPredicate)
        {
            var item = items.Where(searchPredicate).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Removes an Item from the Stack that matches the Search Predicate.
        /// </summary>
        /// <param name="searchPredicate">The LINQ Predicate to use when searching.</param>
        public void RemoveItemFromStack(Func<T, bool> searchPredicate)
        {
            var itemToRemove = this.GetItemFromStack(searchPredicate);
            if (itemToRemove != null)
            {
                RemoveItemFromStack(itemToRemove);
            }
        }

        /// <summary>
        /// Removes an Item from the Stack that matches the Item.
        /// </summary>
        /// <param name="item">The item to be removed from the Stack</param>
        public void RemoveItemFromStack(T item)
        {
            items.Remove(item);
        }

        /// <summary>
        /// Updates the Maximum Capacity of the DropoutStack.
        /// </summary>
        /// <param name="newCapacity">The new Capacity of the DropoutStack.</param>
        public void UpdateMaximumCapacity(int newCapacity)
        {
            // if the new capacity desired is lesser than the existing capacity
            if (newCapacity < this.maximumCapacity)
            {
                // if the the number of items is more than the new capacity desired
                if (items.Count > newCapacity)
                {
                    // remove the old data to adjust to the new capacity
                    var shortenedCapacity = items.Count - newCapacity;
                    this.RemoveOldData(shortenedCapacity);
                }
            }

            maximumCapacity = newCapacity;
        }

        /// <summary>
        /// Pushes a new Item into the DropoutStack.
        /// </summary>
        /// <param name="item">The Item to add to the DropoutStack.</param>
        public void Push(T item)
        {
            // if pushing the new item will exceed the maximum capacity
            if ((items.Count + 1) > this.maximumCapacity)
            {
                this.RemoveOldData();
                items.Add(item);
            }
            else
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Retrieves and Removes the Top-Most Item in the DropoutStack.
        /// </summary>
        /// <returns>
        /// Returns the Top-Most (i.e. Last Item Added) Item.
        /// </returns>
        public T Pop()
        {
            // check if there are actual items to pop
            if (items.Count >=  1)
            {
                var item = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);

                return item;
            }

            return default(T);
        }

        /// <summary>
        /// Retrieves (without Removing) the Top-Most Item in the DropoutStack.
        /// </summary>
        /// <returns>
        /// Returns the Top-Most (i.e. Last Item Added) Item.
        /// </returns>
        public T Peek()
        {
            // check if there are actual items to pop
            if (items.Count >= 1)
            {
                var item = items[items.Count - 1];

                return item;
            }

            return default(T);
        }

        /// <summary>
        /// This method is mainly used when the DropoutStack has reached its Maximum Capacity 
        /// and needs to remove the oldest data to make space for newer ones.
        /// </summary>
        private void RemoveOldData()
        {
            RemoveOldData(1);
        }

        /// <summary>
        /// This method is mainly used when the DropoutStack has reached its Maximum Capacity 
        /// and needs to remove the n oldest data to make space for newer ones.
        /// </summary>
        /// <param name="noOfDataToRemove">How many old data needs to be removed.</param>
        private void RemoveOldData(int noOfDataToRemove)
        {
            for (int i = 0; i < noOfDataToRemove; i++)
            {
                items.RemoveAt(i);
            }
        }
    }
}
