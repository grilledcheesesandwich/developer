using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace RemoteCertificateViewer
{
    public class Stores
    {
        public List<Store> LocalMachine { get; set; }
        public List<Store> CurrentUser { get; set; }

        public Stores()
        {
            LocalMachine = new List<Store>();
            CurrentUser = new List<Store>();
        }
    }

    public class Store
    {
        public string Name { get; set; }
        public List<X509Certificate2> Certificates { get; set; }

        public Store(string name)
        {
            this.Name = name;
            Certificates = new List<X509Certificate2>();
        }
    }
}
