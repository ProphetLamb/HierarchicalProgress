﻿using System.Windows;
using System.Windows.Controls;

using HierarchicalProgress.Demo.ViewModels;

namespace HierarchicalProgress.Demo.Controls
{
    public partial class ProgressReportControl : UserControl
    {
        public ProgressReportControl()
        {
            InitializeComponent();
        }
        
        public static readonly DependencyProperty ProgressProviderProperty = DependencyProperty.Register(
            "ProgressProvider",
            typeof(ProgressViewModel),
            typeof(ProgressReportControl),
            new PropertyMetadata(default(ProgressViewModel), PropertyChangedCallback));
        
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressControl control = (ProgressControl)d!;
            control.DataContext = (ProgressViewModel)e.NewValue!;
        }

        public ProgressViewModel ProgressProvider
        {
            get => (ProgressViewModel)GetValue(ProgressProviderProperty);
            set => SetValue(ProgressProviderProperty, value);
        }
    }
}

