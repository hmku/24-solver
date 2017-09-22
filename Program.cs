using System;
using System.Collections;
using System.Collections.Generic;

namespace Solver
{
    class MainClass
    {
        /*
         a represents the first number spot in the expression, b represents the second spot, etc.
         x represents the first operation spot in the expression, y represents the second spot, etc.
         The value of a, b, c, d represents the index of the number in nums[], 
            and the value of x, y, z represents the index of the operation in operations[]
            
         q represents the order in which a, b, c, d are combined (periods represent operations)
            q=0: ((a.b).c).d
            q=1: (a.(b.c)).d
            q=2: a.((b.c).d)
            q=3: a.(b.(c.d))
            q=4: (a.b).(c.d)
            
         Returns an ArrayList of strings
        */
        public static List<string> Solve (double[] nums)
        {
            string[] operations = new string[] { "+", "-", "*", "/" };
            List<string> solutions = new List<string>();
            
            for (int a = 0; a < 4; a++)
            {
                for (int b = 0; b < 4; b++)
                {
                    if (b == a) continue;

                    for (int c = 0; c < 4; c++)
                    {
                        if (c == b || c == a) continue;

                        for (int d = 0; d < 4; d++)
                        {
                            if (d == c || d == b || d == a) continue;

                            for (int x = 0; x < 4; x++)
                            {
                                for (int y = 0; y < 4; y++)
                                {
                                    for (int z = 0; z < 4; z++)
                                    {
                                        for (int q = 0; q <= 4; q++)
                                        {
                                            ArrayList expression = new ArrayList();
                                        
                                            expression.Add(nums[a]);
                                            expression.Add(operations[x]);
                                            expression.Add(nums[b]);
                                            expression.Add(operations[y]);
                                            expression.Add(nums[c]);
                                            expression.Add(operations[z]);
                                            expression.Add(nums[d]);
                                            expression.Add(q);

                                            if (Evaluate(expression))
                                            {
                                                solutions.Add(Format(expression));
                                            }
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }

            solutions = RemoveDuplicates(solutions);
            return solutions;
        }
        
        /*
         Given an expression in the form of (num, operation, num, operation, num, operation, num, order)
         Solves the expression and checks whether it is equal to 24
         Returns a boolean representing whether the expression is equal to 24
        */
        public static bool Evaluate (ArrayList expression)
        {
            int q = (int)expression[7];
            double abcd = 0.0;

            //((a.b).c).d
            if (q == 0)
            {
                double ab = Combine((double)expression[0], (double)expression[2], (string)expression[1]);
                double abc = Combine(ab, (double)expression[4], (string)expression[3]);
                abcd = Combine(abc, (double)expression[6], (string)expression[5]);
            }

            //(a.(b.c)).d
            else if (q == 1)
            {
                double bc = Combine((double)expression[2], (double)expression[4], (string)expression[3]);
                double abc = Combine((double)expression[0], bc, (string)expression[1]);
                abcd = Combine(abc, (double)expression[6], (string)expression[5]);
            }

            //a.((b.c).d)
            else if (q == 2)
            {
                double bc = Combine((double)expression[2], (double)expression[4], (string)expression[3]);
                double bcd = Combine(bc, (double)expression[6], (string)expression[5]);
                abcd = Combine((double)expression[0], bcd, (string)expression[1]);
            }

            //a.(b.(c.d))
            else if (q == 3)
            {
                double cd = Combine((double)expression[4], (double)expression[6], (string)expression[5]);
                double bcd = Combine((double)expression[2], cd, (string)expression[3]);
                abcd = Combine((double)expression[0], bcd, (string)expression[1]);
            }

            //(a.b).(c.d)
            else if (q == 4)
            {
                double ab = Combine((double)expression[0], (double)expression[2], (string)expression[1]);
                double cd = Combine((double)expression[4], (double)expression[6], (string)expression[5]);
                abcd = Combine(ab, cd, (string)expression[3]);
            }

            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return (Equals24(abcd));
        }
        
        /*
         Formats the expression into a string
         Returns a string representing the expression
        */
        public static string Format (ArrayList expression)
        {
            int q = (int)expression[7];
            string expStr = "";

            //((a.b).c).d
            if (q == 0)
            {
                expStr = "((" + expression[0].ToString() + " " + (string)expression[1] + " " +
                expression[2].ToString() + ") " + (string)expression[3] + " " +
                expression[4].ToString() + ") " + (string)expression[5] + " " +
                expression[6].ToString();
            }

            //(a.(b.c)).d
            else if (q == 1)
            {
                expStr = "(" + expression[0].ToString() + " " + (string)expression[1] + " (" +
                expression[2].ToString() + " " + (string)expression[3] + " " +
                expression[4].ToString() + ")) " + (string)expression[5] + " " +
                expression[6].ToString();
            }

            //a.((b.c).d)
            else if (q == 2)
            {
                expStr = expression[0].ToString() + " " + (string)expression[1] + " ((" +
                expression[2].ToString() + " " + (string)expression[3] + " " +
                expression[4].ToString() + ") " + (string)expression[5] + " " +
                expression[6].ToString() + ")";
            }

            //a.(b.(c.d))
            else if (q == 3)
            {
                expStr = expression[0].ToString() + " " + (string)expression[1] + " (" +
                expression[2].ToString() + " " + (string)expression[3] + " (" +
                expression[4].ToString() + " " + (string)expression[5] + " " +
                expression[6].ToString() + "))";
            }

            //(a.b).(c.d)
            else if (q == 4)
            {
                expStr = "(" + expression[0].ToString() + " " + (string)expression[1] + " " +
                expression[2].ToString() + ") " + (string)expression[3] + " (" +
                expression[4].ToString() + " " + (string)expression[5] + " " +
                expression[6].ToString() + ")";
            }

            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return expStr;
        }
        
        /*
         Removes any duplicate strings from the list of strings
         Returns the new list of strings
        */
        public static List<string> RemoveDuplicates (List<string> solutions)
        {
            List<string> newSolutions = new List<string>();
            
            foreach (string solution in solutions)
            {
                if (!newSolutions.Contains(solution))
                    newSolutions.Add(solution);
            }

            return newSolutions;
        }
        
        /*
         Combines two doubles, given an operation
         Returns the result of the operation
        */
        public static double Combine (double a, double b, string operation)
        {
            if (operation.Equals("+"))
            {
                return a + b;
            }
            else if (operation.Equals("-"))
            {
                return a - b;
            }
            else if (operation.Equals("*"))
            {
                return a * b;
            }
            else if (operation.Equals("/"))
            {
                return a / b;
            }
            return 0.0; //should not happen
        }
        
        /*
         Determines if a value is "close enough" (within 0.001) to 24.
         Returns a boolean representing whether or not the value is equal to 24.
        */
        public static bool Equals24 (double num)
        {
            if (Math.Abs(num - 24.0) < 0.001)
                return true;
                
            return false;
        }
        
        public static void Main(string[] args)
        {
            const int LIMIT = 10; //limit on number of answers to be printed
        
            Console.WriteLine("Welcome to 24Solver!");
            Console.WriteLine("Type four numbers below, pressing enter after each number.");
            Console.WriteLine("This program will print out all the ways these four numbers can be combined to make 24.");

            double[] nums = new double[4];
            for (int i = 0; i < 4; i++)
            {
                nums[i] = double.Parse(Console.ReadLine());
            }

            Console.WriteLine("Solutions:");
                        
            List<string> answers = Solve(nums);
            if (answers.Count == 0)
            {
                Console.WriteLine("No solution!"); 
            }
            else
            {
                int count = 0;
                foreach (string answer in answers)
                {
                    if (count > LIMIT)
                    {
                        Console.WriteLine("That should be enough solutions!");
                        break;
                    }
                    
                    Console.WriteLine(answer);
                    count++; 
                }
            }
        }
    }
}
