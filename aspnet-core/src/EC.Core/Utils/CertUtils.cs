using EC.Utils.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EC.Utils
{
    public class CertUtils
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        public static X509Certificate2 getX509Certificate(string certSerial, StoreLocation location = StoreLocation.LocalMachine)
        {
            X509Store x509Store = new X509Store(StoreName.My, location);
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    X509Certificate2 current = enumerator.Current;
                    if (current.SerialNumber.ToUpper().Equals(certSerial.ToUpper()))
                    {
                        return current;
                    }
                }

                return null;
            }
            finally
            {
                x509Store.Close();
            }
        }


        public static CertInfo getCertificate(StoreLocation location = StoreLocation.CurrentUser)
        {
            X509Store x509Store = new X509Store(StoreName.My, location);
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                X509Certificate2Collection x509Certificate2Collection = X509Certificate2UI.SelectFromCollection(x509Store.Certificates, "Danh sách chứng thư", "Chọn chứng thư để ký số", X509SelectionFlag.SingleSelection, foregroundWindow);
                if (x509Certificate2Collection.Count <= 0)
                {
                    throw new Exception("Action cancelled by the user");
                }

                X509Certificate2Enumerator enumerator = x509Certificate2Collection.GetEnumerator();
                enumerator.MoveNext();
                enumerator.Current.Export(X509ContentType.Cert);
                return new CertInfo
                {
                    CertSerial = enumerator.Current.SerialNumber,
                    CertBase64 = Convert.ToBase64String(enumerator.Current.Export(X509ContentType.Cert))
                };
            }
            finally
            {
                x509Store.Close();
            }
        }
    }
}
