namespace Ae.Engine.Interrogation
{
    /// <summary>
    /// Represents a criterion used to filter interrogation command parameters, supporting negation of the filter
    /// condition.
    /// </summary>
    /// <remarks>Use this class to define filter criteria for command parameters, including the ability to
    /// specify negated conditions by prefixing the filter text with an exclamation mark ('!'). The criterion can be
    /// used to match or exclude parameters based on the provided value.</remarks>
    internal class InterrogationCommandParameterCriterion
    {
        public bool IsNotCriteria { get; set; }
        public string Value { get; private set; }

        public InterrogationCommandParameterCriterion(string filterText)
        {
            if (filterText[0] == '!')
            {
                IsNotCriteria = true;
                filterText = filterText.Substring(1);
            }

            Value = filterText;
        }
    }
}
