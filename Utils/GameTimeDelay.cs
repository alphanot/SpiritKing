using System;

namespace SpiritKing.Utils;
public class GameTimeDelay : IDisposable
{
    private readonly Action _callback;
    private float _sleepTime = 0f;
    private float _timer = 0f;
    private bool _running = false;

    private bool disposedValue;

    public GameTimeDelay(Action callback, float sleepTime)
    {
        _callback = callback;
        _sleepTime = sleepTime;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                disposedValue = true;
            }
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Start()
    {
        _running = true;
    }

    public void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_running)
        {
            _timer += seconds;
            if (_timer > _sleepTime)
            {
                _running = false;
                _timer = 0f;
                _callback.Invoke();

            }
        }
    }

    public void SetSleepTime(float time) => _sleepTime = time;
}
