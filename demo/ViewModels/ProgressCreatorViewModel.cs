using HierarchicalProgress.Demo.Base;

namespace HierarchicalProgress.Demo.ViewModels
{
    public class ProgressCreatorViewModel : NotifyPropertyChangedBase
    {
        private double _progressMinimum;
        private double _progressMaximum = 100;
        private double _reportMinimum;
        private double _reportMaximum = 100;
        
        public double ProgressMinimum
        {
            get => _progressMinimum;
            set => Set(ref _progressMinimum, value);
        }

        public double ProgressMaximum
        {
            get => _progressMaximum;
            set => Set(ref _progressMaximum, value);
        }

        public double ReportMinimum
        {
            get => _reportMinimum;
            set => Set(ref _reportMinimum, value);
        }

        public double ReportMaximum
        {
            get => _reportMaximum;
            set => Set(ref _reportMaximum, value);
        }
    }
}
