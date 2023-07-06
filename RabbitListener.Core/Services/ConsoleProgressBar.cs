namespace RabbitListener.Core.Services;

public class ConsoleProgressBar : IProgress<float>
{
    private readonly object _consoleLock = new();
    private const int ProgressBarWidth = 150;

    private int Value { 
        get => _value;
        set
        {
            _value = value;
            DrawProgressBar();
        }
    }
    private int _value;
    private int _maxValue;

    public void Init(int maxValue)
    {
        _maxValue = maxValue;
        Value = 0;
    }
    
    public void Report(float _ = 0)
    {
        lock (_consoleLock)
        {
            Value++;
        }
    }

    private void DrawProgressBar()
    {
        var percentage = (float)Value * 100 / _maxValue;
        
        var progressWidth = (int)(percentage / 100 * ProgressBarWidth);
        var progressBar = new string('#', progressWidth) + new string('-', ProgressBarWidth - progressWidth);

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write($"Progress: [{progressBar}] {percentage:000.00}%");
        
        if (_value == _maxValue)
        {
            Console.Write("\n");
        }
    }
}
