namespace BankReader.Messages
{
    public class BusyMessage
    {
        public BusyMessage(bool isbusy)
        {
            IsBusy = isbusy;
        }

        public bool IsBusy { get; set; }
    }
}