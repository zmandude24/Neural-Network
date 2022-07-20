using System;
namespace KNN
{
    /* A node is an input data point with coordinates
     * and the output type */
    public class Node
    {
        public double x { get; private set; }
        public double y { get; private set; }
        public string output = "";
        public Node(double x, double y) { this.x = x; this.y = y; }
    }

    public static class Grouping
    {
        public static string mathFunction(double x, double y)
        {
            double fx = Math.Pow(x, 3) - x;
            if (fx >= y) return "red";
            else return "blue";
        }
    }

    public static class Distance
    {
        public static double euclidean(Node a, Node b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) +
                Math.Pow(a.y - b.y, 2));
        }
    }

    public static class Classify
    {
        public static string unweighted(Node[] knn)
        {
            string output = knn[0].output;
            int outputNum = 0;
            int[] outputCounts = new int[knn.Length];
            int numOutputTypes = 0;
            string[] outputValues = new string[knn.Length];
            for (int i = 0; i < knn.Length; i++)
            {
                outputCounts[i] = 0;
                outputValues[i] = "";
            }

            for (int i = 0; i < knn.Length; i++)
            {
                // Check if the output type was already found
                int j;
                for (j = 0; j < numOutputTypes; j++)
                {
                    if (outputValues[j] == knn[i].output)
                    {
                        outputCounts[j]++;
                        break;
                    }
                }

                // If not, then add the new type
                if (j == numOutputTypes)
                {
                    outputCounts[j]++;
                    outputValues[j] = knn[i].output;
                }

                // Check for new leader
                if (outputCounts[j] > outputNum)
                    output = outputValues[j];
            }

            return output;
        }
    }

    class KNN_Program
    {
        static void printNodes(Node[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Console.WriteLine("nodes[" + i + "]: x = " +
                    nodes[i].x + ", y = " + nodes[i].y +
                    ", output = " + nodes[i].output);
            }
            Console.WriteLine("");
        }

        static void printDistances(Node node, Node[] nodes,
            string distanceAlgorithm)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                double dist = 0;
                switch (distanceAlgorithm)
                {
                    case "euclidean":
                        dist = Distance.euclidean(node, nodes[i]);
                        break;

                    default:
                        Console.WriteLine("Error: distanceAlgorithm " +
                            distanceAlgorithm + " was not recognized.");
                        break;
                }

                Console.WriteLine("nodes[" + i + "] distance is "
                    + dist);
            }
            Console.WriteLine("");
        }

        /* range: 0 is the min and 1 is the max */
        static Node getNode(double[] x_range, double[] y_range)
        {
            // NextDouble is between 0 and 1
            Random rdm = new Random();
            double x = (x_range[1] - x_range[0]) * rdm.NextDouble()
                + x_range[0];
            double y = (y_range[1] - y_range[0]) * rdm.NextDouble()
                + y_range[0];
            return new Node(x, y);
        }

        /* n: the number of nodes
         * algorithmName: the name of the method to determine
         *      the correct output */
        static Node[] getTrainingNodes(int n,
            double[] x_range, double[] y_range,
            string groupingAlgorithm)
        {
            Node[] nodes = new Node[n];
            for (int i = 0; i < n; i++)
                nodes[i] = getNode(x_range, y_range);

            switch (groupingAlgorithm)
            {
                case "mathFunction":
                    for (int i = 0; i < nodes.Length; i++)
                        nodes[i].output = Grouping.mathFunction(
                            nodes[i].x, nodes[i].y);
                    break;

                default:
                    Console.WriteLine("Error: groupingAlgorithm " +
                        groupingAlgorithm + " was not recognized.");
                    break;
            }

            return nodes;
        }

        /* return the k nearest neighbors in an array */
        static Node[] getKNN(Node node, Node[] trainingNodes,
            int k, string distanceAlgorithm)
        {
            Node[] knn = new Node[k];
            double[] knn_distances = new double[k];
            for (int i = 0; i < k; i++)
                knn_distances[i] = double.MaxValue;

            for (int i = 0; i < trainingNodes.Length; i++)
            {
                double dist = double.MaxValue;
                switch (distanceAlgorithm)
                {
                    case "euclidean":
                        dist = Distance.euclidean(
                            node, trainingNodes[i]);
                        break;

                    default:
                        Console.WriteLine("Error: distanceAlgorithm " +
                            distanceAlgorithm + " was not recognized.");
                        break;
                }

                if (dist < knn_distances[k - 1])
                {
                    knn_distances[k - 1] = dist;
                    knn[k - 1] = trainingNodes[i];
                    for (int j = k-2; j >= 0; j--)
                    {
                        // check the node above and swap the current
                        // indexed node if the distance is less
                        if (knn_distances[j] > dist)
                        {
                            knn_distances[j+1] = knn_distances[j];
                            knn[j + 1] = knn[j];

                            knn[j] = trainingNodes[i];
                            knn_distances[j] = dist;
                        }
                    }
                }
            }

            return knn;
        }

        static void Main(string[] args)
        {
            int n = 10;
            int k = 3;
            double[] x_range = { -10, 10 };
            double[] y_range = { -10, 10 };
            string groupingAlgorithm = "mathFunction";
            string distanceAlgorithm = "euclidean";

            Console.WriteLine("Starting KNN Demo\n");

            Node[] trainingNodes = getTrainingNodes(n, x_range, y_range,
                groupingAlgorithm);
            printNodes(trainingNodes);

            Node node = getNode(x_range, y_range);
            printDistances(node, trainingNodes, distanceAlgorithm);

            Node[] knn = getKNN(node, trainingNodes, k,
                distanceAlgorithm);
            printNodes(knn);
            printDistances(node, knn, distanceAlgorithm);

            Console.WriteLine(Classify.unweighted(knn));
            Console.WriteLine(Grouping.mathFunction(
                node.x, node.y));
        }
    }
}