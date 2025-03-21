namespace APIs.Helpers
{
    public class PaypalCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Mode { get; set; }
        //public string BaseUrl => "https://sandbox.paypal.com";

        //public PaypalClient(string client, string secret, string mode)
        //{
        //    ClientId = client;
        //    ClientSecret = secret;
        //    Mode = mode;
        //}
    }
}
