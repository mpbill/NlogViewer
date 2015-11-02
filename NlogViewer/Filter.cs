using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Reflection;
using NLog;

namespace NlogViewer
{
    public class Filter : INotifyPropertyChanged
    {
        private bool? _Trace = true;
        private bool? _Warn = true;
        private bool? _Debug = true;
        private bool? _Error = true;
        private bool? _Info = true;
        private bool? _HasException = true;
        private bool? _Fatal = true;
        public bool? Trace
        {
            get { return _Trace; }
            set
            {
                _Trace = value;
                updateList(value, LogLevel.Trace.Name);
            }
        }
        public bool? Warn
        {
            get { return _Warn; }
            set { _Warn = value;updateList(value, LogLevel.Warn.Name); }
        }
        public bool? Debug
        {
            get { return _Debug; }
            set { _Debug = value; updateList(value, LogLevel.Debug.Name); }
        }
        public bool? Error
        {
            get { return _Error; }
            set { _Error = value; updateList(value, NLog.LogLevel.Error.Name); }
        }
        public bool? Info
        {
            get { return _Info; }
            set { _Info = value; updateList(value, LogLevel.Info.Name); }
        }

        
        public bool? HasException
        {
            get { return _HasException; }
            set { _HasException = value; PropertyChanged(this, new PropertyChangedEventArgs("HasException")); }
        }
        public bool? Fatal
        {
            get { return _Fatal; }
            set { _Fatal = value; updateList(value, LogLevel.Fatal.Name); }
        }
        
        public List<string> Filters { get; set; }
        

        public event PropertyChangedEventHandler PropertyChanged;
        public Filter()
        {
            Filters = new List<string>()
            {
                LogLevel.Fatal.Name,
                LogLevel.Debug.Name,
                LogLevel.Error.Name,
                LogLevel.Info.Name,
                LogLevel.Trace.Name,
                LogLevel.Warn.Name
            };
            
        }
        
        public void updateList(bool? value, string name)
        {
            if (value.HasValue && value.Value && !Filters.Contains(name))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                Filters.Add(name);
            }
            else if (value.HasValue && !value.Value && Filters.Contains(name))
            {
                Filters.Remove(name);
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
            else
                PropertyChanged(this, new PropertyChangedEventArgs(name));

        }
    }
}
