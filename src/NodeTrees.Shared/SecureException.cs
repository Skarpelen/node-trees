namespace NodeTrees.Shared
{
    public class SecureException : Exception
    {
        public string QueryParameters { get; set; }
        public string BodyParameters { get; set; }
        public virtual string PublicType { get; }

        public SecureException(string message, string queryParameters = "", string bodyParameters = "", string? publicType = null)
            : base(message)
        {
            QueryParameters = queryParameters;
            BodyParameters = bodyParameters;
            PublicType = string.IsNullOrWhiteSpace(publicType) ? "Secure" : publicType!;
        }
    }

    public class ValidationSecureException : SecureException
    {
        public override string PublicType => "Validation";

        public ValidationSecureException(string message)
            : base(message)
        {
        }
    }

    public class AuthSecureException : SecureException
    {
        public override string PublicType => "Auth";

        public AuthSecureException(string message)
            : base(message)
        {
        }
    }

    public class NotFoundSecureException : SecureException
    {
        public override string PublicType => "NotFound";

        public NotFoundSecureException(string message)
            : base(message)
        {
        }
    }

    public class ConflictSecureException : SecureException
    {
        public override string PublicType => "Conflict";

        public ConflictSecureException(string message)
            : base(message)
        {
        }
    }

    public class ForbiddenSecureException : SecureException
    {
        public override string PublicType => "Forbidden";

        public ForbiddenSecureException(string message)
            : base(message)
        {
        }
    }
}
