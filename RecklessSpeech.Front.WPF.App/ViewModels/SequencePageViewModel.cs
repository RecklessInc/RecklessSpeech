﻿using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecklessSpeech.Front.WPF.App.ViewModels
{
    public class SequencePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<SequenceDto> sequences;

        public ObservableCollection<SequenceDto> Sequences
        {
            get => this.sequences;
            init
            {
                this.sequences = value;
                OnPropertyChanged("Sequences");
            }
        }

        private int progress;
        private readonly HttpBackEndGateway backEndGateway;

        public int Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                this.progress = value;
                OnPropertyChanged("Progress");
            }
        }



        //commands
        public ICommand AddSequencesCommand { get; }
        public ICommand EnrichSequenceCommand { get; }
        public ICommand SendSequenceToAnkiCommand { get; }


        public SequencePageViewModel(HttpBackEndGateway backEndGateway) 
        {
            this.backEndGateway = backEndGateway;
            this.Sequences = new ObservableCollection<SequenceDto>();

            this.AddSequencesCommand = new DelegateCommand<string>(async s => await AddSequences(s));
            this.EnrichSequenceCommand = new DelegateCommand<SequenceDto>(async s => await EnrichSequence(s));
            this.SendSequenceToAnkiCommand = new DelegateCommand<SequenceDto>(async s => await SendSequenceToAnki(s));
        }

        private async Task AddSequences(string filePath)
        {
            await this.backEndGateway.ImportSequencesFromCsvFile(filePath);

            IReadOnlyCollection<SequenceDto> newSequences = await this.backEndGateway.GetAllSequences();
            this.Sequences.Clear();
            
            foreach (SequenceDto newSequence in newSequences)
            {
                this.Sequences.Add(newSequence);
            }
        }
        
        private async Task EnrichSequence(SequenceDto sequence)
        {
            await this.backEndGateway.EnrichSequence(sequence.Id);

            SequenceDto updatedSequence = await this.backEndGateway.GetOneSequence(sequence.Id);

            sequence.Explanation = updatedSequence.Explanation;
        }
        
        private async Task SendSequenceToAnki(SequenceDto sequence)
        {
            await this.backEndGateway.SendSequenceToAnki(sequence.Id);
        }
    }
}