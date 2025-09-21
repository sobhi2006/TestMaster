namespace TestMaster.Exceptions;

public class BusinessRuleException : Exception
{
    public int StatusCode { get; }
    public BusinessRuleException(string message, int StatusCode) : base(message)
    {
        this.StatusCode = StatusCode;
    }
}