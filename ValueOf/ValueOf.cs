﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ValueOf
{
    public class ValueOf<TValue, TThis> where TThis : ValueOf<TValue, TThis>, new()
    {
        private static readonly Func<TThis> Factory;

        /// <summary>
        /// WARNING - THIS FEATURE IS EXPERIMENTAL. I may change it to do
        /// validation in a different way.
        /// Right now, override this method, and throw any exceptionsn you need to.
        /// Access this.Value to check the value
        /// </summary>
        protected virtual void Validate()
        {
        }

        static ValueOf()
        {
            var ctor = typeof(TThis).GetTypeInfo().GetConstructor(new Type[0]);
            Expression[] argsExp = new Expression[0];
            NewExpression newExp = Expression.New(ctor, argsExp);
            LambdaExpression lambda = Expression.Lambda(typeof(Func<TThis>), newExp);
            Factory = (Func<TThis>)lambda.Compile();
        }

        public TValue Value { get; protected set; }

        public static TThis From(TValue item)
        {
            var x = Factory();
            x.Value = item;
            x.Validate();
            return x;
        }


        protected bool Equals(ValueOf<TValue, TThis> other)
        {
            return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueOf<TValue, TThis>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TValue>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
