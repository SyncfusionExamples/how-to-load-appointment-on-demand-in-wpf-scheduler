using Syncfusion.UI.Xaml.Scheduler;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;

namespace LoadOnDemand_Event
{
    public class LoadOnDemandBehavior : Behavior<SfScheduler>
    {
        SfScheduler scheduler;
        protected override void OnAttached()
        {
            this.scheduler = this.AssociatedObject;
            this.scheduler.QueryAppointments += OnSchedulerQueryAppointments;
        }

        private async void OnSchedulerQueryAppointments(object sender, QueryAppointmentsEventArgs e)
        {
            this.scheduler.ShowBusyIndicator = true;
            await Task.Delay(1000);
            await this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                this.scheduler.ItemsSource = this.GenerateSchedulerAppointments(e.VisibleDateRange);
            }));
            this.scheduler.ShowBusyIndicator = false;
        }


        /// <summary>
        /// Method to generate scheduler appointments based on current visible date range.
        /// </summary>
        /// <param name="dateRange">Current visible date range.</param>
        private IEnumerable GenerateSchedulerAppointments(DateRange dateRange)
        {
            var brush = new ObservableCollection<SolidColorBrush>();
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0xC1, 0x39)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0x00, 0x73)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0x71, 0xB8)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0x96, 0x09)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x99, 0x33)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAB, 0xA9)));
            brush.Add(new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0x71, 0xB8)));

            var subjectCollection = new ObservableCollection<string>();
            subjectCollection.Add("Business Meeting");
            subjectCollection.Add("Conference");
            subjectCollection.Add("Medical check up");
            subjectCollection.Add("Performance Check");
            subjectCollection.Add("Consulting");
            subjectCollection.Add("Project Status Discussion");
            subjectCollection.Add("Client Meeting");
            subjectCollection.Add("General Meeting");
            subjectCollection.Add("Yoga Therapy");
            subjectCollection.Add("GoToMeeting");
            subjectCollection.Add("Plan Execution");
            subjectCollection.Add("Project Plan");

            Random ran = new Random();
            int daysCount = (dateRange.ActualEndDate - dateRange.ActualStartDate).Days;
            var appointments = new ObservableCollection<SchedulerModel>();
            for (int i = 0; i < 50; i++)
            {
                var startTime = dateRange.ActualStartDate.AddDays(ran.Next(0, daysCount + 1)).AddHours(ran.Next(0, 24));
                appointments.Add(new SchedulerModel
                {
                    From = startTime,
                    To = startTime.AddHours(1),
                    EventName = subjectCollection[ran.Next(0, subjectCollection.Count)],
                    Color = brush[ran.Next(0, brush.Count)],
                });
            }

            return appointments;
        }

        protected override void OnDetaching()
        {
            this.scheduler.QueryAppointments -= OnSchedulerQueryAppointments;
            this.scheduler = null;
        }
    }
}
