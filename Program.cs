using System;
namespace KNN
{
    public class Node
    {
        public double x = double.NaN;
        public double y = double.NaN;

        public string output = "";

        public double distance = double.NaN;


    }

    class KNN_Program
    {
        private static double f(double x)
        {
            return Math.Pow(x, 3) - x;
        }

        private static Node getNewNode(
            double[] x_range, double[] y_range)
        {
            Node newNode = new Node();
            Random random = new Random();

            // Set Random Inputs
            newNode.x = (x_range[1] - x_range[0]) * random.NextDouble()
                + x_range[0];
            newNode.y = (y_range[1] - y_range[0]) * random.NextDouble()
                + y_range[0];

            return newNode;
        }

        private static List<Node> getTrainingNodes(
            double[] x_range, double[] y_range, int n)
        {
            List<Node> trainingNodes = new List<Node>();

            for (int i = 0; i < n; i++)
            {
                // Set Random Inputs
                Node newNode = getNewNode(x_range, y_range);

                // Set Correct Output
                if (f(newNode.x) >= newNode.y)
                    newNode.output = "red";
                else
                    newNode.output = "blue";

                // Add to List
                trainingNodes.Add(newNode);
            }

            return trainingNodes;
        }

        private static List<Node> getKnnNodes(List<Node> trainingNodes,
            Node testNode, int k)
        {
            List<Node> knnNodes = new List<Node>();

            // Set Distances
            for (int i = 0; i < trainingNodes.Count; i++)
            {
                trainingNodes[i].distance = getDistance(
                    trainingNodes[i], testNode);

                // Is the list full?
                if (knnNodes.Count == k)
                {
                    // Is the current index at one of the k lowest distances?
                    if (trainingNodes[i].distance < knnNodes[k-1].distance)
                    {
                        knnNodes[k-1] = trainingNodes[i];
                        knnNodes = knnNodes.OrderBy(x => x.distance).ToList();
                    }
                }
                // If not then just add the element and then sort the list
                else
                {
                    knnNodes.Add(trainingNodes[i]);
                    knnNodes = knnNodes.OrderBy(x => x.distance).ToList();
                }
            }

            return knnNodes;
        }

        static void Main(string[] args)
        {
            int n = getN();
            double[] x_range = getRange("x-axis");
            double[] y_range = getRange("y-axis");
            int k = getK(n);

            List<Node> trainingNodes = getTrainingNodes(
                x_range, y_range, n);
            Node testNode = getNewNode(x_range, y_range);
            List<Node> knn = getKnnNodes(trainingNodes, testNode, k);

            Console.WriteLine("Training Node Distances:");
            foreach (Node node in trainingNodes)
                Console.WriteLine("distance = " + node.distance);

            Console.WriteLine("KNN Distances:");
            foreach (Node node in knn)
                Console.WriteLine("distance = " + node.distance);
            Console.WriteLine("");
        }

        private static int getN()
        {
            int n = 0;

            while (n == 0)
            {
                try
                {
                    Console.Write("Enter the number of training data points: ");
                    string? dummy = Console.ReadLine();
                    int dummyInt = Convert.ToInt32(dummy);
                    if (dummyInt <= 0)
                        Console.WriteLine("Enter a positive value.");
                    else
                        n = dummyInt;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return n;
        }

        // range[0] is min and range[1] is max
        private static double[] getRange(string coordinateLabel)
        {
            double[] range = { 0, 0 };

            while (range[0] >= range[1])
            {
                try
                {
                    double[] dummyRange = { 0, 0 };

                    Console.Write("Enter the " + coordinateLabel + " minimum: ");
                    string? dummy = Console.ReadLine();
                    dummyRange[0] = Convert.ToDouble(dummy);

                    Console.Write("Enter the " + coordinateLabel + " maximum: ");
                    dummy = Console.ReadLine();
                    dummyRange[1] = Convert.ToDouble(dummy);

                    if (dummyRange[0] >= dummyRange[1])
                        Console.WriteLine("The maximum must be greater than the minimum.");
                    else
                        range = dummyRange;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return range;
        }

        private static int getK(int n)
        {
            int k = 0;

            while (k == 0)
            {
                try
                {
                    Console.Write("Enter the value of k: ");
                    string? dummy = Console.ReadLine();
                    int dummyInt = Convert.ToInt32(dummy);
                    if (dummyInt <= 0)
                        Console.WriteLine("Enter a positive value.");
                    else if (dummyInt >= n)
                        Console.WriteLine("Value must be less than n = " + n);
                    else
                        k = dummyInt;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return k;
        }

        // euclidean distance: sqrt((x1-x2)^2 + (y1-y2)^2)
        private static double getDistance(Node node1, Node node2)
        {
            return Math.Sqrt(Math.Pow(node1.x - node2.x, 2) +
                Math.Pow(node1.y - node2.y, 2));
        }
    }
}