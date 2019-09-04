using McMaster.Extensions.CommandLineUtils;
using ofzza.standaloneservice;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ofzza.ipublic {

  /// <summary>
  /// Defines service installer
  /// </summary>
  public class IPublicClientServiceInstaller : StandaloneServiceInstaller {
    public IPublicClientServiceInstaller () : base(IPublicClientService.Identity) { }
  }

  /// <summary>
  /// Main service/program class
  /// </summary>
  class IPublicClientService : StandaloneService {

    /// <summary>
    /// Service identity
    /// </summary>
    static public StandaloneServiceIdentity Identity = new StandaloneServiceIdentity () {
      Id = "ipublic-client-win32",
      Name = "IPublic Client (Win32)",
      Description = "IPublic client reports your system's IP address to the preconfigured IPublic server in regular intervals."
    };

    /// <summary>
    /// Constructor
    /// </summary>
    public IPublicClientService () : base(IPublicClientService.Identity) { }

    /// <summary>
    /// Main CLI entry point
    /// </summary>
    /// <param name="args">Startup arguments</param>
    static void Main (string[] args) {
      // Parse arguments
      CommandLineApplication.Execute<IPublicClientService>(args);
    }

    /// <summary>
    /// Install as service
    /// </summary>
    [Option(
      CommandOptionType.NoValue,
      ShortName = "i",
      LongName = "install",
      Description = "Installs IPublic Client (Win32) as a service"
    )]
    public bool manageServiceInstall { get; }

    /// <summary>
    /// Uninstall as service
    /// </summary>
    [Option(
      CommandOptionType.NoValue,
      ShortName = "u",
      LongName = "uninstall",
      Description = "Uninstalls IPublic Client (Win32) as a service"
    )]
    public bool manageServiceUninstall { get; }

    /// <summary>
    /// Start service
    /// </summary>
    [Option(
      CommandOptionType.NoValue,
      ShortName = "r",
      LongName = "start",
      Description = "Starts IPublic Client (Win32) service if installed"
    )]
    public bool manageServiceStart { get; }

    /// <summary>
    /// Stop service
    /// </summary>
    [Option(
      CommandOptionType.NoValue,
      ShortName = "t",
      LongName = "stop",
      Description = "Stops IPublic Client (Win32) service if installed"
    )]
    public bool manageServiceStop { get; }

    /// <summary>
    /// Configure IPublic client
    /// </summary>
    [Option(
      CommandOptionType.NoValue,
      ShortName = "c",
      LongName = "config",
      Description = "Configures IPublic Client (Win32) service"
    )]
    public bool manageServiceConfigure { get; }

    /// <summary>
    /// Entry point after having processed startup arguments
    /// </summary>
    private void OnExecute () {
      if (this.manageServiceInstall) {
        // Install service
        (new IPublicClientService()).Install("--install");
      } else if (this.manageServiceUninstall) {
        // Uninstall service
        (new IPublicClientService()).Uninstall("--uninstall");
      } else if (this.manageServiceStart) {
        // Start installed service
        (new IPublicClientService()).Start();
      } else if (this.manageServiceStop) {
        // Stop installed service
        (new IPublicClientService()).Stop();
      } else if (this.manageServiceConfigure) {
        // Configure IPublic client
        this.ExecuteConfiguration();
      } else {
        // Run service functionality as process
        StandaloneService.Run(new IPublicClientService(), new string[] { });
      }
    }

    /// <summary>
    /// Executes (re)configuration of the service
    /// </summary>
    protected void ExecuteConfiguration () {

      // Get current configuration
      string currentServerUrl = Config.ServerUrl,
             currentAuthenticationKey = Config.AuthenticationKey,
             currentAuthenticationToken = Config.AuthenticationToken;

      // Echo current configuration
      Console.WriteLine("Current configuration: ");
      Console.WriteLine("- IPublic server URL = {0}", (currentServerUrl != null ? currentServerUrl : "not set"));
      Console.WriteLine("- IPublic client authentication key = {0}", (currentAuthenticationKey != null ? currentAuthenticationKey : "not set"));
      Console.WriteLine("- IPublic client authentication token = {0}", (currentAuthenticationToken != null ? "****" : "not set"));
      Console.WriteLine("");

      // Prompt for new configuration
      Console.WriteLine("Enter new configuration or leave blank to keep unchanged or Ctrl+C to cancel changes:");
      Console.Write("- IPublic server address ({0}): ", (currentServerUrl != null ? currentServerUrl : "not set"));
      string changedServerUrl = Console.ReadLine();
      Console.Write("- IPublic client authentication key ({0}): ", (currentAuthenticationKey != null ? currentAuthenticationKey : "not set"));
      string changedAuthenticationKey = Console.ReadLine();
      Console.Write("- IPublic client authentication token ({0}): ", (currentAuthenticationToken != null ? "****" : "not set"));
      string changedAuthenticationToken = Console.ReadLine();

      // Update configuration
      if (changedServerUrl.Length > 0) {
        Config.ServerUrl = changedServerUrl;
      }
      if (changedAuthenticationKey.Length > 0) {
        Config.AuthenticationKey = changedAuthenticationKey;
      }
      if (changedAuthenticationToken.Length > 0) {
        Config.AuthenticationToken = changedAuthenticationToken;
      }

    }

    /// <summary>
    /// Extensible execution method, executes service functionality
    /// </summary>
    /// <param name="args">Startup arguments</param>
    protected override void ExecuteServiceFunctionality (string[] args) {
      while (true) {

        // Register with server
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Config.ServerUrl);
        req.ContentType = "application/json";
        req.Method = "POST";

        using (StreamWriter streamWriter = new StreamWriter(req.GetRequestStream())) {
          string json = new JavaScriptSerializer().Serialize(new {
            key = Config.AuthenticationKey,
            auth = Config.AuthenticationToken
          });
          streamWriter.Write(json);
        }

        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        using (StreamReader streamReader = new StreamReader(res.GetResponseStream())) {
          string result = streamReader.ReadToEnd();
          Console.WriteLine(String.Format("> Registered with IPublic server: {0}", result));
        }

        // Wait for next contact
        Thread.Sleep(Config.RefreshInterval * 1000);

      }
    }

  }
}
