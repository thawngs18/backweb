using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using doan_web.Models;

public class VnPayLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    public PaymentResponseModel GetFullResponseData(HttpRequest request, string hashSecret)
    {
        var vnPay = new VnPayLibrary();

        foreach (var key in request.Query.Keys)
        {
            var value = request.Query[key];
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnPay.AddResponseData(key, value);
            }
            Console.WriteLine(key + " " + value);
        }

        var orderId = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef"));
        var vnPayTranId = vnPay.GetResponseData("vnp_TransactionNo");
        var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
        var vnpSecureHash = request.Query["vnp_SecureHash"];
        var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");

        var checkSignature = vnPay.ValidateSignature(vnpSecureHash, hashSecret);

        if (!checkSignature)
            return new PaymentResponseModel()
            {
                Success = false
            };

        return new PaymentResponseModel()
        {
            Success = vnpResponseCode.Equals("00"),
            PaymentMethod = "VnPay",
            OrderDescription = orderInfo,
            OrderId = orderId.ToString(),
            PaymentId = vnPayTranId.ToString(),
            TransactionId = vnPayTranId.ToString(),
            Token = vnpSecureHash,
            VnPayResponseCode = vnpResponseCode
        };
    }

    public string GetIpAddress(HttpContext context)
    {
        var ipAddress = string.Empty;

        try
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress.ToString();

            if (remoteIpAddress != null && remoteIpAddress.Contains(":"))
            {
                remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                    .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            }

            if (remoteIpAddress != null)
            {
                ipAddress = remoteIpAddress;
            }
            else
            {
                ipAddress = "127.0.0.1";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return ipAddress;
    }
    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _responseData.Add(key, value);
        }
    }

    public string GetResponseData(string key)
    {
        return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
    }

    public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
    {
        var data = new StringBuilder();

        foreach (var kv in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            string key = kv.Key;
            string value = kv.Value;

            data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
        }

        var querystring = data.ToString();

        baseUrl += "?" + querystring;
        var signData = querystring;

        if (signData.Length > 0)
        {
            signData = signData.Remove(signData.Length - 1, 1);
        }

        var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
        baseUrl += "vnp_SecureHash=" + vnpSecureHash;

        return baseUrl;
    }


    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var rspRaw = GetResponseData();
        var myChecksum = HmacSha512(secretKey, rspRaw);
        return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string HmacSha512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }

    private string GetResponseData()
    {
        var data = new StringBuilder();

        if (_responseData.ContainsKey("vnp_SecureHashType"))
        {
            _responseData.Remove("vnp_SecureHashType");
        }

        if (_responseData.ContainsKey("vnp_SecureHash"))
        {
            _responseData.Remove("vnp_SecureHash");
        }

        foreach (var kv in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            string key = kv.Key;
            string value = kv.Value;

            data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
        }

        if (data.Length > 0)
        {
            data.Remove(data.Length - 1, 1);
        }

        return data.ToString();
    }

}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}
