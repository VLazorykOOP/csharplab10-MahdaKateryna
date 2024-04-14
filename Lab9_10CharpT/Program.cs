using System;

namespace OceanLinerEvents
{
    public delegate void EmergencyEventHandler(object sender, EmergencyEventArgs e);

    /// <summary>
    /// Модель океанського лайнера з подіями та реакцією служб на них.
    /// </summary>
    public class OceanLiner
    {
        string shipName; // назва судна
        int decks; // кількість палуб
        int days; // кількість днів подій

        // служби на борту
        Security securityTeam;
        Medical medicalTeam;
        Engineering engineeringTeam;

        // події на борту
        public event EmergencyEventHandler Emergency;

        // результати дій служб
        string[] serviceResults;

        // випадкові події
        Random rnd = new Random();

        // ймовірність виникнення аварії на кораблі в поточний день
        double emergencyProbability;

        /// <summary>
        /// Конструктор океанського лайнера.
        /// Створює служби та розпочинає спостереження за подіями.
        /// </summary>
        public OceanLiner(string name, int decks, int days)
        {
            shipName = name;
            this.decks = decks;
            this.days = days;
            emergencyProbability = 5e-3; // встановлення початкової ймовірності аварії

            // створення служб
            securityTeam = new Security(this);
            medicalTeam = new Medical(this);
            engineeringTeam = new Engineering(this);

            // підключення до спостереження за подіями
            securityTeam.On();
            medicalTeam.On();
            engineeringTeam.On();
        }

        /// <summary>
        /// Запуск події.
        /// Послідовно викликаються обробники події.
        /// </summary>
        protected virtual void OnEmergency(EmergencyEventArgs e)
        {
            const string MESSAGE_EMERGENCY = "На кораблі {0} виникла аварія! Палуба {1}. День {2}-й";
            Console.WriteLine(string.Format(MESSAGE_EMERGENCY, shipName, e.Deck, e.Day));

            if (Emergency != null)
            {
                Delegate[] eventHandlers = Emergency.GetInvocationList();
                serviceResults = new string[eventHandlers.Length];
                int k = 0;
                foreach (EmergencyEventHandler eventHandler in eventHandlers)
                {
                    eventHandler(this, e);
                    serviceResults[k++] = e.Result;
                }
            }
        }

        /// <summary>
        /// Моделювання життя на океанському лайнері.
        /// </summary>
        public void LifeOnboard()
        {
            const string ALL_CLEAR = "На кораблі {0} все спокійно! Аварій не було.";
            bool wasEmergency = false;

            for (int day = 1; day <= days; day++)
            {
                for (int deck = 1; deck <= decks; deck++)
                {
                    if (rnd.NextDouble() < emergencyProbability)
                    {
                        EmergencyEventArgs e = new EmergencyEventArgs(deck, day);
                        OnEmergency(e);
                        wasEmergency = true;
                        for (int i = 0; i < serviceResults.Length; i++)
                        {
                            Console.WriteLine(serviceResults[i]);
                        }
                    }
                }
            }

            if (!wasEmergency)
            {
                Console.WriteLine(string.Format(ALL_CLEAR, shipName));
            }
        }
    }

    public abstract class Service
    {
        protected OceanLiner ship;
        protected Random rnd = new Random();

        public Service(OceanLiner ship)
        {
            this.ship = ship;
        }

        public void On()
        {
            ship.Emergency += new EmergencyEventHandler(HandleEmergency);
        }

        public void Off()
        {
            ship.Emergency -= new EmergencyEventHandler(HandleEmergency);
        }

        public abstract void HandleEmergency(object sender, EmergencyEventArgs e);
    }

    public class Security : Service
    {
        public Security(OceanLiner ship) : base(ship) { }

        public override void HandleEmergency(object sender, EmergencyEventArgs e)
        {
            const string OK = "Причину аварії виявлено!";
            const string NOK = "Причину аварії не виявлено! Аварія триває.";
            if (rnd.Next(0, 10) > 6)
                e.Result = OK;
            else
                e.Result = NOK;
        }
    }

    public class Medical : Service
    {
        public Medical(OceanLiner ship) : base(ship) { }

        public override void HandleEmergency(object sender, EmergencyEventArgs e)
        {
            const string OK = "Медичний персонал надав допомогу!";
            const string NOK = "Є поранені! Потрібні медикаменти.";
            if (rnd.Next(0, 10) > 2)
                e.Result = OK;
            else
                e.Result = NOK;
        }
    }

    public class Engineering : Service
    {
        public Engineering(OceanLiner ship) : base(ship) { }

        public override void HandleEmergency(object sender, EmergencyEventArgs e)
        {
            const string OK = "Інженери усунули проблему!";
            const string NOK = "Проблема з технікою! Потрібен ремонт.";
            if (rnd.Next(0, 10) > 4)
                e.Result = OK;
            else
                e.Result = NOK;
        }
    }

    /// <summary>
    /// Клас, що задає вхідні та вихідні параметри події.
    /// </summary>
    public class EmergencyEventArgs : EventArgs
    {
        int deck;
        int day;
        string result;

        public int Deck
        {
            get
            {
                return deck;
            }
        }
        public int Day { get { return day; } }
        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        public EmergencyEventArgs(int deck, int day)
        {
            this.deck = deck;
            this.day = day;
        }
    }

    public class DRomb
    {
        private int d1;
        private int d2;
        private readonly int color;

        public DRomb(int d1, int d2, int color)
        {
            if (d1 <= 0 || d2 <= 0)
            {
                throw new ArgumentException("Діагоналі ромба повинні бути додатніми числами.");
            }

            this.d1 = d1;
            this.d2 = d2;
            this.color = color;
        }

        public int D1
        {
            get { return d1; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Діагональ повинна бути додатнім числом.");
                }
                d1 = value;
            }
        }

        public int D2
        {
            get { return d2; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Діагональ повинна бути додатнім числом.");
                }
                d2 = value;
            }
        }

        public int Color
        {
            get { return color; }
        }

        public double CalculatePerimeter()
        {
            return 2 * (Math.Sqrt(Math.Pow(d1 / 2.0, 2) + Math.Pow(d2 / 2.0, 2)));
        }

        public double CalculateArea()
        {
            return (d1 * d2) / 2.0;
        }

        public bool IsSquare()
        {
            return d1 == d2;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                Console.WriteLine("\t\t\tTask 1 ");
                Console.WriteLine();

                DRomb[] rombs = new DRomb[5];
                rombs[0] = new DRomb(5, 8, 1);
                rombs[1] = new DRomb(7, 7, 2);
                rombs[2] = new DRomb(4, 4, 3);
                rombs[3] = new DRomb(6, 6, 3);
                rombs[4] = new DRomb(8, 3, 4);

                int squareCount = 0;

                foreach (var romb in rombs)
                {
                    Console.WriteLine("Ромб з діагоналями {0} і {1}, має колір {2}.", romb.D1, romb.D2, romb.Color);
                    Console.WriteLine("Периметр ромба: {0}", romb.CalculatePerimeter());
                    Console.WriteLine("Площа ромба: {0}", romb.CalculateArea());
                    if (romb.IsSquare())
                    {
                        squareCount++;
                        Console.WriteLine("Ромб є квадратом.");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Ромб не є квадратом.");
                        Console.WriteLine();
                    }
                }

                Console.WriteLine("Загальна кількість квадратів: {0}", squareCount);
                Console.WriteLine("\n");
            }
            catch (ArrayTypeMismatchException e)
            {
                Console.WriteLine($"Помилка типу масиву: {e.Message}");
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine($"Помилка ділення на нуль: {e.Message}");
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"Вихід за межі масиву: {e.Message}");
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine($"Недійсне приведення типів: {e.Message}");
            }
            catch (OutOfMemoryException e)
            {
                Console.WriteLine($"Недостатньо пам'яті: {e.Message}");
            }
            catch (OverflowException e)
            {
                Console.WriteLine($"Переповнення: {e.Message}");
            }
            catch (StackOverflowException e)
            {
                Console.WriteLine($"Переповнення стеку: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Загальна помилка: {e.Message}");
            }

            Console.WriteLine("\t\t\tTask 2 ");

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Проект 'Життя океанського лайнера'");

            OceanLiner titanic = new OceanLiner("Titanic", 10, 50);
            titanic.LifeOnboard();

        }
    }

}
