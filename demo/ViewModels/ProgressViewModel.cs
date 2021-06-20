using System.Collections.ObjectModel;
using System.ComponentModel;

using HierarchicalProgress.Demo.Base;
using HierarchicalProgress.Demo.DataModels;
using HierarchicalProgress.Events;

namespace HierarchicalProgress.Demo.ViewModels
{
    public class ProgressViewModel : NotifyPropertyChangedBase
    {
        private HierarchicalProgress<ProgressReport> _progressProvider;

        public ProgressViewModel()
        {
            _progressProvider = new HierarchicalProgress<ProgressReport>(0..100, 0..100);
        }

        public ProgressViewModel(HierarchicalProgress<ProgressReport> progressProvider)
        {
            _progressProvider = progressProvider;
        }

        public HierarchicalProgress<ProgressReport> ProgressProvider
        {
            get => _progressProvider;
            set
            {
                _progressProvider.PropertyChanged -= PropertyChangedProcessor;
                _progressProvider.Reported -= ProgressReported;
                PreviousReports.Clear();
                _progressProvider = value;
                _progressProvider!.PropertyChanged += PropertyChangedProcessor;
                _progressProvider.Reported += ProgressReported;
            }
        }

        public double ProgressMinimum => _progressProvider.ProgressBoundaries.Start.Value;
        
        public double ProgressMaximum => _progressProvider.ProgressBoundaries.End.Value;

        public double Progress => _progressProvider.Progress.Value;

        public double AllocatedProgress => _progressProvider.AllocatedProgress;
        
        public double ReportMinimum => _progressProvider.ProgressBoundaries.Start.Value;
        
        public double ReportMaximum => _progressProvider.ProgressBoundaries.End.Value;

        public ProgressChange LatestChange => _progressProvider.LatestChange.Type;

        public double LatestChangeDelta => _progressProvider.LatestChange.Delta;

        public ProgressReport? LatestReport => _progressProvider.LatestReport;

        public bool IsCompleted => _progressProvider.IsCompleted;

        public ObservableCollection<ProgressReport> PreviousReports { get; } = new();

        public ObservableCollection<ProgressViewModel> Slices { get; } = new();

        private void ProgressReported(object? sender, ProgressReportedEventArgs<ProgressReport> e)
        {
            PreviousReports.Add(e.ReportedProgress!);
        }

        private void PropertyChangedProcessor(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_progressProvider.Progress):
                    OnPropertyChanged(nameof(Progress));
                    break;
                case nameof(_progressProvider.AllocatedProgress):
                    OnPropertyChanged(nameof(AllocatedProgress));
                    break;
                case nameof(_progressProvider.ReportBoundaries):
                    OnPropertyChanged(nameof(ReportMinimum));
                    OnPropertyChanged(nameof(ReportMaximum));
                    break;
                case nameof(_progressProvider.LatestChange):
                    OnPropertyChanged(nameof(LatestChange));
                    OnPropertyChanged(nameof(LatestChangeDelta));
                    break;
                case nameof(_progressProvider.LatestReport):
                    OnPropertyChanged(nameof(LatestReport));
                    break;
                case nameof(_progressProvider.IsCompleted):
                    OnPropertyChanged(nameof(IsCompleted));
                    break;
            }
        }
        
    }
}
