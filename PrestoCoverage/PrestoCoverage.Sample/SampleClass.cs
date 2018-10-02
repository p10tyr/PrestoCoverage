namespace PrestoCoverage.Sample
{
    public class SampleClass
    {

        public int NeverUsed { get; set; }
        public int Counter { get; set; }

        private int _privateCounter;


        public int Count(int a, int b)
        {
            var addition = a + b;

            //var division = a / b;

            var multiplied = a * b;

            return multiplied;

        }

        public string Untested(string arbitrary)
        {
            if (string.IsNullOrWhiteSpace(arbitrary))
                return "The string of characters was in fact a NULL reference";
            else
                return arbitrary;
        }

        public int MissingBranchtested(int number)
        {
            if (number == 0)
                return number ^ number;
            else if (number == 1)
                return number * number;
            else if (number == 2)
                return number / number;
            else
                return -1;
        }


    }
}
