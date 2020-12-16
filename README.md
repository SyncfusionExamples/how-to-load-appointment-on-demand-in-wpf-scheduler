# How to load appointments on demand in wpf scheduler
## About the sample
This examples demonstrates to load appointment on demand in wpf scheduler on two ways.
* LoadOnDemand_Event
* LoadOnDemand_Command
## LoadOnDemandEvent
Define a [Behavior](https://docs.microsoft.com/en-us/previous-versions/visualstudio/design-tools/expression-studio-4/ff726531(v=expression.40)) class for sheduler that implements `QueryAppointments` event for on-demand loading.

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

And called the LoadOnDemandBehavior class to scheduler control.

<Window x:Class="LoadOnDemand_Event.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:system="clr-namespace:System;assembly=mscorlib" 
        xmlns:interactivity="http://schemas.microsoft.com/expression/2010/interactivity"
        xmlns:local="clr-namespace:LoadOnDemand_Event"
        xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        mc:Ignorable="d" 
        WindowStartupLocation="CenterScreen"
        Title="LoadOnDemandEvent" Height="450" Width="800">

    <Window.Resources>
        <ObjectDataProvider x:Key="schedulerviewtypes" MethodName="GetValues"
                            ObjectType="{x:Type system:Enum}">
            <ObjectDataProvider.MethodParameters>
                <x:Type  Type="{x:Type syncfusion:SchedulerViewType}"/>
            </ObjectDataProvider.MethodParameters>
        </ObjectDataProvider>
    </Window.Resources>
    <Grid>
        <syncfusion:SfScheduler x:Name="scheduler"
                              ViewType="{Binding ElementName=schedulerViewType, Path=SelectedValue}">
            <syncfusion:SfScheduler.AppointmentMapping>
                <syncfusion:AppointmentMapping
                    Subject="EventName"
                    StartTime="From"
                    EndTime="To"
                    AppointmentBackground="Color"/>
            </syncfusion:SfScheduler.AppointmentMapping>
            <interactivity:Interaction.Behaviors>
                <local:LoadOnDemandBehavior />
            </interactivity:Interaction.Behaviors>
        </syncfusion:SfScheduler>
              <StackPanel
            HorizontalAlignment="Right"
            VerticalAlignment="Top"
            Margin="0,4,8,0">
            <ComboBox x:Name="viewtypecombobox" 
                      ItemsSource="{Binding Source={StaticResource schedulerviewtypes}}"
                      SelectedIndex="2" 
                      SelectionChanged="Viewtypecombobox_SelectionChanged"  
                      Width="150" Height="24"
                      VerticalAlignment="Center"/>
        </StackPanel>
    </Grid>
</Window>

## LoadOnDemandCommand
Define a ViewModel class that implements [Command](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.icommand?view=net-5.0) and handle it by CanExecute and Execute methods to check and execute on-demand loading.

    public class LoadOnDemandViewModel : NotificationObject
    {
        /// <summary>
        /// Gets or Sets event collection.
        /// </summary>
        private IEnumerable events;

        /// <summary>
        /// Gets or sets a value indicating whether to show the busy indicator.
        /// </summary>
        private bool showBusyIndicator;

        /// <summary>
        /// Gets or sets load on demand command.
        /// </summary>
        public ICommand LoadOnDemandCommand { get; set; }

        /// <summary>
        /// Gets or sets event collection.
        /// </summary>
        public IEnumerable Events
        {
            get { return events; }
            set
            {
                events = value;
                this.RaisePropertyChanged("Events");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the busy indicator.
        /// </summary>
        public bool ShowBusyIndicator
        {
            get { return showBusyIndicator; }
            set
            {
                showBusyIndicator = value;
                this.RaisePropertyChanged("ShowBusyIndicator");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadOnDemandViewModel" /> class.
        /// </summary>
        public LoadOnDemandViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                this.LoadOnDemandCommand = new DelegateCommand(ExecuteOnDemandLoading, CanExecuteOnDemandLoading);
            }
        }

        /// <summary>
        /// Method to excute load on demand command and set scheduler appointments.
        /// </summary>
        /// <param name="parameter">QueryAppointmentsEventArgs object.</param>
        public async void ExecuteOnDemandLoading(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            this.ShowBusyIndicator = true;
            await Task.Delay(500);
            await Application.Current.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                this.Events = this.GenerateSchedulerAppointments((parameter as QueryAppointmentsEventArgs).VisibleDateRange);
            }));
            this.ShowBusyIndicator = false;
        }

        /// <summary>
        /// Method to check whether the load on demand command can be invoked or not.
        /// </summary>
        /// <param name="sender">QueryAppointmentsEventArgs object.</param>
        private bool CanExecuteOnDemandLoading(object sender)
        {
            return true;
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
    }

And set the ICommand to LoadOnDemandCommand property of scheduler.

<Window x:Class="LoadOnDemand_Command.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:system="clr-namespace:System;assembly=mscorlib" 
        xmlns:local="clr-namespace:LoadOnDemand_Command"
        xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        mc:Ignorable="d" 
        WindowStartupLocation="CenterScreen"
        Title="LoadOnDemandCommand" Height="450" Width="800">

    <Window.Resources>
        <ObjectDataProvider x:Key="schedulerviewtypes" MethodName="GetValues"
                            ObjectType="{x:Type system:Enum}">
            <ObjectDataProvider.MethodParameters>
                <x:Type  Type="{x:Type syncfusion:SchedulerViewType}"/>
            </ObjectDataProvider.MethodParameters>
        </ObjectDataProvider>
    </Window.Resources>
    <Window.DataContext>
        <local:LoadOnDemandViewModel/>
    </Window.DataContext>
    <Grid>
        <syncfusion:SfScheduler x:Name="scheduler" 
                              ViewType="{Binding ElementName=viewtypecombobox, Path=SelectedValue}" 
                              ItemsSource="{Binding Events}"
                              ShowBusyIndicator="{Binding ShowBusyIndicator}"
                              LoadOnDemandCommand="{Binding LoadOnDemandCommand}">
            <syncfusion:SfScheduler.AppointmentMapping>
                <syncfusion:AppointmentMapping
                Subject="EventName"
                StartTime="From"
                EndTime="To"
                AppointmentBackground="Color"/>
            </syncfusion:SfScheduler.AppointmentMapping>
        </syncfusion:SfScheduler>
              <StackPanel
            HorizontalAlignment="Right"
            VerticalAlignment="Top"
            Margin="0,4,8,0">
            <ComboBox x:Name="viewtypecombobox" 
                      ItemsSource="{Binding Source={StaticResource schedulerviewtypes}}"
                      SelectedIndex="2" 
                      SelectionChanged="Viewtypecombobox_SelectionChanged"  
                      Width="150" Height="24"
                      VerticalAlignment="Center"/>
        </StackPanel>
    </Grid>
</Window>

