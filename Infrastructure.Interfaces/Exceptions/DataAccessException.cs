using Entities;
using System.Runtime.Serialization;
using Utils;
using Utils.Attributes;
using Utils.Consts;

namespace DataAccess.Interfaces
{
    [Serializable]
    public class DataAccessException : DescribedException
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.DataAccess;

        public DataAccessException(DbErrors error) :
            base((int)error, error.GetResponseMessageFromEnum())
        {
        }

        public DataAccessException(DbErrors error, ExceptionFormatArgs args) :
            base((int)error, string.Format(error.GetResponseMessageFromEnum(), args.ToArgs()))
        {
        }

        public DataAccessException(DbErrors error, Exception ex) :
           base((int)error, error.GetResponseMessageFromEnum(), ex)
        {
        }

        public DataAccessException(DbErrors error, Exception ex, ExceptionFormatArgs args) :
           base((int)error, string.Format(error.GetResponseMessageFromEnum(), args.ToArgs()), ex)
        {
        }

        public DataAccessException(DbErrors error, string description, Exception ex) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, ex)
        {
        }

        public DataAccessException(DbErrors error, string description, ExceptionFormatArgs args) : 
            base((int)error, string.Format(error.GetResponseMessageFromEnum(), args.ToArgs()), description)
        {
        }


        protected DataAccessException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {

        }
    }

    #region exception format

    public abstract class ExceptionFormatArgs
    {
        public abstract object?[] ToArgs();
    }
    public class ExceptionFormatDataEntityType : ExceptionFormatArgs
    {
        public Type EntityType { get; set; }

        public override object?[] ToArgs()
        {
            return new object?[] { EntityType.FullName };
        }
    }

    public class ExceptionFormatDataEntityTypeWithSpecification : ExceptionFormatDataEntityType
    {
        public Type? SpecificationType { get; set; }

        public override object?[] ToArgs()
        {
            return new object?[] { EntityType.FullName, SpecificationType?.FullName };
        }
    }

    public class ExceptionFormatDataEntityKey<TId> : ExceptionFormatDataEntityType
    {
        public TId EntityKey { get; set; }

        public override object?[] ToArgs()
        {
            return new object?[] { EntityType.FullName, EntityKey };
        }
    }

    public class ExceptionFormatDataEntityVersionInfo<TId> : ExceptionFormatDataEntityKey<TId>
    {
        public string CurrentDbVersion { get; set; }
        public string TryUpdateToVersion { get; set; }

        public override object?[] ToArgs()
        {
            return new object?[] { EntityType.FullName, EntityKey, CurrentDbVersion, TryUpdateToVersion };
        }
    }

    #endregion exception format

}
