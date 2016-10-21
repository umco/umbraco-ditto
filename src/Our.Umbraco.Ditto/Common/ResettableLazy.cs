using Our.Umbraco.Ditto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Ditto
{
    /// <summary>Provides support for lazy initialization that can be reset</summary>
    /// <typeparam name="TValue">The type of object that is being lazily initialized.</typeparam>
    public abstract class ResettableLazy<TValue>
    {
        private Lazy<TValue> _lazyObj;
        private readonly Func<TValue> _valueFunc;

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{TModel}"/> class.</summary>
        /// <param name="valueFunc"></param>
        protected ResettableLazy(Func<TValue> valueFunc)
        {
            _valueFunc = valueFunc;
            Reset();
        }

        /// <summary>Resets the value, causing it to be repopulated on next access of <see cref="Value" /></summary>
        public void Reset() => _lazyObj = new Lazy<TValue>(_valueFunc);

        /// <summary>Gets the lazily initialized value of the current <see cref="T:System.Lazy`1" /> instance.</summary>
        /// <returns>The lazily initialized value of the current <see cref="T:System.Lazy`1" /> instance.</returns>
        public TValue Value => _lazyObj.Value;
    }
}
