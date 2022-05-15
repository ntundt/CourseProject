using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.Model;
using CourseProjectClient.Services;
using DataTransferObject;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class TestEditViewModel : INotifyPropertyChanged, IDropTarget
    {
        public ICommand AddAnswerOptionCommand { get; set; }
        public ICommand CreateQuestionCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        List<EventHandler<PropertyChangedEventArgs>> changedHandlers =
            new List<EventHandler<PropertyChangedEventArgs>>();

        public void AddQuestion(Question question)
        {
            _questions.Add(question);
            question.PropertyChanged += SelectedQuestionChanged;
            changedHandlers.Add(SelectedQuestionChanged);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
        }
        public void SelectedQuestionChanged(object sender, PropertyChangedEventArgs e)
        {
            Question child = sender as Question;
            if (_questions.Contains(child))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddAnswerVisible)));
            }
        }

        public void SetSelectedQuestion(int index)
        {
            SelectedQuestion = _questions.First(x => x.Index == index);
        }

        public void DragEnter(IDropInfo dropInfo) { }

        public void DragOver(IDropInfo dropInfo) {
            Question question = dropInfo.TargetItem as Question;

            if (question != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void DragLeave(IDropInfo dropInfo) { }

        public void Drop(IDropInfo dropInfo) 
        {
            Question source = dropInfo.Data as Question;
            Questions.Remove(source);
            Questions.Insert(dropInfo.InsertIndex, source);

            SelectedQuestion = source;

            // TODO: Server request

            UpdateQuestionIndexes();
        }

        private Attempt _attempt;
        public Attempt Attempt
        {
            get => _attempt;
            set
            {
                _attempt = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Attempt)));
            }
        }

        private ObservableCollection<Question> _questions = new ObservableCollection<Question>();
        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
            }
        }
        private void UpdateQuestionIndexes()
        {
            int index = 1;
            foreach (Question question in _questions)
            {
                question.Index = index++;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddAnswerVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionType)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckAlgorithm)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionText)));
            }
        }

        public ObservableCollection<AnswerOption> AnswerOptions
        {
            get => _selectedQuestion.AnswerOptions;
            set
            {
                _selectedQuestion.AnswerOptions = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
            }
        }

        private string _answerOptionConstructed;
        public string AnswerOptionConstructed
        {
            get => _answerOptionConstructed;
            set
            {
                _answerOptionConstructed = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptionConstructed)));
            }
        }

        public QuestionType QuestionType
        {
            get => _selectedQuestion.QuestionType;
            set
            {
                _selectedQuestion.QuestionType = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionType)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddAnswerVisible)));
            }
        }

        public CheckAlgorithm? CheckAlgorithm
        {
            get => _selectedQuestion.CheckAlgorithm;
            set
            {
                _selectedQuestion.CheckAlgorithm = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckAlgorithm)));
            }
        }

        public string QuestionText
        {
            get => _selectedQuestion.Text;
            set
            {
                _selectedQuestion.Text = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionText)));
            }
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

        public bool AddAnswerVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.MultipleChoise
                || _selectedQuestion.QuestionType == QuestionType.SingleChoise;
        }

        public TestEditViewModel()
        {
            Question question = new Question()
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

            AddQuestion(question);

            _selectedQuestion = question;

            _answerOptionConstructed = "";

            AddAnswerOptionCommand = new RelayCommand(() =>
            {
                SelectedQuestion.AnswerOptions.Add(new AnswerOption { Text = AnswerOptionConstructed });
                AnswerOptionConstructed = "";
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
            }, () => !_answerOptionConstructed.Equals(""));

            CreateQuestionCommand = new RelayCommand(() =>
            {
                Question newQuestion = new Question 
                {
                    Index = _questions.Select(x => x.Index).Max() + 1
                };
                _questions.Add(newQuestion);
                SelectedQuestion = newQuestion;
            });

            SaveCommand = new RelayCommand(() =>
            {
                Task.Run(async () => await CommunicationService.GetAuth("Mikita"));
            });
        }
    }
}
