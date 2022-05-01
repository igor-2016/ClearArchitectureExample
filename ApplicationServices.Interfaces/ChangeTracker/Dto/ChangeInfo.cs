using Utils;

namespace ApplicationServices.Interfaces.ChangeTracker
{
    public class ChangeInfo<TSource, TTarget>
    { 
        public ChangeType ChangeType { get; }
        public TSource? Source { get;  }
        public TTarget? Target { get;  }

        protected ChangeInfo(ChangeType changeType, TSource? source, TTarget? target)
        {
            ChangeType = changeType;

            if(changeType == ChangeType.Add && source is null)
                throw new ArgumentException(nameof(source));
            if(changeType == ChangeType.Remove && target is null)
                throw new ArgumentException(nameof(target));
            if (changeType == ChangeType.Update && (target is null || source is null))
                throw target is null ? new ArgumentException(nameof(target)) : new ArgumentException(nameof(source));

            Source = source;
            Target = target;
        }

        public static ChangeInfo<TSource, TTarget> Add(TSource? source) => 
            new ChangeInfo<TSource, TTarget>(ChangeType.Add, source, default);

        public static ChangeInfo<TSource, TTarget> Remove(TTarget? target) =>
           new ChangeInfo<TSource, TTarget>(ChangeType.Remove, default, target);

        public static ChangeInfo<TSource, TTarget> Update(TSource? source, TTarget? target) =>
           new ChangeInfo<TSource, TTarget>(ChangeType.Update, source, target);

    }
}
