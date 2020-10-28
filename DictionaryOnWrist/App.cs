using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Web;

using Newtonsoft.Json;
using System.Data;
using Tizen.Network.Connection;
using Tizen.System;

namespace DictionaryOnWrist
{
    public class transResult
    {
        public string src;
        public string dst;
    }
    public class JsonResponse
    {
        public string from;
        public string to;
        public List<transResult> trans_result;
        public int error_code = -1;
    }
    public class App : Application
    {
        public App()
        {
            //NetworkService ns = new NetworkService();
            //if(ns.IsConnected == false)
            //{
            //    Toast.DisplayIconText("您尚未连接到互联网", "NetworkOffline.png");
            //}
            MainPage = new NavigationPage(new QueryPage());
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

    public class NetworkService
    {
        
        bool isWiFiSupported, isTelephonySupported, isBTTetheringSupported, isEthernetSupported;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_vm">WeatherWatchPageModel</param>
        public NetworkService()
        {

            Information.TryGetValue("http://tizen.org/feature/network.wifi", out isWiFiSupported);
            Information.TryGetValue("http://tizen.org/feature/network.telephony", out isTelephonySupported);
            Information.TryGetValue("http://tizen.org/feature/network.tethering.bluetooth", out isBTTetheringSupported);
            Information.TryGetValue("http://tizen.org/feature/network.ethernet", out isEthernetSupported);
            Console.WriteLine("Network feature : wifi : " + isWiFiSupported);
            Console.WriteLine("Network feature : telephony : " + isTelephonySupported);
            Console.WriteLine("Network feature : tethering.bluetooth : " + isBTTetheringSupported);
            Console.WriteLine("Network feature : network.ethernet : " + isEthernetSupported);

            ConnectionItem connection = ConnectionManager.CurrentConnection;
            Console.WriteLine("connection type : " + connection.Type + ", State: " + connection.State);
        }

        /// <summary>
        /// whether any network connection is available or not
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    ConnectionItem connection = ConnectionManager.CurrentConnection;
                    Console.WriteLine("[NetworkService.IsConnected] connection type:" + connection.Type + ", state: " + connection.State);

                    ConnectionProfile profile = ConnectionProfileManager.GetCurrentProfile();
                    if (profile != null)
                    {
                        Console.WriteLine("[NetworkService.IsConnected] CurrentProfile type:" + profile.Type + ", name:" + profile.Name);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[NetworkService.IsConnected] " + e.Message + ", " + e.StackTrace + ", " + e.InnerException);
                }

                return CheckConnection();

            }
        }

        /// <summary>
        /// Check whether any network connection is available or not
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckConnection()
        {
            bool _isConnected = false;

            try
            {
                if (isBTTetheringSupported && (ConnectionManager.BluetoothState == Tizen.Network.Connection.ConnectionState.Connected))
                {
                    Console.WriteLine("[NetworkService.CheckConnection()] BluetoothState:" + ConnectionManager.BluetoothState);
                    _isConnected = true;
                }

                if (isTelephonySupported && (ConnectionManager.CellularState == CellularState.Connected))
                {
                    Console.WriteLine("[NetworkService.CheckConnection()] CellularState :" + ConnectionManager.CellularState);
                    _isConnected = true;
                }

                if (isEthernetSupported && (ConnectionManager.EthernetCableState == EthernetCableState.Attached && ConnectionManager.EthernetState == Tizen.Network.Connection.ConnectionState.Connected))
                {
                    Console.WriteLine("[NetworkService.CheckConnection()] EthernetCableState:" + ConnectionManager.EthernetCableState);
                    Console.WriteLine("[NetworkService.CheckConnection()] EthernetState:" + ConnectionManager.EthernetState);
                    _isConnected = true;
                }

                if (isWiFiSupported && (ConnectionManager.WiFiState == Tizen.Network.Connection.ConnectionState.Connected))
                {
                    Console.WriteLine("[NetworkService.CheckConnection()] WiFiState:" + ConnectionManager.WiFiState);
                    _isConnected = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[NetworkService.CheckConnection()] " + e.Message + ", " + e.StackTrace + ", " + e.InnerException);
            }

            return _isConnected;
        }

        //public void RegisterEvent()
        //{
        //    ConnectionManager.IPAddressChanged += ConnectionManager_IPAddressChanged;
        //    ConnectionManager.ConnectionTypeChanged += ConnectionManager_ConnectionTypeChanged;
        //}

        //public void UnregisterEvent()
        //{
        //    ConnectionManager.IPAddressChanged -= ConnectionManager_IPAddressChanged;
        //    ConnectionManager.ConnectionTypeChanged -= ConnectionManager_ConnectionTypeChanged;
        //}

        //private void ConnectionManager_ConnectionTypeChanged(object sender, ConnectionTypeEventArgs e)
        //{
        //    Console.WriteLine("[ConnectionManager_ConnectionTypeChanged] type : " + connection.Type + ", State: " + connection.State);
        //}

        //private void ConnectionManager_IPAddressChanged(object sender, AddressEventArgs e)
        //{
        //    Console.WriteLine("[ConnectionManager_IPAddressChanged] IPv4: " + e.IPv4Address + ", IPv6: " + e.IPv6Address);
        //}
    }
}
