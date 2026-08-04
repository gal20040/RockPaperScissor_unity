

using UnityEngine;

namespace admob
{
	public class Admob
	{
		public delegate void AdmobEventHandler(string eventName, string msg);

		private class InnerAdmobListener : IAdmobListener
		{
			internal Admob admobInstance;

			public void onAdmobEvent(string adtype, string eventName, string paramString)
			{
				if (adtype == "banner")
				{
					if (admobInstance.bannerEventHandler != null)
					{
						admobInstance.bannerEventHandler(eventName, paramString);
					}
				}
				else if (adtype == "interstitial")
				{
					if (admobInstance.interstitialEventHandler != null)
					{
						admobInstance.interstitialEventHandler(eventName, paramString);
					}
				}
				else if (adtype == "rewardedVideo")
				{
					if (admobInstance.rewardedVideoEventHandler != null)
					{
						admobInstance.rewardedVideoEventHandler(eventName, paramString);
					}
				}
				else if (adtype == "nativeBanner" && admobInstance.nativeBannerEventHandler != null)
				{
					admobInstance.nativeBannerEventHandler(eventName, paramString);
				}
			}
		}

		private static Admob _instance;

		private AndroidJavaObject jadmob;

		public event AdmobEventHandler bannerEventHandler;

		public event AdmobEventHandler interstitialEventHandler;

		public event AdmobEventHandler rewardedVideoEventHandler;

		public event AdmobEventHandler nativeBannerEventHandler;

		public static Admob Instance()
		{
			if (_instance == null)
			{
				_instance = new Admob();
				_instance.preInitAdmob();
			}
			return _instance;
		}

		private void preInitAdmob()
		{
			if (jadmob == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.admob.plugin.AdmobUnityPlugin");
				jadmob = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				InnerAdmobListener innerAdmobListener = new InnerAdmobListener();
				innerAdmobListener.admobInstance = this;
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
				jadmob.Call("setContext", @static, new AdmobListenerProxy(innerAdmobListener));
			}
		}

		public void initAdmob(string bannerID, string fullID)
		{
			jadmob.Call("initAdmob", bannerID, fullID);
		}

		public void showBannerRelative(AdSize size, int position, int marginY, string instanceName = "defaultBanner")
		{
			jadmob.Call("showBannerRelative", size.Width, size.Height, position, marginY, instanceName);
		}

		public void showBannerAbsolute(AdSize size, int x, int y, string instanceName = "defaultBanner")
		{
			jadmob.Call("showBannerAbsolute", size.Width, size.Height, x, y, instanceName);
		}

		public void removeBanner(string instanceName = "defaultBanner")
		{
			jadmob.Call("removeBanner", instanceName);
		}

		public void loadInterstitial()
		{
			jadmob.Call("loadInterstitial");
		}

		public bool isInterstitialReady()
		{
			return jadmob.Call<bool>("isInterstitialReady", new object[0]);
		}

		public void showInterstitial()
		{
			jadmob.Call("showInterstitial");
		}

		public void loadRewardedVideo(string rewardedVideoID)
		{
			jadmob.Call("loadRewardedVideo", rewardedVideoID);
		}

		public bool isRewardedVideoReady()
		{
			return jadmob.Call<bool>("isRewardedVideoReady", new object[0]);
		}

		public void showRewardedVideo()
		{
			jadmob.Call("showRewardedVideo");
		}

		public void setTesting(bool value)
		{
			jadmob.Call("setTesting", value);
		}

		public void setForChildren(bool value)
		{
			jadmob.Call("setForChildren", value);
		}

		public void showNativeBannerRelative(AdSize size, int position, int marginY, string nativeBannerID, string instanceName = "defaultNativeBanner")
		{
			jadmob.Call("showNativeBannerRelative", size.Width, size.Height, position, marginY, nativeBannerID, instanceName);
		}

		public void showNativeBannerAbsolute(AdSize size, int x, int y, string nativeBannerID, string instanceName = "defaultNativeBanner")
		{
			jadmob.Call("showNativeBannerAbsolute", size.Width, size.Height, x, y, nativeBannerID, instanceName);
		}

		public void removeNativeBanner(string instanceName = "defaultNativeBanner")
		{
			jadmob.Call("removeNativeBanner", instanceName);
		}
	}
}
