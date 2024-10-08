﻿namespace Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Merge<TK, TV>(this IDictionary<TK, TV> target, IDictionary<TK, TV> source, bool overwrite = false)
        {
            source.ToList().ForEach(_ => {
                if ((!target.ContainsKey(_.Key)) || overwrite)
                    target[_.Key] = _.Value;
            });
        }
    }
}