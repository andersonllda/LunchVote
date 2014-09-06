using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NHibernate.UserTypes;
using NHibernate;
using NHibernate.SqlTypes;

namespace HMV.Core.Framework.Types
{
    public class NullableString : IUserType
    {
        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var valueToGet = NHibernateUtil.String.NullSafeGet(rs, names[0]);
            return valueToGet ?? string.Empty;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var stringObject = value as string;
            object valueToSet = string.IsNullOrEmpty(stringObject) ? null : stringObject;
            NHibernateUtil.String.NullSafeSet(cmd, valueToSet, index);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return DeepCopy(cached);
        }

        public object Disassemble(object value)
        {
            return DeepCopy(value);
        }

        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { new SqlType(DbType.String) };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(string); }
        }

        public bool IsMutable
        {
            get { return false; }
        }
    }
}
