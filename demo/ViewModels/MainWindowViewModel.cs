using System.Collections.ObjectModel;
using System.Windows.Input;

using HierarchicalProgress.Demo.Base;
using HierarchicalProgress.Demo.DataModels;

using Prism.Commands;

namespace HierarchicalProgress.Demo.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ProgressViewModel _selectedProgress = new();
        private ObservableCollection<ProgressViewModel> _progressProviders = new();
        private ProgressCreatorViewModel _progressCreator = new();

        public ProgressViewModel SelectedProgress
        {
            get => _selectedProgress;
            set => Set(ref _selectedProgress, value);
        }

        public ObservableCollection<ProgressViewModel> ProgressProviders
        {
            get => _progressProviders;
            set => Set(ref _progressProviders, value);
        }

        public ProgressCreatorViewModel ProgressCreator
        {
            get => _progressCreator;
            set => Set(ref _progressCreator, value);
        }

        public static ICommand CreateProgressProvider => new DelegateCommand<MainWindowViewModel>(model => {
            var provider = new HierarchicalProgress<ProgressReport>(
                (model.ProgressCreator.ProgressMinimum, model.ProgressCreator.ProgressMaximum), 
                (model.ProgressCreator.ReportMinimum, model.ProgressCreator.ReportMaximum));
            model.ProgressProviders.Add(new ProgressViewModel(provider));
        }, _ => true);
    }
}
