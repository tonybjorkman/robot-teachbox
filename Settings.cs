using System;

namespace console_jogger
{
    public class MovementType
    {
        Command Type;
        int MinStep;
        int MaxStep;
        int CurrentStep;
        public MovementType(Command type,int min_step,int max_step,int start_step)
        {
            Type=type;
            MinStep=min_step;
            MaxStep=max_step;
            CurrentStep=start_step;
        }

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
    public class Settings
    {
        MyScreen View; //skipping observer pattern in this simple app. direct access.
        
        public Settings(MyScreen view){
            View = view;
            XyzMove = new MovementType(Command.MoveXYZ,1,50,20);
            AngleMove = new MovementType(Command.MoveAngle,1,50,20);
            CurrentMoveType = XyzMove;
        }
        private MovementType XyzMove;
        private MovementType AngleMove;
        private MovementType CurrentMoveType;

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
        }

        public int GetCurrentStep()
        {
            return CurrentMoveType.GetStep();
        }
    }
}