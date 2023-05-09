using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace DigitalAgencyTheme
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static ResourceManifest _manifest;

        public ResourceManagementOptionsConfiguration()
        {
            _manifest = new ResourceManifest();

            _manifest
                .DefineStyle("DigitalAgencyTheme-css")
                .SetUrl("~/DigitalAgencyTheme/assets/css/theme.min.css")
                .SetVersion("1.0.0");

            _manifest
                .DefineScript("DigitalAgencyTheme-js")
                .SetUrl("~/DigitalAgencyTheme/assets/js/theme.min.js")
                .SetVersion("1.0.0");
        }
        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(_manifest);
        }
    }
}