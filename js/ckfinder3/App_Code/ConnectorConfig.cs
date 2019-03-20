namespace CKSource.CKFinder.Connector.WebApp
{
    using System.Configuration;
    using System.Linq;

    using CKSource.CKFinder.Connector.Config;
    using CKSource.CKFinder.Connector.Core.Builders;
    using CKSource.CKFinder.Connector.Host.Owin;
    using CKSource.CKFinder.Connector.KeyValue.FileSystem;
    using CKSource.FileSystem.Amazon;
    using CKSource.FileSystem.Azure;
    using CKSource.FileSystem.Dropbox;
    using CKSource.FileSystem.Ftp;
    using CKSource.FileSystem.Local;

    using Owin;

    public class ConnectorConfig
    {
        public static void RegisterFileSystems()
        {
            FileSystemFactory.RegisterFileSystem<LocalStorage>();
            FileSystemFactory.RegisterFileSystem<DropboxStorage>();
            FileSystemFactory.RegisterFileSystem<AmazonStorage>();
            FileSystemFactory.RegisterFileSystem<AzureStorage>();
            FileSystemFactory.RegisterFileSystem<FtpStorage>();
        }

        public static void SetupConnector(IAppBuilder builder)
        {
            var allowedRoleMatcherTemplate = ConfigurationManager.AppSettings["ckfinderAllowedRole"];
            var authenticator = new RoleBasedAuthenticator(allowedRoleMatcherTemplate);

            var connectorFactory = new OwinConnectorFactory();
            var connectorBuilder = new ConnectorBuilder();
            var connector = connectorBuilder
                .LoadConfig()
                .SetAuthenticator(authenticator)
                .SetRequestConfiguration(
                    (request, config) =>
                    {
                        config.LoadConfig();

                        var defaultBackend = config.GetBackend("default");
                        var keyValueStoreProvider = new FileSystemKeyValueStoreProvider(defaultBackend);
                        config.SetKeyValueStoreProvider(keyValueStoreProvider);

                        // Remove dummy resource type
                        config.RemoveResourceType("dummy");

                        var queryParameters = request.QueryParameters;

                        // This code lacks some input validation - make sure the user is allowed to access passed appGuid
                        string appGuid = queryParameters.ContainsKey("appGuid") ? Enumerable.FirstOrDefault(queryParameters["appGuid"]) : string.Empty;

                        var fileSystem = new AmazonStorage(secret: "SECRETHERE",
                                                           key: "KEYHERE",
                                                           bucket: "BUCKETHERE",
                                                           region: "us-east-1",
                                                           root: string.Format("images/{0}/UserImages/", appGuid),
                                                           signatureVersion: "4");
                        config.AddBackend("s3", fileSystem);
                        config.AddResourceType("Images", resourceBuilder =>
                        {
                            resourceBuilder.SetBackend("s3", "/");
                        });

                    })
                .Build(connectorFactory);

            builder.UseConnector(connector);
        }
    }
}