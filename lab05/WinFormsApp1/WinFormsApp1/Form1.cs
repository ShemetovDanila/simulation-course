namespace WinFormsApp1
{
    public class MultRandom
    {
        private long x_mult;                                    // seed = x_0 для мультипликативного метода
        private const long c = (long)int.MaxValue / 2 + 3;      // множитель
        private const long m = (long)int.MaxValue;              // модуль

        public MultRandom(long seed)
        {
            x_mult = seed;
        }

        public MultRandom() : this(DateTime.Now.Ticks)          // текущее время в конструкторе по умолчанию
        {
        }

        public double Next()
        {
            x_mult = (c * x_mult) % m;
            return (double)x_mult / m;
        }
    }

    public partial class Form1 : Form
    {
        private MultRandom random;

        private List<(string name, double probability)> yesNoOutcomes;
        private List<(string className, double probability)> classProbabilities;

        public Form1()
        {
            InitializeComponent();

            random = new MultRandom();

            InitYesNoOutcomes(); 
            InitClassProbabilities();
        }

        private void InitYesNoOutcomes()
        {
            yesNoOutcomes = new List<(string name, double probability)>
            {
                ("Да", 0.77),
                ("Нет, идём поднимать рейтинг в 2с, ВароПал, 2к->2.5к, чардж+бш=покинуть тело", 0.23)
            };

            NormalizeProbabilities(yesNoOutcomes);
        }

        private void InitClassProbabilities() 
        {
            classProbabilities = new List<(string, double)>
            {
                ("Воин", 0.2),
                ("Разбойник", 0.07),
                ("Шаман", 0.2),
                ("Жрец", 0.02),
                ("Паладин", 0.2),
                ("Чернокнижник", 0.1),
                ("Охотник", 0.11),
                ("Друид", 0.1)
            };

            NormalizeProbabilities(classProbabilities);         // нормировать если сумма != 1           

            SortProbabilitiesDescending();                      // отсортировать по убыванию
        }

        private void NormalizeProbabilities(List<(string name, double probability)> items)
        {
            double sum = 0;
            for (int i = 0; i < classProbabilities.Count; i++)
            {
                sum += classProbabilities[i].probability;
            }

            if (sum != 1)                                       // Можно сделать: Math.Abs(sum - 1.0) > 0.0001 если можем допустить погрешность
            {
                for (int i = 0; i < classProbabilities.Count; i++)
                {
                    items[i] = (items[i].name, items[i].probability / sum);
                }
            }
        }

        private void SortProbabilitiesDescending()              // пузырьковая сортировка)
        {
            for (int i = 0; i < classProbabilities.Count - 1; i++)
            {
                for (int j = 0; j < classProbabilities.Count - 1 - i; j++)
                {
                    if (classProbabilities[j].probability < classProbabilities[j + 1].probability)
                    {
                        var temp = classProbabilities[j];
                        classProbabilities[j] = classProbabilities[j + 1];
                        classProbabilities[j + 1] = temp;
                    }
                }
            }
        }

        private string GetRandomByProbability(List<(string name, double probability)> items)
        {
            double randomValue = random.Next();

            foreach (var item in items)
            {
                randomValue -= item.probability;
                if (randomValue < 0)
                {
                    return item.name;
                }
            }
            return items[items.Count - 1].name;                 // чтобы мы точно вышли из метода - последний элемент 
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = GetRandomByProbability(yesNoOutcomes);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label5.Text = GetRandomByProbability(classProbabilities);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
