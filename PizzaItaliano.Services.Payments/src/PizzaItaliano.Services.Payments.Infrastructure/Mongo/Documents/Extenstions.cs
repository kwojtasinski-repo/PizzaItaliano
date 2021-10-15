using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents
{
    internal static class Extenstions
    {
        public static PaymentDocument AsDocument(this Payment payment)
        {
            var document = new PaymentDocument
            {
                Id = payment.Id,
                Cost = payment.Cost,
                Number = payment.Number,
                CreateDate = payment.CreateDate,
                ModifiedDate = payment.ModifiedDate,
                OrderId = payment.OrderId,
                PaymentStatus = payment.PaymentStatus
            };

            return document;
        }

        public static Payment AsEntity(this PaymentDocument paymentDocument)
        {
            var payment = new Payment(paymentDocument.Id, paymentDocument.Number, paymentDocument.Cost, paymentDocument.OrderId, 
                paymentDocument.CreateDate, paymentDocument.ModifiedDate, paymentDocument.PaymentStatus);
            return payment;
        }

        public static PaymentDto AsDto(this PaymentDocument paymentDocument)
        {
            var paymentDto = new PaymentDto
            {
                Id = paymentDocument.Id,
                Cost = paymentDocument.Cost,
                Number = paymentDocument.Number,
                OrderId = paymentDocument.OrderId,
                CreateDate = paymentDocument.CreateDate,
                ModifiedDate = paymentDocument.ModifiedDate,
                PaymentStatus = paymentDocument.PaymentStatus
            };

            return paymentDto;
        }

        public static IQueryable<TDest> Map<TSource, TDest>(this IQueryable<TSource> source)
        {
            List<TDest> destinationList = new List<TDest>();
            List<TSource> sourceList = source.ToList<TSource>();

            var sourceType = typeof(TSource);
            var destType = typeof(TDest);
            foreach (TSource sourceElement in sourceList)
            {
                TDest destElement = Activator.CreateInstance<TDest>();
                //Get all properties from the object 
                PropertyInfo[] sourceProperties = typeof(TSource).GetProperties();
                foreach (PropertyInfo sourceProperty in sourceProperties)
                {
                    //and assign value to each propery according to property name.
                    PropertyInfo destProperty = destType.GetProperty(sourceProperty.Name);
                    var descriptor = TypeDescriptor.GetProperties(destElement)[sourceProperty.Name];
                    var converter = descriptor.Converter;
                    if (converter.ToString().Contains("Custom")) // custom converters (np AggregateId w Root patrz adnotacja) konwencja z dopisaniem Custom
                    {
                        var value = sourceProperty.GetValue(sourceElement, null);
                        destProperty.SetValue(destElement, converter.ConvertFrom(value), null);
                    }
                    else
                    {
                        destProperty.SetValue(destElement, sourceProperty.GetValue(sourceElement, null), null);
                    }
                }
                destinationList.Add(destElement);
            }

            return destinationList.AsQueryable();
        }

        public static Expression<Func<TTo, bool>> Convert<TFrom, TTo>(this Expression<Func<TFrom, bool>> expr)
        {
            Dictionary<Expression, Expression> substitutues = new Dictionary<Expression, Expression>();
            var oldParam = expr.Parameters[0];
            var newParam = Expression.Parameter(typeof(TTo), oldParam.Name);
            substitutues.Add(oldParam, newParam);
            Expression body = ConvertNode(expr.Body, substitutues);
            return Expression.Lambda<Func<TTo, bool>>(body, newParam);
        }

        private static Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst)
        {
            if (node == null) return null;
            if (subst.ContainsKey(node)) return subst[node];

            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    return node;
                case ExpressionType.MemberAccess:
                    {
                        var me = (MemberExpression)node;
                        var newNode = ConvertNode(me.Expression, subst);
                        return Expression.MakeMemberAccess(newNode, newNode.Type.GetMember(me.Member.Name).Single());
                    }
                case ExpressionType.Equal: /* will probably work for a range of common binary-expressions */
                    {
                        var be = (BinaryExpression)node;
                        return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                    }
                case ExpressionType.LessThan:
                    {
                        var be = (BinaryExpression)node;
                        return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                    }
                case ExpressionType.GreaterThan:
                    {
                        var be = (BinaryExpression)node;
                        return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                    }
                default:
                    throw new NotSupportedException(node.NodeType.ToString());
            }
        }
    }
}
