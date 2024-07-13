using AuctionService.Entities;

namespace AuctionService.Extensions;

public static class AuctionStatusExtensions
{
    // Converts Status enum to its string representation
    public static string StatusToString(Status status)
    {
        return status.ToString();
    }

    // Converts a string representation to the Status enum
    // Returns Status.Live if the string does not match any Status enum value
    public static Status StringToStatus(string statusString)
    {
        if (Enum.TryParse<Status>(statusString, true, out var status))
        {
            return status;
        }
        return Status.Live;
    }
}
