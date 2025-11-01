namespace WheelOfFortune.Models;

public class HistoryEntry
{
	public HistoryEntry(string result)
	{
		Timestamp = DateTime.Now;
		Result = result;
	}

	public DateTime Timestamp { get; }
	public string Result { get; }
}
