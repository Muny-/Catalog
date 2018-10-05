using System;
using System.Runtime.Serialization;
using Avalonia.Media;
using ReactiveUI;

namespace Catalog.DigiKey.Responses
{
    [DataContract]
    public class BarcodeResponse : ReactiveObject
    {
        [IgnoreDataMember]
        private string _status;
        [IgnoreDataMember]
        private string _digiPartNo;
        [IgnoreDataMember]
        private string _mfgPartNo;

        //TODO there's a typo in the DigiKey api...for deserialization we need to keep it going!
        [IgnoreDataMember]
        private string _mfgName;

        [IgnoreDataMember]
        private string _desc;
        [IgnoreDataMember]
        private int _qty;

        [IgnoreDataMember]
        private SolidColorBrush _statusColor;

        [IgnoreDataMember]
        public SolidColorBrush StatusColor
        {
            get { return _statusColor; }
            set { this.RaiseAndSetIfChanged(ref _statusColor, value); }
        }

        [DataMember]
        public string Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        [DataMember]
        public string DigiKeyPartNumber
        {
            get { return _digiPartNo; }
            set { this.RaiseAndSetIfChanged(ref _digiPartNo, value); }
        }

        [DataMember]
        public string ManufacturerPartNumber
        {
            get { return _mfgPartNo; }
            set { this.RaiseAndSetIfChanged(ref _mfgPartNo, value); }
        }

        [DataMember]
        public string ManufacurerName
        {
            get { return _mfgName; }
            set { this.RaiseAndSetIfChanged(ref _mfgName, value); }
        }

        [DataMember]
        public string Description
        {
            get { return _desc; }
            set { this.RaiseAndSetIfChanged(ref _desc, value); }
        }

        [DataMember]
        public int Quantity
        {
            get { return _qty; }
            set { this.RaiseAndSetIfChanged(ref _qty, value); }
        }
    }
}