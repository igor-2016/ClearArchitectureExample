namespace ApplicationServices.Interfaces.ChangeTracker
{
    public class ChangePropertyInfo<TSource, TTarget>
    { 
        public TSource Source { get;  }
        public TTarget Target { get;  }

        public string PropertyName { get; }
        public object? SourceValue { get; }
        public object? TargetValue { get; }

        public ChangePropertyInfo(string propertyName, TSource source, object? sourceValue,  TTarget target, object? targetValue )
        {
            PropertyName = propertyName;
            Source = source;
            SourceValue = sourceValue;
            Target = target;
            TargetValue = targetValue;
        }

        public virtual TSourceProperty GetSourceValue<TSourceProperty>()
        { 
            if(SourceValue is TSourceProperty value)
            {
                return value;
            }
            throw new InvalidCastException($"Cannot cast source property {PropertyName} to type {typeof(TSourceProperty)}");
        }
        public virtual TTargetProperty GetTargetValue<TTargetProperty>()
        {
            if (TargetValue is TTargetProperty value)
            {
                return value;
            }
            throw new InvalidCastException($"Cannot cast target property {PropertyName} to type {typeof(TTargetProperty)}");
        }

    }
}
