using UnityEngine;
using System.Collections;
using System.Net;
/*
 * I'll be transferring these scripts to my library sooner or later.
 * It's just faster to make them here on the fly.
 * 
 * **/
public class GeneralUtils : MonoBehaviour 
{
    public static string GetIP()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[addr.Length - 1].ToString();
    }
}
