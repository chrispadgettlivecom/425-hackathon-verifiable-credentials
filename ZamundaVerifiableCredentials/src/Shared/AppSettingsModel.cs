namespace Shared
{
    public class AppSettingsModel
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CredentialManifest { get; set; }

        public string IssuerAuthority { get; set; }

        public string RequestServiceEndpointAddress { get; set; }

        public string RequestServiceScope { get; set; }
    }
}
