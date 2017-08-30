using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using DE.Wirecard.Paymentsdk;
using DE.Wirecard.Paymentsdk.Models;
using Android.Support.V7.App;

namespace TestWireCardPayment
{
	[Activity(Label = "TestWireCardPayment", MainLauncher = true, Icon = "@mipmap/icon")]

    public class MainActivity : AppCompatActivity,IWirecardResponseListener
	{
        private const string WD_MERCHANT_ACCOUNT_ID = @"33f6d473-3036-4ca5-acb5-8c64dac862d1";
        private const string WD_MERCHANT_SECRET_KEY = @"9e0130f6-2e1e-4185-b0d5-dc69079c75cc";

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);
            button.Text = "BUY";

			button.Click += delegate { MakeTransaction(WirecardPaymentType.Card,WirecardTransactionType.Purchase); };
		}

        private void MakeTransaction(WirecardPaymentType wirecardPaymentType, WirecardTransactionType transactionType)
        {
            var wirecardClient = WirecardClientBuilder.NewInstance(this, WirecardEnvironment.Test.Value).Build();


            string merchantID = WD_MERCHANT_ACCOUNT_ID;
            string secrectKey = WD_MERCHANT_SECRET_KEY;
            string requestId = System.Guid.NewGuid().ToString();
            string time = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string amount = "10.01";
            Java.Math.BigDecimal amountJava = new Java.Math.BigDecimal(amount);
            string currency = "USD";
            string signature = GenerateSignatureV2(time,merchantID,requestId,WirecardTransactionType.Purchase.Value,amount,currency,secrectKey);
            WirecardCardPayment payment = new WirecardCardPayment(signature, time, requestId, merchantID,transactionType, amountJava,currency);
            payment.SetAttempt3d(Java.Lang.Boolean.True);

            wirecardClient.MakePayment(payment, null, this);

        }

        private string GenerateSignatureV2(string timestamp, string merchantID, string requestId, string transactionType, string amount, string currency,string secretKey)
        { 
            string signature = timestamp + requestId + merchantID + transactionType + amount + currency + secretKey;
            signature = Sha256_hash(signature);
            return signature;
        }

        //private WDPayment Merchant(string merchantId, WDPayment payment, string signature)
        //{
        //    payment.MerchantAccountID = merchantId;
        //    //[[NSUUID UUID] UUIDString];
        //    payment.RequestID = System.Guid.NewGuid().ToString();
        //    //payment.RequestID = "123merchant";
        //    //payment.Signature = signature;
        //    payment.RequestTimestamp = NSDate.Now;

        //    string date = ConvertToUtcDateTime(payment.RequestTimestamp).ToString("yyyyMMddHHmmss");

        //    string transactionType = (payment.TransactionType).ToString().ToLower();
        //    string currency = (payment.AmountCurrency).ToString().ToUpper();
        //    string secret = date + payment.RequestID + payment.MerchantAccountID + transactionType + payment.Amount.ToString() + currency + payment.IPAddress + signature;
        //    System.Diagnostics.Debug.WriteLine($"Secrect : {secret}");

        //    secret = Sha256_hash(secret);

        //    System.Diagnostics.Debug.WriteLine($"Secrect 2>>>> : {secret}");

        //    payment.RequestSignature = secret;
        //    return payment;
        //}

        public static String Sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")))
                  .Trim();

            }
        }

        public void OnError(WirecardResponseError p0)
        {
            //throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine($"Error >>> {p0.ErrorMessage}");
        }

        public void OnResponse(WirecardPaymentResponse p0)
        {
            //throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine($"Error >>> {p0}");
        }
    }
}

