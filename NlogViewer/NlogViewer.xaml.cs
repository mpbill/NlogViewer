using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using NLog;
using NLog.Common;
using OLinq;
namespace NlogViewer
{
    /// <summary>
    /// Interaction logic for NlogViewer.xaml
    /// </summary>
    public partial class NlogViewer : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<LogEventViewModel> FilteredLogEntries { get; set; }
        public int LogCount
        {
            get
            {
                return (int) GetValue(LogCountProperty);
            }
            set
            {
                SetValue(LogCountProperty, value);
                
            }
        }
        public static readonly DependencyProperty LogCountProperty = DependencyProperty.Register("LogCount",typeof(int), typeof(NlogViewer) );
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
            set { _AutoScroll = value; }
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
        public Visibility FilterVisibility { get { return (Visibility)GetValue(FilterVisibilityProperty); } set {SetValue(FilterVisibilityProperty, value);} }
        public static  readonly DependencyProperty FilterVisibilityProperty = DependencyProperty.Register("FilterVisibility", typeof(Visibility), typeof(NlogViewer));

        public NlogViewer()
        {
            LogListFilter = new Filter();
            IsTargetConfigured = false;
            LogEntries = new ObservableCollection<LogEventViewModel>();
            FilteredLogEntries = new ObservableCollection<LogEventViewModel>(LogEntries);
            
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

        public void UpdateFilteredList()
        {
            
                FilteredLogEntries.Clear();            
                
                foreach (var log in (from log in LogEntries
                                        where LogListFilter.Filters.Contains(log.Level)
                                        select log))
                {
                    FilteredLogEntries.Add(log);
                }
                
                logView.Items.Refresh();
                logView.UpdateLayout();

           
            





        }
        private void LogListFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(UpdateFilteredList));
        }
        

        protected void LogReceived(NLog.Common.AsyncLogEventInfo log)
        {
           
            LogEventViewModel vm = new LogEventViewModel(log.LogEvent);           
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if(LogCount!=0 && LogEntries.Count>=LogCount)
                    LogEntries.RemoveAt(0);
                
                LogEntries.Add(vm);
                UpdateFilteredList();
                if (AutoScroll)
                {
                    logView.ScrollIntoView(vm);
                }
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
