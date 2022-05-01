using System.Reflection;

namespace Utils
{

    public enum ChangeType
    {
        Unknown = 0,
        Add = 1,
        Remove = 2,
        Update = 3
    }

    public delegate Task OnPropertyNavigate<in TTarget, in TSource>(TSource source, 
        PropertyInfo sourcePropertyInfo, object? sourceValue,
        TTarget target, PropertyInfo targetPropertyInfo, object? targetValue, CancellationToken cancellationToken);


    //public delegate void OnPropertyNavigate<in TTarget, in TTargetProperty, in TSource, in TSourceProperty>
    //    (TSource source, TSourceProperty? sourceValue, TTarget target, TTargetProperty? targetValue)

    public delegate Task OnPropertyChanged<in TTarget, in TSource>(
        TSource source, PropertyInfo sourcePropertyInfo, object? sourceValue,
        TTarget target, PropertyInfo targetPropertyInfo, object? targetValue, CancellationToken cancellationToken);

    public delegate Task OnNamedPropertyChanged<in TTarget, in TSource>(string propertyName,
       TSource source, object? sourceValue,
       TTarget target, object? targetValue, CancellationToken cancellationToken);

    public delegate Task<bool> IsChanged<in TTarget, in TSource>(
        TSource source, PropertyInfo sourcePropertyInfo, object? sourceValue,
        TTarget target, PropertyInfo targetPropertyInfo, object? targetValue, CancellationToken cancellationToken);

    public delegate Task<TSource?> SearchInSourcesByCurrentTarget<TSource, in TTarget>(
        IEnumerable<TSource> sources, TTarget target, CancellationToken cancellationToken);

    public delegate Task OnEntityChanged<in TSource, in TTarget>(ChangeType changeType, TSource? source, TTarget? target,
        CancellationToken cancellationToken);

    public static class ChangeTrackerExtensions
    {

        public static async Task NavigateEntityList<TSource, TTarget>(this IEnumerable<TSource> sources,
            IEnumerable<TTarget> targets, SearchInSourcesByCurrentTarget<TSource, TTarget> searchInSourcesByCurrentTarget,
            OnEntityChanged<TSource, TTarget> onEntityChanged, CancellationToken cancellationToken)
        {
            if (searchInSourcesByCurrentTarget is null)
            {
                throw new ArgumentNullException(nameof(searchInSourcesByCurrentTarget));
            }

            if (onEntityChanged is null)
            {
                throw new ArgumentNullException(nameof(onEntityChanged));
            }

            var restSources = new List<TSource>(sources);
            foreach (var target in targets)
            {
                var foundSource = await searchInSourcesByCurrentTarget(restSources, target, cancellationToken);
                if (foundSource is null)
                {
                    await onEntityChanged(ChangeType.Remove, default, target, cancellationToken);
                }
                else
                {
                    await onEntityChanged(ChangeType.Update, foundSource, target, cancellationToken);
                }
                restSources.Remove(foundSource);
            }

            if (restSources.Any())
            {
                foreach (var newSource in restSources)
                {
                    await onEntityChanged(ChangeType.Add, newSource, default, cancellationToken);
                }
            }
        }

        public static async Task NavigatePropertiesAsync<TTarget, TSource>(this TTarget target, TSource source,
            CancellationToken cancellationToken,
            OnNamedPropertyChanged<TTarget, TSource>? onNamedPropertyChanged = null,
            OnPropertyChanged<TTarget, TSource>? onPropertyChanged = null,
            IsChanged<TTarget, TSource>? isChanged = null,
            OnPropertyNavigate<TTarget, TSource>? onNavigateProperty = null)
        {
            Type targetType = typeof(TTarget);
            Type sourceType = typeof(TSource); //is base type
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            foreach (PropertyInfo sourcePropertyInfo in sourceProperties)
            {
                // Skip unreadable ones
                if (!sourcePropertyInfo.CanRead) continue;
                
                //Skip complex fields
                if (!IsSimple(sourcePropertyInfo.PropertyType.GetTypeInfo())) continue;

                // Get Property of target class
                PropertyInfo? targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name);
                
                //Skip property, if it's not exists
                if (targetPropertyInfo == null) continue;
                
                // Skip writeprotected ones
                if (!targetPropertyInfo.CanWrite) continue;
                
                // Read source value
                object? sourceValue = sourcePropertyInfo.GetValue(source, null);
                
                // Read target value
                object? targetValue = targetPropertyInfo.GetValue(target, null);

                // detect is changed
                bool changed; 
                if(isChanged != null)
                {
                    changed = await isChanged(source, sourcePropertyInfo, sourceValue,
                        target, targetPropertyInfo, targetValue, cancellationToken);
                }
                else
                {
                    if (sourceValue == default && targetValue == default)
                        changed = false;
                    else if (targetValue == default)
                        changed = !(sourceValue.Equals(targetValue));
                    else
                        changed = !(targetValue.Equals(sourceValue));
                    //changed = !(sourceValue?.Equals(targetValue) ?? (targetValue != default))
                }

                // notify about all
                if (onNavigateProperty != null)
                {
                    await onNavigateProperty(source, sourcePropertyInfo, sourceValue,  
                        target, targetPropertyInfo, targetValue, cancellationToken);
                }

                if(onNamedPropertyChanged != null && changed)
                {
                    await onNamedPropertyChanged(sourcePropertyInfo.Name, source, sourceValue,
                        target, targetValue, cancellationToken);
                }

                // notify changed only
                if(onPropertyChanged != null && changed)
                {
                    await onPropertyChanged(source, sourcePropertyInfo, sourceValue,
                        target, targetPropertyInfo, targetValue, cancellationToken);
                }

                // memo
                // targetPropertyInfo.SetValue(target, sourceValue, null)
            }
        }

       
        
        private static bool IsSimple(TypeInfo type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //, out bool? isNullableisNullable = true;
                // nullable type, check if the nested type is simple.
                return IsSimple((type.GetGenericArguments()[0]).GetTypeInfo());
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal))
              || type.Equals(typeof(DateTime))
              || type.Equals(typeof(Guid))
              || type.Equals(typeof(DateTimeOffset))
              || type.Equals(typeof(TimeSpan))
              || type.Equals(typeof(byte[]))
              ;
        }
    }
}
