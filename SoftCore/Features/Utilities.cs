using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features
{
    public static class Utilities
    {
        /*public static string GetMemberName(Expression<Func<object>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }*/
    }
}
