using System;
using System.Collections;
using System.Collections.Generic;

namespace XPlugin.Data.Json {
    public class JArray : JCollection, IList<JToken> {
        protected IList<JToken> _list;


        public static new JArray Parse(string json) {
            return JToken.Parse(json) as JArray;
        }

        public static JArray OptParse(string json) {
            var ret = JToken.OptParse(json) as JArray;
            if (ret == null) {
                ret = new JArray();
            }
            return ret;
        }

        public static JArray OptParse(string json, JArray def) {
            var ret = JToken.OptParse(json) as JArray;
            if (ret == null) {
                ret = def;
            }
            return ret;
        }

        public static explicit operator JArray(string json) {
            return Parse(json);
        }
        
        public static JArray From<T>(IEnumerable<T> list) where T : IToJson {
            JArray array = new JArray();
            foreach (T t in list) {
                array.Add(t.ToJson());
            }
            return array;
        }

        public static JArray From<T>(IEnumerable<T> list, Func<T, JToken> func) {
            JArray array = new JArray();
            foreach (T t in list) {
                array.Add(func(t));
            }
            return array;
        }

        public JArray()
            : base(JType.Array) {
            _list = new List<JToken>();
        }

        public JArray(IEnumerable list)
            : this() {
            AddRange(list);
        }

        public override int Count {
            get { return _list.Count; }
        }

        public bool IsReadOnly { get; }

        public override JToken this[string name] {
            get {
                int index;
                if (int.TryParse(name, out index)) {
                    return this[index];
                } else {
                    return base[name];
                }
            }
            set {
                int index;
                if (int.TryParse(name, out index)) {
                    this[index] = value;
                } else {
                    base[name] = value;
                }
            }
        }

        public override JToken this[int index] {
            get {
                if (index < 0 || index >= _list.Count) {
                    throw new JIndexOutOfRangeException(this, index);
                } else {
                    return _list[index];
                }
            }
            set {
                if (index < 0 || index >= _list.Count) {
                    throw new JIndexOutOfRangeException(this, index);
                } else {
                    if (value == null) {
                        value = new JNull();
                    }
                    _list[index] = value;
                }
            }
        }

        public JToken First {
            get {
                if (Count > 0) {
                    return this[0];
                } else {
                    return null;
                }
            }
        }

        public JToken Last {
            get {
                if (Count > 0) {
                    return this[Count - 1];
                } else {
                    return null;
                }
            }
        }

        public void Add(JToken item) {
            if (item == null) {
                item = new JNull();
            }
            _list.Add(item);
        }

        public void AddRange(IEnumerable list) {
            if (list != null) {
                foreach (object obj in list) {
                    Add(Create(obj));
                }
            }
        }

        public void Insert(int index, JToken item) {
            if (item == null) {
                item = new JNull();
            }
            _list.Insert(index, item);
        }

        public void CopyTo(JToken[] array, int arrayIndex) {
            this._list.CopyTo(array,arrayIndex);
        }

        public bool Remove(JToken item) {
            return _list.Remove(item);
        }

        public void RemoveAt(int index) {
            _list.RemoveAt(index);
        }

        public void Clear() {
            _list.Clear();
        }

        public int IndexOf(JToken item) {
            return _list.IndexOf(item);
        }

        public bool Contains(JToken item) {
            return _list.Contains(item);
        }


        public override IEnumerator<JToken> GetEnumerator() {
            return _list.GetEnumerator();
        }

        public T[] ToArray<T>(Func<JToken, T> func) {
            T[] array = new T[Count];
            if (func != null) {
                for (int i = 0; i < Count; i++) {
                    array[i] = func(this[i]);
                }
            }
            return array;
        }

        public List<T> ToList<T>(Func<JToken, T> func) {
            List<T> list = new List<T>(Count);
            if (func != null) {
                for (int i = 0; i < Count; i++) {
                    list.Add(func(this[i]));
                }
            }
            return list;
        }


        public override JArray AsArray() {
            return this;
        }

        public override JArray GetArray() {
            return this;
        }

        public override JArray OptArray(JArray def) {
            return this;
        }

        public override void Write(JsonWriter writer) {
            writer.WriteArrayStart();
            foreach (JToken token in this) {
                token.Write(writer);
            }
            writer.WriteArrayEnd();
        }
    }
}