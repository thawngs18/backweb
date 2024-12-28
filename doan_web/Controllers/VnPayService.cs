using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using doan_web.Models;

namespace doan_web.Controllers
{
    public class VnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            // Cấu hình các giá trị cứng (hard-coded)
            string vnp_Version = "2.1.0";
            string vnp_Command = "pay";
            string orderType = "other";
            string bankCode = "NCB";  // Có thể thay đổi nếu cần

            string vnp_TmnCode = "0S7T01T8";  // TmnCode từ cấu hình
            string vnp_HashSecret = "BEZLUPOPOTXTDYZHCBGDJBHFJPBLSARL";  // Secret key
            string vnp_BaseUrl = "https://localhost:7028/Cart/PaymentCallback";  // URL callback
            string vnp_CurrCode = "VND";  // Mã tiền tệ
            string vnp_Locale = "vn";  // Ngôn ngữ
            string vnp_IpAddr = "127.0.0.1";  // Địa chỉ IP
            string vnp_ReturnUrl = "https://localhost:7028/Cart/PaymentCallback";  // URL callback khi trả về

            // Lấy thời gian hiện tại theo múi giờ Việt Nam
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();

            // Khởi tạo đối tượng VNPayLibrary (Giả sử bạn đã có lớp này tương tự như trong Java)
            var pay = new VnPayLibrary();

            // Tạo các tham số yêu cầu
            var vnp_Params = new Dictionary<string, string>
    {
        { "vnp_Version", vnp_Version },
        { "vnp_Command", vnp_Command },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", (100000 * 100).ToString() },  // Số tiền cần thanh toán, ví dụ: 100.000 VND
        { "vnp_CurrCode", vnp_CurrCode },
        { "vnp_BankCode", bankCode },
        { "vnp_TxnRef", tick },
        { "vnp_OrderInfo", $"Thanh toan don hang: {tick}" },
        { "vnp_OrderType", orderType },
        { "vnp_Locale", vnp_Locale },
        { "vnp_ReturnUrl", vnp_ReturnUrl },
        { "vnp_IpAddr", vnp_IpAddr },
    };

            // Tạo ngày tạo và ngày hết hạn
            string vnp_CreateDate = timeNow.ToString("yyyyMMddHHmmss");
            vnp_Params["vnp_CreateDate"] = vnp_CreateDate;

            // Thêm 15 phút cho ngày hết hạn
            var vnp_ExpireDate = timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss");
            vnp_Params["vnp_ExpireDate"] = vnp_ExpireDate;

            // Tạo chuỗi query và dữ liệu hash
            var sortedKeys = vnp_Params.Keys.OrderBy(k => k).ToList();
            StringBuilder hashData = new StringBuilder();
            StringBuilder query = new StringBuilder();

            foreach (var key in sortedKeys)
            {
                string value = vnp_Params[key];
                if (!string.IsNullOrEmpty(value))
                {
                    // Xây dựng chuỗi hashData
                    hashData.Append(key);
                    hashData.Append('=');
                    hashData.Append(UrlEncode(value));

                    // Xây dựng chuỗi query
                    query.Append(UrlEncode(key));
                    query.Append('=');
                    query.Append(UrlEncode(value));
                    query.Append('&');
                    hashData.Append('&');
                }
            }

            string queryUrl = query.ToString();
            string vnp_SecureHash = VnPayLibrary.HmacSha512(vnp_HashSecret, hashData.ToString());
            queryUrl += "&vnp_SecureHash=" + vnp_SecureHash;
            string paymentUrl = vnp_BaseUrl + "?" + queryUrl;

            Console.WriteLine(paymentUrl);

            return paymentUrl;
        }

        private string UrlEncode(string value)
        {
            return System.Web.HttpUtility.UrlEncode(value);
        }


        public PaymentResponseModel PaymentExecute(HttpRequest request)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(request, "BEZLUPOPOTXTDYZHCBGDJBHFJPBLSARL");

            return response;
        }
    }
}
