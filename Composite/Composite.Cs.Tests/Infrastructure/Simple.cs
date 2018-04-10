namespace Composite.Cs.Tests.Infrastructure
{
    internal class Simple
    {
        public int Number { get; set; }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}