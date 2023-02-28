namespace SerialConsole.Models
{
    public class AppSettings
    {
        public string PortName { get; set; } = null!;
        public int BaudRate { get; set; }
        public int Parity { get; set; } // None = 0, Odd = 1, Even = 2, Mark = 3, Space = 4
        public int DataBits { get; set; }
        public int StopBits { get; set; } // None = 0, One = 1, Two = 2, OnePointFive = 3
    }
}