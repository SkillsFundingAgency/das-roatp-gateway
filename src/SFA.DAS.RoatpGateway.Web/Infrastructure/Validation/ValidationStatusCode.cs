using System.Runtime.Serialization;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;

public enum ValidationStatusCode
{
    [EnumMember(Value = "BadRequest")]
    BadRequest,
    [EnumMember(Value = "AlreadyExists")]
    AlreadyExists,
    [EnumMember(Value = "NotFound")]
    NotFound
}
