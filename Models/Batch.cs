namespace BeerMachineApi.Models
{
    public class Batch
    {
        private int _id;
        private BeerType _type;
        private int _amount;
        private int _speed;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public BeerType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public int Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Batch(int id, BeerType type, int amount, int speed)
        {
            _id = id;
            _type = type;
            _amount = amount;
            _speed = speed;
        }
    }
}