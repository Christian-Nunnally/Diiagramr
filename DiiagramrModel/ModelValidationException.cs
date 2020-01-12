namespace DiiagramrModel
{
    using System;

    [Serializable]
    public class ModelValidationException : InvalidOperationException
    {
        public ModelValidationException(ModelBase model, string solutionRecomendation)
            : base($"{model.ToString()} - {solutionRecomendation}")
        {
        }
    }
}