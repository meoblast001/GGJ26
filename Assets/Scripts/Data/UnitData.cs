using System;
namespace Data
{
    [Serializable]
    public class UnitData
    {
        public int id;
        public int x;
        public int y;
        public string state;

        public UnitData(int id, int x, int y, string state)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.state = state;
        }
    }
}
