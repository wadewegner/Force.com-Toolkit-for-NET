//TODO: add license header
//TODO: refactor casing?

namespace Salesforce.Common.Models
{
    public enum Error
    {
        Unknown,
        InvalidClient,
        UnsupportedGrantType,
        InvalidGrant,
        AuthenticationFailure,
        InvalidPassword,
        ClientIdentifierInvalid,
        NotFound,
        MalFormedQuery,
        FieldCustomValidationException,
        InvalidFieldForInsertUpdate,
        InvalidClientId,
        InvalidField,
        RequiredFieldMissing,
        StringTooLong,
        EntityIsDeleted,
        MalFormedId,
        InvalidQueryFilterOperator
    }
    
}
