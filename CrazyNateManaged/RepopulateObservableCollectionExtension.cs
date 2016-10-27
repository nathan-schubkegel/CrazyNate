using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CrazyNateManaged
{
  /// <summary>
  /// An extension method for repopulating an ObservableCollection from a list (that ideally contains
  /// mostly the same items) with minimal changes to the ObservableCollection.
  /// </summary>
  public static class RepopulateObservableCollectionExtension
  {
    /// <summary>
    /// Adds and removes items from the ObservableCollection until it matches the given list,
    /// with minimal changes to the ObservableCollection.
    /// </summary>
    /// <typeparam name="T">The type of the item in the collections.</typeparam>
    /// <param name="observableCollection">The collection to modify.</param>
    /// <param name="items">The collection that represents what the modified collection should
    /// look like when modifications are done.</param>
    /// <param name="matchItemsOrder">IF true, the order of 'items' is matched in 'observableCollection'.
    /// If false, new items are added to the end of 'observableCollection'.</param>
    public static void RepopulateFrom<T>(this IList<T> observableCollection, IList<T> items, bool matchItemsOrder)
    {
      // first count how many times each item is present in each collection
      Dictionary<T, int> oldItemCounts, newItemCounts;
      int oldNullCount, newNullCount;
      GetItemsDictionary(observableCollection, out oldItemCounts, out oldNullCount);
      GetItemsDictionary(items, out newItemCounts, out newNullCount);

      // remove items from 'observableCollection' that were removed from 'items'
      for (int index = observableCollection.Count - 1; index >= 0; index--)
      {
        T item = observableCollection[index];
        bool isNull = object.Equals(item, null);
        int oldCount = isNull ? oldNullCount : oldItemCounts[item];
        int newCount = isNull ? newNullCount : newItemCounts.GetValueOrDefault(item, 0);

        // if there are more of this item in 'observableCollection' than 'items',
        // then remove this instance of the item. (this may remove the item at the wrong location! 
        // But that will be corrected by reassignments at the end of the method. This algorithm
        // isn't efficient at handling duplicates, but it still arrives at a correct answer)
        if (oldCount > newCount)
        {
          observableCollection.RemoveAt(index);

          // update the item count
          oldCount--;
          if (isNull)
          {
            oldNullCount = oldCount;
          }
          else
          {
            oldItemCounts[item] = oldCount;
          }
        }
      }

      // the prior code is supposed to guarantee this
      if (observableCollection.Count > items.Count)
      {
        throw new InvalidOperationException("algorithm fail");
      }

      // insert and reassign items in 'observableCollection' so its contents match 'items'
      for (int index = 0; index < items.Count; index++)
      {
        T newItem = items[index];
        if (index >= observableCollection.Count)
        {
          observableCollection.Add(newItem);
        }
        else
        {
          T oldItem = observableCollection[index];
          if (!object.Equals(newItem, oldItem))
          {
            // if 'newItem' still needs to be added to the list, then insert it here.
            bool newIsNull = object.Equals(newItem, null);
            bool oldIsNull = object.Equals(oldItem, null);
            int oldCount = newIsNull ? oldNullCount : oldItemCounts[oldItem];
            int newCount = newIsNull ? newNullCount : newItemCounts[newItem];
            if (newCount > oldCount)
            {

            }
          }
        }
      }

      // Remove all things in 'old' not present in 'new'
      for (int oldIndex = observableCollection.Count - 1; oldIndex >= 0; oldIndex--)
      {
        T oldItem = observableCollection[oldIndex];
        int newCount;
        newItemCounts.TryGetValue(oldItem, out newCount);
        if ()
      }
      foreach (T oldItem in oldItemCounts.Keys)
      {
        int oldCount = oldItemCounts[oldItem];
        int newCount;
        newItemCounts.TryGetValue(oldItem, out newCount);

        for (int i = newCount; i < oldCount; i++)
        {
          observableCollection.Remove
        }
      }
    }

    /// <summary>
    /// Converts a list to a dictionary that indicates how many times each item is present in the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the item in the list.</typeparam>
    /// <param name="items">The list to convert.</param>
    /// <param name="dictionary">The resulting dictionary.</param>
    /// <param name="nulls">The count for 'null' entries (can't be represented in the dictionary).</param>
    private static void GetItemsDictionary<T>(IList<T> items, out Dictionary<T, int> dictionary, out int nulls)
    {
      nulls = 0;
      dictionary = new Dictionary<T, int>();

      foreach (T item in items)
      {
        if (object.Equals(item, null))
        {
          nulls++;
        }
        else
        {
          int count;
          dictionary[item] = dictionary.TryGetValue(item, out count) ? count : 1;
        }
      }
    }
  }

  public static class SomeExtension
  {
    public static TValue GetValueOrDefault<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
      TValue value;
      if (dictionary.TryGetValue(key, out value))
      {
        return value;
      }
      else
      {
        return defaultValue;
      }
    }
  }
}
