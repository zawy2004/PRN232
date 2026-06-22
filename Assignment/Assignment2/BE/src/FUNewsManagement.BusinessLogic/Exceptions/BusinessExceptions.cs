namespace FUNewsManagement.BusinessLogic.Exceptions;

/// <summary>Thrown when an operation violates a data-integrity business rule (e.g. delete-in-use). Maps to HTTP 409.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

/// <summary>Thrown when an operation is not permitted for the current actor (e.g. edit-lock expired). Maps to HTTP 403.</summary>
public class ForbiddenOperationException : Exception
{
    public ForbiddenOperationException(string message) : base(message)
    {
    }
}

/// <summary>Thrown when the requested entity does not exist. Maps to HTTP 404.</summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }
}
