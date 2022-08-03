namespace TTI_Calculations
{
    public enum Type
    {
        None = 0,
        Capacitor = 1,
        Resistor = 2,
        Power = 3,
        Cost = 4
    }
    public class Component
    {
        public Type type = Type.None;
        public double value = 0;
        public double power = 0;
        public double cost = 0;
        public string tag = "";
    }
    public class ADC
    {
        public Component R1;
        public Component R2;
        public Component C;
        public double totalCost = double.PositiveInfinity;

        public ADC()
        {
            R1 = new Component();
            R2 = new Component();
            C = new Component();
        }
    }
    
    class Program
    {
        static double R1divR2 = 46.7 / 3.3;
        static double Rz = 1 / (15 * Math.Pow(10, -6));
        static double fs = 5000;
        static double maxError = 0.001;
        static double maxRatedRatio = 0.5;
        static double maxVoltage = 50;

        static void Main(string[] args)
        {
            List<ADC> adcList = new List<ADC>();
            ADC adc = new ADC();
            string[] commandList = { "quit", "print", "print list",
                "R1 range", "R2 range", "C nom",
                "set R1", "set R2", "set C",
                "max power", "add to list", "sort by cost" };

            while (true)
            {
                Console.Write("Enter a Command: ");
                string? command = Console.ReadLine();
                
                if (Array.IndexOf(commandList, command) == -1)
                {
                    PrintCommands(commandList);
                }
                else if (command == "quit")
                {
                    break;
                }
                else if (command == "print")
                {
                    PrintADC(adc);
                }
                else if (command == "print list")
                {
                    if (adcList.Count == 0)
                        Console.WriteLine("The list is empty.");
                    else foreach (ADC aDC in adcList)
                            PrintADC(aDC);
                }
                else if (command == "R1 range")
                {
                    PrintR1Range(adc.R2);
                }
                else if (command == "R2 range")
                {
                    PrintR2Range(adc.R1);
                }
                else if (command == "C nom")
                {
                    PrintCnomValue(adc.R1, adc.R2);
                }
                else if (command == "set R1")
                {
                    adc.R1 = NewComponent(Type.Resistor);
                }
                else if (command == "set R2")
                {
                    adc.R2 = NewComponent(Type.Resistor);
                }
                else if (command == "set C")
                {
                    adc.C = NewComponent(Type.Capacitor);
                }
                else if (command == "max power")
                {
                    PrintIfRisAcceptable(adc);
                }
                else if (command == "add to list")
                {
                    if (IsRAcceptable(adc))
                        adcList.Add(adc);
                    else
                        Console.WriteLine("The power rating is too low");
                }
                else if (command == "sort by cost")
                {
                    adcList = SortByCost(adcList);
                }
                else
                {
                    Console.WriteLine("Command not implemented.");
                }

                // Update Total Cost
                if ((adc.R1.cost != 0) && (adc.R2.cost != 0)
                    && (adc.C.cost != 0))
                {
                    adc.totalCost = adc.R1.cost + adc.R2.cost
                        + adc.C.cost;
                }

                Console.WriteLine();
            }
        }

        private static void PrintCommands(string[] commandList)
        {
            Console.WriteLine("Command List:");
            foreach (string command in commandList)
                Console.WriteLine(command);
        }

        private static void PrintADC(ADC adc)
        {
            if (adc == null)
            {
                Console.WriteLine("Error: adc = null");
                return;
            }

            PrintComponent(adc.R1, "R1");
            PrintComponent(adc.R2, "R2");
            PrintComponent(adc.C, "C");

            if (adc.totalCost == double.PositiveInfinity)
                Console.WriteLine("The total cost isn't set.");
            else
                Console.WriteLine("The Total Cost is $" + adc.totalCost);
        }
        private static void PrintComponent(Component component,
            string name)
        {
            if (component == null)
            {
                Console.WriteLine("Error: " + name + " = null");
                return;
            }
            else if (component.value == 0)
            {
                Console.WriteLine("Component is not set");
            }
            else
            {
                Console.WriteLine(name);
                if (component.type == Type.Resistor)
                {
                    Console.WriteLine("R = "
                        + PrintValue(component.value));
                    Console.WriteLine("Power Rating: "
                        + Math.Round(component.power, 5));
                }
                else if (component.type == Type.Capacitor)
                {
                    Console.WriteLine("C = "
                        + PrintValue(component.value));
                }
                else
                {
                    Console.WriteLine("Error: component is invalid "
                        + "type");
                    return;
                }
                Console.WriteLine("Cost = $"
                    + Math.Round(component.cost, 2));
                Console.WriteLine("Tag: " + component.tag);
            }
        }

        private static Component NewComponent(Type type)
        {
            Component component = new Component();
            component.type = type;
            component.value = ReadNumber(type);
            if (type == Type.Resistor)
                component.power = ReadNumber(Type.Power);
            component.cost = ReadNumber(Type.Cost);
            component.tag = ReadTag();
            return component;
        }
        private static double ReadNumber(Type type)
        {
            if (type == Type.None) return double.NaN;

            double value;
            while (true)
            {
                if (type == Type.Resistor)
                    Console.Write("R = ");
                else if (type == Type.Capacitor)
                    Console.Write("C = ");
                else if (type == Type.Power)
                    Console.Write("Power Rating = ");
                else if (type == Type.Cost)
                    Console.Write("Cost = $");

                string? input = Console.ReadLine();
                if (input == null) continue;
                try
                {
                    char prefix = input[input.Length - 1];
                    value = Convert.ToDouble(
                        input.Substring(0, input.Length - 1));
                    switch (prefix)
                    {
                        case 'M':
                            value *= Math.Pow(10, 6);
                            break;
                        case 'k':
                            value *= Math.Pow(10, 3);
                            break;
                        case 'm':
                            value *= Math.Pow(10, -3);
                            break;
                        case 'u':
                            value *= Math.Pow(10, -6);
                            break;
                        case 'n':
                            value *= Math.Pow(10, -9);
                            break;
                        case 'p':
                            value *= Math.Pow(10, -12);
                            break;
                        default:
                            value = Convert.ToDouble(input);
                            break;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return value;
        }
        private static string ReadTag()
        {
            Console.Write("Tag: ");
            string? tag = Console.ReadLine();
            if (tag != null) return tag;
            else return "";
        }

        private static void PrintR1Range(Component R2)
        {
            if (R2.value == 0) return;

            double R2prime = R2.value * Rz / (Rz + R2.value);
            double R1 = R2.value * R1divR2;
            Console.WriteLine(PrintValue((1 - maxError) * R1)
                + " <= R1 <= " + PrintValue((1 + maxError) * R1));
        }
        private static void PrintR2Range(Component R1)
        {
            if (R1.value == 0) return;

            double R2prime = R1.value / R1divR2;
            double R2 = R2prime * Rz / (Rz - R2prime);
            Console.WriteLine(PrintValue((1 - maxError) * R2)
                + " <= R2 <= " + PrintValue((1 + maxError) * R2));
        }
        private static void PrintCnomValue(Component R1, Component R2)
        {
            if ((R1.value == 0) || (R2.value == 0)) return;

            double R2prime = R1.value / R1divR2;
            double C = (R1.value + R2prime) / (R1.value * R2prime)
                * (1 / fs);
            Console.WriteLine("C = " + PrintValue(C));
        }

        private static void PrintIfRisAcceptable(ADC adc)
        {
            if (adc == null) return;
            if ((adc.R1.value == 0) || (adc.R1.power == 0)) return;

            double maxPower = Math.Pow(maxVoltage, 2) / adc.R1.value;
            Console.WriteLine("Max Power is " + PrintValue(maxPower));
            if (!IsRAcceptable(adc))
                Console.WriteLine("The power rating is not acceptable.");
            else
                Console.WriteLine("The power rating is acceptable.");
        }

        private static List<ADC> SortByCost(List<ADC> unsorted)
        {
            return unsorted.OrderBy(x => x.totalCost).ToList();
        }

        private static string PrintValue(double value, int sigFigs = 5)
        {
            double number = value;
            string prefix = "";

            int decimalPlace = (int)Math.Floor(Math.Log10(number));
            if (decimalPlace >= 6)
            {
                prefix = "M";
                number *= Math.Pow(10, -6);
                decimalPlace += -6;
            }
            else if (decimalPlace >= 3)
            {
                prefix = "k";
                number *= Math.Pow(10, -3);
                decimalPlace += -3;
            }
            else if (decimalPlace >= 0)
            {
                // Do nothing
            }
            else if (decimalPlace >= -3)
            {
                prefix = "m";
                number *= Math.Pow(10, 3);
                decimalPlace += 3;
            }
            else if (decimalPlace >= -8)
            {
                prefix = "u";
                number *= Math.Pow(10, 6);
                decimalPlace += 6;
            }
            else if (decimalPlace >= -12)
            {
                prefix = "p";
                number *= Math.Pow(10, 12);
                decimalPlace += 12;
            }
            else
            {
                return "0";
            }

            if (decimalPlace > sigFigs)
            {
                number *= Math.Pow(10, sigFigs - decimalPlace);
                number = Math.Round(number);
                number *= Math.Pow(10, decimalPlace - sigFigs);
            }
            else
            {
                number = Math.Round(number, sigFigs - decimalPlace);
            }

            return number.ToString() + prefix;
        }

        private static bool IsRAcceptable(ADC adc)
        {
            if (adc == null) return false;
            if ((adc.R1.value == 0) || (adc.R1.power == 0)) return false;

            double maxPower = Math.Pow(maxVoltage, 2) / adc.R1.value;
            if (maxPower >= maxRatedRatio * adc.R1.power) return false;
            else return true;
        }
    }
}
