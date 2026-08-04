using UnityEngine;

namespace admob
{
    public class AdmobListenerProxy : AndroidJavaProxy
    {
        private readonly IAdmobListener listener;

        internal AdmobListenerProxy(IAdmobListener listener)
            : base("com.admob.plugin.IAdmobListener") => this.listener = listener;

        private void onAdmobEvent(string adtype, string eventName, string paramString) => listener?.onAdmobEvent(adtype, eventName, paramString);

        private string toString() => "AdmobListenerProxy";
    }
}
