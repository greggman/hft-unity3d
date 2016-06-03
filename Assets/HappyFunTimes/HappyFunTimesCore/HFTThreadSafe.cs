using System.Collections.Generic;
using System.Linq;

namespace HappyFunTimes {

    public class HFTThreadSafeList<T> {
        public void Add(T v)
        {
            lock(m_lock)
            {
                m_list.Add(v);
            }
        }

        public void Clear()
        {
            lock(m_lock)
            {
                m_list.Clear();
            }
        }

        public T[] ToArray()
        {
            lock(m_lock)
            {
                return m_list.ToArray();
            }
        }

        public int RemoveAll(System.Predicate<T> p)
        {
            lock(m_lock)
            {
                return m_list.RemoveAll(p);
            }
        }

        System.Object m_lock = new System.Object();
        List<T> m_list = new List<T>();
    }

    public class HFTThreadSafeDictionary<K, V> {
        public int Count
        {
            get
            {
                lock(m_lock)
                {
                    return m_dict.Count;
                }
            }
        }

        public V this[K key]
        {
            get
            {
                lock(m_lock)
                {
                    return m_dict[key];
                }
            }
            set
            {
                lock(m_lock)
                {
                    m_dict[key] = value;
                }
            }
        }

        public bool TryGetValue(K key, out V value)
        {
            lock(m_lock)
            {
                return m_dict.TryGetValue(key, out value);
            }
        }

        public bool Remove(K key)
        {
            lock(m_lock)
            {
                return m_dict.Remove(key);
            }
        }

        public V GetAnyValue()
        {
            IEnumerator<KeyValuePair<K, V>> it = m_dict.GetEnumerator();
            if (it.MoveNext())
            {
                return it.Current.Value;
            }
            else
            {
                return default(V);
            }
        }

        public KeyValuePair<K, V>[] GetAll()
        {
            return m_dict.ToList().ToArray();
        }

        public V[] Values
        {
            get
            {
                lock(m_lock)
                {
                    return m_dict.Values.ToArray();
                }
            }
        }

        public K[] Keys
        {
            get
            {
                lock(m_lock)
                {
                    return m_dict.Keys.ToArray();
                }
            }
        }

        System.Object m_lock = new System.Object();
        Dictionary<K, V> m_dict = new Dictionary<K, V>();
    }

}
