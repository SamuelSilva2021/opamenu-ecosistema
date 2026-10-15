using Opamenu.Desktop.Models.Enums;

namespace Opamenu.Desktop.Models.Printing;

public sealed record PrintJobCreateRequest(
    EPrintDestination Destination,
    string PayloadType,
    string PayloadJson
);

