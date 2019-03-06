using System;
using System.Collections.Generic;

namespace FunctionApp2
{
    public class CommandTest
    {
        public CommandTest()
        {
            var command = new ConcreateCommand(new Receiver());
            var invoker = new Invoker(command);
            invoker.ExecuteCommand();
        }

        abstract class Command
        {
            protected Receiver receiver;

            public abstract void Execute();

            public Command(Receiver receiver)
            {
                this.receiver = receiver;
            }
        }

        class ConcreateCommand : Command
        {
            public ConcreateCommand(Receiver receiver) : base(receiver)
            {
            }

            public override void Execute()
            {
                receiver.Action();
            }
        }

        class Receiver
        {
            public void Action()
            {
                Console.WriteLine("Called Receiver.Action()");
            }
        }

        class Invoker
        {
            private Command command;

            public Invoker(Command command)
            {
                this.command = command;
            }

            public void ExecuteCommand()
            {
                command.Execute();
            }
        }
    }

    class CommandReal
    {
        public CommandReal()
        {
            var user = new UserInvoker();
            user.Compute('+', 1);
            user.Compute('-', 0);
            user.Compute('*', 5);
            user.Compute('/', 2);

            user.Undo(4);
            user.Redo(3);
        }

        abstract class Command
        {
            public abstract void Execute();
            public abstract void UnExecute();
        }

        class CalculatorCommand : Command
        {
            private char _operator;
            private int operand;
            private Calculator calculator;

            public CalculatorCommand(Calculator calculator, char @operator, int operand)
            {
                this._operator = @operator;
                this.operand = operand;
                this.calculator = calculator;
            }

            public char Operator { set => _operator = value; }

            public int Operand { set => operand = value; }

            public override void Execute()
            {
                calculator.Operation(_operator, operand);
            }

            private char Undo(char @operator)
            {
                switch (@operator)
                {
                    case '+': return '-';
                    case '-': return '+';
                    case '*': return '/';
                    case '/': return '*';
                    default: throw new ArgumentException("@operator argument exception.");
                }
            }

            public override void UnExecute()
            {
                throw new NotImplementedException();
            }
        }

        class Calculator
        {
            private int curr = 0;

            public void Operation(char @operator, int operand)
            {
                switch (@operator)
                {
                    case '+': curr += operand; break;
                    case '-': curr -= operand; break;
                    case '*': curr *= operand; break;
                    case '/': curr /= operand; break;
                    default:
                        throw new ArgumentException("@operator Argument Exception.");
                }

                Console.WriteLine($"Current value = {curr} (following {@operator} {operand})");
            }
        }

        class UserInvoker
        {
            private Calculator calculator = new Calculator();
            private List<Command> commands = new List<Command>();
            private int current = 0;

            public void Redo(int levels)
            {
                Console.WriteLine($"\n-------Redo {levels} levels.");

                for (int i = 0; i < levels; i++)
                {
                    if (current < commands.Count - 1)
                    {
                        var command = commands[current++];
                        command.Execute();
                    }
                }
            }

            public void Undo(int levels)
            {
                Console.WriteLine($"\n--------Undo {levels} levels.");

                for (int i = 0; i < levels; i++)
                {
                    if (current > 0)
                    {
                        var command = commands[--current] as Command;
                        command.UnExecute();
                    }
                }
            }

            public void Compute(char @operator, int operand)
            {
                var command = new CalculatorCommand(calculator, @operator, operand);
                command.Execute();

                commands.Add(command);
                current++;
            }
        }
    }
}
