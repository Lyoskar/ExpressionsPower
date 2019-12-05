using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ExpressionsPower.Tests")]

namespace ExpressionsPower
{
    public static class PropertiesModifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="modifyFunc"></param>
        /// <returns></returns>
        public static IList<Action<TRoot>> ModifyProperties<TRoot, TValue>(Func<TValue, TValue> modifyFunc)
        {
            var modifiers = new List<Action<TRoot>>();
            var getters = GetPropertiesGetters<TValue>(typeof(TRoot));

            foreach (var getter in getters)
            {
                var modifyExp = Expression.Call(modifyFunc.Method, getter.Body);    // modifyFunc(obj.propery)
                var assigner = Expression.Assign(getter.Body, modifyExp);           // obj.property = modifyFunc(obj.propery)

                var modifier = Expression                                           // obj => { obj.property = modifyFunc(obj.propery) }
                    .Lambda<Action<TRoot>>(
                        assigner,
                        getter.Parameters.First());

                modifiers.Add(modifier.Compile());
            }

            return modifiers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentObjectParameter"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static LambdaExpression GetPropertyGetter(ParameterExpression parentObjectParameter, PropertyInfo property)
        {
            var getPropertyParameter = Expression.Property(parentObjectParameter, property);    // obj.property
            return Expression.Lambda(getPropertyParameter, parentObjectParameter);              // obj => obj.property
        }

        /// <summary>
        /// Get collection of lambda expressions that represent getters of properties of specified type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of desired properties.</typeparam>
        /// <param name="type">The type of parent object.</param>
        /// <returns>Collection of properties getters.</returns>
        private static IEnumerable<LambdaExpression> GetPropertiesGetters<TValue>(Type type)
        {
            var allProperties = type.GetProperties().ToList();
            var result = new List<LambdaExpression>();

            var originalTypeParameter = Expression.Parameter(type, type.Name);                          // obj

            foreach (var complexProperty in allProperties.Where(p => p.PropertyType != typeof(TValue) && !p.PropertyType.IsValueType))
            {
                var complexPropertyGetter = GetPropertyGetter(originalTypeParameter, complexProperty);  // obj => obj.complexProperty
                var childPropertiesGetters = GetPropertiesGetters<TValue>(complexProperty.PropertyType);

                foreach (var childPropertyGetter in childPropertiesGetters)
                {
                    // childPropertyGetter == (complexProperty => complexProperty.childProperty)
                    var lambda = LambdaExpressionsCombiner.Combine(complexPropertyGetter, childPropertyGetter); // obj => obj.complexProperty.childProperty
                    result.Add(lambda);
                }
            }

            // TODO: обход коллекций внутри объекта

            foreach (var valueProperty in allProperties.Where(p => p.PropertyType == typeof(TValue)))
            {
                var valuePropertyGetter = GetPropertyGetter(originalTypeParameter, valueProperty);  // obj => t.property
                result.Add(valuePropertyGetter);
            }

            return result;
        }
    }
}
