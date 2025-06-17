namespace ArpellaStores.Features.PaymentManagement.Models;
/// <summary>
/// Represents system response code
/// </summary>
public static class SystemResponseCode
{
    /// <summary>
    /// Successfull
    /// </summary>
    public static string Successfull = "00";

    /// <summary>
    /// Error Occured
    /// </summary>
    public static string ErrorOccured = "05";

    /// <summary>
    /// Invalid account exception
    /// </summary>
    public static string InvalidAccountException = "39";

    /// <summary>
    /// Invalid merchant exception
    /// </summary>
    public static string InvalidMerchantException = "51";

    /// <summary>
    /// Connection timeout exception
    /// </summary>
    public static string ConnectionTimeoutException = "91";
}
