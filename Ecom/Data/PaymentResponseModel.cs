namespace Ecom.Data
{
    public class PaymentResponseModel
    {
        public string txnid { get; set; }
        public string amount { get; set; }
        public string productinfo { get; set; }
        public string firstname { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string hash { get; set; }
        public string error_Message { get; set; }
        public string mode { get; set; }
        public string mihpayid { get; set; }
        public string addedon { get; set; }
        public string bank_ref_num { get; set; }
        // Other fields you might need
    }

}
