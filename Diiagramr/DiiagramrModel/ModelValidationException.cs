namespace DiiagramrModel
{
    using System;

    /// <summary>
    /// Execption thrown when a model object would have gotten in to a bad state.
    /// </summary>
    [Serializable]
    public class ModelValidationException : InvalidOperationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="ModelValidationException"/>.
        /// </summary>
        /// <param name="model">The model object throwing the exception.</param>
        /// <param name="solutionRecomendation">The recomended solution to fix the exception.</param>
        public ModelValidationException(ModelBase model, string solutionRecomendation)
            : base($"{model.ToString()} - {solutionRecomendation}")
        {
        }
    }
}