using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.Printing;

public sealed record PrintJobCreateRequest(
    EPrintDestination Destination,
    string PayloadType,
    string PayloadJson
);

