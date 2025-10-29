namespace BeerMachineApi.Models
{
    public class Batch
    {
        private float _id;
        private BeerType _type;
        private float _amount;
        private float _speed;

        public float Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public BeerType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public float Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Batch(float id, BeerType type, float amount, float speed)
        {
            _id = id;
            _type = type;
            _amount = amount;
            _speed = speed;
        }
    }
}