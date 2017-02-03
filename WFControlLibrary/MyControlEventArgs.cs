using System;

namespace WFControlLibrary
{
    public class MyControlEventArgs : EventArgs
    {
        private string _Name;
        private string _StreetAddress;
        private string _City;
        private string _State;
        private string _Zip;
        private bool _IsOK;

        public MyControlEventArgs(bool result,
            string name,
            string address,
            string city,
            string state,
            string zip)
        {
            _IsOK = result;
            _Name = name;
            _StreetAddress = address;
            _City = city;
            _State = state;
            _Zip = zip;
        }

        public string MyName
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string MyStreetAddress
        {
            get { return _StreetAddress; }
            set { _StreetAddress = value; }
        }
        public string MyCity
        {
            get { return _City; }
            set { _City = value; }
        }
        public string MyState
        {
            get { return _State; }
            set { _State = value; }
        }
        public string MyZip
        {
            get { return _Zip; }
            set { _Zip = value; }
        }
        public bool IsOK
        {
            get { return _IsOK; }
            set { _IsOK = value; }
        }
    }
}