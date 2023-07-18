using System;

namespace ModelValidation.Exceptions
{
    /// <summary>
    /// This exception is thrown when no model validator is found for the specified type.
    /// </summary>
    [System.Serializable]
    public class MissingModelValidatorException : Exception
    {
        public MissingModelValidatorException() { }

        public MissingModelValidatorException(string message) : base(message) { }

        public MissingModelValidatorException(string message, System.Exception inner) : base(message, inner) { }

        protected MissingModelValidatorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}