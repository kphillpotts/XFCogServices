﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFCogServices
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnalysisPage : ContentPage
    {
        public AnalysisPage()
        {
            InitializeComponent();
            this.BindingContext = App.MainViewModel;
        }
    }
}