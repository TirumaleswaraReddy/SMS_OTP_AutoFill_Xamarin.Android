using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Telephony;

namespace SMS_OTP_AutoFill.Droid
{
	[BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
	[IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
	public class SMSBroadcastReceiver : BroadcastReceiver
	{
		/// <summary>
		///  Broadcast Action: A new data based Short Message Service message has been received by the device.
		/// </summary>
		private const string IntentAction = "android.provider.Telephony.SMS_RECEIVED";
		public string verificationCode;
		/// <summary>
		/// This is a BroadcastReceiver which receives the intent-filtered Short Message Service messages. onReceive, 
		/// we extract the Short Message Service message bundle and show a toast message and also update the UI by adding the SMS message to the SMS inbox list. 
		/// When a SMS message arrives, the inbox is automatically refreshed.
		/// </summary>
		/// <param name="context">Context The current context of the People application</param>
		/// <param name="intent">Intent containing the Short Message Service data</param>
		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				if (intent.Action != IntentAction)
				{
					return;
				}
				var bundle = intent.Extras;
				if (bundle == null)
				{
					return;
				}
				var pdus = bundle.Get("pdus");
				var castedPdus = JNIEnv.GetArray<Java.Lang.Object>(pdus.Handle);
				var msgs = new SmsMessage[castedPdus.Length];
				for (var i = 0; i < msgs.Length; i++)
				{
					var bytes = new byte[JNIEnv.GetArrayLength(castedPdus[i].Handle)];
					JNIEnv.CopyArray(castedPdus[i].Handle, bytes);
					msgs[i] = SmsMessage.CreateFromPdu(bytes);
					if (null != msgs[i].DisplayMessageBody && msgs[i].DisplayMessageBody.StartsWith("SMS otp is:"))
					{
						verificationCode = msgs[i].DisplayMessageBody.Split(':')[1].Substring(0, 8).Replace("-", "");
						Intent otpIntent = new Intent(Application.Context, typeof(MainActivity));
						otpIntent.PutExtra("verificationCode", verificationCode.Trim());
						otpIntent.PutExtra("fromsms", "OK");
						otpIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);
						context.StartActivity(otpIntent);
					}
				}
			}
			catch (Exception ex)
			{
				//Log.Error(Constants.APPNAME, ex.Message);
			}
		}
	}
}
