using System;

namespace Eos.DataStructures
{
    /// <summary>
    /// Lightweight support for lazy initialization within a method that avoids the GC on LazySlim if not created.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    public struct LazySlim<T>
        where T : class
    {
        private readonly Func<T> _valueFactory;
        private T _value;
        private bool _isValueCreated;

        public LazySlim(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            _valueFactory = valueFactory;
            _value = default(T);
            _isValueCreated = false;
        }

        public T Value
        {
            get
            {
                return GetValue();
            }
        }

        public bool IsValueCreated
        {
            get
            {
                return _isValueCreated;
            }
        }

        private T GetValue()
        {
            return _value ?? CreateValue();
        }

        private T CreateValue()
        {
            if (_valueFactory != null)
            {
                try
                {
                    _value = _valueFactory();
                    _isValueCreated = true;

                    return _value;
                }
                catch (Exception ex)
                {
                    throw new LazySlimDynamicInitializationException(typeof(T), ex);
                }
            }

            try
            {
                _value = (T)Activator.CreateInstance(typeof(T));
                _isValueCreated = true;

                return _value;
            }
            catch (MissingMethodException ex)
            {
                throw new LazySlimDynamicInitializationException(typeof(T), ex);
            }
        }
    }
}
