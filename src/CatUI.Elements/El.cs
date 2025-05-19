using System;
using CatUI.Data;

namespace CatUI.Elements
{
    public static class El
    {
        /// <summary>
        /// Creates a mechanism that calls the given function whenever one of the given dependencies changes.
        /// </summary>
        /// <param name="function">
        /// A function that has as an argument an array of all the given dependencies but which have their values
        /// modified (because that called the function in the first place).
        /// </param>
        /// <param name="dependencies">
        /// A variable argument list of <see cref="ObservableProperty{T}"/> that will call the given function whenever
        /// their value changes.
        /// </param>
        public static void ReactiveFn(
            Action<ObservableProperty<object>[]> function,
            params ObservableProperty<object>[] dependencies)
        {
            foreach (ObservableProperty<object> dependency in dependencies)
            {
                dependency.ValueChangedEvent += _ => function(dependencies);
            }
        }
    }
}
