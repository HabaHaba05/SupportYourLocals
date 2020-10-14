using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;

namespace SuppLocals
{
    public class Config
    {
        public Config() { }


        //BING
        public static readonly CredentialsProvider Bing_Api_Key = new ApplicationIdCredentialsProvider("vuOU7tN47KBhly1BAyhi~SKpEroFcVqMGYOJVSj-2HA~AhGXS-dV_H6Ofvn920LLMyvxfUUaLfjpZTD54fSc3WO-qRE7x6225O22AP_0XjDn");


        //GOOGLE
        public static readonly string Google_Api_Key = "AIzaSyBVs4wsiyCVvFbpPlX-NJyz_fj8db04R78";
        public static readonly string Host = "https://maps.googleapis.com";
        public static readonly string Path = "/maps/api/place/autocomplete/json";

    }
}
