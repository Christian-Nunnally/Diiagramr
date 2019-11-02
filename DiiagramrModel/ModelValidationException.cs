namespace DiiagramrModel
{
    using System;

    public class ModelValidationException : InvalidOperationException
    {
        public ModelValidationException(ModelBase model, string solutionRecomendation)
            : base($"{model.ToString()} - {solutionRecomendation}")
        {
        }
    }
}
