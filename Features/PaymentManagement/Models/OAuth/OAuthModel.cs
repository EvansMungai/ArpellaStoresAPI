using System;
using System.Net.WebSockets;
using System.Text;

namespace ArpellaStores.Features.PaymentManagement.Models;

/// <summary>
/// Represents OAuth model
/// </summary>
public class OAuthModel
{
    /// <summary>
    /// Gets or sets the consumer key
    /// </summary>
    public string ConsumerKey { get; set; }

    /// <summary>
    /// Gets or sets the consumer secret
    /// </summary>
    public string ConsumerSecret { get; set; }

    public string EncodedKeySecret
    {
        get
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.ConsumerKey + ":" + this.ConsumerSecret));
        }
    }
}
