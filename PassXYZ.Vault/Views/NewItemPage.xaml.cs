﻿using PassXYZ.Vault.Models;
using PassXYZ.Vault.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace PassXYZ.Vault.Views
{
    public partial class NewItemPage : ContentPage
    {
        public NewItemPage(NewItemViewModel viewModel)
        {
            InitializeComponent();
            if(viewModel != null ) { throw new ArgumentNullException(nameof(viewModel)); }
            BindingContext = viewModel;
        }
    }
}