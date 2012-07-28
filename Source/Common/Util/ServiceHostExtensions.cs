using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Common
{
    /// <summary>
    /// WCF Service Host extension Methods
    /// </summary>
    public static class ServiceHostExtensions
    {
        public static void EnableIncludeExceptionInFaultBehavior(this ServiceHost host)
        {
            ServiceBehaviorAttribute debuggingBehavior =
                host.Description.Behaviors.Find<ServiceBehaviorAttribute>();

            debuggingBehavior.IncludeExceptionDetailInFaults = true;
        }

        public static void AddDefaultMEXEndPoint(this ServiceHost host)
        {
            const string MexExtension = "Mex";

            ServiceMetadataBehavior mexBehavior = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mexBehavior);

            foreach (Uri baseAddress in host.BaseAddresses)
            {
                if (baseAddress.Scheme == Uri.UriSchemeHttp)
                {
                    mexBehavior.HttpGetEnabled = true;
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                        MetadataExchangeBindings.CreateMexHttpBinding(), MexExtension);
                }
                else if (baseAddress.Scheme == Uri.UriSchemeHttps)
                {
                    mexBehavior.HttpsGetEnabled = true;
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                        MetadataExchangeBindings.CreateMexHttpsBinding(), MexExtension);
                }
                else if (baseAddress.Scheme == Uri.UriSchemeNetPipe)
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                        MetadataExchangeBindings.CreateMexNamedPipeBinding(), MexExtension);
                }
                else if (baseAddress.Scheme == Uri.UriSchemeNetTcp)
                {

                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                        MetadataExchangeBindings.CreateMexTcpBinding(), MexExtension);
                }
            }
        }

        public static string GetHostedServiceEndPoints(this ServiceHost host)
        {
            StringBuilder logBuilder = new StringBuilder();
            ServiceDescription desc = host.Description;

            logBuilder.AppendLine("ServiceEndPoints:");
            foreach (ServiceEndpoint endpoint in desc.Endpoints)
            {
                logBuilder.AppendFormat("{0}", endpoint.Name);
                logBuilder.AppendLine();
                logBuilder.AppendFormat("Endpoint Address:{0}", endpoint.Address);
                logBuilder.AppendLine();
                logBuilder.AppendFormat("Binding Name:{0}", endpoint.Binding.Name);
                logBuilder.AppendLine();
                logBuilder.AppendFormat("Contract Name:{0}", endpoint.Contract.Name);
            }

            return logBuilder.ToString();
        }
    }
}
