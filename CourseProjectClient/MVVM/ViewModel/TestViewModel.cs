using CourseProjectClient.MVVM.Model;
using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class TestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private Attempt _attempt;
        public Attempt Attempt
        { 
            get => _attempt; 
            set
            {
                _attempt = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Attempt"));
            } 
        }

        private ObservableCollection<Question> _questions;
        public ObservableCollection<Question> Questions 
        { 
            get => _questions; 
            set
            {
                _questions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Questions"));
            }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion 
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedQuestion"));
            } 
        }

        public string TimeLeft
        {
            get {
                if (_attempt.Ended == null) return "";
                TimeSpan timeLeft = _attempt.Ended.Value - DateTime.Now;
                return $"{timeLeft.TotalHours}:{timeLeft.TotalMinutes}:{timeLeft.TotalSeconds}";
            }
        }

        public bool HasTimeLimit
        {
            get => _attempt.Ended <= _attempt.Started;
        }

        public bool RadioButtonsVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.SingleChoise;
        }

        public bool CheckboxesVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.MultipleChoise;
        }

        public bool StringInputVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.StringInput;
        }

        public TestViewModel()
        {
            _selectedQuestion = new Question()
            {
                Id = 1,
                Text = "what is obamas last name",
                Index = 1,
                QuestionType = QuestionType.MultipleChoise,
                AnswerOptions = new ObservableCollection<AnswerOption>()
                {
                    new AnswerOption()
                    {
                        Id = 1,
                        Text = "barack",
                        IsChecked = true
                    },
                    new AnswerOption()
                    {
                        Id = 1,
                        Text = "oabma"
                    }
                }
            };

            _attempt = new Attempt
            {
                Id = 2,
                TestId = 1,
                UserId = 2,
                Started = DateTime.Now,
                Ended = DateTime.Now + new TimeSpan(1,0,0)
            };

            _questions = new ObservableCollection<Question>()
            {
               _selectedQuestion
            };
        }
    }
}
