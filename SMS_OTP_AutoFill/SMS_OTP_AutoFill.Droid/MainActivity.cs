using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace SMS_OTP_AutoFill.Droid
{
	[Activity(Label = "SMS_OTP_AutoFill", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		EditText edtext;
		SMSBroadcastReceiver smsReceiver;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			// Get our edittext from the layout resource,
			// and attach an event to it
			edtext = FindViewById<EditText>(Resource.Id.editText);
		}
		protected override void OnResume()
		{
			base.OnResume();
			try
			{
				// Registering the broad cast receiver to detect sms from local phone
				if (null == this.smsReceiver)
				{
					smsReceiver = new SMSBroadcastReceiver();
					this.RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
				}
				else
				{
					edtext.Text = smsReceiver.verificationCode.Trim();
				}
			}
			catch (Exception ex)
			{
				//Log.Debug(Constants.APPNAME, ex.Message);
			}
		}
	}
}

