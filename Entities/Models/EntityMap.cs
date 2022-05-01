using Entities.Models.Expansion;
using System.Collections.Concurrent;

namespace Entities.Models
{

    public class EntityMap<TId, TFrom, TTo> : Entity<TId>
    {
        protected ConcurrentDictionary<TFrom, TTo> CachedFromTo = new ConcurrentDictionary<TFrom, TTo>();
        protected ConcurrentDictionary<TTo, TFrom> CachedToFrom = new ConcurrentDictionary<TTo, TFrom>();

        protected virtual TFrom FromId { get; set; }

        protected virtual TTo ToId { get; set; }

        public virtual bool LoadFromTo(TFrom from, TTo to)
        {
            return CachedFromTo.TryAdd(from, to);
        }

        public virtual bool LoadToFrom(TTo to, TFrom from)
        {
            return CachedToFrom.TryAdd(to, from);
        }

        public virtual TTo GetFrom(TFrom fromId, TTo defaultTo = default)
        {
            if(CachedFromTo.TryGetValue(fromId, out var to))
            {
                return to;  
            }
            return defaultTo;
        }

        public virtual TFrom GetTo(TTo toId, TFrom defaultFrom = default)
        {
            if (CachedToFrom.TryGetValue(toId, out var from))
            {
                return from;
            }
            return defaultFrom;
        }

        public virtual void ClearAll()
        {
            CachedToFrom.Clear();   
            CachedFromTo.Clear();   
        }
    }
    
}
