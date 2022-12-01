using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace RecklessSpeech.Front.WPF.ViewModels
{
    public class SequenceDto : INotifyPropertyChanged //todo move to dtos
    {
        public Guid Id { get; set; }

        public string Word { get; set; }

        private string explanation;
        public string? Explanation
        {
            get
            {
                return this.explanation;
            }
            set
            {
                this.explanation = value;
                OnPropertyChanged("explanation");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new(name));
        }
    }
}