using System.Windows.Input;

using HierarchicalProgress.Demo.Base;
using HierarchicalProgress.Demo.DataModels;

using Prism.Commands;

namespace HierarchicalProgress.Demo.ViewModels
{
    public class ProgressReportViewModel : NotifyPropertyChangedBase
    {
        private ProgressReport _report;
        private ProgressViewModel _progress;

        public ProgressReportViewModel()
        {
            _report = new ProgressReport();
            _progress = new ProgressViewModel();
        }

        public ProgressReportViewModel(ProgressReport report, HierarchicalProgress<ProgressReport> progress)
        {
            _report = report;
            _progress = new ProgressViewModel(progress);
        }

        public ProgressViewModel Progress
        {
            get => _progress;
            set
            {
                Set(ref _progress, value);
                OnPropertyChanged(nameof(ReportProgress));
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(Inner));
            }
        }

        public double ReportProgress
        {
            get => _report.ReportProgress;
            set => Set(_report.ReportProgress, value, val => _report.ReportProgress = val);
        }

        public string? Message
        {
            get => _report.Message;
            set => Set(_report.Message, value, val => _report.Message = val);
        }

        public ProgressReportViewModel Inner => new((ProgressReport)_report.Inner!, _progress.ProgressProvider);

        public static ICommand Report = new DelegateCommand<ProgressReportViewModel>(model =>
        {
            model._progress.ProgressProvider.Report(model._report);
        }, _ => true);
    }
}
