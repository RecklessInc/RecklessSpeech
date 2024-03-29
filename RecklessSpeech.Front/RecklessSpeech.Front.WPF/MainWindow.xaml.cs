﻿using Microsoft.Win32;
using RecklessSpeech.Front.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RecklessSpeech.Front.WPF
{
    public partial class MainWindow : Window
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public SequencePageViewModel ViewModel => (SequencePageViewModel)this.DataContext;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new SequencePageViewModel(new HttpBackEndGateway(new HttpBackEndGatewayAccess()));
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() is false) return;

            string filePath = openFileDialog.FileName;

            this.ViewModel.AddSequencesCommand.Execute(filePath);
        }

        private void Import_json_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() is false) return;

            string filePath = openFileDialog.FileName;

            this.ViewModel.ImportSequenceDetailsCommand.Execute(filePath);
        }

        private void ContextMenu_Enrich_Dutch_Click(object sender, RoutedEventArgs e)
        {
            int total = SequenceListView.SelectedItems.Count;
            int count = 0;

            this.ViewModel.Progress = 0;
            foreach (SequenceDto sequence in this.SequenceListView.SelectedItems)
            {
                this.ViewModel.EnrichDutchSequenceCommand.Execute(sequence);
                this.ViewModel.Progress = ++count / total * 100;
            }
        }

        private void ContextMenu_Enrich_English_Click(object sender, RoutedEventArgs e)
        {
            int total = SequenceListView.SelectedItems.Count;
            int count = 0;

            this.ViewModel.Progress = 0;
            foreach (SequenceDto sequence in this.SequenceListView.SelectedItems)
            {
                this.ViewModel.EnrichEnglishSequenceCommand.Execute(sequence);
                this.ViewModel.Progress = ++count / total * 100;
            }
        }

        private void ContextMenu_Send_to_Anki_Click(object sender, RoutedEventArgs e)
        {
            int total = SequenceListView.SelectedItems.Count;
            int count = 0;

            this.ViewModel.Progress = 0;
            foreach (SequenceDto sequence in this.SequenceListView.SelectedItems)
            {
                this.ViewModel.SendSequenceToAnkiCommand.Execute(sequence);
                count++;
                decimal progress = Math.Round((decimal)(count / total * 100), 0);

                this.ViewModel.Progress = (int)progress;
            }
        }
    }
}
