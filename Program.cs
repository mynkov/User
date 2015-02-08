using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Moq;

namespace ConsoleApplication17
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            var handlerProvider = new HandlerProvider(new ServiceProvider());
            handlerProvider.Register<EventHandler>();
            var eventManager = new EventManager(handlerProvider);
            eventManager.RaiseEvent(new MyEvent());
            eventManager.RaiseEvent(new MyEvent2());
            eventManager.RaiseEvent(new MyEvent());
        }

        static void Test()
        {
            var testEvent = new TestEvent();
            var handler = new Mock<IEventHandler<TestEvent>>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IEventHandler<TestEvent>))).Returns(() => handler.Object);
            var handlerProvider = new HandlerProvider(serviceProvider.Object);
            handlerProvider.Register<IEventHandler<TestEvent>>();
            var eventManager = new EventManager(handlerProvider);          
   
            eventManager.RaiseEvent(testEvent);

            handler.Verify(x => x.Handle(testEvent));
        }
    }

    /// <summary>
    /// Интерфейс для управления событиями.
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Сгенерировать событие.
        /// </summary>
        void RaiseEvent<TEvent>(TEvent @event)
            where TEvent : Event;
    }

    /// <summary>
    /// Класс для управления событиями.
    /// </summary>
    public class EventManager : IEventManager
    {
        private readonly HandlerProvider _handlerProvider;

        public EventManager(HandlerProvider handlerProvider)
        {
            _handlerProvider = handlerProvider;
        }

        /// <summary>
        /// Сгенерировать событие.
        /// </summary>
        public void RaiseEvent<TEvent>(TEvent @event)
            where TEvent : Event
        {
            foreach (dynamic handler in _handlerProvider.GetHandlers(typeof(TEvent)))
            {
                handler.Handle(@event);
            }
        }
    }

    /// <summary>
    /// Класс предоставляющий обработчиков событий.
    /// </summary>
    public class HandlerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Type> _handlerTypes = new List<Type>();
        private readonly ConcurrentDictionary<Type, List<Type>> _map = new ConcurrentDictionary<Type, List<Type>>();

        public HandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Зарегистрировать тип обработчика события. 
        /// </summary>
        public void Register<THandler>()
            where THandler : IEventHandler
        {
            _handlerTypes.Add(typeof(THandler));
        }

        /// <summary>
        /// Получить экземпляры обработчиков для типа события.
        /// </summary>
        public List<object> GetHandlers(Type eventType)
        {
            var handlerTypes = _map.GetOrAdd(eventType, GetHandlerTypes);
            return handlerTypes.Select(_serviceProvider.GetService).ToList();
        }

        /// <summary>
        /// Получить типы обработчиков для типа события.
        /// </summary>
        private List<Type> GetHandlerTypes(Type eventType)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handlers = _handlerTypes.Where(handlerType.IsAssignableFrom).ToList();
            return handlers;
        }
    }

    public class ServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new EventHandler();
        }
    }

    /// <summary>
    /// Базовый интерфейс для обработчиков событий.
    /// </summary>
    public interface IEventHandler
    {     
    }

    /// <summary>
    /// Интерфейс для обработчиков событий.
    /// </summary>
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : Event
    {
        void Handle(TEvent @event);
    }

    /// <summary>
    /// Базовый класс для событий.
    /// </summary>
    public abstract class Event
    {
    }

    public class TestEvent : Event
    {
    }

    public class MyEvent : Event
    {
    }

    public class MyEvent2 : Event
    {
    }

    public class EventHandler : IEventHandler<MyEvent>, IEventHandler<MyEvent2>
    {
        public void Handle(MyEvent @event)
        {
            Console.WriteLine("Event");
        }

        public void Handle(MyEvent2 @event)
        {
            Console.WriteLine("Event2");
        }
    }
}
