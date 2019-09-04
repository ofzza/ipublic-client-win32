using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ofzza.ipublic {

  /// <summary>
  /// Provides get/set passthrough to service configurable properties
  /// </summary>
  public class Config {

    /// <summary>
    /// Gets a configured parameter
    /// </summary>
    /// <param name="key">Parameter key</param>
    /// <returns>Configured parameter value</returns>
    static private string GetValue (string key) {
      RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);
      reg = reg.OpenSubKey("ofzza");
      reg = reg.OpenSubKey("ipublic");
      reg = reg.OpenSubKey("client-win32");
      try {
        return (string)reg.GetValue(key, Microsoft.Win32.RegistryValueKind.String);
      } catch (Exception ex) {
        return null;
      }
    }
    /// <summary>
    /// Sets a configured value
    /// </summary>
    /// <param name="key">Parameter key</param>
    /// <param name="value">Parameter value</param>
    static private void SetValue (string key, string value) {
      RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);
      reg = reg.CreateSubKey("ofzza");
      reg = reg.CreateSubKey("ipublic");
      reg = reg.CreateSubKey("client-win32");
      reg.SetValue(key, value, RegistryValueKind.String);
    }

    /// <summary>
    /// IPublic server IP or DNS address
    /// </summary>
    static public string ServerUrl {
      get { return Config.GetValue("ServerUrl"); }
      set { Config.SetValue("ServerUrl", value); }
    }

    /// <summary>
    /// IPublic client authentication key
    /// </summary>
    static public string AuthenticationKey {
      get { return Config.GetValue("AuthenticationKey"); }
      set { Config.SetValue("AuthenticationKey", value); }
    }

    /// <summary>
    /// IPublic client authentication token
    /// </summary>
    static public string AuthenticationToken {
      get { return Config.GetValue("AuthenticationToken"); }
      set { Config.SetValue("AuthenticationToken", value); }
    }

    /// <summary>
    /// IPublic client IP refresh interval [sec]
    /// </summary>
    static public int RefreshInterval {
      get {
        string interval = Config.GetValue("RefreshInterval");
        try {
          // Return parsed value
          return int.Parse(interval);
        } catch (Exception ex) {
          // Return default value
          return 300;
        }
      }
      set { Config.SetValue("RefreshInterval", value.ToString()); }
    }


  }

}
