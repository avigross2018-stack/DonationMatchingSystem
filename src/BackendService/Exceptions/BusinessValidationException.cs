namespace BackendService.Exceptions
{
    public sealed class BusinessValidationException : Exception
    {
        public BusinessValidationException(string message)
            : base(message)
        {
        }
    }
}