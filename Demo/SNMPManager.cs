using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

namespace Demo
{
    class SNMPManager
    {
        private static SNMPManager _instance = null;

        //string[] strOID = null;
        public ApplicationDataContainer roamingSettings = null;
        ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
        ApplicationData applicationData = null;
        const int SNMP_PORT = 161;
        const string COMMUNITY = "public";
        const int iTimeout = 10000;
        
        //OID List
        public const string OID_NAME = "1.3.6.1.2.1.43.5.1.1.16.1";
        public const string OID_FAX_ENABLED = "1.3.6.1.4.1.1129.2.3.50.1.2.4.1.11.1.1";
        public const string OID_MODEL = "1.3.6.1.2.1.1.5.0";
        public const string OID_BRAND = "1.3.6.1.4.1.1129.2.3.50.1.2.4.1.7.1.1";
        public const string OID_MAC_ADDRESS = "1.3.6.1.2.1.2.2.1.6.2";
        public const string OID_SERIAL_NO = "1.3.6.1.2.1.43.5.1.1.17.1";
        public const string OID_LANG = "1.3.6.1.2.1.43.7.1.1.2.1.1";

        // local settings keys
        public const string NAME = "NAME";
        public const string FAX = "FAX";
        public const string MODEL = "MODEL";
        public const string BRAND = "BRAND";
        public const string MAC_ADDRESS = "MAC_ADDRESS";
        public const string SNO = "SNO";
        public const string LANGUAGE = "LANGUAGE";

        Dictionary<string, string> dOIDMap = null;
        Dictionary<string, string> dOIDRes = null;

        public const string settingName = "PrinterInfo";
        public const string settingName1 = "NAME";
        public const string settingName2 = "MODEL";
        public const string settingName3 = "BRAND";
        public const string settingName4 = "SNO";
        public const string settingName5 = "LANGUAGE";
        public const string settingName6 = "FAX";
        public const string settingName7 = "MAC_ADDRESS";
          
        

        public Dictionary<string, string> ResultList
        {
            get => dOIDRes;
        }

        enum eState
        {
            None = 0,
            Enabled = 3,
            Disabled
        };

        public static SNMPManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SNMPManager();
            }
            return _instance;
        }

        SNMPManager()
        {
            /*strOID = new string[] { "1.3.6.1.2.1.43.5.1.1.16.1", "1.3.6.1.4.1.1129.2.3.50.1.2.4.1.11.1.1", "1.3.6.1.2.1.1.5.0", "1.3.6.1.4.1.1129.2.3.50.1.2.4.1.7.1.1",
                                    "1.3.6.1.2.1.2.2.1.6.2", "1.3.6.1.2.1.25.1.1.0", "1.3.6.1.2.1.43.5.1.1.17.1", "1.3.6.1.2.1.43.7.1.1.2.1.1" };*/
            dOIDMap = new Dictionary<string, string>();
            dOIDRes = new Dictionary<string, string>();

            dOIDMap.Add(OID_NAME, "Name");
            dOIDMap.Add(OID_FAX_ENABLED, "Fax");
            dOIDMap.Add(OID_MODEL, "Model");
            dOIDMap.Add(OID_BRAND, "Brand");
            dOIDMap.Add(OID_MAC_ADDRESS, "Physical Address");
            dOIDMap.Add(OID_SERIAL_NO, "Serial No");
            dOIDMap.Add(OID_LANG, "Language");

            //initialize roamingsettings
            initRoamingSettings();
            
        }

        public void initRoamingSettings()
        {
            applicationData = ApplicationData.Current;
            roamingSettings = applicationData.RoamingSettings;
            applicationData.SignalDataChanged();
            // ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();            
        }

		public void ResetLocalSettings()
        {
            ApplicationData.Current.ClearAsync();
        }
		
        public Task<bool> FetchMFPInfo(string strIPAddress)
        {
            return Task.Run(() =>
            {
                bool bValid = false;
                dOIDRes.Clear();

                try
                {
                    /*List<Variable> lstTotal = new List<Variable>();
                    int iRet = await Messenger.BulkWalkAsync(VersionCode.V2, new System.Net.IPEndPoint(IPAddress.Parse("10.188.101.80"), 161), new OctetString("public"), new ObjectIdentifier("1.3.6.1.2.1.43.5.1.1.16.1"), lstTotal, 3, WalkMode.Default, null, null);*/

                    List<Variable> lstVars = new List<Variable>();
                    foreach (string OID in dOIDMap.Keys)
                    {
                        Variable vTemp = new Variable(new ObjectIdentifier(OID));
                        lstVars.Add(vTemp);
                    }

                    IList<Variable> lstResult = Messenger.Get(VersionCode.V2, new System.Net.IPEndPoint(IPAddress.Parse(strIPAddress), SNMP_PORT), new OctetString(COMMUNITY), lstVars, iTimeout);

                    //strParam = "Walk Result Total: " + lstTotal.Count.ToString() + "\n";
                    //strParam = "Input OID Count: " + lstVars.Count + " Output Result Count: " + lst.Count + "\n------------------------------------------------------------------------\n";

                    if (lstResult.Count > 0)
                    {
                        foreach (Variable vTmp in lstResult)
                        {
                            string strTemp = CheckForSpecialParams(vTmp);
                            dOIDRes.Add(vTmp.Id.ToString(), strTemp);
                            //strParam += "OID: " + vTmp.Id + " Value: " + vTmp.Data.ToString() + "\n";
                            //strParam += dOIDMap[vTmp.Id.ToString()] + ": " + strTemp + "\n";
                        }
                        roamingSettings.Values[settingName] = composite;                        
                        bValid = true;
                    }
                }
                catch (Lextm.SharpSnmpLib.Messaging.TimeoutException tx)
                {
                    //ContentDialog dlg = GenericHelper.GetDialog();
                    //dlg.Content = DetailPage.ERROR_MSG;
                    //dlg.ShowAsync();
                }
                catch (Exception ex)
                {
                    //ContentDialog dlg = GenericHelper.GetDialog();
                    //dlg.Content = ex.Message;
                    //dlg.ShowAsync();
                }
                return bValid;
            });
        }

        private string CheckForSpecialParams(Variable vParam)
        {
            string strID = vParam.Id.ToString();
            string strValue = vParam.Data.ToString();

            try
            {
                switch(strID)
                {
                    case OID_NAME:
                        composite[settingName1] = strValue;
                        break;
                    case OID_MODEL:
                        composite[settingName2] = strValue;
                        break;
                    case OID_BRAND:
                        composite[settingName3] = strValue;
                        break;
                    case OID_SERIAL_NO:
                        composite[settingName4] = strValue;
                        break;
                    case OID_LANG:
                        composite[settingName5] = HTTPRequest.GetInstance().EngineVersion;// strValue;
                        break;
                    case OID_FAX_ENABLED:
                        {
                            int iTmp = Convert.ToInt32(strValue);
                            strValue = ((eState)iTmp).ToString();
                            composite[settingName6] = strValue;
                        }
                        break;

                    case OID_MAC_ADDRESS:
                        {
                            string stTemp = null;
                            char[] cArr = strValue.ToCharArray();
                            foreach (char c in cArr)
                            {
                                stTemp += Convert.ToInt32(c).ToString("X2") + ":";
                            }
                            strValue = stTemp.TrimEnd(':');
                            composite[settingName7] = strValue;
                        }
                        break;
                }

                if ((string.IsNullOrEmpty(strValue) || (string.Equals(strValue, "NoSuchObject", StringComparison.OrdinalIgnoreCase) == true)))
                {
                    strValue = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SNMPManager :: CheckForSpecialParams() :: Exception Handled: " + ex);
            }
            return strValue;
        }
    }
}