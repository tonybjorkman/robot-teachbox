using System;
using System.ComponentModel;

namespace robot_teachbox
{
    public class MovementType : INotifyPropertyChanged
    {
        public Command Type;
        int MinStep;
        int MaxStep;
        int _currentStep;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int CurrentStep { get { return _currentStep; } set { _currentStep = value;RaisePropertyChanged("CurrentStep"); } }
        public MovementType(Command type,int min_step,int max_step,int start_step)
        {
            Type=type;
            MinStep=min_step;
            MaxStep=max_step;
            CurrentStep=start_step;
        }

        private string _name;
        public string Name { get { return this.Type.ToString(); } set {} }

        public int GetStep(){
            return CurrentStep;
        }
        public void IncStep(){
            if (StepInLimit(CurrentStep*2)){
                CurrentStep*=2;
            }
        }

        public void DecStep(){
            if (StepInLimit(CurrentStep/2)){
                CurrentStep/=2;
            }
        }

        private bool StepInLimit(int val){
            return (val <= MaxStep && val >= MinStep);
        }

        public override string ToString(){
            return Type.ToString()+" Step:"+CurrentStep.ToString();
        }
    }
    public class Settings : INotifyPropertyChanged
    {
        public MyScreen view; //skipping observer pattern in this simple app. direct access.

        public int StepLength { get { return CurrentMoveType.GetStep(); } set { } }

        public Settings(MyScreen view) {
            this.view = view;
            XyzMove = new MovementType(Command.MoveXYZ, 1, 50, 20);
            AngleMove = new MovementType(Command.MoveAngle, 1, 50, 20);
            CurrentMoveType = XyzMove;
        }
        private MovementType XyzMove;
        private MovementType AngleMove;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private MovementType _currentMoveType;
        public MovementType CurrentMoveType { get { return _currentMoveType; } set { _currentMoveType = value; RaisePropertyChanged("CurrentMoveType"); } }

        public string port { get; private set; }

        public void Inc(){
            CurrentMoveType.IncStep();
            System.Console.WriteLine(CurrentMoveType);
        }
        
        public void Dec(){
            CurrentMoveType.DecStep();
            System.Console.WriteLine(CurrentMoveType);

        }

        public void SetMovType(Command type){
            var newType = type switch  {
                Command.MoveAngle => AngleMove,
                Command.MoveXYZ => XyzMove,
                _ => null
            };
            CurrentMoveType = newType;
            System.Console.WriteLine($"Movetype: {CurrentMoveType.ToString()}");

        }

        internal void SetPort(string port)
        {
            this.port = port;
        }

        public int GetCurrentStep()
        {
            return CurrentMoveType.GetStep();
        }
    }
}