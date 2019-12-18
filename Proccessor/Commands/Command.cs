using System;
using System.Threading.Tasks;
using DomoticzToRouterSmsBot.Loader;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
  internal abstract class Command : ICommand
  {
    private readonly Func<Sms, Task> _eventHandler;
    protected Command Next { get; set; }

    protected Command()
    {
      _eventHandler = MiddlewareHandler;
    }

    public async Task Handle(Sms sms)
    {
      await OnInvoke(sms);
    }

    public virtual async Task OnInvoke(Sms e)
    {
      await _eventHandler?.Invoke(e);
      await Next?.MiddlewareHandler(e);
    }

    public abstract Task MiddlewareHandler(Sms e);

    public Command Use(Command next)
    {
      return Next = next;
    }
  }
}