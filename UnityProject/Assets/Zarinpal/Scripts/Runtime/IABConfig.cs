using UnityEngine;

namespace ZarinpalIAB
{
    [CreateAssetMenu]
    public class IABConfig : ScriptableObject
    {
        public bool Enable;

        //ZarinpalSetting
        public string MerchantID;
        public bool AutoStartPurchase = true;
        public bool AutoVerifyPurchase = true;
        public string Scheme = "return";
        public string Host = "zarinpalpayment";
        public string CallbackUrl;
        public bool UseSchemeAndHostAsCallbackUrl;
        public bool LogEnabled = true;

    }
}

