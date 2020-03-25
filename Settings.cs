namespace console_jogger
{
    public class Settings
    {
        MyScreen View; //skipping observer pattern in this simple app. direct access.
        
        public Settings(MyScreen view){
            View = view;
        }
        public Command MoveType = Command.MoveXYZ;

        int _xyzStep = 1;
        public int xyzStep {get{return _xyzStep;}
                            set{_xyzStep = value;}
        }
        public int angleStep {get;set;}

        public void Inc(){
            if(_xyzStep < 50) {
                _xyzStep*=2;
                System.Console.WriteLine($"Step increased to {_xyzStep}");
            }
        }
        
        public void Dec(){
            if(_xyzStep >= 2){
                _xyzStep/=2;
                System.Console.WriteLine($"Step decreased to {_xyzStep}");
            }

        }
        
    }
}