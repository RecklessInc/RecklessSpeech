using Microsoft.Win32;
using RecklessSpeech.Front.WPF.Dtos;
using RecklessSpeech.Front.WPF.ViewModels;
using System;
using System.Collections;
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
        private List<MenuItem> contextMenuItems;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new SequencePageViewModel(new(new HttpBackEndGatewayAccess()));
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button? button = (sender as Button);
            DictionaryDto? dictionaryDto = button!.DataContext as DictionaryDto;
            IList selectedItems = this.SequenceListView.SelectedItems;

            foreach (SequenceDto sequenceDto in selectedItems)
            {
                AssignDictionaryToASequenceDto parameter = new(sequenceDto, dictionaryDto!.Id);
                this.ViewModel.AssignDictionaryToASequenceCommand.Execute(parameter);
            }
        }
    }
}
