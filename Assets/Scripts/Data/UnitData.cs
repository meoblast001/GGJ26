using System;
namespace Data
{
    [Serializable]
    public class UnitData
    {
        public int id;
        public float x;
        public float y;
        public string state;

        public UnitData(int id, float x, float y, string state)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.state = state;
        }
    }
}
