using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_NewWaveSanya.Adapter
{
    class Can
    {
        static public Ixxat SelectedAdapter;
        static public List<Ixxat> CanList = new List<Ixxat>();

        static public void AddAdapter(Ixxat ad)
        {
            if (ad == null) return;
            CanList.Add(ad);
        }

        static public bool SelectAdapter(Ixxat ad)
        {
            SelectedAdapter = ad;

            return ConnectAdapter(ad);
        }

        static public bool ConnectAdapter(Ixxat ad)
        {
            //activate adapter
            return true;
        }
    }

    class Ixxat
    {
        public String Name;
        //PVCIDEVICEINFO PIxxat;
    }
}
