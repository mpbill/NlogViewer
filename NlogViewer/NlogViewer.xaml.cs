using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OLinq;
namespace NlogViewer
{
    /// <summary>
    /// Interaction logic for NlogViewer.xaml
    /// </summary>
    public partial class NlogViewer : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<LogEventViewModel> FilteredLogEntries { get; set; }
        
        public Filter LogListFilter 
        { 
            get; 
            set; 
        }
        private bool _AutoScroll = false;
        [Description("True/False Is AutoScroll Enabled")]
        [TypeConverterAttribute(typeof(BooleanConverter))]
        public bool AutoScroll
        {
            get
            {
                return _AutoScroll;
            }
            set
            {
                if (value == true)
                {
                    logView.LayoutUpdated += LogView_LayoutUpdated;
                    _AutoScroll = value;
                }
                else
                {
                    logView.LayoutUpdated -= LogView_LayoutUpdated;
                    _AutoScroll = value;
                }
            }
        }
        public ObservableCollection<LogEventViewModel> LogEntries { get; private set; }
        
        public bool IsTargetConfigured { get; private set; }

        [Description("Width of time column in pixels"), Category("Data")]
        [TypeConverterAttribute(typeof(LengthConverter))]
        
        public double TimeWidth { get; set; }

        [Description("Width of Logger column in pixels, or auto if not specified"), Category("Data")]
        [TypeConverterAttribute(typeof(LengthConverter))]
        public double LoggerNameWidth { set; get; }

        [Description("Width of Level column in pixels"), Category("Data")]
        [TypeConverterAttribute(typeof(LengthConverter))]
        public double LevelWidth { get; set; }
        [Description("Width of Message column in pixels"), Category("Data")]
        [TypeConverterAttribute(typeof(LengthConverter))]
        public double MessageWidth { get; set; }
        [Description("Width of Exception column in pixels"), Category("Data")]
        [TypeConverterAttribute(typeof(LengthConverter))]
        public double ExceptionWidth { get; set; }


        public NlogViewer()
        {
            LogListFilter = new Filter();
            IsTargetConfigured = false;
            LogEntries = new ObservableCollection<LogEventViewModel>();
            FilteredLogEntries = new ObservableCollection<LogEventViewModel>(LogEntries);
            LogEntries.CollectionChanged += LogEntries_CollectionChanged;
            LogListFilter.PropertyChanged += LogListFilter_PropertyChanged;
            InitializeComponent();
            

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (NlogViewerTarget target in NLog.LogManager.Configuration.AllTargets.Where(t => t is NlogViewerTarget).Cast<NlogViewerTarget>())
                {
                    IsTargetConfigured = true;
                    target.LogReceived += LogReceived;
                }
            }
            
        }

        private void LogEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            updateFilteredList();
        }
        public void updateFilteredList()
        {
            System.Threading.Thread.Sleep(100);
            FilteredLogEntries.Clear();
            foreach(var log in (from log in LogEntries
             where LogListFilter.Filters.Contains(log.Level)
             select log))
            {
                FilteredLogEntries.Add(log);
            }
            logView.Items.Refresh();
            PropertyChanged(this, new PropertyChangedEventArgs("FilteredLogEntries"));




        }
        private void LogListFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            updateFilteredList();
            
        }

        private void LogView_LayoutUpdated(object sender, EventArgs e)
        {
            if (logView.Items.Count > 2)
                logView.ScrollIntoView(logView.Items[logView.Items.Count - 1]);
        }
        

        protected void LogReceived(NLog.Common.AsyncLogEventInfo log)
        {
            
            LogEventViewModel vm = new LogEventViewModel(log.LogEvent);           
            Dispatcher.BeginInvoke(new Action(() =>
            {
                
                if (LogEntries.Count >= 50)
                    LogEntries.RemoveAt(0);
                
                LogEntries.Add(vm);
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
